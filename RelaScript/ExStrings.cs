using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    public static class ExStrings
    {
        public static Expression GetConcatExpression(Expression a, Expression b)
        {
            return Expression.Call(typeof(ExStrings).GetTypeInfo().GetDeclaredMethod("Concat"),
                Expression.Convert(a, typeof(object)),
                Expression.Convert(b, typeof(object)));
        }

        public static string Concat(object a, object b)
        {
            return a.ToString() + b.ToString();
        }

        public static Expression GetParseIntExpression(Expression s)
        {
            return Expression.Call(typeof(ExStrings).GetTypeInfo().GetDeclaredMethod("ParseInt"), s);
        }

        private static int ParseInt(string s)
        {
            if (int.TryParse(s, out int r))
                return r;
            return 0; // todo: should we handle failed parse in any special way? throw exception?
        }

        public static Expression GetParseDoubleExpression(Expression s)
        {
            return Expression.Call(typeof(ExStrings).GetTypeInfo().GetDeclaredMethod("ParseDouble"), s);
        }

        private static double ParseDouble(string s)
        {
            if (double.TryParse(s, out double r))
                return r;
            return 0; // todo: see above
        }

        public static Expression GetParseBoolExpression(Expression s)
        {
            return Expression.Call(typeof(ExStrings).GetTypeInfo().GetDeclaredMethod("ParseBool"), s);
        }

        private static bool ParseBool(string s)
        {
            string ls = s.ToLower();
            if (ls == "true" || ls == "t")
                return true;
            if (ls == "false" || ls == "f")
                return false;
            return false; // todo: see above
        }
    }
}
