using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using task07;

namespace task09
{
    public class ClassLibraryMetadata
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Ошибка: не указан путь к динамической библиотеке");
                return;
            }

            string dllpath = args[0];

            if (!File.Exists(dllpath))
            {
                Console.WriteLine($"Ошибка: Файл по пути '{dllpath}' не найден.");
                return;
            }

            Assembly assembly;
            try
            {
                assembly = Assembly.LoadFrom(dllpath);
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("Ошибка: Файл не является валидной .NET сборкой.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке библиотеки: {ex.Message}");
                return;
            }

            Type[] typesclasses;
            try
            {
                typesclasses = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine("Предупреждение: Некоторые типы в сборке ссылаются на отсутствующие библиотеки.");
                typesclasses = ex.Types.Where(t => t != null).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Непредвиденная ошибка при чтении типов: {ex.Message}");
                return;
            }

            foreach (Type t in typesclasses)
            {
                Console.WriteLine($"\nКласс: {t.Name}");

                MethodInfo[] methods = t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                if (methods != null)
                {
                    foreach (MethodInfo mi in methods)
                    {
                        if (mi.IsSpecialName) continue;

                        Console.WriteLine($"  Метод: {mi.Name}");
                        ParameterInfo[] parametrsmethod = mi.GetParameters();
                        var parametrsinfo = from p in parametrsmethod
                                            select new
                                            {
                                                name = p.Name,
                                                type = p.ParameterType,
                                            };

                        foreach (var paramses in parametrsinfo)
                        {
                            Console.WriteLine($"    Параметр метода: {paramses.type} {paramses.name}");
                        }
                    }
                }

                object[] atributes = t.GetCustomAttributes(true);
                if (atributes != null)
                {
                    foreach (object atr in atributes)
                    {
                        string atrName = atr.GetType().Name;
                        Console.WriteLine($"  Атрибут класса: {atrName}");

                        if (atr is task07.DisplayNameAttribute displayNameAttr)
                        {
                            Console.WriteLine($"    [DisplayName: {displayNameAttr.DisplayName}]");
                        }
                        else if (atr is VersionAttribute versionAttr)
                        {
                            Console.WriteLine($"    [Version: {versionAttr.Major}.{versionAttr.Minor}]");
                        }
                    }
                }

                ConstructorInfo[] constructs = t.GetConstructors();
                if (constructs != null)
                {
                    foreach (ConstructorInfo construct in constructs)
                    {
                        Console.WriteLine($"  Конструктор: {construct.Name}");
                        ParameterInfo[] parametrsconstruct = construct.GetParameters();
                        var constinfo = from p in parametrsconstruct
                                        select new
                                        {
                                            name = p.Name,
                                            type = p.ParameterType,
                                        };

                        foreach (var paramses in constinfo)
                        {
                            Console.WriteLine($"    Параметр конструктора: {paramses.type} {paramses.name}");
                        }
                    }
                }
            }
        }
    }
}
