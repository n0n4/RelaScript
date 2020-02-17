using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RelaScript.Objects
{
    public static class ExClasses
    {
        public static AccessorResult AccessVarObject(InputContext context, InputVar var, string field)
        {
            // first find the object
            if(var.Value is string)
            {
                // treat it like a regular string lookup
                return AccessFindObject(context, var.Value as string, field);
            }
            if(var.Value is InputObject)
            {
                return AccessObject(context, var.Value as InputObject, field);
            }
            throw new Exception("Var '" + var.Name + "' did not contain an object reference when accessing field '" + field + "'.");
        }

        public static AccessorResult AccessFindObject(InputContext context, string name, string field)
        {
            // first find the object
            name = name.ToLower();
            InputObject o = context.GetObject(name);
            if (o == null)
                throw new Exception("Object '" + name + "' undefined when accessing field '" + field + "'.");

            return AccessObject(context, o, field);
        }

        public static AccessorResult AccessObject(InputContext context, InputObject o, string field)
        {
            field = field.ToLower();
            if (field.StartsWith("v"))
            {
                return new AccessorResult(o.Implementation.GetVar(field));
            }
            else if (field.StartsWith("o"))
            {
                return new AccessorResult(o.Implementation.GetObject(field));
            }
            else if (field.StartsWith("f"))
            {
                return new AccessorResult(o.Implementation.GetFunc(field));
            }

            // the issue here:
            // at compile time, we won't have any idea what kind of object this is.
            // so how do we use it properly?
            // e.g. a func would return as an object so how would we know it's an exline
            // how would we know to execute it
            // I think this isn't going to work as an accessing methodology...

            throw new Exception("Field '" + field + "' not found in object '" + o.Name + "'.");
        }
    }
}
