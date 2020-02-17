using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace RelaScript
{
    public class CompileContext
    {
        public string LastDebugView = string.Empty;
        public InputVar[] CompiledInputVars = null;
        public Func<object[], InputVar[], InputContext[], object> CompiledExpression = null;
        public InputContext[] CompiledContexts = null;
        public int CompiledContext = 0;

        public CompileContext(Expression exp,
            ParameterExpression argParams, ParameterExpression inputParams, ParameterExpression contextParams,
            List<InputVar> compiledInputVarsList,
            InputContext[] compiledContexts,
            int compiledContext)
        {
            Expression total = Expression.Convert(exp, typeof(object));

            // compile the expression
            LastDebugView = total.ToString();
            CompiledInputVars = compiledInputVarsList.ToArray();
            CompiledExpression = Expression.Lambda<Func<object[], InputVar[], InputContext[], object>>
                (total, argParams, inputParams, contextParams)
                .Compile();
            CompiledContexts = compiledContexts;
            CompiledContext = compiledContext;
        }
    }
}
