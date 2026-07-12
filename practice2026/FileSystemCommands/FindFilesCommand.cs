using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemCommands
{
    public class FindFilesCommand : CommandLib.ICommand
    {
        public string Path { get; }
        public string PatternSearch { get; }

        public FindFilesCommand(string path, string patternSearch)
        {
            Path = path;
            PatternSearch = patternSearch;
        }

        public void Execute()
        {
            if (Directory.Exists(Path))
            {
                try
                {
                    string[] files = Directory.GetFiles(Path, PatternSearch, SearchOption.TopDirectoryOnly);
                    foreach (string file in files)
                    {
                        Console.WriteLine($"{System.IO.Path.GetFileName(file)}");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Ошибка: Нет доступа к указанной директории.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при поиске файлов: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Данная директория не найдена");
            }
        }
    }
}
