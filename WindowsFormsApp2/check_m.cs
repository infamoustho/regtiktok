using System;
using System.Reflection;
using System.Reflection.Emit;

class Program
{
    static void Main()
    {
        var asm = Assembly.LoadFrom(@"d:\New folder\WindowsFormsApp2\WindowsFormsApp2\bin\Debug\Infamous.dll");
        var type = asm.GetType("Infamous.ifm");
        var m = type.GetMethod("ClickByText", BindingFlags.Public | BindingFlags.Static);
        var mb = m.GetMethodBody();
        var il = mb.GetILAsByteArray();
        Console.WriteLine("IL bytes length: " + il.Length);
    }
}
