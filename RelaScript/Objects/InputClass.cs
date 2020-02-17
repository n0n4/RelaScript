﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Objects
{
    public class InputClass
    {
        public string Name = string.Empty;
        public InputContext Definition = new InputContext();

        public InputClass(string name, InputContext defn)
        {
            Name = name;
            Definition = defn;
        }

        public InputObject PrintObject(string name)
        {
            InputObject o = new InputObject(name);
            Definition.CopyVars(o.Implementation);
            Definition.CopyFuncs(o.Implementation);
            Definition.CopyLibraries(o.Implementation);
            return o; 
        }

        public InputClass PrintSubclass(string name)
        {
            InputClass o = new InputClass(name, new InputContext());
            Definition.CopyVars(o.Definition);
            Definition.CopyFuncs(o.Definition);
            Definition.CopyLibraries(o.Definition);
            return o;
        }
    }
}
