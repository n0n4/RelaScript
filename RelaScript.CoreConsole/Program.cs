using System;

namespace RelaScript.CoreConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            InputContext context = new InputContext();

            Exline quicktest = new Exline("f:newvar(v:iterator, 1);"
                + "\r\nf:newvar(v:alpha, 1.0);"
                + "\r\nf:while(v:iterator <= 100, ("
                + "\r\n\tf:setvar(v:alpha, v:alpha * 1.0135);"
                + "\r\n\tf:setvar(v:iterator, v:iterator + 1);"
                + "\r\n));"
                + "\r\nc:d(v:alpha)");

            context.CompileLine(quicktest);

            var result = quicktest.Execute(new object[] { 0 });

            Console.WriteLine(" " + result);

            Console.Read();
        }
    }
}
