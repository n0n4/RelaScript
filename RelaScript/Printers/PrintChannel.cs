using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Printers
{
    public class PrintChannel
    {
        private List<IPrintReceiver> PrintReceivers = new List<IPrintReceiver>();

        public void Register(IPrintReceiver pr)
        {
            PrintReceivers.Add(pr);
        }
        public void Deregister(IPrintReceiver pr)
        {
            PrintReceivers.Remove(pr);
        }

        public void Print(string s)
        {
            foreach (IPrintReceiver pr in PrintReceivers)
                pr.Print(s);
        }
    }
}
