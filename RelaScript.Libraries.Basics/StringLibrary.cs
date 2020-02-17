using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript.Libraries.Basics
{
    public class StringLibrary : ILibrary
    {
        public string GetLibraryName()
        {
            return "basic:string";
        }

        private List<string> DefaultFunctions = new List<string>()
        {
            "f:substring","f:length","f:comma","f:commaand"
        };

        public List<string> GetDefaultFunctions()
        {
            return DefaultFunctions;
        }

        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            switch (funcname)
            {
                case "f:substring":
                    if(args.Length > 2)
                        return Substring(ExCasts.GetObjectAsString(args[0]), ExCasts.GetObjectAsInt(args[1]), ExCasts.GetObjectAsInt(args[2]));
                    return Substring(ExCasts.GetObjectAsString(args[0]), ExCasts.GetObjectAsInt(args[1]));
                case "f:length":
                    return Length(ExCasts.GetObjectAsString(args[0]));
                case "f:comma":
                    return Comma(ExCasts.UnwrapStringArray(args));
                case "f:commaand":
                    return CommaAnd(ExCasts.UnwrapStringArray(args));
                case "f:replace":
                    return Replace(ExCasts.GetObjectAsString(args[0]), ExCasts.GetObjectAsString(args[1]), ExCasts.GetObjectAsString(args[2]));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public Expression GetFunctionExpression(string funcname, Expression argexp, Expression argParams, ParameterExpression inputParams, Expression inputContextParam, List<InputVar> compiledInputVarsList)
        {
            switch (funcname)
            {
                case "f:substring":
                    if (argexp is NewArrayExpression && (argexp as NewArrayExpression).Expressions.Count > 2)
                    {
                        return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Substring",
                            new[] { typeof(string), typeof(int), typeof(int) }),
                            ExFuncs.GetArgAsString(ExFuncs.GetArgIndex(argexp, 0)),
                            ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)),
                            ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 2)));
                    }
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Substring",
                        new[] { typeof(string), typeof(int) }),
                        ExFuncs.GetArgAsString(ExFuncs.GetArgIndex(argexp,0)),
                        ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:length":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Length",
                        new[] { typeof(string) }),
                        ExFuncs.GetArgAsString(argexp));
                case "f:comma":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Comma",
                        new[] { typeof(string[]) }),
                        Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("UnwrapStringArray"),
                            ExCasts.WrapArguments(argexp)));
                case "f:commaand":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("CommaAnd",
                        new[] { typeof(string[]) }),
                        Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("UnwrapStringArray"),
                            ExCasts.WrapArguments(argexp)));
                case "f:replace":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Replace",
                        new[] { typeof(string), typeof(string), typeof(string) }),
                        ExFuncs.GetArgAsString(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsString(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsString(ExFuncs.GetArgIndex(argexp, 2)));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public string Substring(string s, int start)
        {
            return s.Substring(start);
        }

        public string Substring(string s, int start, int count)
        {
            return s.Substring(start, count);
        }

        public int Length(string s)
        {
            return s.Length;
        }

        public string Comma(string[] ss)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < ss.Length; i++)
            {
                sb.Append(ss[i]);
                if (i + 1 < ss.Length)
                    sb.Append(", ");
            }
            return sb.ToString();
        }

        public string CommaAnd(string[] ss)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ss.Length; i++)
            {
                sb.Append(ss[i]);
                if (i + 1 < ss.Length)
                {
                    sb.Append(", ");
                    if (i + 2 == ss.Length)
                        sb.Append("and ");
                }
            }
            return sb.ToString();
        }

        public string Replace(string s, string a, string b)
        {
            return s.Replace(a, b);
        }
    }
}
