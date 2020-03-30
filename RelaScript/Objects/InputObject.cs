using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Objects
{
    public class InputObject
    {
        public string Name = string.Empty;
        public InputContext Implementation = new InputContext();
        public bool Freed = false;

        public InputObject(string name, int scopeid)
        {
            Name = name;
            Implementation.ScopeId = scopeid;
        }

        public void Free()
        {
            Freed = true;
            if(Implementation != null)
            {
                Implementation.Free();
            }
            Implementation = null;
        }
    }
}
