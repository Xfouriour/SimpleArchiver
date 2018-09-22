using System;

namespace SimpleArchiver
{
    static class ConsoleInterface //класс взаимодействия с консолью
    {
        //обязанности:
        //команда архивации указанного файла с указанием адреса в качестве параметра
        //вопрос на вывод файла, либо вывод файла по умолчанию с новым расширением
        //команда распаковки
        private static void WriteLine(string s)
        {
            Console.WriteLine(s);
        }

        public static int ConsoleRequest()
        {
            Console.Write("Команда (справка help): ");
            return CommandRecognize(Console.ReadLine());
        }

        private static int CommandRecognize(string cmd)
        {
            string[] inputStrings = cmd.Split(' ');
            if (inputStrings.Length == 0)
            {
                return -1;
            }
            switch (inputStrings[0])
            {
                case "pack":
                    {
                        if (inputStrings.Length < 2)
                        {
                            WriteLine("Необходимо указать путь к файлу");
                            return -1;
                        }
                        string path = cmd.Substring(cmd.IndexOf(' ') + 1);
                        WriteLine("Пытаемся упаковать " + path);
                        if (Control.Packing(path) < 0)
                        {
                            WriteLine("Ошибка открытия файла");
                            return -1;
                        }
                        WriteLine("Упаковка успешно завершена, полученный файл: " + path + ".pac");
                        return 1;
                    }
                case "unpack":
                    {
                        if (inputStrings.Length < 2)
                        {
                            WriteLine("Необходимо указать путь к файлу");
                            return -1;
                        }
                        string path = cmd.Substring(cmd.IndexOf(' ') + 1);
                        WriteLine("Пытаемся распаковать " + path);
                        if (Control.Unpacking(path) < 0)
                        {
                            WriteLine("Ошибка открытия файла");
                            return -1;
                        }
                        WriteLine("Распаковка успешно завершена, полученный файл: " + path.Substring(0, path.Length - 4));
                        return 1;
                    }
                case "help":
                    {
                        WriteLine("Основные команды:");
                        WriteLine("pack <путь_к_файлу> - упаковка указанного файла");
                        WriteLine("unpack <путь к_файлу> - распаковка указанного файла");
                        WriteLine("exit - выход");
                        return 1;
                    }
                case "exit":
                    {
                        WriteLine("Выходим");
                        return 0;
                    }
                default:
                    {
                        WriteLine("Неизвестная команда");
                        return 1;
                    }
            }
        }
    }
}
