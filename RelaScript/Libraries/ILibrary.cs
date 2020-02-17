using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RelaScript
{
    public interface ILibrary
    {
        string GetLibraryName();

        Expression GetFunctionExpression(string funcname, Expression argexp,
            Expression argParams, ParameterExpression inputParams, Expression inputContextParam,
            List<InputVar> compiledInputVarsList);

        // DEPRECIATED
        // this is no longer needed because libraries have been repurposed so that they 
        // are always expanded at compiletime rather than at runtime
        // object ExecuteFunction(string funcname, object[] args, InputContext context);

        List<string> GetDefaultFunctions();
    }
}
