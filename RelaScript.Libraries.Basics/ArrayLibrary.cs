using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript.Libraries.Basics
{
    public class ArrayLibrary : ILibrary
    {
        public string GetLibraryName()
        {
            return "basic:array";
        }

        private List<string> DefaultFunctions = new List<string>()
        {
            "f:subset","f:length","f:append","f:insert","f:remove"
        };

        public List<string> GetDefaultFunctions()
        {
            return DefaultFunctions;
        }

        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            switch (funcname)
            {
                case "f:subset":
                    if (args.Length > 2)
                        return Subset(
                            ExCasts.InspectObjectForObjectArray(args[0]),
                            ExCasts.GetObjectAsInt(args[1]),
                            ExCasts.GetObjectAsInt(args[2]));
                    return Subset(
                            ExCasts.InspectObjectForObjectArray(args[0]),
                            ExCasts.GetObjectAsInt(args[1]));
                case "f:length":
                    return Length(args);
                case "f:append":
                    return Append(
                        ExCasts.InspectObjectForObjectArray(args[0]),
                        ExCasts.UnwrapAtRuntime(args[1])
                        );
                case "f:insert":
                    return Insert(
                        ExCasts.InspectObjectForObjectArray(args[0]),
                        ExCasts.UnwrapAtRuntime(args[1]),
                        ExCasts.GetObjectAsInt(args[2])
                        );
                case "f:remove":
                    return Remove(
                        ExCasts.InspectObjectForObjectArray(args[0]),
                        ExCasts.GetObjectAsInt(args[1])
                        );
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public Expression GetFunctionExpression(string funcname, Expression argexp, Expression argParams, ParameterExpression inputParams, Expression inputContextParam, List<InputVar> compiledInputVarsList)
        {
            switch (funcname)
            {
                case "f:subset":
                    if (argexp is NewArrayExpression)
                    {
                        if ((argexp as NewArrayExpression).Expressions.Count > 2)
                        {
                            return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Subset",
                                new[] { typeof(object[]), typeof(int), typeof(int) }),
                                ExCasts.WrapArguments(ExFuncs.GetArgIndex(argexp, 0)),
                                ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)),
                                ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 2)));
                        }
                        return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Subset",
                            new[] { typeof(object[]), typeof(int) }),
                            ExCasts.WrapArguments(ExFuncs.GetArgIndex(argexp, 0)),
                            ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)));
                    }
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("SubsetRuntime",
                            new[] { typeof(object[]) }),
                            ExCasts.WrapArguments(argexp));
                case "f:length":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Length",
                        new[] { typeof(object[]) }),
                        ExCasts.WrapArguments(argexp));
                case "f:append":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Append",
                            new[] { typeof(object[]), typeof(object) }),
                            ExCasts.WrapArguments(ExFuncs.GetArgIndex(argexp, 0)),
                            ExFuncs.GetArgAsObject(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:insert":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Insert",
                            new[] { typeof(object[]), typeof(object), typeof(int) }),
                            ExCasts.WrapArguments(ExFuncs.GetArgIndex(argexp, 0)),
                            ExFuncs.GetArgAsObject(ExFuncs.GetArgIndex(argexp, 1)),
                            ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:remove":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Remove",
                            new[] { typeof(object[]), typeof(int) }),
                            ExCasts.WrapArguments(ExFuncs.GetArgIndex(argexp, 0)),
                            ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public void Inject(InputContext context, string asname)
        {
            LibraryUtility.DefaultInject(this, context, asname);
        }

        public int Length(object[] array)
        {
            return array.Length;
        }

        public object[] SubsetRuntime(object[] args)
        {
            if(args.Length > 2)
            {
                return Subset(
                    (object[])args[0],
                    ExCasts.GetObjectAsInt(args[1]),
                    ExCasts.GetObjectAsInt(args[2]));
            }
            return Subset(
                (object[])args[0],
                ExCasts.GetObjectAsInt(args[1]));
        }

        public object[] Subset(object[] array, int start)
        {
            object[] sub = new object[array.Length - start];
            for(int i = 0; i < array.Length - start; i++)
            {
                sub[i] = array[start + i];
            }
            return sub;
        }

        public object[] Subset(object[] array, int start, int count)
        {
            object[] sub = new object[count];
            for (int i = 0; i < count; i++)
            {
                sub[i] = array[start + i];
            }
            return sub;
        }

        public object[] Append(object[] array, object value)
        {
            object[] sup = new object[array.Length + 1];
            for(int i = 0; i < array.Length; i++)
            {
                sup[i] = array[i];
            }
            sup[array.Length] = value;
            return sup;
        }

        public object[] Insert(object[] array, object value, int position)
        {
            object[] sup = new object[array.Length + 1];
            int c = 0;
            for(int i = 0; i < sup.Length; i++)
            {
                if(i == position)
                {
                    sup[i] = value;
                }
                else
                {
                    sup[i] = array[c];
                    c++;
                }
            }
            return sup;
        }

        public object[] Remove(object[] array, int position)
        {
            object[] sub = new object[array.Length - 1];
            int c = 0;
            for(int i = 0; i < array.Length; i++)
            {
                if(i != position)
                {
                    sub[c] = array[i];
                    c++;
                }
            }
            return sub;
        }
    }
}
