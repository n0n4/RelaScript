using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using RelaScript.Objects;

namespace RelaScript
{
    public static class ExCasts
    {
        public static Expression GetCastExpression(Expression castexp, Expression argexp)
        {
            // take the value out of the var, if we've got a var
            if (argexp.Type == typeof(InputVar)) 
                argexp = Expression.Field(argexp, "Value");

            string castname = (castexp as ConstantExpression).Value as string;
            switch (castname.ToLower())
            {
                case "c:o":
                    return Expression.Convert(argexp, typeof(object));
                case "c:b":
                    if (argexp.Type == typeof(string))
                        return ExStrings.GetParseBoolExpression(argexp);
                    return Expression.Convert(argexp, typeof(bool));
                case "c:b2":
                    return Expression.Convert(argexp, typeof(bool[]));
                case "c:d":
                    if (argexp.Type == typeof(string))
                        return ExStrings.GetParseDoubleExpression(argexp);
                    return Expression.Convert(argexp, typeof(double));
                case "c:d2":
                    return Expression.Convert(argexp, typeof(double[]));
                case "c:i":
                    if (argexp.Type == typeof(string))
                        return ExStrings.GetParseIntExpression(argexp);
                    return Expression.Convert(argexp, typeof(int));
                case "c:i2":
                    return Expression.Convert(argexp, typeof(int[]));
                case "c:s":
                    return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("ConvertToString"), 
                        Expression.Convert(argexp, typeof(object)));
                //return Expression.Convert(argexp, typeof(string));
                case "c:s2":
                    return Expression.Convert(argexp, typeof(string[]));
                default:
                    // we didn't recognize the cast
                    throw new Exception("Unrecognized cast '" + castname + "'");
            }
        }

