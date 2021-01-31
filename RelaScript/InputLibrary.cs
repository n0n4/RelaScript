using RelaScript.Libraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    /// <summary>
    /// InputLibrary is compiled from Exlines
    /// </summary>
    public class InputLibrary : ILibrary
    {
        public string Name = string.Empty;
        public Dictionary<string, Exline> Funcs = new Dictionary<string, Exline>();

        public InputLibrary(string name)
        {
            Name = name;
        }

        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            return Funcs[funcname.ToLower()].Execute(args);
        }

        public List<string> GetDefaultFunctions()
        {
            return Funcs.Keys.ToList();
        }

        public Expression GetFunctionExpression(string funcname, Expression argexp, Expression argParams, ParameterExpression inputParams, Expression inputContextParam, List<InputVar> compiledInputVarsList)
        {
            // we can't precompile these, unfortunately.
            // so instead, use a dynamic call
            Expression argexparray = ExCasts.WrapArguments(argexp);
            return Expression.Call(Expression.Constant(this),
                this.GetType().GetRuntimeMethod("ExecuteFunction", new[] { typeof(string), typeof(object[]), typeof(InputContext) }),
                Expression.Constant(funcname), argexparray, inputContextParam);
        }

        public string GetLibraryName()
        {
            return Name;
        }

        public void Inject(InputContext context, string asname)
        {
            // TODO: either of these approaches should work, need to
            // spend some time thinking about which is better
            foreach (KeyValuePair<string, Exline> kvp in Funcs)
            {
                context.DefineFuncFromExline(kvp.Key, kvp.Value);
            }
            //LibraryUtility.DefaultInject(this, context, asname);
        }
    }
}
