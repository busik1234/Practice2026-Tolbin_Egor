using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type type)
        {
            if (type == null) return;

            var dispatribute = type.GetCustomAttribute<DisplayNameAttribute>();
            if (dispatribute != null)
            {
                Console.WriteLine($"{dispatribute.DisplayName}");
            }

            var veratribute = type.GetCustomAttribute<VersionAttribute>();
            if (veratribute != null)
            {
                Console.WriteLine($"{veratribute.Major} {veratribute.Minor}");
            }

            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            if (methods != null)
            {
                foreach (var method in methods)
                {
                    if (method.IsSpecialName)
                    {
                        continue;
                    }
                    var atributethismethod = method.GetCustomAttribute<DisplayNameAttribute>();
                    if (atributethismethod != null)
                    {
                        Console.WriteLine($"{atributethismethod.DisplayName}");
                    }
                }
            }

            var parametrs = type.GetProperties();
            if (parametrs != null) 
            {
                foreach (var prop in parametrs)
                {
                    var atributethisparametrs = prop.GetCustomAttribute<DisplayNameAttribute>();
                    if (atributethisparametrs != null)
                    {
                        Console.WriteLine($"{atributethisparametrs.DisplayName}");
                    }
                }
            }
        }
    }
}