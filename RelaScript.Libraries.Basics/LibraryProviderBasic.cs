using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Libraries.Basics
{
    public class LibraryProviderBasic : ILibraryProvider
    {
        public ILibrary LoadLibrary(string name)
        {
            switch (name)
            {
                case "basic:math":
                    return new MathLibrary();
                case "basic:random":
                    return new RandomLibrary();
                case "basic:string":
                    return new StringLibrary();
                default:
                    return null;
            }
        }
    }
}
