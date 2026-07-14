using System;
namespace task10
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class PluginLoadAttribute : Attribute
    {
        public string Name { get; }
        public List<string> Neighbours { get; }

        public PluginLoadAttribute(string name, List<string> neighbours)
        {
            Name = name;
            Neighbours = neighbours;
        }
    }

    public interface Iplugin
    {
        public void Execute() { }

    }
}
