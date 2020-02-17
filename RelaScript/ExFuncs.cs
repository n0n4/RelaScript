using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using RelaScript.Intermediates;
using RelaScript.Objects;

namespace RelaScript
{
    public static class ExFuncs
    {
        public static List<string> DefaultFunctions { get; private set; } = new List<string>()
        {
            "f:while", "f:for", "f:if",
            "f:newfunc", "f:setfunc", "f:newvar", "f:setvar", "f:getvar", "f:import",
            "f:sin",
            "f:random", "f:randomint", "f:roll"
        };

        public static Expression GetFunctionExpression(Expression funcexp, Expression argexp,
            List<InputContext> compiledContexts, int scopeContext,
            Expression argParams, ParameterExpression inputParams, ParameterExpression contextParams,
            List<InputVar> compiledInputVarsList)
        {
            // special case: library function
            bool doDynamicLibraryCall = false;
            if(funcexp is ConstantExpression && funcexp.Type == typeof(LibraryFunctionIntermediate))
            {
                LibraryFunctionIntermediate lfi = (funcexp as ConstantExpression).Value as LibraryFunctionIntermediate;
                if (lfi.Library == null)
                {
                    // library hasn't been populated yet, so we have to do dynamic call
                    doDynamicLibraryCall = true;
                }
                else
                {
                    return lfi.Library.GetFunctionExpression(lfi.FuncName,
                        argexp, argParams, inputParams, 
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)), compiledInputVarsList);
                }
            }
            else if (funcexp.Type == typeof(LibraryFunctionIntermediate))
            {
                doDynamicLibraryCall = true;
            }

            if(doDynamicLibraryCall)
            {
                // the dynamic case: the library function intermediate can't be rendered at compiled time
                // TODO: put some explanation here as to how and why this happens
                throw new Exception("Attempted to access library dynamically, which is not supported.");
                /*Expression argexparray = ExCasts.WrapArguments(argexp);
                return Expression.Call(
                    Expression.Field(funcexp, typeof(LibraryFunctionIntermediate), "LibraryPointer"), 
                    typeof(LibraryPointer).GetRuntimeMethod(
                        "ExecuteFunction",
                        new[] { typeof(string), typeof(object[]), typeof(InputContext) }),
                    Expression.Field(funcexp, typeof(LibraryFunctionIntermediate), "FuncName"),
                    argexparray,
                    Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));*/
            }

            // special case: object was accessed
            if(funcexp.Type == typeof(AccessorResult))
            {
                // this can't be rendered at compile time so we need to call out at runtime
                // to try to access the func that may be in the access result
                Expression argexparray = ExCasts.WrapArguments(argexp);
                return Expression.Call(funcexp,
                    typeof(AccessorResult).GetRuntimeMethod("ExecuteFunc", new Type[] { typeof(object[]) }),
                    argexparray);
            }


