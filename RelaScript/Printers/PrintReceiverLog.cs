using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Printers
{
    public class PrintReceiverLog : IPrintReceiver
    {
        public List<string> Logs = new List<string>();

        public void Print(string s)
        {
            Logs.Add(s);
        }
    }
}
