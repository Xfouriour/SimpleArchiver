using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleArchiver
{
    static class Control //для управления сжатием
    {
        const int size = 32; //size < 256
        public static int Packing(string inputFilePath)
        {
            //открываем упаковываемый файл
            FileOperation InputFile;
            if (File.Exists(inputFilePath))
            {
                InputFile = new FileOperation(inputFilePath, true);
            }
            else
            {
                return -1;
            }

            //создаем таблицу частот
            int bytesRead;
            byte[] buf = new byte[size];
            FrequencyTable frequency = new FrequencyTable();

            while((bytesRead = InputFile.ReadFile(buf, size)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    frequency.Add(buf[i]);
                }
            }

            //строим бинарное дерево и получаем таблицу кодов
            BinaryTree tree = new BinaryTree(frequency);
            Dictionary<byte, short[]> codeTable = tree.GetCodeTable(frequency);

            //открываем файлы, пишем таблицу частот
            InputFile.Reset();
            FileOperation outputFile = new FileOperation(inputFilePath + ".pac", false);

            outputFile.WriteFile(new byte[] { 0 }, 1); //резерв места под число пустых бит в конце

            for (short i = 0; i < frequency.Length; i++) //записываем таблицу частот
            {
                outputFile.WriteFile(BitConverter.GetBytes(frequency[i]), sizeof(long));
            }

            Stopwatch timer = new Stopwatch();
            timer.Start();

            //упаковываем
            int outPos = 0; //текущий индекс в выходном массиве в _битах_
            byte[] outBuf = new byte[32]; //выходной массив
            bytesRead = InputFile.ReadFile(buf, size);
            while (bytesRead > 0)
            {
                for (byte i = 0; i < bytesRead; i++)
                {
                    byte value = buf[i];
                    short[] valueCode = codeTable[value];
                    for (byte t = 0; t < valueCode.Length; t++) //длина массива для каждого символа получаемого из buf
                    {
                        outBuf[outPos >> 3] |= (byte)(valueCode[t] * (1 << (7 - outPos & 7)));
                        outPos++;
                        if (outPos >= 32 * 8)
                        {
                            outputFile.WriteFile(outBuf, 32);
                            outPos = 0;
                            Array.Clear(outBuf, 0, 32);
                            //запись в файл
                        }
                    }
                }
                bytesRead = InputFile.ReadFile(buf, size);
            }
            if (outPos > 0) //запись оставшихся бит
            {
                outputFile.WriteFile(outBuf, outPos / 8 + ((outPos % 8 > 0) ? 1 : 0));
            }
            long outputFileLength = outputFile.GetPos();
            outputFile.Reset();
            outputFile.WriteFile(new byte[] { (byte)((8 - outPos % 8) % 8)}, 1); //записываем количество пустых бит в конце
            outputFile.CloseFile();
            InputFile.CloseFile();

            timer.Stop();
            long ms = timer.ElapsedMilliseconds;
            Console.WriteLine("Average packing speed: " + (int)(outputFileLength / 1024d / (ms / 1000d)) + " kb\\s"); //debug

            return 1;
        }

        public static int Unpacking(string inputFilePath)
        {
            FileOperation inputFile;
            if (File.Exists(inputFilePath))
            {
                inputFile = new FileOperation(inputFilePath, true);
            }
            else
            {
                return -1;
            }

            int bytesRead;
            byte[] endBytes = new byte[1]; //получаем число байт в конце
            inputFile.ReadFile(endBytes, 1);
            FrequencyTable frequency = new FrequencyTable(); //получаем таблицу частот
            byte[] buf = new byte[2048];
            if (inputFile.ReadFile(buf, 2048) < 2048)
                return -1;
            for (short i = 0; i < 256; i++)
            {
                frequency[i] = BitConverter.ToInt64(buf, i * sizeof(long));
            }

            //создаем дерево
            BinaryTree tree = new BinaryTree(frequency);

            //распаковываем
            FileOperation outputFile = new FileOperation(inputFilePath.Substring(0, inputFilePath.Length - 4), false); //убираем расширение .pac
            
            int outPos = 0, outVal;
            buf = new byte[size];
            byte[] outBuf = new byte[size];

            Stopwatch timer = new Stopwatch();
            timer.Start();

            bytesRead = inputFile.ReadFile(buf, size);
            while (bytesRead > 0)
            {
                int emptyBits = endBytes[0] * inputFile.EndOfFile();
                for (short i = 0; i < bytesRead * 8 - emptyBits; i++) //нумеруем _биты_
                {
                    byte currentValue = buf[i >> 3];
                    byte bit = (byte)(((currentValue & 1 << (7 - i & 7)) >= 1) ? 1 : 0); //получаем значение бита под номером i
                    outVal = tree.Decode(bit); //поочередно отправляем биты расшифровщику. если результат положительный - мы получили значение
                    if (outVal >= 0)
                    {
                        outBuf[outPos] = (byte)outVal;
                        outPos++;
                        if (outPos >= size)
                        {
                            outPos = 0;
                            outputFile.WriteFile(outBuf, size);
                        }
                    }
                }
                bytesRead = inputFile.ReadFile(buf, size);
            }
            //записываем оставшиеся байты, закрываем файлы
            if (outPos > 0)
            {
                outputFile.WriteFile(outBuf, outPos);
            }

            timer.Stop();
            long ms = timer.ElapsedMilliseconds;
            Console.WriteLine("Average unpacking speed: " + (int)(inputFile.GetPos() / 1024d / (ms / 1000d)) + " kb\\s"); //debug

            outputFile.CloseFile();
            inputFile.CloseFile();
            return 1;
        }
    }
}
