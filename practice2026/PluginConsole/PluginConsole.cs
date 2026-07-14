using System;
using System.IO;
using task10;

namespace task10Console
{
    internal class PluginConsole
    {
        static void Main(string[] args)
        {
            string targetPath = string.Empty;

            if (args.Length > 0)
            {
                targetPath = args[0];
            }
            else
            {
                Console.WriteLine("Путь к папке с библиотеками не передан в аргументах запуска.");
                Console.Write("Пожалуйста, введите путь к директории вручную: ");
                targetPath = Console.ReadLine();
            }

            if (string.IsNullOrWhiteSpace(targetPath))
            {
                Console.WriteLine("Ошибка: введен пустой путь.");
                Console.ReadKey();
                return;
            }

            if (!Directory.Exists(targetPath))
            {
                Console.WriteLine($"Ошибка: директория по адресу '{targetPath}' не найдена.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Папка найдена. Начинается поиск и запуск плагинов..");
            Console.WriteLine();

            PluginLoad.PluginSearch(targetPath);

            Console.WriteLine();
            Console.WriteLine("Работа завершена. Нажмите любую клавишу для выхода");
            Console.ReadKey();
        }
    }
}
