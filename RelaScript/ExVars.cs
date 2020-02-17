using RelaScript.Objects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    public static class ExVars
    {
        public static Expression GetVarExpression(Expression inputContext, Expression name)
        {
            return Expression.Call(inputContext, typeof(InputContext).GetRuntimeMethod("GetVar",
                new[] { typeof(string), typeof(bool) }),
                name, Expression.Constant(true));
        }

        private static InputVar GetVarAtRuntime(InputContext context, object o, string errtype)
        {
            InputVar var = null;
            if (o is InputVar)
            {
                var = o as InputVar;
            }
            else if (o is AccessorResult)
            {
                AccessorResult a = o as AccessorResult;
                if (!a.IsVar)
                    throw new Exception(errtype + " not supported for non-var types.");
                var = a.Var;
            }
            else if (o is string)
            {
                var = context.GetVar(o as string);
                if (var == null)
                    throw new Exception("Var " + o.ToString() + " not found at runtime.");
            }
            if (var == null)
                throw new Exception("Could not parse '" + o.ToString() + "' as var for " + errtype + ".");
            return var;
        }

        public static InputVar AddAssignAtRuntime(InputContext context, object o, object v)
        {
            InputVar var = GetVarAtRuntime(context, o, "add-assign");
            if(var.Value is string)
            {
                var.Value = ExStrings.Concat(var.Value, v);
                return var;
            }
            else if(var.Value is double)
            {
                if (v is double)
                    var.Value = ((double)var.Value) + ((double)v);
                else if (v is int)
                    var.Value = ((double)var.Value) + ((double)((int)v));
                else
                    throw new Exception("Tried to add-assign unsupported type '" + v.GetType() + "' to double var '" + var.Name + "'.");
                return var;
            }
            else if(var.Value is int)
            {
                if (v is double)
                    var.Value = ((int)var.Value) + ((int)((double)v));
                else if (v is int)
                    var.Value = ((int)var.Value) + ((int)v);
                else
                    throw new Exception("Tried to add-assign unsupported type '" + v.GetType() + "' to int var '" + var.Name + "'.");
                return var;
            }
            throw new Exception("Tried to add-assign unsupported type '" + var.Value.GetType() + "'.");
        }

        public static InputVar SubAssignAtRuntime(InputContext context, object o, object v)
        {
            InputVar var = GetVarAtRuntime(context, o, "sub-assign");
            if (var.Value is double)
            {
                if (v is double)
                    var.Value = ((double)var.Value) - ((double)v);
                else if (v is int)
                    var.Value = ((double)var.Value) - ((double)((int)v));
                else
                    throw new Exception("Tried to sub-assign unsupported type '" + v.GetType() + "' to double var '" + var.Name + "'.");
                return var;
            }
            else if (var.Value is int)
            {
                if (v is double)
                    var.Value = ((int)var.Value) - ((int)((double)v));
                else if (v is int)
                    var.Value = ((int)var.Value) - ((int)v);
                else
                    throw new Exception("Tried to sub-assign unsupported type '" + v.GetType() + "' to int var '" + var.Name + "'.");
                return var;
            }
            throw new Exception("Tried to sub-assign unsupported type '" + var.Value.GetType() + "'.");
        }

        public static InputVar MultAssignAtRuntime(InputContext context, object o, object v)
        {
            InputVar var = GetVarAtRuntime(context, o, "mult-assign");
            if (var.Value is double)
            {
                if (v is double)
                    var.Value = ((double)var.Value) * ((double)v);
                else if (v is int)
                    var.Value = ((double)var.Value) * ((double)((int)v));
                else
                    throw new Exception("Tried to mult-assign unsupported type '" + v.GetType() + "' to double var '" + var.Name + "'.");
                return var;
            }
            else if (var.Value is int)
            {
                if (v is double)
                    var.Value = ((int)var.Value) * ((int)((double)v));
                else if (v is int)
                    var.Value = ((int)var.Value) * ((int)v);
                else
                    throw new Exception("Tried to mult-assign unsupported type '" + v.GetType() + "' to int var '" + var.Name + "'.");
                return var;
            }
            throw new Exception("Tried to mult-assign unsupported type '" + var.Value.GetType() + "'.");
        }

        public static InputVar DivAssignAtRuntime(InputContext context, object o, object v)
        {
            InputVar var = GetVarAtRuntime(context, o, "div-assign");
            if (var.Value is double)
            {
                if (v is double)
                    var.Value = ((double)var.Value) / ((double)v);
                else if (v is int)
                    var.Value = ((double)var.Value) / ((double)((int)v));
                else
                    throw new Exception("Tried to div-assign unsupported type '" + v.GetType() + "' to double var '" + var.Name + "'.");
                return var;
            }
            else if (var.Value is int)
            {
                if (v is double)
                    var.Value = ((int)var.Value) / ((int)((double)v));
                else if (v is int)
                    var.Value = ((int)var.Value) / ((int)v);
                else
                    throw new Exception("Tried to div-assign unsupported type '" + v.GetType() + "' to int var '" + var.Name + "'.");
                return var;
            }
            throw new Exception("Tried to dic-assign unsupported type '" + var.Value.GetType() + "'.");
        }
    }
}
