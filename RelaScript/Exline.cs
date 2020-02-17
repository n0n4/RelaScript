using RelaScript.Intermediates;
using RelaScript.Objects;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace RelaScript
{
    /// <summary>
    /// An expression line, compiles from string format down to a Func.
    /// </summary>
    public class Exline
    {
        /// <summary>
        /// Used if this is a func in the inputcontext. Otherwise, null.
        /// </summary>
        public string FuncName { get; private set; }

        /// <summary>
        /// The original string this Exline is built from.
        /// </summary>
        public string Line { get; private set; }

        /// <summary>
        /// Post Compile, the executable function.
        /// In: args, vars
        /// Out: results
        /// </summary>
        private Func<object[], InputVar[], InputContext[], object> CompiledExpression = null;
        /// <summary>
        /// At end of compile, CompiledInputVarsList is converted into an array and stored here 
        /// for use in Execute ( so we don't need to do the cost of .toarray each time...)
        /// </summary>
        private InputVar[] CompiledInputVars = null;
        /// <summary>
        /// Only used during compile, a list of all the vars referenced by this exline.
        /// </summary>
        private List<InputVar> CompiledInputVarsList = null;
        /// <summary>
        /// Which context was used to compile this line?
        /// </summary>
        private InputContext CompiledContext = null;
        // TODO: why pass the context in as an arg each time if it never changes?
        // to improve performance, we should fix the context and stop treating it as a param
        // but instead as a constant
        private List<InputContext> CompiledContextsList = null;
        private InputContext[] CompiledContexts = null;

        public bool DebugMode = false;
        public List<string> CompileLog = null;
        public string LastDebugView = string.Empty;

        public Exline(string line, string funcname = null)
        {
            Line = line;
            FuncName = funcname;
        }

        public void ChangeLine(string line)
        {
            Line = line;
            CompiledInputVars = null;
            CompiledInputVarsList = null;
            CompiledContext = null;
            CompiledExpression = null;
            CompileLog = null;
        }

        public void EnableDebugMode()
        {
            DebugMode = true;
            CompileLog = new List<string>();
        }

        private void Log(string s)
        {
            CompileLog.Add(s);
        }

        private enum ETermType
        {
            VAR,
            ARG,
            PARENS,
            BRACES,
            NUMBER,
            FUNC,
            CAST,
            STRING,
            BOOL,
            LIBRARY,
            LIBRARYFUNC,
            CLASS,
            OBJECT
        }

        public void Compile(InputContext context)
        {
            CompiledInputVarsList = new List<InputVar>();

            // an important note:
            // we need to assemble ONE list of args / inputs for the whole line
            // so every compiled portion needs to be using the same running list of these
            ParameterExpression argParams = Expression.Parameter(typeof(object[]));
            Expression argParamsUnravelled = Expression.Convert(argParams, typeof(object[]));
            ParameterExpression inputParams = Expression.Parameter(typeof(InputVar[]));
            ParameterExpression contextParams = Expression.Parameter(typeof(InputContext[]));
            Dictionary<string, int> varNameToIndex = new Dictionary<string, int>();
            CompiledContext = context;
            CompiledContextsList = new List<InputContext>();
            CompiledContextsList.Add(context);

            // do preprocessing on the line
            string cleanedLine = CleanLine(Line);

            Expression total = LinesCompile(cleanedLine, argParamsUnravelled, inputParams, contextParams, 
                varNameToIndex, 0, false);
            total = Expression.Convert(total, typeof(object));
            //total = Expression.NewArrayInit(typeof(object[]), 
            //    Expression.IfThenElse(Expression.TypeIs())

            // compile the expression
            LastDebugView = total.ToString();
            CompiledInputVars = CompiledInputVarsList.ToArray();
            CompiledContexts = CompiledContextsList.ToArray();
            CompiledExpression = Expression.Lambda<Func<object[], InputVar[], InputContext[], object>>
                (total, argParams, inputParams, contextParams)
                .Compile();
        }

        public void Compile(CompileContext precompile)
        {
            LastDebugView = precompile.LastDebugView;
            CompiledInputVars = precompile.CompiledInputVars;
            CompiledExpression = precompile.CompiledExpression;
            CompiledContexts = precompile.CompiledContexts;
            CompiledContext = CompiledContexts[precompile.CompiledContext];
        }

        public void Compile(Exline copy, InputContext newContext)
        {
            LastDebugView = copy.LastDebugView;
            CompiledExpression = copy.CompiledExpression;

            // recreate the context tree
            CompiledContexts = copy.CopyContextTree(newContext);

            // recreat the input vars list
            CompiledInputVars = copy.CopyInputVars(CompiledContexts);


            CompiledContext = newContext;
        }

        public InputVar[] CopyInputVars(InputContext[] newContextTree)
        {
            InputVar[] vars = new InputVar[CompiledInputVars.Length];
            for(int i = 0; i < CompiledInputVars.Length; i++)
            {
                vars[i] = newContextTree[CompiledInputVars[i].ScopeId].GetVar(CompiledInputVars[i].Name);
            }
            return vars;
        }

        public InputContext[] CopyContextTree(InputContext newContext)
        {
            List<InputContext> cs = new List<InputContext>();
            cs.Add(newContext);

            // note: iterating forward ensures that each parent scope is added before its children
            for(int i = 1; i < CompiledContexts.Length; i++)
            {
                InputContext parent = cs[CompiledContexts[i].ParentScope.ScopeId];
                InputContext nc = new InputContext(parent, i);
                cs.Add(nc);
            }

            return cs.ToArray();
        }

        private static char[] WhileChars = new char[] { 'w', 'h', 'i', 'l', 'e' };
        private static char[] IfChars = new char[] { 'i', 'f' };
        private static char[] ElseChars = new char[] { 'e', 'l', 's', 'e' };
        private static char[] ReturnChars = new char[] { 'r', 'e', 't', 'u', 'r', 'n' };
        private static char[] ClassChars = new char[] { 'd', 'e', 'f', 'n' };
        private static char[] ObjectChars = new char[] { 'o', 'b', 'j', 'e', 'c', 't' };
        private static char[] ImportChars = new char[] { 'i', 'm', 'p', 'o', 'r', 't' };
        private static char[] FreeChars = new char[] { 'f', 'r', 'e', 'e' };

        private string CleanLine(string text)
        {
            StringBuilder sb = new StringBuilder();
            bool inquote = false;
            bool singlequote = false;
            bool incomment = false;
            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                char nextchar = ' ';
                char prevchar = ' ';
                if (i + 1 < text.Length)
                    nextchar = text[i + 1];
                if (i - 1 >= 0)
                    prevchar = text[i - 1];

                bool safechecknext(char[] cs)
                {
                    for(int o = 0; o < cs.Length; o++)
                    {
                        if (i + o >= text.Length)
                            return false;
                        if (text[i + o] != cs[o])
                            return false;
                    }
                    return true;
                }

                if (!inquote)
                {
                    if (c == '\r' || c == '\n')
                    {
                        sb.Append(' '); // add a space
                        incomment = false;
                        continue;
                    }
                    else if (incomment)
                        continue;
                    if(c == '\t')
                        continue;
                    // handle comment behavior
                    if(c == '/' && nextchar == '/')
                    {
                        sb.Append(' '); // add a space
                        incomment = true;
                        continue;
                    }
                    // mask the fact that [] behavior is kludgey by converting [1] to [(1) 
                    // [ is an operator
                    else if (c == '[')
                    {
                        sb.Append(c).Append('(');
                    }
                    else if(c == ']')
                    {
                        sb.Append(')');
                    }
                    // handle quotes (don't clean inside of quotes)
                    else if(c == '"')
                    {
                        sb.Append('"');
                        inquote = true;
                        singlequote = false;
                    }
                    else if(c == '\'')
                    {
                        sb.Append('\'');
                        inquote = true;
                        singlequote = true;
                    }
                    // handle operator reduction
                    // operators like ==, >=, && reduce to single character equivalents
                    else if (c == '=' && nextchar == '=')
                    {
                        sb.Append('˭');
                        i++;
                    }
                    else if(c == '>' && nextchar == '=')
                    {
                        sb.Append('≥');
                        i++;
                    }
                    else if(c == '<' && nextchar == '=')
                    {
                        sb.Append('≤');
                        i++;
                    }
                    else if(c == '|' && nextchar == '|')
                    {
                        sb.Append('‖');
                        i++;
                    }
                    else if(c == '&' && nextchar == '&')
                    {
                        sb.Append('Ѯ');
                        i++;
                    }
                    else if(c == '!' && nextchar == '=')
                    {
                        sb.Append('≠');
                        i++;
                    }
                    else if (c == ':' && nextchar == '=')
                    {
                        sb.Append('ⱻ');
                        i++;
                    }
                    else if (c == '+' && nextchar == '=')
                    {
                        sb.Append('ꝑ');
                        i++;
                    }
                    else if (c == '-' && nextchar == '=')
                    {
                        sb.Append('₥');
                        i++;
                    }
                    else if (c == '/' && nextchar == '=')
                    {
                        sb.Append('÷');
                        i++;
                    }
                    else if (c == '*' && nextchar == '=')
                    {
                        sb.Append('×');
                        i++;
                    }
                    else if (safechecknext(WhileChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('○');
                        i += 4;
                    }
                    else if (safechecknext(IfChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('ᴥ');
                        i++;
                    }
                    else if (safechecknext(ElseChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('є');
                        i += 3;
                    }
                    else if (safechecknext(ReturnChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('Ꞧ');
                        i += 5;
                    }
                    else if (safechecknext(ClassChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('¶');
                        i += 3;
                    }
                    else if (safechecknext(ObjectChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('⌂');
                        i += 5;
                    }
                    else if (safechecknext(ImportChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('ꜛ');
                        i += 5;
                    }
                    else if (safechecknext(FreeChars) && prevchar != ':' && !char.IsLetter(prevchar))
                    {
                        sb.Append('□');
                        i += 3;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    // don't do any cleaning on quotes
                    sb.Append(c);
                    if((c == '"' && !singlequote) || (c == '\'' && singlequote))
                    {
                        inquote = false;
                        singlequote = false;
                    }
                }
            }
            return sb.ToString();
        }

        private Expression LinesCompile(string text,
            Expression argParams, ParameterExpression inputParams, ParameterExpression contextParams,
            Dictionary<string, int> varNameToIndex,
            int scopeContext,
            bool returnSingle)
        {
            // split the line by semi-colons
            // note that we need to ignore semi-colons in parens and quotes
            List<Expression> lines = new List<Expression>();
            int linestart = 0;
            int linecount = 0;
            bool inquote = false;
            bool singlequote = false;
            bool inparens = false;
            int extraparens = 0;
            bool inbraces = false;
            int extrabraces = 0;

            for(int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                linecount++;
                if (inbraces)
                {
                    if (c == '{')
                    {
                        extrabraces++;
                    }
                    else if (c == '}')
                    {
                        if (extrabraces > 0)
                            extrabraces--;
                        else
                            inbraces = false;
                    }
                }
                else if (inparens)
                {
                    if(c == '(')
                    {
                        extraparens++;
                    }
                    else if(c == ')')
                    {
                        if (extraparens > 0)
                            extraparens--;
                        else
                            inparens = false;
                    }
                }
                else if (inquote)
                {
                    if((c == '"' && !singlequote) || (c=='\'' && singlequote))
                        inquote = false;
                }
                else
                {
                    if(c == '"')
                    {
                        inquote = true;
                        singlequote = false;
                    }
                    else if(c == '\'')
                    {
                        inquote = true;
                        singlequote = true;
                    }
                    else if(c == '(')
                    {
                        inparens = true;
                    }
                    else if (c == '{')
                    {
                        inbraces = true;
                    }
                    else if(c == ';')
                    {
                        if (linecount - 1 <= 0)
                            lines.Add(Expression.Constant(0));
                        else
                        {
                            string linetext = text.Substring(linestart, linecount - 1);
                            lines.Add(ExpressionCompile(linetext, 
                                argParams, inputParams, contextParams,
                                varNameToIndex, scopeContext, returnSingle,
                                out int remainder));
                            // what's happening here:
                            // when a line is compiled, sometimes it may have a 'remainder'
                            // this occurs if a semi-colon is elided by the user
                            // e.g. 3 5 --> 3; 5
                            // the line returns 'remainder', which is how much of the line
                            // it couldn't finish. We compile this remainder separately 
                            // until no more remainders are returned, capturing each 
                            // missing semi-colon group as a new line
                            while(remainder < linetext.Length)
                            {
                                linetext = linetext.Substring(remainder);
                                lines.Add(ExpressionCompile(linetext, 
                                    argParams, inputParams, contextParams,
                                    varNameToIndex, scopeContext, returnSingle,
                                    out remainder));
                            }
                        }
                        linestart = i + 1;
                        linecount = 0;
                    }
                }
            }
            // add the last line, if it didn't end with a ;
            if(linestart < text.Length)
            {
                if (linecount <= 0)
                    lines.Add(Expression.Constant(0));
                else
                {
                    string linetext = text.Substring(linestart, linecount);
                    lines.Add(ExpressionCompile(linetext,
                        argParams, inputParams, contextParams,
                        varNameToIndex, scopeContext, returnSingle,
                        out int remainder));
                    // see above comment on remainder
                    while (remainder < linetext.Length)
                    {
                        linetext = linetext.Substring(remainder);
                        lines.Add(ExpressionCompile(linetext,
                            argParams, inputParams, contextParams,
                            varNameToIndex, scopeContext, returnSingle,
                            out remainder));
                    }
                }
            }

            if (lines.Count <= 0)
                return Expression.Constant(0);
            else if(lines.Count == 1)
                return lines[0];

            return Expression.Block(lines.ToArray());
        }

        private Expression ExpressionCompile(
            string text,
            Expression argParams, ParameterExpression inputParams, ParameterExpression contextParams,
            Dictionary<string, int> varNameToIndex,
            int scopeContext,
            bool returnSingle,
            out int remainder)
        {
            remainder = text.Length;

            // begin pulling apart the Line
            List<List<ETermType>> termtype = new List<List<ETermType>>();
            List<List<string>> terms = new List<List<string>>();
            List<List<char>> ops = new List<List<char>>();

            // each line has at least one output, so add one list here to everything
            termtype.Add(new List<ETermType>());
            terms.Add(new List<string>());
            ops.Add(new List<char>());

            int termStart = 0;
            int termCount = 0;
            bool opExpected = false;
            bool inVar = false;
            bool inArg = false;
            bool inNumber = false;
            bool inParens = false;
            int extraParens = 0;
            bool inBraces = false;
            int extraBraces = 0;
            bool inParensOrBracesQuote = false;
            bool inParensOrBracesSingleQuote = false;
            bool inFunc = false;
            bool inCast = false;
            bool inString = false;
            bool inStringSingle = false; // single quote?
            bool inBool = false;
            bool inLibrary = false;
            bool inClass = false;
            bool inObject = false;

            bool twoTermsExpected = false;
            bool threeTermsExpected = false;

            bool forceNextString = false;
            bool forceNextTwoString = false;

            ETermType getTermType()
            {
                if (forceNextString)
                {
                    forceNextString = false;
                    return ETermType.STRING;
                }
                if (forceNextTwoString)
                {
                    forceNextString = true;
                    forceNextTwoString = false;
                    return ETermType.STRING;
                }
                if (inVar)
                    return ETermType.VAR;
                if (inArg)
                    return ETermType.ARG;
                if (inParens)
                    return ETermType.PARENS;
                if (inBraces)
                    return ETermType.BRACES;
                if (inFunc)
                    return ETermType.STRING; // only gets upgraded to FUNC by special logic in finisher
                if (inCast)
                    return ETermType.CAST;
                if (inString)
                    return ETermType.STRING;
                if (inBool)
                    return ETermType.BOOL;
                if (inLibrary)
                    return ETermType.LIBRARY;
                if (inClass)
                    return ETermType.CLASS;
                if (inObject)
                    return ETermType.OBJECT;
                return ETermType.NUMBER;
            }

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                bool readNext(string test)
                {
                    for(int o = 0; o < test.Length; o++)
                    {
                        if (i + o >= text.Length)
                            return false;
                        if (text[i + o] != test[o])
                            return false;
                    }
                    // special test: make sure the next char is not a letter or number
                    if (i + test.Length < text.Length && (text[i + test.Length].IsNumber() || char.IsLetter(text[i + test.Length])))
                        return false;
                    return true;
                }

                if (!inVar && !inNumber && !inArg && !inParens && !inBraces && !inFunc && !inCast && !inString && !inBool
                    && !inLibrary && !inClass && !inObject)
                {
                    // we have nothing currently
                    if (opExpected || c == '○' || c == 'ᴥ' || c == 'Ꞧ' || c == '¶' || c == '⌂' || c == 'ꜛ' || c == '□' || c == '!') // some operators are allowed to come first
                    {
                        // looking for an operator
                        if(terms.Count > 0 && opExpected && (c == '○' || c == 'ᴥ' || c == 'Ꞧ' || c == '¶' || c == '⌂' || c == 'ꜛ' || c == '□' || c == '!'))
                        {
                            // special case:
                            // though these operators are allowed to come first, 
                            // they should not come after a term without an operator between them
                            // in that case, a semicolon has been elided and we should render it
                            remainder = i;
                            break;
                        }
                        else if (c == ',')
                        {
                            // special behavior, we begin the next output
                            opExpected = false;
                            termtype.Add(new List<ETermType>());
                            terms.Add(new List<string>());
                            ops.Add(new List<char>());
                        }
                        else if (c.IsOperator())
                        {
                            ops[ops.Count - 1].Add(c);
                            opExpected = false;
                            if (c == '○' || c == 'ᴥ' || c == '¶' || c == 'ꜛ') // some operators expect two input terms
                                twoTermsExpected = true;
                            if (c == '⌂')
                                threeTermsExpected = true;
                            if (c == 'ꜛ')
                                forceNextTwoString = true;
                        }
                        else if(c != ' ')
                        {
                            // we expected an operator, but none appeared
                            // this means this next term should be part of the next semicolon group
                            remainder = i;
                            break; // end searching the text, since the rest will be unused...
                        }
                    }
                    else if (readNext("true") || readNext("false"))
                    {
                        inBool = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (readNext("anon"))
                    {
                        inObject = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if(c == '"')
                    {
                        inString = true;
                        inStringSingle = false;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == '\'')
                    {
                        inString = true;
                        inStringSingle = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'v')
                    {
                        inVar = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'a')
                    {
                        inArg = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'b')
                    {
                        inBool = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'f')
                    {
                        inFunc = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'c')
                    {
                        inCast = true;
                        termStart = i;
                        termCount = 1;
                        ops[ops.Count - 1].Add('©');
                    }
                    else if(c == 'l')
                    {
                        inLibrary = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'd')
                    {
                        inClass = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == 'o')
                    {
                        inObject = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == '(')
                    {
                        inParens = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c == '{')
                    {
                        inBraces = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }
                    else if (c.IsNumber())
                    {
                        inNumber = true;
                        termStart = i;
                        termCount = 1;
                        opExpected = true;
                    }

                    if(opExpected && threeTermsExpected)
                    {
                        opExpected = false;
                        threeTermsExpected = false;
                        twoTermsExpected = true;
                    }
                    else if(opExpected && twoTermsExpected)
                    {
                        opExpected = false;
                        twoTermsExpected = false;
                        // by some magic, this should mean that the operator ends up BETWEEN
                        // the next two terms, so for instance while (cond) {do} becomes
                        // (cond) while {do}
                        // which is infinitely more convenient for us
                    }
                }
                else
                {
                    if(inString)
                    {
                        // string only ends if we hit another "
                        termCount++;
                        if((c == '"' && !inStringSingle) || (c == '\'' && inStringSingle))
                        {
                            termCount -= 2; // remove the beginning and ending quotes
                            if (termCount <= 0)
                                terms[terms.Count - 1].Add(string.Empty);
                            else
                                terms[terms.Count - 1].Add(text.Substring(termStart + 1, termCount));
                            termtype[terms.Count - 1].Add(getTermType());
                            inString = false;
                        }
                    }
                    else if (inBraces)
                    {
                        // braces only ends if we hit a }
                        termCount++;
                        if (inParensOrBracesQuote)
                        {
                            if (c == '"')
                                inParensOrBracesQuote = false;
                        }
                        else if (inParensOrBracesSingleQuote)
                        {
                            if (c == '\'')
                                inParensOrBracesSingleQuote = false;
                        }
                        else
                        {
                            if (c == '"')
                                inParensOrBracesQuote = true;
                            if (c == '\'')
                                inParensOrBracesSingleQuote = true;
                            if (c == '{')
                                extraBraces++;
                            if (c == '}')
                            {
                                if (extraBraces > 0)
                                {
                                    extraBraces--;
                                }
                                else
                                {
                                    terms[terms.Count - 1].Add(text.Substring(termStart, termCount));
                                    termtype[terms.Count - 1].Add(getTermType());
                                    inBraces = false;
                                }
                            }
                        }
                    }
                    else if (inParens)
                    {
                        // parens only ends if we hit a )
                        termCount++;
                        if (inParensOrBracesQuote)
                        {
                            if (c == '"')
                                inParensOrBracesQuote = false;
                        }
                        else if (inParensOrBracesSingleQuote)
                        {
                            if (c == '\'')
                                inParensOrBracesSingleQuote = false;
                        }
                        else
                        {
                            if (c == '"')
                                inParensOrBracesQuote = true;
                            if (c == '\'')
                                inParensOrBracesSingleQuote = true;
                            if (c == '(')
                                extraParens++;
                            if (c == ')')
                            {
                                if (extraParens > 0)
                                {
                                    extraParens--;
                                }
                                else
                                {
                                    terms[terms.Count - 1].Add(text.Substring(termStart, termCount));
                                    termtype[terms.Count - 1].Add(getTermType());
                                    inParens = false;
                                }
                            }
                        }
                    }
                    else if ((c.IsOperator() && (!inNumber || c != '.'))
                        || c == ' ' || c == ',' || (inFunc && c == '(') || (inCast && c == '('))
                    {
                        // end the term
                        terms[terms.Count - 1].Add(text.Substring(termStart, termCount));
                        termtype[terms.Count - 1].Add(getTermType());

                        // some special considerations here: if this term is a func, we need to alter
                        // its usage depending on context.
                        // if it is followed by a (, then we treat it as a func call
                        // if it isn't, then we treat it as a string
                        if (inFunc)
                        {
                            bool nextisparen = false;
                            for (int o = i; o < text.Length; o++)
                            {
                                if(text[o] == '(')
                                {
                                    nextisparen = true;
                                    break;
                                }
                                else if(text[o] != ' ')
                                {
                                    break;
                                }
                            }
                            if (nextisparen)
                            {
                                termtype[terms.Count - 1][termtype[terms.Count - 1].Count - 1] = ETermType.FUNC;
                                ops[ops.Count - 1].Add('←');
                                opExpected = false;
                            }
                        }

                        // special case: if we hit a parenthetical after a function, move the cursor back
                        // so we hit the ( again and capture the term
                        //if (c == ',' || (inFunc && c == '(') || (inCast && c == '('))
                        //    i--;

                        // rethinking: really if it's any non whitespace character we should scroll back, right?
                        if (c != ' ')
                            i--;
                        
                        inVar = false;
                        inArg = false;
                        inNumber = false;
                        inFunc = false;
                        inCast = false;
                        inBool = false;
                        inLibrary = false;
                        inClass = false;
                        inObject = false;
                    }
                    else
                    {
                        termCount++;
                    }
                }
            }
            // clean up last term
            if (inVar || inArg || inNumber || inParens || inBraces || inFunc || inCast || inBool || inLibrary || inClass || inObject)
            {
                // end the term
                terms[terms.Count - 1].Add(text.Substring(termStart, termCount));
                termtype[terms.Count - 1].Add(getTermType());
            }
            else if (inString)
            {
                termCount -= 1; // remove the beginning quote
                if (termCount <= 0)
                    terms[terms.Count - 1].Add(string.Empty);
                else
                    terms[terms.Count - 1].Add(text.Substring(termStart + 1, termCount));
                termtype[terms.Count - 1].Add(getTermType());
            }

            // now parse all the vars in order
            // nov19: no more placeholders for anything. these changes needed to allow
            //        classes and scopes
            /*for (int i = 0; i < terms.Count; i++)
            {
                for (int o = 0; o < terms[i].Count; o++)
                {
                    // deciding whether to find some convoluted way to still use placeholders
                    // or to just go straight to "always lookup at runtime"
                    // by replacing e.g. the compiled vars list and the 
                    // var term down there with just a call to compiledcontext . getvar
                    if (termtype[i][o] == ETermType.VAR)
                    {
                        // lookup this var
                        if (!varNameToIndex.ContainsKey(terms[i][o]))
                        {
                            if (!vars.ContainsKey(terms[i][o]))
                            {
                                // If the var doesn't exist, add a placeholder
                                vars.Add(terms[i][o], new InputVar(terms[i][o],"UNDEFINED"));
                            }
                            CompiledInputVarsList.Add(vars[terms[i][o]]);
                            varNameToIndex.Add(terms[i][o], varNameToIndex.Count);
                        }
                    }
                    else if(termtype[i][o] == ETermType.FUNC)
                    {
                        // lookup this func, if it doesn't exist add a placeholder
                        string lowerfunc = terms[i][o].ToLower();
                        if (!customFuncs.ContainsKey(lowerfunc) && !ExFuncs.DefaultFunctions.Contains(lowerfunc))
                        {
                            customFuncs.Add(lowerfunc, new Exline("'UNDEFINED'", lowerfunc));
                        }
                    }
                    else if(termtype[i][o] == ETermType.LIBRARY)
                    {
                        // lookup this library, if it doesn't exist yet, add a placeholder
                        string lowerlib = terms[i][o].ToLower();
                        CompiledContext.SetPlaceholderLibrary(lowerlib);
                    }
                }
            }*/

            // now construct the expression tree
            List<Expression> outputExpressions = new List<Expression>();

            Expression determineExpression(int output, int term)
            {
                switch (termtype[output][term])
                {
                    case ETermType.VAR:
                        // check if this var is already compiletime defined
                        // if it is, we should add it to the compiledlist (unless it's already present)
                        // and return that index instead
                        InputVar vtest = CompiledContextsList[scopeContext].GetVar(terms[output][term]);
                        if(vtest != null)
                        {
                            // found a match
                            int cindex = -1;
                            for(int i = 0; i < CompiledInputVarsList.Count; i++)
                            {
                                if(CompiledInputVarsList[i].Name == vtest.Name)
                                {
                                    cindex = i;
                                    break;
                                }
                            }
                            if (cindex == -1)
                            {
                                cindex = CompiledInputVarsList.Count;
                                CompiledInputVarsList.Add(vtest);
                            }
                            return Expression.ArrayIndex(inputParams, Expression.Constant(cindex));
                        }
                        return ExVars.GetVarExpression(
                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                            Expression.Constant(terms[output][term]));
                        //return Expression.ArrayIndex(inputParams, Expression.Constant(varNameToIndex[terms[output][term]]));
                    case ETermType.ARG:
                        string rest = terms[output][term].Substring(2);
                        if(rest[0].IsNumber()) // if arg is a number, access it directly (e.g. a:0)
                            return Expression.ArrayIndex(argParams, Expression.Constant(int.Parse(rest)));
                        // if it's not a number, access it via argmap (e.g. a:red)
                        return Expression.ArrayIndex(argParams, Expression.Call(
                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                            typeof(InputContext).GetRuntimeMethod("GetArg", new Type[] { typeof(string) }),
                            Expression.Constant(rest)
                            ));
                    case ETermType.NUMBER:
                        return Expression.Constant(double.Parse(terms[output][term]));
                    case ETermType.FUNC:
                        return Expression.Constant(terms[output][term]);
                    case ETermType.CAST:
                        return Expression.Constant(terms[output][term]);
                    case ETermType.PARENS:
                        string nextline = terms[output][term];
                        if (nextline.StartsWith("("))
                            nextline = nextline.Substring(1);
                        if (nextline.EndsWith(")"))
                            nextline = nextline.Substring(0, nextline.Length - 1);
                        // if the previous term is a function, return the full array of outputs
                        //if(term > 0 && termtype[output][term - 1] == ETermType.FUNC)
                        // change: always return the full array of outputs, and expect users to index appropriately
                        // special case: if nextline is empty, return a constant 0
                        if (nextline.Length <= 0 || nextline.Replace(" ", "").Length <= 0)
                            return Expression.Convert(Expression.Constant(0.0), typeof(object));
                            // see below comment on empty-scope
                            //return Expression.NewArrayInit(typeof(object),
                            //    Expression.Convert(Expression.Constant(0), typeof(object)));

                        return LinesCompile(nextline, argParams, inputParams, contextParams, varNameToIndex, scopeContext, false);
                    case ETermType.BRACES:
                        string nextlineb = terms[output][term];
                        if (nextlineb.StartsWith("{"))
                            nextlineb = nextlineb.Substring(1);
                        if (nextlineb.EndsWith("}"))
                            nextlineb = nextlineb.Substring(0, nextlineb.Length - 1);
                        // if the previous term is a function, return the full array of outputs
                        //if(term > 0 && termtype[output][term - 1] == ETermType.FUNC)
                        // change: always return the full array of outputs, and expect users to index appropriately
                        // special case: if nextline is empty, return a constant 0
                        if (nextlineb.Length <= 0 || nextlineb.Replace(" ", "").Length <= 0)
                        {
                            InputContext newemptyscope = new InputContext(CompiledContextsList[scopeContext], CompiledContextsList.Count);
                            CompiledContextsList.Add(newemptyscope);
                            return LinesCompile("0", argParams, inputParams, contextParams, varNameToIndex,
                                newemptyscope.ScopeId, false);
                            //return Expression.Convert(Expression.Constant(0.0), typeof(object));
                            // Note: used to return an array here instead of a flat 0. Think this may be vestigal as
                            // none of the tests broke after changing this (and this fixed the empty-scope test
                            // leaving this here in case I realize this was necessary for some reason.
                            //return Expression.NewArrayInit(typeof(object),
                            //    Expression.Convert(Expression.Constant(0), typeof(object)));
                        }

                        // create a new scope
                        InputContext newscope = new InputContext(CompiledContextsList[scopeContext], CompiledContextsList.Count);
                        CompiledContextsList.Add(newscope);
                        return LinesCompile(nextlineb, argParams, inputParams, contextParams, varNameToIndex, 
                            newscope.ScopeId, false);
                    case ETermType.STRING:
                        return Expression.Constant(terms[output][term]);
                    case ETermType.BOOL:
                        string booltext = terms[output][term].ToLower();
                        if (booltext == "true" || booltext == "b:t" || booltext == "b:true")
                            return Expression.Constant(true);
                        if (booltext == "false" || booltext == "b:f" || booltext == "b:false")
                            return Expression.Constant(false);
                        return Expression.Constant(false);
                    case ETermType.LIBRARY:
                        return Expression.Constant(CompiledContextsList[scopeContext].GetLibrary(terms[output][term]));
                    case ETermType.CLASS:
                        return Expression.Constant(terms[output][term]);
                    case ETermType.OBJECT:
                        return Expression.Constant(terms[output][term]);
                    default:
                        return Expression.Constant(0.0); // couldn't determine the type...
                }
            }

            /*Expression takeFirst(Expression inp)
            {
                // for instance, if the exp was a function, it will return double[]
                // but we may only want one double going forward
                //left off; // need to find a way to deal with distinguishing between double[] and double results
                // because a function might return double[], or you may want to pass double[] into a function
                // or even chain double[] into a function like funcw2outputs2input(funcw2outputs2input(5,1))
                return Expression.IfThenElse(Expression.TypeIs(inp, typeof(double[])),
                    Expression.ArrayIndex(inp, Expression.Constant(0)),
                    inp);
            }*/

            
            
            for (int i = 0; i < terms.Count; i++)
            {
                // sort the ops based on order of operations
                List<int> oporder = new List<int>();

                if(ops[i].Count > 0)
                    oporder.Add(0);

                for(int o = 1; o < ops[i].Count; o++)
                {
                    int opriority = ops[i][o].GetOperatorPriority();
                    // iterate over the oporder and insert this
                    bool found = false;
                    for(int p = 0; p < oporder.Count; p++)
                    {
                        int ppriority = ops[i][oporder[p]].GetOperatorPriority();
                        if(opriority < ppriority)
                        {
                            oporder.Insert(p, o);
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        oporder.Add(o);
                    }
                }

                List<Expression> exprs = new List<Expression>();
                for (int o = 0; o < terms[i].Count; o++)
                    exprs.Add(determineExpression(i, o));

                if (oporder.Count > 0)
                {
                    for (int o = 0; o < oporder.Count; o++)
                    {
                        if (ops[i][oporder[o]] == ' ')
                            continue; // if an op has been annihilated, don't try to run it

                        int getNextIndex(int startpos)
                        {
                            int nindex = -1;
                            for (int p = startpos + 1; p < terms[i].Count; p++)
                            {
                                if (exprs[p] != null)
                                {
                                    nindex = p;
                                    break;
                                }
                            }
                            if (nindex == -1)
                                throw new Exception("Not enough terms in lineterm '" + text + "' of line '" + Line + "'");
                            return nindex;
                        }

                        int getPrevIndex(int startpos)
                        {
                            int nindex = -1;
                            for (int p = startpos; p >= 0; p--)
                            {
                                if (exprs[p] != null)
                                {
                                    nindex = p;
                                    break;
                                }
                            }
                            if (nindex == -1)
                                throw new Exception("Not enough terms in lineterm '" + text + "' of line '" + Line + "'");
                            return nindex;
                        }

                        bool annihilateTerm = true;

                        // find the leftleast non-null and find the rightleast non-null
                        int leftindex = -1;
                        int rightindex = -1;
                        if (ops[i][oporder[o]] == 'Ꞧ' || ops[i][oporder[o]] == '!' || ops[i][oporder[o]] == '□')
                        {
                            // some operators only take one term
                            rightindex = getPrevIndex(oporder[o]);
                            annihilateTerm = false;
                        }
                        else
                        {
                            leftindex = getPrevIndex(oporder[o]);
                            rightindex = getNextIndex(oporder[o]);
                        }

                        // replace the left term with the combination 
                        switch (ops[i][oporder[o]])
                        {
                            // special operators
                            case '□': // free operator
                                {
                                    exprs[rightindex] = Expression.Call(
                                        typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("FreeAtRuntime"),
                                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                        exprs[rightindex]
                                        );
                                }
                                break;
                            case 'ꜛ': // import operator
                                {
                                    // import libname asname
                                    // import logic works differently from other systems
                                    // instead of happening at runtime, import commands are triggered at compiletime
                                    if (!(exprs[leftindex] is ConstantExpression))
                                        throw new Exception("Import must be passed static string name of library");
                                    if (!(exprs[rightindex] is ConstantExpression))
                                        throw new Exception("Import must be passed static string asname of library");
                                    ExLibs.ImportLibrary(
                                        (string)(exprs[leftindex] as ConstantExpression).Value,
                                        (string)(exprs[rightindex] as ConstantExpression).Value,
                                        CompiledContextsList[scopeContext]);
                                    exprs[leftindex] = Expression.Constant(0);
                                    /*exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:import"),
                                    Expression.NewArrayInit(typeof(object), new Expression[] {
                                        Expression.Convert(exprs[leftindex], typeof(object)),
                                        Expression.Convert(exprs[rightindex], typeof(object))}),
                                    CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);*/
                                }
                                break;
                            case '¶':
                                { // class operator
                                    // defn name {defn}
                                    // note: the block is here because otherwise the {defn} term will be lost
                                    //       and if it doesn't actually execute none of the class' definitions
                                    //       will be stored. (none of the function assignments, for instance)
                                    exprs[leftindex] = 
                                        Expression.Block(
                                            exprs[rightindex],
                                            Expression.Call(
                                                Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                                typeof(InputContext).GetRuntimeMethod("DefineClass", new Type[] { typeof(string), typeof(InputContext) }),
                                                exprs[leftindex],
                                                Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext + 1)))
                                        );
                                        // this is a bit of (a lot of) a hack: we assume THE NEXT CONTEXT SCOPE is
                                        // the definition context. however, this could fail in this specific case:
                                        // class {"classname"} {defn}
                                        // because the {"classname"} scope would be the next scope...
                                    // TODO: need to think about where to store classes -- in input context? or 
                                    // a more global space? a class registry / class provider that
                                    // all inputcontexts share?
                                }
                                break;
                            case '⌂':
                                { // object operator
                                    // object name class (init args)
                                    int thirdindex = getNextIndex(rightindex); // the args term
                                    
                                    exprs[leftindex] = Expression.Call(
                                        Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                        typeof(InputContext).GetRuntimeMethod("DefineObject", new Type[] { typeof(string), typeof(string), typeof(object[]) }),
                                        exprs[leftindex],
                                        exprs[rightindex],
                                        ExCasts.WrapArguments(exprs[thirdindex])
                                        );

                                    exprs[thirdindex] = null; // annihilate the args term
                                }
                                break;
                            case 'Ꞧ':
                                { // return operator
                                    // how to handle this? the difficult thing is determining where to place the return label
                                    // and I don't want to use return labels unless actually necessary, so I don't want to 
                                    // always litter the expr tree with return labels.

                                    // what is the expected behavior?
                                    // return labels should be spawned when there is a 'return' operator in a function definition
                                    // and return should jump to the end of that function
                                    throw new Exception("TODO");
                                }
                                break;
                            case '○':
                                // while operator
                                exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:while"),
                                    Expression.NewArrayInit(typeof(object), new Expression[] {
                                        Expression.Convert(exprs[leftindex], typeof(object)),
                                        Expression.Convert(exprs[rightindex], typeof(object))}),
                                    CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                break;
                            case 'ᴥ':
                                // if operator
                                exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:if"),
                                    Expression.NewArrayInit(typeof(object), new Expression[] {
                                        Expression.Convert(exprs[leftindex], typeof(object)),
                                        Expression.Convert(exprs[rightindex], typeof(object))}),
                                    CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                break;
                            case 'є':
                                // else operator
                                // all we do is seek out the last if operator and upgrade it to an ifelse operator
                                // ᵜ -> ifelse operator
                                for (int p = oporder[o] - 1; p >= 0; p--)
                                {
                                    if(ops[i][p] == 'ᴥ')
                                    {
                                        ops[i][p] = 'ᵜ';
                                        break;
                                    }
                                }
                                annihilateTerm = false; // don't get rid of this term afterwards
                                break;
                            case 'ᵜ':
                                // ifelse operator
                                // scan forward and find as many following ifs/ifelses as possible
                                Expression third = null;
                                int extraiflen = 0;
                                for(int p = oporder[o] + 1; p < ops[i].Count; p++)
                                {
                                    if (ops[i][p] == 'ᴥ')
                                    {
                                        extraiflen++;
                                        // found an adjacent if
                                        // append this then break
                                        break;
                                    }
                                    else if (ops[i][p] == 'ᵜ' || ops[i][p] == 'є')
                                    {
                                        extraiflen++;
                                        // found an adjacent ifelse
                                        // append this then keep searching
                                    }
                                    else
                                        break;
                                }

                                for(int p = oporder[o] + extraiflen; p > oporder[o] + 1; p--)
                                {
                                    if (ops[i][p] == 'є')
                                        continue;
                                    if(ops[i][p] == 'ᵜ')
                                    {
                                        // annihilate the op so it doesn't try to run it later
                                        ops[i][p] = ' ';

                                        // ifelse term, find the surrounding terms
                                        int ifleftindex = getPrevIndex(p);
                                        int ifrightindex = getNextIndex(p);
                                        if(third == null)
                                        {
                                            // no existing else term, find it
                                            int ifthirdindex = getNextIndex(ifrightindex);
                                            third = ExFuncs.GetFunctionExpression(Expression.Constant("f:ifelse"),
                                                Expression.NewArrayInit(typeof(object), new Expression[] {
                                                    Expression.Convert(exprs[ifleftindex], typeof(object)),
                                                    Expression.Convert(exprs[ifrightindex], typeof(object)),
                                                    Expression.Convert(exprs[ifthirdindex], typeof(object))
                                                }),
                                                CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                            exprs[ifleftindex] = null; // annihilate all the spent terms
                                            exprs[ifrightindex] = null;
                                            exprs[ifthirdindex] = null;
                                        }
                                        else
                                        {
                                            // already an existing term for the else (stick third in as the else term)
                                            third = ExFuncs.GetFunctionExpression(Expression.Constant("f:ifelse"),
                                                Expression.NewArrayInit(typeof(object), new Expression[] {
                                                    Expression.Convert(exprs[ifleftindex], typeof(object)),
                                                    Expression.Convert(exprs[ifrightindex], typeof(object)),
                                                    third
                                                }),
                                                CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                            exprs[ifleftindex] = null; // annihilate all the spent terms
                                            exprs[ifrightindex] = null;
                                        }
                                        
                                    }
                                    else if(ops[i][p] == 'ᴥ')
                                    {
                                        // annihilate the op so it doesn't try to run it later
                                        ops[i][p] = ' ';

                                        // if term, find the following term and if it
                                        int ifleftindex = getPrevIndex(p);
                                        int ifrightindex = getNextIndex(p);
                                        third = ExFuncs.GetFunctionExpression(Expression.Constant("f:if"),
                                                Expression.NewArrayInit(typeof(object), new Expression[] {
                                                Expression.Convert(exprs[ifleftindex], typeof(object)),
                                                Expression.Convert(exprs[ifrightindex], typeof(object))}),
                                                CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                        exprs[ifleftindex] = null; // annihilate all the spent terms
                                        exprs[ifrightindex] = null;
                                    }
                                }

                                if(third == null)
                                {
                                    // no chaining occured, so simply take the third expression and use it as else
                                    // find the next term beyond rightterm
                                    int thirdindex = getNextIndex(rightindex);
                                    third = Expression.Convert(exprs[thirdindex], typeof(object));
                                    exprs[thirdindex] = null; // annihilate it
                                }
                               
                                exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:ifelse"),
                                    Expression.NewArrayInit(typeof(object), new Expression[] {
                                        Expression.Convert(exprs[leftindex], typeof(object)),
                                        Expression.Convert(exprs[rightindex], typeof(object)),
                                        third
                                    }),
                                    CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                break;
                            // bitwise operators
                            case '|':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Or(lie, rie);
                                }
                                break;
                            case '&':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.And(lie, rie);
                                }
                                break;
                            // assignment operators
                            case '=':
                                { // set operator
                                    if(exprs[leftindex].Type == typeof(object))
                                    {
                                        // unknown type at compile time, we will have to inspect it at runtime
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExFuncs).GetTypeInfo().GetDeclaredMethod("AssignAtRuntime"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            ExFuncs.GetArgAsObject(exprs[rightindex]),
                                            Expression.Constant(new CompileContext(
                                                exprs[rightindex],
                                                (argParams as UnaryExpression).Operand as ParameterExpression,
                                                inputParams,
                                                contextParams,
                                                CompiledInputVarsList,
                                                CompiledContextsList.ToArray(),
                                                scopeContext))
                                            );
                                        break;
                                    }
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        // thought & why I'm changing all of these:
                                        // if we already have the inputvar here... why lookup for it again? 
                                        // just set it directly...
                                        exprs[leftindex] = Expression.Call(
                                            exprs[leftindex],
                                            typeof(InputVar).GetRuntimeMethod("SetAndReturn", new Type[] { typeof(object) }),
                                            ExFuncs.GetArgAsObject(exprs[rightindex]));
                                        /*exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("SetAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(exprs[rightindex]));*/
                                    }
                                    else if(exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:setfunc"),
                                            Expression.NewArrayInit(typeof(object), new Expression[] {
                                                Expression.Convert(exprs[leftindex], typeof(object)),
                                                Expression.Convert(exprs[rightindex], typeof(object))}),
                                            CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                    }
                                }
                                break;
                            case 'ⱻ':
                                { // definition operator
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        // change: if possible, define at compiletime so we can store vars at compiletime
                                        //  remove this, handled by getvar
                                        /*if(exprs[leftindex] is ConstantExpression)
                                        {
                                            // can only optimize here if the varname is constant
                                            string varname = (exprs[leftindex] as ConstantExpression).Value as string;
                                            CompiledContextsList[scopeContext].DefineAndReturnVar(varname, "COMPILEDEFINED");
                                        }*/
                                        
                                        exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("DefineAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(exprs[rightindex]));
                                    }
                                    else if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        exprs[leftindex] = ExFuncs.GetFunctionExpression(Expression.Constant("f:newfunc"),
                                            Expression.NewArrayInit(typeof(object), new Expression[] {
                                                Expression.Convert(exprs[leftindex], typeof(object)),
                                                Expression.Convert(exprs[rightindex], typeof(object))}),
                                            CompiledContextsList, scopeContext, argParams, inputParams, contextParams, CompiledInputVarsList);
                                    }
                                }
                                break;
                            case 'ꝑ':
                                { // add set operator
                                    if (exprs[leftindex].Type == typeof(object))
                                    {
                                        // unknown type at compile time, we will have to inspect it at runtime
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExVars).GetTypeInfo().GetDeclaredMethod("AddAssignAtRuntime"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            ExFuncs.GetArgAsObject(exprs[rightindex])
                                            );
                                        break;
                                    }
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        // special case, for string concat:
                                        Expression addexp = null;
                                        if (exprs[rightindex].Type == typeof(string))
                                        {
                                            // string types override and take precedence
                                            addexp = ExStrings.GetConcatExpression(ExCasts.UnwrapVariable(exprs[leftindex]), ExCasts.UnwrapVariable(exprs[rightindex]));
                                        }
                                        else
                                        {
                                            ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                            addexp = Expression.Add(lie, rie);
                                        }
                                        exprs[leftindex] = Expression.Call(
                                            exprs[leftindex],
                                            typeof(InputVar).GetRuntimeMethod("SetAndReturn", new Type[] { typeof(object) }),
                                            ExFuncs.GetArgAsObject(addexp));
                                        /*exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("SetAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(addexp));*/
                                    }
                                    else if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        throw new Exception("+= operator not supported for functions");
                                    }
                                }
                                break;
                            case '₥':
                                { // minus set operator
                                    if (exprs[leftindex].Type == typeof(object))
                                    {
                                        // unknown type at compile time, we will have to inspect it at runtime
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExVars).GetTypeInfo().GetDeclaredMethod("SubAssignAtRuntime"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            ExFuncs.GetArgAsObject(exprs[rightindex])
                                            );
                                        break;
                                    }
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    Expression subexp = Expression.Subtract(lie, rie);
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        exprs[leftindex] = Expression.Call(
                                            exprs[leftindex],
                                            typeof(InputVar).GetRuntimeMethod("SetAndReturn", new Type[] { typeof(object) }),
                                            ExFuncs.GetArgAsObject(subexp));
                                        /*exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("SetAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(subexp));*/
                                    }
                                    else if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        throw new Exception("-= operator not supported for functions");
                                    }
                                }
                                break;
                            case '×':
                                { // multiply set operator
                                    if (exprs[leftindex].Type == typeof(object))
                                    {
                                        // unknown type at compile time, we will have to inspect it at runtime
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExVars).GetTypeInfo().GetDeclaredMethod("MultAssignAtRuntime"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            ExFuncs.GetArgAsObject(exprs[rightindex])
                                            );
                                        break;
                                    }
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    Expression multexp = Expression.Multiply(lie, rie);
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        exprs[leftindex] = Expression.Call(
                                            exprs[leftindex],
                                            typeof(InputVar).GetRuntimeMethod("SetAndReturn", new Type[] { typeof(object) }),
                                            ExFuncs.GetArgAsObject(multexp));
                                        /*exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("SetAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(multexp));*/
                                    }
                                    else if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        throw new Exception("*= operator not supported for functions");
                                    }
                                }
                                break;
                            case '÷':
                                { // divide set operator
                                    if (exprs[leftindex].Type == typeof(object))
                                    {
                                        // unknown type at compile time, we will have to inspect it at runtime
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExVars).GetTypeInfo().GetDeclaredMethod("DivAssignAtRuntime"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            ExFuncs.GetArgAsObject(exprs[rightindex])
                                            );
                                        break;
                                    }
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    Expression divexp = Expression.Divide(lie, rie);
                                    if (exprs[leftindex].Type == typeof(AccessorResult))
                                        exprs[leftindex] = Expression.Field(exprs[leftindex], "Var");
                                    if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        exprs[leftindex] = Expression.Call(
                                            exprs[leftindex],
                                            typeof(InputVar).GetRuntimeMethod("SetAndReturn", new Type[] { typeof(object) }),
                                            ExFuncs.GetArgAsObject(divexp));
                                        /*exprs[leftindex] = Expression.Call(
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            typeof(InputContext).GetRuntimeMethod("SetAndReturnVar", new Type[] { typeof(string), typeof(object) }),
                                            Expression.Field(exprs[leftindex], typeof(InputVar).GetRuntimeField("Name")),
                                            ExFuncs.GetArgAsObject(divexp));*/
                                    }
                                    else if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // treat it like a func
                                        throw new Exception("/= operator not supported for functions");
                                    }
                                }
                                break;
                            // boolean operators
                            case '!':
                                {
                                    exprs[rightindex] = Expression.Not(
                                        Expression.Convert(
                                            ExCasts.UnwrapVariable(exprs[rightindex]),
                                            typeof(bool)));
                                }
                                break;
                            case '˭': // ==
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Equal(lie, rie);
                                }
                                break;
                            case '≠': // !=
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.NotEqual(lie, rie);
                                }
                                break;
                            case '≥': // >=
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.GreaterThanOrEqual(lie, rie);
                                }
                                break;
                            case '>':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.GreaterThan(lie, rie);
                                }
                                break;
                            case '≤': // <=
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.LessThanOrEqual(lie, rie);
                                }
                                break;
                            case '<':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.LessThan(lie, rie);
                                }
                                break;
                            case '‖': // ||
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(bool));
                                    exprs[leftindex] = Expression.OrElse(lie, rie);
                                }
                                break;
                            case 'Ѯ': // &&
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(bool));
                                    exprs[leftindex] = Expression.AndAlso(lie, rie);
                                }
                                break;
                            // standard math operators
                            case '+':
                                {
                                    // special case, for string concat:
                                    if(exprs[leftindex].Type == typeof(string) || exprs[rightindex].Type == typeof(string))
                                    {
                                        // string types override and take precedence
                                        exprs[leftindex] = ExStrings.GetConcatExpression(ExCasts.UnwrapVariable(exprs[leftindex]), ExCasts.UnwrapVariable(exprs[rightindex]));
                                        break;
                                    }
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Add(lie, rie);
                                }
                                break;
                            case '-':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Subtract(lie, rie);
                                }
                                break;
                            case '/':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Divide(lie, rie);
                                }
                                break;
                            case '*':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Multiply(lie, rie);
                                }
                                break;
                            case '^':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Power(lie, rie);
                                }
                                break;
                            case '%':
                                {
                                    ExCasts.InferType(exprs[leftindex], exprs[rightindex], out Expression lie, out Expression rie, typeof(double));
                                    exprs[leftindex] = Expression.Modulo(lie, rie);
                                }
                                break;
                            // special function operator
                            case '←':
                                // function operator
                                exprs[leftindex] = ExFuncs.GetFunctionExpression(exprs[leftindex], exprs[rightindex],
                                    CompiledContextsList, scopeContext,
                                    argParams, inputParams, contextParams, CompiledInputVarsList);
                                break;
                            // special cast operator
                            case '©':
                                exprs[leftindex] = ExCasts.GetCastExpression(exprs[leftindex], exprs[rightindex]);
                                break;
                            // array index operator
                            case '[':
                                //  unpack from var
                                exprs[leftindex] = ExCasts.UnwrapVariable(exprs[leftindex]);
                                // need to ensure that the array is in the right format 
                                // (if returned from a function, it's currently an object! and not an object[]!
                                if (exprs[leftindex].Type == typeof(object))
                                {
                                    exprs[leftindex] = Expression.ArrayIndex(
                                        Expression.Convert(exprs[leftindex], typeof(object[])),
                                        Expression.Convert(exprs[rightindex], typeof(int)));
                                }
                                else
                                {
                                    exprs[leftindex] = Expression.ArrayIndex(
                                        exprs[leftindex],
                                        Expression.Convert(exprs[rightindex], typeof(int)));
                                }
                                break;
                            // dot accessor operator
                            case '.':
                                // this is context dependent
                                // TODO: unwrap exprs[leftindex] before checking its type...
                                // a note: we must use this convoluted check here because the Type won't be
                                // ILibrary, it will be e.g. MathLibrary, the specific class type.
                                // we need to instead see if whatever type it is, implements the ILibrary interface
                                bool islib = false;
                                foreach (Type iface in exprs[leftindex].Type.GetTypeInfo().ImplementedInterfaces)
                                    if (iface == typeof(ILibrary))
                                        islib = true;
                                if(islib)
                                {
                                    // case: library being accessed
                                    exprs[leftindex] = ExLibs.GetLibraryAccessorExpression(exprs[leftindex], exprs[rightindex]);
                                    break;
                                } else {
                                    // not a library lookup, so it must be an object lookup
                                    // if the right term is a var, then we should take it as its name instead
                                    if (exprs[rightindex].Type == typeof(InputVar))
                                        exprs[rightindex] = Expression.Field(exprs[rightindex], "Name");
                                    if (exprs[leftindex].Type == typeof(string))
                                    {
                                        // assume it's an object
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExClasses).GetTypeInfo().GetDeclaredMethod("AccessFindObject"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            exprs[rightindex]);
                                    }
                                    else if (exprs[leftindex].Type == typeof(InputObject))
                                    {
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExClasses).GetTypeInfo().GetDeclaredMethod("AccessObject"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            exprs[rightindex]);
                                    }
                                    else if (exprs[leftindex].Type == typeof(InputVar))
                                    {
                                        // assume the interior of the var is an object
                                        exprs[leftindex] = Expression.Call(
                                            typeof(ExClasses).GetTypeInfo().GetDeclaredMethod("AccessVarObject"),
                                            Expression.ArrayIndex(contextParams, Expression.Constant(scopeContext)),
                                            exprs[leftindex],
                                            exprs[rightindex]);
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        // replace the right term with null
                        if (annihilateTerm)
                            exprs[rightindex] = null;
                    }
                    // finally, the leftmost term should hold everything by this point
                    // so that's our final term.
                    outputExpressions.Add(exprs[0]);
                }
                else
                {
                    if (exprs.Count > 0)
                        outputExpressions.Add(exprs[0]);
                    else
                        outputExpressions.Add(Expression.Constant(0));
                }

                // drop VVVV
                /*Expression curE = null;
                if (terms[i].Count > 0)
                    curE = determineExpression(i, oporder[0]);

                // TODO: ACCOUNT FOR PARENTHESES, ORDER OF OPERATIONS, FUNCTIONS ( like sin(4 + v:alpha) )
                for (int o = 1; o < terms[i].Count; o++)
                {
                    Expression nextE = determineExpression(i, o);
                    
                    switch (ops[i][o - 1])
                    {
                        case '+':
                            curE = Expression.Add(curE, nextE);
                            break;
                        case '-':
                            curE = Expression.Subtract(curE, nextE);
                            break;
                        case '/':
                            curE = Expression.Divide(curE, nextE);
                            break;
                        case '*':
                            curE = Expression.Multiply(curE, nextE);
                            break;
                        case '^':
                            curE = Expression.Power(curE, nextE);
                            break;
                        case '%':
                            curE = Expression.Modulo(curE, nextE);
                            break;
                        default:
                            break;
                    }
                }

                outputExpressions.Add(curE);*/
            }

            //left off; // if this is the last one...we need to return an object[]? 
            // OR do we need to like... in the exfuncs wrapper, wrap this in an object[]...
            // ^^ that I think
            // well here's the problem... I don't need to know whether the func returns an object[]
            // I need to input an object[] into it
            // so I need to know at the time getexpressionfunc is called, whether it is an object or an object[]
            // really need to just sit down and draw this out
            // so I understand where the issue is
            // believe it's mostly in exfuncs and the way args are passed
            // if only I had a way to easily convert double[] to object[] and double to object[]

            // IDEA: could call a method which takes in an object and inspects it and returns an object[]
            // that is either it converted to object[] or it made into an object[]

            if (returnSingle || outputExpressions.Count == 1)
            {
                return outputExpressions[0];
            }

            // convert outputExpressions to an object[]
            // each item must be converted to an object
            Expression[] objs = new Expression[outputExpressions.Count];
            for(int i = 0; i < outputExpressions.Count; i++)
            {
                objs[i] = Expression.Convert(outputExpressions[i], typeof(object));
            }

            return Expression.NewArrayInit(typeof(object), objs);
        }

        public object Execute(object[] args)
        {
            // assemble inputs from the Vars we have
            /*double[] inputs = new double[CompiledInputVars.Count];
            for (int i = 0; i < CompiledInputVars.Count; i++)
                inputs[i] = CompiledInputVars[i].Value;*/
            
            return CompiledExpression(args, CompiledInputVars, CompiledContexts);
        }

        
    }
}
