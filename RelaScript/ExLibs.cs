using RelaScript.Intermediates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    public static class ExLibs
    {
        /*public static Expression GetFunctionExpression(Expression funcexp, Expression argexp,
            Dictionary<string, Exline> customFuncs,
            Expression argParams, ParameterExpression inputParams, ParameterExpression inputContextParam,
            List<InputVar> compiledInputVarsList,
            InputContext context)
        {

        }*/

        public static void ImportLibrary(string libname, string asname, InputContext context)
        {
            foreach(ILibraryProvider libProvider in context.LibraryProviders)
            {
                ILibrary lib = libProvider.LoadLibrary(libname);
                if(lib != null)
                {
                    context.SetLibrary(asname, lib);
                    return; // found the library
                }
            }
            // did not find the library
            throw new Exception("Library not found '" + libname + "'");
        }

        private static ConstructorInfo LibraryFunctionIntermediateConstructor
        {
            get {
                if (LibraryFunctionIntermediateConstructor == null)
                    LibraryFunctionIntermediateConstructor = typeof(LibraryFunctionIntermediate).GetTypeInfo().DeclaredConstructors.ElementAt(0);
                return LibraryFunctionIntermediateConstructor;
            }
            set
            {
                LibraryFunctionIntermediateConstructor = value;
            }
        }

        public static Expression GetLibraryAccessorExpression(Expression left, Expression right)
        {
            if (right.Type == typeof(string) && right is ConstantExpression)
            {
                // case: constant string accessor
                string accstr = ((right as ConstantExpression).Value as string).ToLower();
                if (accstr.StartsWith("f"))
                {
                    // case: function accessor
                    // this will become a LibraryFunction call
                    if (left is ConstantExpression)
                    {
                        // lefthand side is just a constant library, we can optimize this by taking the
                        // library value out directly
                        return Expression.Constant(new LibraryFunctionIntermediate(
                            (left as ConstantExpression).Value as ILibrary,
                            accstr));
                    }
                    else
                    {
                        // if lefthand is not constant, we need to render the intermediate dynamically
                        throw new Exception("Attempted to formulate library dynamically, which is not allowed.");
                        return Expression.New(LibraryFunctionIntermediateConstructor, 
                            new Expression[] { left, Expression.Constant(right) });
                    }
                }
            }
            throw new Exception("Library Accessor not recognized.");
        }
    }
}
