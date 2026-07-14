using System;
using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace task11
{
    public class CalculatorGeneration
    {
        public static ICalculator CreateCalculatorFromString(string sourceCode)
        {
            string modifiedCode = sourceCode.Replace(
                "public class Calculator",
                "public class Calculator : task11.ICalculator"
            );

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(modifiedCode);

            var references = new List<MetadataReference>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            CSharpCompilation compilation = CSharpCompilation.Create(
                "DynamicCalculatorAssembly",
                new[] { syntaxTree },
                references, 
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            using (MemoryStream ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Console.WriteLine("Ошибка при компиляции динамического кода:");
                    foreach (var diagnostic in result.Diagnostics)
                    {
                        Console.WriteLine(diagnostic.ToString());
                    }
                    return null;
                }

                ms.Seek(0, SeekOrigin.Begin);

                Assembly assembly = Assembly.Load(ms.ToArray());

                Type type = assembly.GetType("Calculator");

                object instance = Activator.CreateInstance(type);

                return (ICalculator)instance;
            }
        }
    }
}