        public static Expression WrapArguments(Expression args)
        {
            if (args.Type == typeof(double))
                return Expression.NewArrayInit(typeof(object), Expression.Convert(args, typeof(object)));
            else if (args.Type == typeof(double[]))
                return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("RewrapDoubleArray"), args);
            else if (args.Type == typeof(int))
                return Expression.NewArrayInit(typeof(object), Expression.Convert(args, typeof(object)));
            else if (args.Type == typeof(int[]))
                return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("RewrapIntArray"), args);
            else if (args.Type == typeof(string))
                return Expression.NewArrayInit(typeof(object), Expression.Convert(args, typeof(object)));
            else if (args.Type == typeof(string[]))
                return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("RewrapStringArray"), args);
            else if (args.Type == typeof(object[]))
                return args;
            else if (args.Type == typeof(InputVar))
                return Expression.NewArrayInit(typeof(object), Expression.Field(args, "Value"));
            else if (args.Type == typeof(AccessorResult))
                return Expression.NewArrayInit(typeof(object), Expression.Convert(args, typeof(object)));
            else if (args.Type == typeof(object))
            {
                // in this case, it needs to be inspected at the time, incase it is returned from a
                // function call that returns an object[] as an object.
                // if it is an object[] masquereding as an object, leave it as is.
                return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("InspectObjectForObjectArray"), args);
                //return Expression.IfThenElse(Expression.IsTrue(Expression.TypeIs(args, typeof(object[]))),
                //    args, Expression.NewArrayInit(typeof(object), args));
                //return Expression.NewArrayInit(typeof(object), args);
            }
            return args;
        }

        private static object[] RewrapDoubleArray(double[] ds)
        {
            // I couldn't figure out how to represent this in Expressions... so we just call this method
            object[] os = new object[ds.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                os[i] = (object)ds[i];
            }
            return os;
        }

        private static object[] RewrapIntArray(int[] ds)
        {
            // I couldn't figure out how to represent this in Expressions... so we just call this method
            object[] os = new object[ds.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                os[i] = (object)ds[i];
            }
            return os;
        }

        private static object[] RewrapStringArray(string[] ds)
        {
            // I couldn't figure out how to represent this in Expressions... so we just call this method
            object[] os = new object[ds.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                os[i] = (object)ds[i];
            }
            return os;
        }

        public static string[] UnwrapStringArray(object[] ds)
        {
            // I couldn't figure out how to represent this in Expressions... so we just call this method
            string[] os = new string[ds.Length];
            for (int i = 0; i < ds.Length; i++)
            {
                os[i] = GetObjectAsString(ds[i]);
            }
            return os;
        }

        private static object[] InspectObjectForObjectArray(object o)
        {
            if (o is object[])
                return (object[])o;
            return new object[] { o };
        }

        public static Expression UnwrapVariable(Expression v)
        {
            // if it's a variable, get the value out.
            if (v.Type == typeof(object))
            {
                // fancy unpacking: we can observe if it's just come out of a convert->object
                // and unwrap that at compiletime to hurry things along
                if(v is UnaryExpression)
                {
                    UnaryExpression u = v as UnaryExpression;
                    if(u.NodeType == ExpressionType.Convert)
                    {
                        if(u.Operand.Type == typeof(InputVar))
                        {
                            return Expression.Field(u.Operand, "Value");
                        }
                        if(u.Operand.Type == typeof(AccessorResult))
                        {
                            return Expression.Field(Expression.Field(u.Operand, "Var"), "Value");
                        }
                    }
                }
                // weren't able to unpack at runtime, must genuinely be unknown.
                return Expression.Call( // unknown type at runtime, must look at it to see if we should unwrap
                    typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("UnwrapAtRuntime"),
                    v);
            }
            if(v.Type == typeof(InputVar))
                return Expression.Field(v, "Value");
            if (v.Type == typeof(AccessorResult))
                return Expression.Field(Expression.Field(v, "Var"), "Value");
            return v;
        }

        public static object UnwrapAtRuntime(object o)
        {
            if (o is InputVar)
                return (o as InputVar).Value;
            if (o is AccessorResult)
                return (o as AccessorResult).Var.Value;
            return o;
        }

        public static string GetInputVarName(object o)
        {
            if (o is InputVar)
                return ((InputVar)o).Name;
            return (string)o;
        }

        public static InputVar GetInputVar(object o, InputContext context)
        {
            // special case: if a string is passed in, look up that var dynamically
            if (o is string)
                return ExFuncs.LookupVar((string)o, context);
            // otherwise, assume it's already an inputvar.
            return (InputVar)o;
        }

        public static string ConvertToString(object o)
        {
            return o.ToString();
        }



        public static int GetObjectAsInt(object o)
        {
            if (o is int)
                return (int)o;
            if (o is double)
                return (int)(double)o;
            return (int)o;
        }

        public static Expression GetExpressionObjectAsInt(Expression exp)
        {
            if (exp.Type == typeof(int))
                return exp;
            if (exp.Type == typeof(double))
                return Expression.Convert(ExCasts.UnwrapVariable(exp), typeof(int));
            return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("GetObjectAsInt"),
                exp);
        }

        public static double GetObjectAsDouble(object o)
        {
            if (o is double)
                return (double)o;
            if (o is int)
                return (double)(int)o;
            return (double)o;
        }

        public static double[] GetObjectsAsDouble(object[] os)
        {
            double[] outs = new double[os.Length];
            for (int i = 0; i < os.Length; i++)
                outs[i] = GetObjectAsDouble(os[i]);
            return outs;
        }

        public static double GetObjectAsDoubleUnwrapping(object o)
        {
            o = UnwrapAtRuntime(o);
            if (o is double)
                return (double)o;
            if (o is int)
                return (double)(int)o;
            return (double)o;
        }

        public static double[] GetObjectsAsDoubleUnwrapping(object[] os)
        {
            double[] outs = new double[os.Length];
            for (int i = 0; i < os.Length; i++)
                outs[i] = GetObjectAsDoubleUnwrapping(os[i]);
            return outs;
        }

        public static Expression GetExpressionObjectAsDouble(Expression exp)
        {
            if (exp.Type == typeof(double))
                return exp;
            if (exp.Type == typeof(int))
                return Expression.Convert(ExCasts.UnwrapVariable(exp), typeof(double));
            return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("GetObjectAsDouble"),
                exp);
        }

        public static string GetObjectAsString(object o)
        {
            if (o is string)
                return (string)o;
            return o.ToString();
        }

        public static Type[] InferTypes = new Type[] { typeof(double), typeof(double[]), typeof(int), typeof(int[]), typeof(bool) };
        public static void InferType(Expression a, Expression b, out Expression aout, out Expression bout, Type expected = null)
        {
            // preprocessing: if they are inputvar types, replace them with their values
            a = UnwrapVariable(a);
            b = UnwrapVariable(b);

            // if they're both expected case already, just return as-is
            if (expected != null && a.Type == expected && b.Type == expected)
            {
                aout = a;
                bout = b;
                return;
            }
            else if (expected != null && a.Type == expected)
            {
                // if at least one is the expected type, go with the expected type
                aout = a;
                bout = Expression.Convert(b, expected);
            }
            else if (expected != null && b.Type == expected)
            {
                bout = b;
                aout = Expression.Convert(a, expected);
            }

            for (int i = 0; i < InferTypes.Length; i++)
            {
                if (a.Type == InferTypes[i] && b.Type == InferTypes[i])
                {
                    aout = a;
                    bout = b;
                    return;
                }
                else if (a.Type == InferTypes[i])
                {
                    aout = a;
                    bout = Expression.Convert(b, InferTypes[i]);
                    return;
                }
                else if (b.Type == InferTypes[i])
                {
                    bout = b;
                    aout = Expression.Convert(a, InferTypes[i]);
                    return;
                }
            }
            // see if we have an expected type
            if (expected != null)
            {
                if (a.Type != expected)
                    aout = Expression.Convert(a, expected);
                else
                    aout = a;
                if (b.Type != expected)
                    bout = Expression.Convert(b, expected);
                else
                    bout = b;
                return;
            }
            // not recognized, throw it to the wind and pray
            aout = a;
            bout = b;
        }
    }
}
