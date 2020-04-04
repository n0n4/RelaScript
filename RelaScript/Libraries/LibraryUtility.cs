using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Libraries
{
    public static class LibraryUtility
    {
        public static void DefaultInject(ILibrary library, InputContext context, string asname)
        {
            StringBuilder script = new StringBuilder();
            script.Append("import ").Append(library.GetLibraryName()).Append(" ").AppendLine(asname);
            foreach (string fname in library.GetDefaultFunctions())
            {
                script.Append(fname).Append(" := { ").Append(asname).Append('.').Append(fname)
                    .AppendLine("(a:all) }");
            }
            Exline injection = new Exline(script.ToString());
            injection.Compile(context);
            injection.Execute(new object[] { 0 });
        }
    }
}
