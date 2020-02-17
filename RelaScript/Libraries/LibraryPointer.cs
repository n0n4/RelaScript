using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    public class LibraryPointer
    {
        public string LibraryName = string.Empty;
        public string AsName = string.Empty;
        public ILibrary Library { private set; get; } = null;

        public LibraryPointer(string asname, ILibrary lib)
        {
            throw new Exception("DEPRECIATED");
            LibraryName = lib != null ? lib.GetLibraryName() : string.Empty;
            AsName = asname;
            Library = lib;
        }

        public void LoadLibrary(ILibrary lib)
        {
            Library = lib;
            LibraryName = lib.GetLibraryName();
        }

        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            throw new Exception("DEPRECIATED");
            //return Library.ExecuteFunction(funcname, args, context);
        }

        public Expression GetExecuteFunctionExpression(string funcname, Expression argexp, Expression inputContextParam)
        {
            Expression argexparray = ExCasts.WrapArguments(argexp);
            return Expression.Call(Expression.Constant(this),
                this.GetType().GetRuntimeMethod("ExecuteFunction", new[] { typeof(string), typeof(object[]), typeof(InputContext) }),
                Expression.Constant(funcname), argexparray, inputContextParam);
        }
    }
}
