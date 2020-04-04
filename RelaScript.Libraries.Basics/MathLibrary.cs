using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace RelaScript.Libraries.Basics
{
    public class MathLibrary : ILibrary
    {
        public string GetLibraryName()
        {
            return "basic:math";
        }

        private List<string> DefaultFunctions = new List<string>()
        {
            "f:sin",
            "f:cos",
            "f:tan",
            "f:abs",
            "f:min",
            "f:max",
            "f:avg",
            "f:sign",
            "f:pow",
            "f:sqrt",
            "f:ln",
            "f:log10",
            "f:log",
            "f:round",
            "f:floor",
            "f:ceiling",
            "f:acos",
            "f:asin",
            "f:atan",
            "f:atan2",
            "f:cosh",
            "f:sinh",
            "f:tanh",
            "f:exp",
            "f:clamp",
            "f:blend",
            // trigonometric
            "f:cot",
            "f:sec",
            "f:csc",
            "f:cas",
            // hyperbolic
            "f:asinh",
            "f:acosh",
            "f:atanh",
            "f:acoth",
            "f:asech",
            "f:acsch",
            "f:dasinh",
            "f:dacosh",
            "f:datanh",
            "f:dacoth",
            "f:dasech",
            "f:dacsch",
            // periodics
            "f:clausen",
            "f:cycloid",
            "f:squarewave",
            "f:trianglewave",
            "f:sawtoothwave",
            // parametric
            "f:trochoid",
            "f:trochoidx",
            "f:trochoidy",
            "f:hypotrochoid",
            "f:hypotrochoidx",
            "f:hypotrochoidy",
            "f:epitrochoid",
            "f:epitrochoidx",
            "f:epitrochoidy",
            "f:epicycloid",
            "f:epicycloidx",
            "f:epicycloidy",
            "f:hypocycloid",
            "f:hypocycloidx",
            "f:hypocycloidy",
        };

        public List<string> GetDefaultFunctions()
        {
            return DefaultFunctions;
        }

        // Depreciated VVV
        public object ExecuteFunction(string funcname, object[] args, InputContext context)
        {
            switch (funcname)
            {
                case "f:sin":
                    return Sin(ExCasts.GetObjectAsDouble(args[0]));
                case "f:cos":
                    return Cos(ExCasts.GetObjectAsDouble(args[0]));
                case "f:tan":
                    return Tan(ExCasts.GetObjectAsDouble(args[0]));
                case "f:abs":
                    return Abs(ExCasts.GetObjectAsDouble(args[0]));
                case "f:min":
                    return Min(ExCasts.GetObjectsAsDoubleUnwrapping(args));
                case "f:max":
                    return Max(ExCasts.GetObjectsAsDoubleUnwrapping(args));
                case "f:avg":
                    return Avg(ExCasts.GetObjectsAsDoubleUnwrapping(args));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public Expression GetFunctionExpression(string funcname, Expression argexp, Expression argParams, ParameterExpression inputParams, Expression inputContextParam, List<InputVar> compiledInputVarsList)
        {
            switch (funcname)
            {
                case "f:sin":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Sin",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:cos":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Cos",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:tan":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Tan",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:abs":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Abs",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:min":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Min",
                        new[] { typeof(double[]) }),
                        ExFuncs.GetArgsAsDouble(argexp));
                case "f:max":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Max",
                        new[] { typeof(double[]) }),
                        ExFuncs.GetArgsAsDouble(argexp));
                case "f:avg":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Avg",
                        new[] { typeof(double[]) }),
                        ExFuncs.GetArgsAsDouble(argexp));
                case "f:sign":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Sign",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:pow":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Pow",
                        new[] { typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:sqrt":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Sqrt",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:ln":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Ln",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:log10":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Log10",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:log":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Log",
                        new[] { typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:round":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Round",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:floor":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Floor",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:ceiling":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Ceiling",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:acos":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Acos",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:asin":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Asin",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:atan":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Atan",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:atan2":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Atan2",
                        new[] { typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:cosh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Cosh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:sinh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Sinh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:tanh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Tanh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:exp":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Exp",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:clamp":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Clamp",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:blend":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Blend",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));

                // trigonometric
                case "f:cot":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Cot",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:sec":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Sec",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:csc":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Csc",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:cas":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Cas",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                // hyperbolic
                case "f:asinh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Asinh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:acosh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Acosh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:atanh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Atanh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:acoth":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Acoth",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:asech":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Asech",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:acsch":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Acsch",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:dasinh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Dasinh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:dacosh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Dacosh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:datanh":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Datanh",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:dacoth":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Dacoth",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:dasech":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Dasech",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:dacsch":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Dacsch",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                // periodics
                case "f:clausen":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Clausen",
                        new[] { typeof(double[]) }),
                        ExFuncs.GetArgsAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:cycloid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Cycloid",
                        new[] { typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)));
                case "f:squarewave":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("SquareWave",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:trianglewave":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("TriangleWave",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                case "f:sawtoothwave":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("SawtoothWave",
                        new[] { typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)));
                // parametric
                case "f:trochoid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Trochoid",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:trochoidx":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("TrochoidX",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:trochoidy":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("TrochoidY",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:hypotrochoid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Hypotrochoid",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:hypotrochoidx":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("HypotrochoidX",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:hypotrochoidy":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("HypotrochoidY",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:epitrochoid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Epitrochoid",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:epitrochoidx":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("EpitrochoidX",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:epitrochoidy":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("EpitrochoidY",
                        new[] { typeof(double), typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 3)));
                case "f:epicycloid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Epicycloid",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:epicycloidx":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("EpicycloidX",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:epicycloidy":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("EpicycloidY",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:hypocycloid":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("Hypocycloid",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:hypocycloidx":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("HypocycloidX",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                case "f:hypocycloidy":
                    return Expression.Call(Expression.Constant(this), this.GetType().GetRuntimeMethod("HypocycloidY",
                        new[] { typeof(double), typeof(double), typeof(double) }),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 0)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 1)),
                        ExFuncs.GetArgAsDouble(ExFuncs.GetArgIndex(argexp, 2)));
                default:
                    throw new Exception("Func '" + funcname + "' not found in library '" + GetLibraryName() + "'");
            }
        }

        public void Inject(InputContext context, string asname)
        {
            LibraryUtility.DefaultInject(this, context, asname);
        }

        public double Sin(double i)
        {
            return Math.Sin(i);
        }

        public double Cos(double i)
        {
            return Math.Cos(i);
        }

        public double Tan(double i)
        {
            return Math.Tan(i);
        }

        public double Abs(double i)
        {
            return Math.Abs(i);
        }

        public double Min(double[] a)
        {
            double lowest = double.MaxValue;
            foreach (double i in a)
                if (i < lowest)
                    lowest = i;
            return lowest;
        }

        public double Max(double[] a)
        {
            double highest = double.MinValue;
            foreach (double i in a)
                if (i > highest)
                    highest = i;
            return highest;
        }


        public double Avg(double[] a)
        {
            double sum = 0;
            foreach (double i in a)
                sum += i;
            return sum / a.Length;
        }

        public double Sign(double i)
        {
            return Math.Sign(i);
        }

        public double Pow(double i, double e)
        {
            return Math.Pow(i, e);
        }

        public double Sqrt(double i)
        {
            return Math.Sqrt(i);
        }

        public double Ln(double i)
        {
            return Math.Log(i);
        }

        public double Log10(double i)
        {
            return Math.Log10(i);
        }

        public double Log(double i, double e)
        {
            return Math.Log(i, e);
        }

        public double Round(double i)
        {
            return Math.Round(i);
        }

        public double Floor(double i)
        {
            return Math.Floor(i);
        }

        public double Ceiling(double i)
        {
            return Math.Ceiling(i);
        }

        public double Acos(double i)
        {
            return Math.Acos(i);
        }

        public double Asin(double i)
        {
            return Math.Asin(i);
        }

        public double Atan(double i)
        {
            return Math.Atan(i);
        }

        public double Atan2(double i, double e)
        {
            return Math.Atan2(i, e);
        }

        public double Cosh(double i)
        {
            return Math.Cosh(i);
        }

        public double Sinh(double i)
        {
            return Math.Sinh(i);
        }

        public double Tanh(double i)
        {
            return Math.Tanh(i);
        }

        public double Exp(double i)
        {
            return Math.Exp(i);
        }

        public double Clamp(double i, double min, double max)
        {
            if (i >= max)
                return max;
            if (i <= min)
                return min;
            return i;
        }

        public double Blend(double frac, double min, double max)
        {
            return frac * ((max - min) + min);
        }

        #region Trigonometric
        public double Cot(double x)
        {
            return 1 / Math.Tan(x);
        }
        public double Sec(double x)
        {
            return 1 / Math.Cos(x);
        }
        public double Csc(double x)
        {
            return 1 / Math.Sin(x);
        }

        public double Cas(double x)
        {
            return Math.Sqrt(2) * Math.Sin(x + (Math.PI / 4));
        }
        #endregion Trigonometric

        #region Hyperbolic
        public double Asinh(double x)
        {
            return Math.Log(x + (Math.Sqrt((x * x) + 1)));
        }
        public double Acosh(double x)
        {
            return Math.Log(x + (Math.Sqrt((x * x) - 1)));
        }
        public double Atanh(double x)
        {
            return (0.5) * Math.Log((1 + x) / (1 - x));
        }
        public double Acoth(double x)
        {
            return (0.5) * Math.Log((x + 1) / (x - 1));
        }
        public double Asech(double x)
        {
            return Math.Log((1 + Math.Sqrt(1 - (x * x))) / x);
        }
        public double Acsch(double x)
        {
            return Math.Log((1 + Math.Sqrt(1 + (x * x))) / x);
        }

        // derivatives
        public double Dasinh(double x)
        {
            return 1 / (Math.Sqrt((x * x) + 1));
        }
        public double Dacosh(double x)
        {
            return 1 / (Math.Sqrt((x * x) - 1));
        }
        public double Datanh(double x)
        {
            return 1 / (1 - (x * x));
        }
        public double Dacoth(double x)
        {
            return Datanh(x);
        }
        public double Dasech(double x)
        {
            return -1 / (x * Math.Sqrt(1 - (x * x)));
        }
        public double Dacsch(double x)
        {
            return -1 / (Math.Abs(x) * Math.Sqrt(1 + (x * x)));
        }
        #endregion Hyperbolic

        #region Periodic
        public double Clausen(double[] a)//double x, double z = 2.0, int maxk = 10)
        {
            double x = a[0];
            double z = 2.0;
            if (a.Length > 1)
                z = a[1];
            int maxk = 10;
            if (a.Length > 2)
                maxk = (int)a[2];
            // increase maxk for a better approximation
            int k = 0;
            double sum = 0;
            while (k < maxk)
            {
                sum += Math.Sin(k * x) / Math.Pow(k, z);
                k++;
            }
            return sum;
        }
        public double Cycloid(double x, double r)
        {
            return r * Math.Acos((1 - x) / r) - Math.Sqrt(2 * r * x - x * x);
        }
        public double SquareWave(double x)
        {
            return Math.Sign(Math.Sin(x));
        }
        public double TriangleWave(double x)
        {
            return (Math.Abs((x % (2 * Math.PI)) - Math.PI) / (Math.PI / 2)) - 1.0;
        }
        public double SawtoothWave(double x)
        {
            return ((x % Math.PI) / Math.PI) - 0.5;
        }
        #endregion

        #region Parametric
        public double[] Trochoid(double theta, double a, double b)
        {
            double[] v = new double[2];
            v[0] = TrochoidX(theta, a, b); // X
            v[1] = TrochoidY(theta, a, b); // Y
            return v;
        }
        public double TrochoidX(double theta, double a, double b)
        {
            return a * theta - b * Math.Sin(theta);
        }
        public double TrochoidY(double theta, double a, double b)
        {
            return a - b * Math.Cos(theta);
        }

        public double[] Hypotrochoid(double theta, double R, double r, double d)
        {
            double[] v = new double[2];
            v[0] = HypotrochoidX(theta, R, r, d); // X
            v[1] = HypotrochoidY(theta, R, r, d); // Y
            return v;
        }
        public double HypotrochoidX(double theta, double R, double r, double d)
        {
            return (R - r) * Math.Cos(theta) + d * Math.Cos(((R - r) / r) * theta);
        }
        public double HypotrochoidY(double theta, double R, double r, double d)
        {
            return (R - r) * Math.Sin(theta) - d * Math.Sin(((R - r) / r) * theta);
        }

        public double[] Epitrochoid(double theta, double R, double r, double d)
        {
            double[] v = new double[2];
            v[0] = EpitrochoidX(theta, R, r, d); // X
            v[1] = EpitrochoidY(theta, R, r, d); // Y
            return v;
        }
        public double EpitrochoidX(double theta, double R, double r, double d)
        {
            return (R + r) * Math.Cos(theta) - d * Math.Cos(((R + r) / r) * theta);
        }
        public double EpitrochoidY(double theta, double R, double r, double d)
        {
            return (R + r) * Math.Sin(theta) - d * Math.Sin(((R + r) / r) * theta);
        }

        public double[] Epicycloid(double theta, double R, double r)
        {
            double[] v = new double[2];
            v[0] = EpicycloidX(theta, R, r); // X
            v[1] = EpicycloidY(theta, R, r); // Y
            return v;
        }
        public double EpicycloidX(double theta, double R, double r)
        {
            return (R + r) * Math.Cos(theta) - r * Math.Cos(((R + r) / r) * theta);
        }
        public double EpicycloidY(double theta, double R, double r)
        {
            return (R + r) * Math.Sin(theta) - r * Math.Sin(((R + r) / r) * theta);
        }

        public double[] Hypocycloid(double theta, double R, double r)
        {
            double[] v = new double[2];
            v[0] = HypocycloidX(theta, R, r); // X
            v[1] = HypocycloidY(theta, R, r); // Y
            return v;
        }
        public double HypocycloidX(double theta, double R, double r)
        {
            return (R - r) * Math.Cos(theta) + r * Math.Cos(((R - r) / r) * theta);
        }
        public double HypocycloidY(double theta, double R, double r)
        {
            return (R - r) * Math.Sin(theta) - r * Math.Sin(((R - r) / r) * theta);
        }
        #endregion Parametric
    }
}
