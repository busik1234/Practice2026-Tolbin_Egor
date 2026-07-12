using System;
using CommandLib;
using System.IO;
using System.Reflection;

namespace CommandRunner
{
    public class CommandRunner
    {
        public static void Main()
        {
            string testDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ProgrammTests");
            if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

            if (File.Exists(dllPath))
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(dllPath);
                    Type typeofDirectorySizeCommand = assembly.GetType("FileSystemCommands.DirectorySizeCommand");
                    Type typeofFindFilesCommand = assembly.GetType("FileSystemCommands.FindFilesCommand");

                    if (typeofDirectorySizeCommand != null)
                    {
                        try
                        {
                            object dirsizecom = Activator.CreateInstance(typeofDirectorySizeCommand, new object[] { testDir });
                            ICommand comm1 = (ICommand)dirsizecom;
                            comm1.Execute();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при выполнении DirectorySizeCommand: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Класс FileSystemCommands.DirectorySizeCommand не найден в DLL.");
                    }

                    if (typeofFindFilesCommand != null)
                    {
                        try
                        {
                            object findlilescom = Activator.CreateInstance(typeofFindFilesCommand, new object[] { testDir, "*.txt" });
                            ICommand comm2 = (ICommand)findlilescom;
                            comm2.Execute();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при выполнении FindFilesCommand: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка: Класс FileSystemCommands.FindFilesCommand не найден в DLL.");
                    }
                }
                catch (BadImageFormatException)
                {
                    Console.WriteLine("Ошибка: Файл DLL поврежден или имеет неверный формат.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Критическая ошибка при работе с DLL: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Ошибка: Файл динамической библиотеки не найден по пути: {dllPath}");
            }
        }
    }
}
