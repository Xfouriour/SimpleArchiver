using System;
using System.Collections.Generic;

namespace SimpleArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            //работа приложения начинается с запуска интерфейса
            //который затем уже осуществляет управление:
            //сжатие, распаковка и, может быть, подсчет чексуммы
            while(ConsoleInterface.ConsoleRequest() != 0);
        }
    }

}
