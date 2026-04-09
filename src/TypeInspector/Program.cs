using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        var asm = Assembly.Load("Microsoft.OpenApi");
        var secRefType = asm.GetType("Microsoft.OpenApi.OpenApiSecuritySchemeReference");
        if (secRefType != null) {
            foreach (var ctor in secRefType.GetConstructors())
            {
                var p = string.Join(", ", ctor.GetParameters().Select(param => $"{param.ParameterType.Name} {param.Name}"));
                Console.WriteLine($"Constructor: {p}");
            }
        }
    }
}
