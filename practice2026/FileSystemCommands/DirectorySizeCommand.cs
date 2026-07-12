using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    public class DirectorySizeCommand : CommandLib.ICommand
    {
        public string Path { get; }

        public DirectorySizeCommand(string path)
        {
            Path = path;
        }

        public void Execute()
        {
            long sizeallfiles = 0;
            if (Directory.Exists(Path))
            {
                try
                {
                    var directory = new DirectoryInfo(Path);
                    var filesinfo = directory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                    foreach (FileInfo file in filesinfo)
                    {
                        long sizethisfile = file.Length;
                        sizeallfiles += sizethisfile;
                    }
                    Console.WriteLine($"{sizeallfiles} - размер каталога");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка: Нет доступа к одному из файлов или каталогов.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при подсчете размера: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Данная директория не найдена");
            }
        }
    }
}