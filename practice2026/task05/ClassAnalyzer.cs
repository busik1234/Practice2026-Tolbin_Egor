using System;
using System.Reflection;
using System.Collections.Generic;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }
        public IEnumerable<string> GetPublicMethods()
        {
            var methods = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            return methods.Select(m => m.Name);
        }
        public IEnumerable<string> GetMethodParams(string methodname)
        {
            var method = _type.GetMethod(methodname, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (method != null) //Если такой параметр найден
            {
                ParameterInfo[] parameters = method.GetParameters();
                var parametrsallinfo = from parametrs in parameters
                                       select $"{parametrs.Name}{parametrs.ParameterType}";
                return parametrsallinfo;
            }
            else
            {
                return Enumerable.Empty<string>();
            }

        }
        public IEnumerable<string> GetAllFields()
        {
            var fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return fields.Select(m => m.Name);
        }
        public IEnumerable<string> GetProperties()
        {
            var properties = _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return properties.Select(m => m.Name);
        }
        public bool HasAttribute<T>() where T : Attribute
        {
            return _type.IsDefined(typeof(T), true);
        }
    }
}
