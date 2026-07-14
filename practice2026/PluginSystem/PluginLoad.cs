using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace task10
{
    public class PluginLoad
    {
        public class DataPlugin
        {
            public string DisplayName { get; set; }
            public List<string> Neighbours { get; set; }
            public Type typePlugin { get; set; }
        }

        public static List<DataPlugin> SortPluginDFS(List<DataPlugin> plugins)
        {
            List<DataPlugin> sort = new List<DataPlugin>();
            List<string> visited = new List<string>();
            Stack<string> stack = new Stack<string>();
            List<string> readyplugin = new List<string>();

            foreach (var startplugin in plugins)
            {
                if (!visited.Contains(startplugin.DisplayName))
                {
                    stack.Push(startplugin.DisplayName);
                    while (stack.Count > 0)
                    {
                        string thispluginName = stack.Peek();
                        if (!visited.Contains(thispluginName))
                        {
                            visited.Add(thispluginName);
                            List<string> neighborthisplugin = (from p in plugins
                                                               where p.DisplayName == thispluginName
                                                               select p.Neighbours).FirstOrDefault();
                            if (neighborthisplugin != null)
                            {
                                foreach (string neighbor in neighborthisplugin)
                                {
                                    if (visited.Contains(neighbor) && !readyplugin.Contains(neighbor))
                                    {
                                        Console.WriteLine($"Обнаружена циклическая зависимость у плагина: {neighbor}");
                                        return new List<DataPlugin>();
                                    }
                                    if (!visited.Contains(neighbor))
                                    {
                                        stack.Push(neighbor);
                                    }
                                }
                            }
                        }
                        else
                        {
                            stack.Pop();
                            if (!readyplugin.Contains(thispluginName))
                            {
                                readyplugin.Add(thispluginName);

                                DataPlugin plugpop = (from p in plugins
                                                      where p.DisplayName == thispluginName
                                                      select p).FirstOrDefault();

                                if (plugpop != null)
                                {
                                    sort.Add(plugpop);
                                }
                            }
                        }
                    }
                }
            }
            return sort;
        }
        public static void PluginSearch(string directorypath)
        {
            if (Directory.Exists(directorypath))
            {
                var filesdll = Directory.GetFiles(directorypath, "*.dll");
                List<DataPlugin> plugins = new List<DataPlugin>();

                foreach (var file in filesdll)
                {
                    try
                    {
                        string fullPath = Path.GetFullPath(file);
                        Assembly assembly = Assembly.LoadFrom(fullPath);
                        Type[] types = assembly.GetTypes();

                        foreach (Type type in types)
                        {
                            var thisattribute = type.GetCustomAttribute<PluginLoadAttribute>();

                            if (thisattribute != null &&
                                !type.IsAbstract &&
                                type.IsClass &&
                                typeof(Iplugin).IsAssignableFrom(type))
                            {
                                plugins.Add(new DataPlugin
                                {
                                    DisplayName = thisattribute.Name,
                                    Neighbours = thisattribute.Neighbours.ToList(),
                                    typePlugin = type
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при загрузке файла {file}: {ex.Message}");
                    }
                }

                List<DataPlugin> newsortplugin = SortPluginDFS(plugins);

                if (newsortplugin.Count == 0 && plugins.Count > 0)
                {
                    Console.WriteLine("Запуск плагинов невозможен из-за ошибок в зависимостях.");
                    return;
                }

                foreach (DataPlugin plugin in newsortplugin)
                {
                    try
                    {
                        object obj = Activator.CreateInstance(plugin.typePlugin);
                        Iplugin plug = (Iplugin)obj;
                        plug.Execute();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Не удалось запустить плагин {plugin.DisplayName}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Директория не найдена");
            }
        }

    }

}
