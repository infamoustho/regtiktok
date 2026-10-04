using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            var asm = Assembly.LoadFrom(@"d:\New folder\WindowsFormsApp2\WindowsFormsApp2\bin\Debug\Infamous.dll");
            foreach (var type in asm.GetTypes())
            {
                Console.WriteLine("Type: " + type.FullName);
                foreach (var m in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
                {
                    if (m.DeclaringType == type)
                    {
                        Console.WriteLine("  Method: " + m.Name);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
        }
    }
}
