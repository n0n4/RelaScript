using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    public interface ILibraryProvider
    {
        // return null if not found in this provider
        ILibrary LoadLibrary(string name);
    }
}
