using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript.Libraries.Basics
{
    public class RandomLibrary : ILibrary
    {
        public string GetLibraryName()
        {
            return "basic:random";
        }

        private List<string> DefaultFunctions = new List<string>()
        {
            "f:random", "f:randomint", "f:roll"
        };

        public List<string> GetDefaultFunctions()
        {
            return DefaultFunctions;
        }

        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            switch (funcname)
            {
                case "f:random":
                    return RandomDouble(ExCasts.GetObjectAsDouble(args[0]), ExCasts.GetObjectAsDouble(args[1]), context.Random);
                case "f:randomint":
                    return RandomInt(ExCasts.GetObjectAsInt(args[0]), ExCasts.GetObjectAsInt(args[1]), context.Random);
                case "f:roll":
                    return Roll(ExCasts.GetObjectAsInt(args[0]), ExCasts.GetObjectAsInt(args[1]), context.Random);
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public Expression GetFunctionExpression(string funcname, Expression argexp, Expression argParams, ParameterExpression inputParams, Expression inputContextParam, List<InputVar> compiledInputVarsList)
        {
            switch (funcname)
            {
                case "f:random":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("RandomDouble",
                        new[] { typeof(double), typeof(double), typeof(IRandomProvider) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetRandomArg(inputContextParam));
                case "f:randomint":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("RandomInt",
                        new[] { typeof(int), typeof(int), typeof(IRandomProvider) }),
                        ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetRandomArg(inputContextParam));
                case "f:roll":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Roll",
                        new[] { typeof(int), typeof(int), typeof(IRandomProvider) }),
                        ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsInt(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetRandomArg(inputContextParam));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public void Inject(InputContext context, string asname)
        {
            LibraryUtility.DefaultInject(this, context, asname);
        }

        public double RandomDouble(double min, double max, IRandomProvider random)
        {
            return random.RandomDouble(min, max);
        }

        public int RandomInt(int min, int max, IRandomProvider random)
        {
            return random.RandomInt(min, max);
        }

        public int Roll(int dice, int sides, IRandomProvider random)
        {
            int sum = 0;
            for (int i = 0; i < dice; i++)
                sum += random.RandomInt(1, sides);
            return sum;
        }
    }
}
