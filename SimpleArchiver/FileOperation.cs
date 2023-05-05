using System.IO;

namespace SimpleArchiver
{
    class FileOperation //для чтения файла и записи результата
    {
        //чтение файла целиком и возврат указателя на него
        //запись файла, целиком
        private FileStream fileStream;
        private bool isReadMode;

        public FileOperation(string path, bool readMode)
        {
            isReadMode = readMode;
            if (isReadMode)
            {
                fileStream = new FileStream(@path, FileMode.Open);
            }
            else
            {
                fileStream = new FileStream(@path, FileMode.Create);
            }
        }

        public bool EndOfFile()
        {
            return fileStream.Position == fileStream.Length;
        }

        public int ReadFile(byte[] buffer, int size)
        {
            if (!isReadMode)
                return 0;
            int bytesRead = fileStream.Read(buffer, 0, size);
            return bytesRead;
        }

        public int WriteFile(byte[] buffer, int size)
        {
            if (isReadMode)
                return 0;
            fileStream.Write(buffer, 0, size);
            return 1;
        }

        public int Reset()
        {
            fileStream.Position = 0;
            return 1;
        }

        public int CloseFile()
        {
            fileStream.Close();
            return 1;
        }

        public bool IsReadMode()
        {
            return isReadMode;
        }

        public long GetPos()
        {
            return fileStream.Position;
        }
    }
}
