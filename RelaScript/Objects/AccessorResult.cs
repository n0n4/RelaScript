using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript.Objects
{
    public class AccessorResult
    {
        public bool IsVar = false;
        public InputVar Var = null;

        public bool IsFunc = false;
        public Exline Func = null;

        public bool IsObject = false;
        public InputObject Object = null;

        public AccessorResult(InputVar var)
        {
            IsVar = true;
            Var = var;
        }

        public AccessorResult(Exline func)
        {
            IsFunc = true;
            Func = func;
        }

        public AccessorResult(InputObject obj)
        {
            IsObject = true;
            Object = obj;
        }

        public object ExecuteFunc(object[] inputs)
        {
            // TODO: consider removing this function call and instead just doing
            // a call execute on expression.field(accessor, "Func")?
            // need to evaluate the performance difference
            // (conversely, if there is little to no performance difference, should
            //  change var to use a function in here instead of direct access so we
            //  can provide better error messages...)
            if (!IsFunc)
                throw new Exception("Accessor result was not a function, yet was called to execute.");
            if (Func == null)
                throw new Exception("Accessor result was a function, but was undefined.");
            return Func.Execute(inputs);
        }
    }
}
