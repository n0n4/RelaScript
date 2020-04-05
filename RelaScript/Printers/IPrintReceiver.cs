using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Printers
{
    public interface IPrintReceiver
    {
        void Print(string s);
    }
}
