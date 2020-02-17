using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Intermediates
{
    /// <summary>
    /// Used during compilation for cases like l:math . f:sin
    /// the dot accessor constructs this intermediary, which is then passed
    /// to exfuncs and the function call is rendered at that point
    /// </summary>
    public class LibraryFunctionIntermediate
    {
        public ILibrary Library = null;
        public string FuncName = string.Empty;

        public LibraryFunctionIntermediate(ILibrary library, string funcname)
        {
            Library = library;
            FuncName = funcname;
        }
    }
}