            string funcname = (funcexp as ConstantExpression).Value as string;
            switch (funcname.ToLower())
            {
                //case "f:index":
                //    return Expression.ArrayIndex(argexp, 
                //        Expression.Add(Expression.Constant(1), Expression.Convert(Expression.ArrayIndex(argexp, Expression.Constant(0)), typeof(int))));
                //        //Expression.Convert(Expression.ArrayIndex(argexp, Expression.Subtract(Expression.ArrayLength(argexp), Expression.Constant(1))), typeof(int)));
                case "f:return":
                    // todo: make a returnlabel at the end of the expression and return to it
                    //return Expression.Return()
                    // this needs to be done with a special operator instead
                    throw new Exception("TODO");
                case "f:import":
                    // load a library
                    // switching this so it happens at compiletime
                    // doing some really convoluted unwrapping here...
                    ExLibs.ImportLibrary(
                        (string)(((argexp as NewArrayExpression).Expressions[0] as UnaryExpression).Operand as ConstantExpression).Value,
                        (string)(((argexp as NewArrayExpression).Expressions[1] as UnaryExpression).Operand as ConstantExpression).Value,
                        compiledContexts[scopeContext]);
                    return Expression.Constant(0);
                    /*return Expression.Call(typeof(ExLibs).GetTypeInfo().GetDeclaredMethod("ImportLibrary"),
                        GetArgAsString(GetArgIndex(argexp, 0)),
                        GetArgAsString(GetArgIndex(argexp, 1)),
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));*/
                case "f:while":
                    // TODO: remove these comments...
                    // always returns 0
                    //RuntimeVariablesExpression whileruntimevars = Expression.RuntimeVariables(
                    //    Expression.Variable(typeof(object[]), "whileargsparam"));
                    // whileruntimevars.Variables[0]

                    // a breakdown of the issue:
                    // every time we reference the full arguments, the whole thing is executed
                    // this means that the "then" part of the if is ALWAYS executed
                    // when we access the condition, the THEN is executed as well

                    // how can we avoid this???

                    // could we like... wrap the args in an ifthenelse that is always false so
                    // it doesn't execute upon assignment... and then access the then element directly?
                    // uhhh idk

                    // test with GetArgIndexUnbox
                    LabelTarget breaklabel = Expression.Label("WhileBreak");
                    return Expression.Block(
                        Expression.Loop(
                            Expression.IfThenElse(
                                GetArgAsBool(GetArgIndexUnbox(argexp, 0)),
                                GetArgIndexUnbox(argexp, 1),
                                Expression.Break(breaklabel)
                                ),
                            breaklabel
                            ),
                        Expression.Constant(0)
                        );
                    // unused prototype
                    // TODO: remove this once no longer needed 
                    /*ParameterExpression whileargsparam = Expression.Variable(typeof(object[]), "whileargsparam");
                    LabelTarget breaklabel = Expression.Label("WhileBreak");
                    return Expression.Block(
                        new[] { whileargsparam },
                        //whileruntimevars,
                        Expression.Assign(whileargsparam, argexp),
                        Expression.Loop(
                            Expression.IfThenElse(
                                GetArgAsBool(GetArgIndex(whileargsparam, 0)),
                                Expression.Assign(whileargsparam, argexp), // reassign, forcing re-execute
                                //GetArgIndex(whileargsparam, 1),
                                Expression.Break(breaklabel)
                                ),
                            breaklabel
                            ),
                        Expression.Constant(0)
                        );*/
                case "f:for":
                    throw new Exception("TODO");
                    return null;
                /*ParameterExpression foriterator = Expression.Parameter(typeof(int), "foriterator");
                LabelTarget breakforlabel = Expression.Label("ForBreak");
                return Expression.Block(
                    Expression.Loop(
                        Expression.IfThenElse(
                            GetArgIndex(argexp, 0),
                            GetArgIndex(argexp, 1),
                            Expression.Break(breakforlabel)
                            ),
                        breakforlabel
                        ),
                    Expression.Constant(0)
                    );*/
                case "f:if":
                    ParameterExpression ifresult = Expression.Parameter(typeof(object), "ifresult");
                    // if three args were given, divert to ifelse
                    if(argexp is NewArrayExpression && (argexp as NewArrayExpression).Expressions.Count > 2)
                    {
                        return Expression.Block(
                            new[] { ifresult },
                            Expression.IfThenElse(
                                GetArgAsBool(GetArgIndex(argexp, 0)),
                                Expression.Assign(ifresult, GetArgAsObject(GetArgIndex(argexp, 1))),
                                Expression.Assign(ifresult, GetArgAsObject(GetArgIndex(argexp, 2)))),
                            ifresult);
                    }
                    return Expression.Block(
                        new[] { ifresult },
                        Expression.IfThenElse(
                            GetArgAsBool(GetArgIndex(argexp, 0)),
                            Expression.Assign(ifresult, GetArgAsObject(GetArgIndex(argexp, 1))),
                            Expression.Assign(ifresult, GetArgAsObject(Expression.Constant(0)))),
                        ifresult);
                case "f:ifelse":
                    ParameterExpression ifelseresult = Expression.Parameter(typeof(object), "ifelseresult");
                    return Expression.Block(
                        new[] { ifelseresult },
                        Expression.IfThenElse(
                            GetArgAsBool(GetArgIndex(argexp, 0)),
                            Expression.Assign(ifelseresult, GetArgAsObject(GetArgIndex(argexp, 1))),
                            Expression.Assign(ifelseresult, GetArgAsObject(GetArgIndex(argexp, 2)))),
                        ifelseresult);
                case "f:newfunc":
                    // branching behavior here:
                    // if arg[1] is a string, we'll compile by text
                    // if arg[1] is not a string, we'll treat it as the expression to use for the new func
                    Expression newfuncarg1 = GetArgIndex(argexp, 1);
                    // THOUGHT: I don't want to unwrap the var that returns from a func, right?
                    // it would be better to return it as a var. Why were we unwrapping it here?
                    // maybe we just thought that a func should return a primitive? that's not right
                    //newfuncarg1 = ExCasts.UnwrapVariable(newfuncarg1);
                    
                    if(newfuncarg1.Type != typeof(string))
                        // NOTE: disabling this case... this case would handle dynamic compilation where we 
                        // would allow a string to be constructed at runtime and then compiled, like
                        // "f:newfunc(f:test, "a:0 +" + "51")" 
                        // but this makes compilation fail for string result functions
                        // later we can reintroduce this with a special function name like f:newdynamicfunc
                        // that will expect this behavior 
                        //&& !(newfuncarg1 is UnaryExpression && (newfuncarg1 as UnaryExpression).Operand.Type == typeof(string)))
                    {
                        // TODO: remove this comments...
                        // left off here
                        // debating just doing a preproc to transform this arg into a string
                        // for this specific function so that we don't need to handle this case
                        // some downsides though:
                        // 1. may violate expectations?
                        // 2. will be much less efficient than if I can get this working
                        // but getting this working means
                        // 1. line needs to store its paramexpressions so they can be ref'd by other lines
                        // 2. need a way to pass this line thru to the method
                        // 3. (hardest) need a way to pass the Expression itself as an arg to the method...

                        // could I:
                        // 1. take the expression block HERE, outside of the exp. tree
                        // 2. compile it witohut using expression.call, so new funcs are compiled
                        //    at compile time
                        // 3. call a method and pass the resulting exline.Compiled lambda into it
                        // 4. that method just replaces the func's compiled with ^ this one
                        // would that actually violate anything?

                        CompileContext funcCompile = new CompileContext(newfuncarg1,
                            (argParams as UnaryExpression).Operand as ParameterExpression, // this is because argparams 
                            // gets wrapped in a convert (object[]) , so we need to unwrap it here...
                            inputParams, contextParams, compiledInputVarsList, compiledContexts.ToArray(), scopeContext);
                        return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("NewFuncCompiled"),
                            GetArgAsString(GetArgIndex(argexp, 0)),
                            Expression.Constant(funcCompile),
                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                    }
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("NewFunc"),
                        GetArgAsString(GetArgIndex(argexp, 0)),
                        GetArgAsString(GetArgIndex(argexp, 1)),
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                case "f:setfunc":
                    Expression setfuncarg1 = GetArgIndex(argexp, 1);
                    setfuncarg1 = ExCasts.UnwrapVariable(setfuncarg1);
                    if (setfuncarg1.Type != typeof(string))
                        // see above note for f:newfunc
                        //&& !(setfuncarg1 is UnaryExpression && (setfuncarg1 as UnaryExpression).Operand.Type == typeof(string)))
                    {
                        CompileContext funcCompile = new CompileContext(setfuncarg1,
                            (argParams as UnaryExpression).Operand as ParameterExpression, // this is because argparams 
                                                                                           // gets wrapped in a convert (object[]) , so we need to unwrap it here...
                            inputParams, contextParams, compiledInputVarsList, compiledContexts.ToArray(), scopeContext);
                        return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("SetFuncCompiled"),
                            GetArgAsString(GetArgIndex(argexp, 0)),
                            Expression.Constant(funcCompile),
                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                    }
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("SetFunc"),
                        GetArgAsString(GetArgIndex(argexp, 0)),
                        GetArgAsString(GetArgIndex(argexp, 1)),
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                case "f:newvar":
                    // change: if possible, define at compiletime so we can store vars at compiletime
                    {
                        // remove this...
                        /*Expression arg0 = (argexp as NewArrayExpression).Expressions[0];
                        if (arg0 is UnaryExpression)
                        {
                            // can only optimize here if the varname is constant
                            Expression arg0op = (arg0 as UnaryExpression).Operand;
                            if(arg0op is BinaryExpression)
                            {
                                InputVar argvar = compiledInputVarsList[(int)((arg0op as BinaryExpression).Right as ConstantExpression).Value];
                                compiledContexts[scopeContext].InsertVar(argvar);
                            }
                        }*/
                        return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("NewVar"),
                            GetArgAsInputVarName(GetArgIndex(argexp, 0)),
                            GetArgAsObject(GetArgIndex(argexp, 1)),
                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                    }
                case "f:setvar":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("SetVar"),
                        GetArgAsInputVar(GetArgIndex(argexp, 0), Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext))),
                        GetArgAsObject(GetArgIndex(argexp, 1)));
                case "f:getvar":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("LookupVar"),
                        GetArgAsString(argexp),
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)));
                case "f:sin":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("Sin"),
                        GetArgAsDouble(argexp));
                case "f:random":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("RandomDouble"),
                        GetArgAsDouble(GetArgIndex(argexp, 0)),
                        GetArgAsDouble(GetArgIndex(argexp, 1)),
                        GetRandomArg(Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext))));
                case "f:randomint":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("RandomInt"),
                        GetArgAsInt(GetArgIndex(argexp, 0)),
                        GetArgAsInt(GetArgIndex(argexp, 1)),
                        GetRandomArg(Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext))));
                case "f:roll":
                    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("Roll"),
                        GetArgAsInt(GetArgIndex(argexp, 0)),
                        GetArgAsInt(GetArgIndex(argexp, 1)),
                        GetRandomArg(Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext))));
                default:
                    // we didn't recognize the function, we should check if it's part of the 
                    // custom defined functions dictionary
                    Expression argexparray = ExCasts.WrapArguments(argexp);
                    return Expression.Call(GetFuncExpression(
                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)), funcexp),
                        typeof(Exline).GetRuntimeMethod("Execute", new[] { typeof(object[]) }),
                        argexparray);

                    // no longer doing precompile function routing due to elimination of placeholders for scoping
                    // may achieve performance gains in the future by reinstation placeholders to allow compiler
                    // to find pointer to function ahead of runtime
                    // TODO
                    /*Exline exl = context.GetFunc(funcname.ToLower());
                    if (exl != null)
                    {
                        //Expression argexparray = Expression.NewArrayInit(typeof(object), Expression.Convert(argexp, typeof(object)));
                        //Expression argexparray = argexp;
                        //if (!(argexp is NewArrayExpression))
                        //    argexparray = Expression.NewArrayInit(typeof(object), Expression.Convert(argexp, typeof(object)));
                        //else
                        //argexparray = Expression.NewArrayInit(typeof(object), argexp);
                        //argexparray = Expression.Convert(argexp, typeof(object[]));
                        //Expression argexparray = Expression.Convert(argexp, typeof(object));
                        Expression argexparray = ExCasts.WrapArguments(argexp);
                        return Expression.Call(GetFuncExpression(Expression.Constant(context), funcexp),
                            exl.GetType().GetRuntimeMethod("Execute", new[] { typeof(object[]) }),
                            argexparray);
                    }
                    return Expression.Constant(0);*/
            }
        }

        public static Expression GetFuncExpression(Expression inputContext, Expression name)
        {
            return Expression.Call(inputContext, typeof(InputContext).GetRuntimeMethod("GetFunc",
                new[] { typeof(string), typeof(bool) }),
                name, Expression.Constant(true));
        }

        public static Expression GetArgIndex(Expression argexp, int index)
        {
            // Note: depreciating this because it is more efficient to unbox than to access
            // if we access, ALL the indices get evaluated before the desired one is pulled out
            // (this can cause defects where vars get set more times than desired...)
            // if we unbox, ONLY the indice we want is taken and evaluated.

            // TODO: remove calls to GetArgIndex instead of doing this kludge override
            return GetArgIndexUnbox(argexp, index);

            //return Expression.ArrayAccess(ExCasts.UnwrapVariable(argexp), Expression.Constant(index));
        }

        public static Expression GetArgIndexUnbox(Expression argexp, int index)
        {
            NewArrayExpression arx = argexp as NewArrayExpression;
            return arx.Expressions[index];
        }

        public static Expression GetArgAsDouble(Expression argexp)
        {
            // see below comment on GetArgAsInt
            return ExCasts.GetExpressionObjectAsDouble(ExCasts.UnwrapVariable(argexp));
            //return Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(double));
        }

        public static Expression GetArgsAsDouble(Expression args)
        {
            return Expression.Call(
                typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("GetObjectsAsDoubleUnwrapping"),
                ExCasts.WrapArguments(args));
        }

        public static Expression GetArgAsInt(Expression argexp)
        {
            // so: going through convert directly means that auto-casting like double -> int 
            //     is not done for us. If we use this instead, we can have that luxury
            // additionally, I want to move the logic for this out of exfuncs and into excasts
            // as that locale is more appropriate
            return ExCasts.GetExpressionObjectAsInt(ExCasts.UnwrapVariable(argexp));
            //return Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(int));
        }

        public static Expression GetArgAsString(Expression argexp)
        {
            return Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(string));
        }

        public static Expression GetArgAsInputVar(Expression argexp, Expression inputContextParam)
        {
            return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("GetInputVar"), argexp, inputContextParam);
            // special case: if a string is passed in, look up that var dynamically
            //if(argexp.Type == typeof(string))
            //    return Expression.Call(typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("LookupVar"),
            //        GetArgAsString(argexp),
            //        inputContextParam);
            // otherwise assume it's an inputvar type already
            //return Expression.Convert(argexp, typeof(InputVar));
        }

        public static Expression GetArgAsInputVarName(Expression argexp)
        {
            // if it's an inputvar, extract the name field
            // if it's not, assume it's a string
            return Expression.Call(typeof(ExCasts).GetTypeInfo().GetDeclaredMethod("GetInputVarName"), argexp);
            //return Expression.IfThenElse(Expression.IsTrue(Expression.TypeIs(argexp, typeof(InputVar))),
            //    Expression.Field(Expression.Convert(argexp, typeof(InputVar)), "Name"),
            //    Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(string)));
            //if(argexp.Type == typeof(InputVar))
            //    return Expression.Field(Expression.Convert(argexp, typeof(InputVar)), "Name");
            // otherwise assume it's a string
            //return Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(string));
        }

        public static Expression GetArgAsObject(Expression argexp)
        {
            return Expression.Convert(ExCasts.UnwrapVariable(argexp), typeof(object));
        }

        public static Expression GetArgAsBool(Expression argexp)
        {
            return ExCasts.GetCastExpression(Expression.Constant("c:b"), ExCasts.UnwrapVariable(argexp));
        }

        public static Expression GetRandomArg(Expression inputContextParam)
        {
            return Expression.Field(inputContextParam, "Random");
        }


        public static object NewFunc(string funcname, string line, InputContext context)
        {
            context.DefineFunc(funcname, line);
            return 0;
        }

        public static object SetFunc(string funcname, string line, InputContext context)
        {
            context.SetFunc(funcname, line);
            return 0;
        }

        public static object NewFuncCompiled(string funcname, CompileContext precompile, InputContext context)
        {
            context.DefineFuncCompiled(funcname, precompile);
            return 0;
        }

        public static object SetFuncCompiled(string funcname, CompileContext precompile, InputContext context)
        {
            context.SetFuncCompiled(funcname, precompile);
            return 0;
        }

        public static InputVar NewVar(string varname, object value, InputContext context)
        {
            return context.DefineAndReturnVar(varname, value);
        }

        public static InputVar SetVar(InputVar var, object value)
        {
            var.Value = value;
            return var;
        }

        public static InputVar LookupVar(string varname, InputContext context)
        {
            InputVar var = context.GetVar(varname);
            //if (var == null)
            //    context.DefineVar(varname, "UNDEFINED");
            return var;
        }

        public static object AssignAtRuntime(InputContext context, object o, object v, CompileContext precompile)
        {
            if(o is InputVar)
            {
                (o as InputVar).Value = v;
                return o;
            }
            if(o is AccessorResult)
            {
                AccessorResult a = o as AccessorResult;
                if(a.IsVar)
                {
                    a.Var.Value = v;
                    return a.Var.Value;
                }
                if (a.IsFunc)
                {
                    // this is why we had to pass in the compile context
                    if(v is string)
                    {
                        // setting func equal to string decl
                        a.Func.ChangeLine(v as string);
                        context.CompileLine(a.Func);
                    }
                    else
                    {
                        // setting func equal to compile context
                        a.Func.ChangeLine("COMPILED");
                        a.Func.Compile(precompile);
                    }
                    return 0;
                }
                if (a.IsObject)
                {
                    throw new Exception("at-runtime object assignment not supported yet.");
                }
            }
            if(o is string)
            {
                string s = o as string;
                if(s[0] == 'v')
                {
                    return context.SetAndReturnVar(s, v);
                }
                else if(s[0] == 'f')
                {
                    if(v is string)
                    {
                        context.SetFunc(s, v as string);
                        return 0;
                    }
                    else
                    {
                        context.SetFuncCompiled(s, precompile);
                        return 0;
                    }
                }
                else if(s[0] == 'o')
                {
                    throw new Exception("at-runtime object assignment not supported yet.");
                }
                throw new Exception("Unrecognized at-runtime assigment: '" + s + "'.");
            }
            throw new Exception("Attempted to assign to unsupported type '" + o.GetType().ToString() + "':'" + o.ToString() + "'.");
        }

        public static object FreeAtRuntime(InputContext context, object o)
        {
            // free an object
            if(o is string)
            {
                // if o is a string, we further need to see if it's a v: or an o:
                string s = o as string;
                if (s.StartsWith("v"))
                {
                    context.FreeVar(s);
                }
                else if (s.StartsWith("o"))
                {
                    context.FreeObject(s);
                }
                else
                {
                    throw new Exception("Could not free '" + s + "' because it was an unsupported type.");
                }
            }
            else if(o is InputVar)
            {
                InputVar v = o as InputVar;
                context.FreeVar(v.Name);
            }
            else if(o is InputObject)
            {
                InputObject ob = o as InputObject;
                context.FreeObject(ob.Name);
            }
            else
            {
                throw new Exception("Could not free '" + o.ToString() + "' because it was an unsupported type.");
            }
            return 0;
        }

        public static double Sin(double i)
        {
            return Math.Sin(i);
        }

        public static double RandomDouble(double min, double max, IRandomProvider random)
        {
            return random.RandomDouble(min, max);
        }

        public static int RandomInt(int min, int max, IRandomProvider random)
        {
            return random.RandomInt(min, max);
        }

        public static int Roll(int dice, int sides, IRandomProvider random)
        {
            int sum = 0;
            for (int i = 0; i < dice; i++)
                sum += random.RandomInt(1, sides);
            return sum;
        }
    }
}
