using System;
using System.Collections.Generic;
using System.Text;

namespace RelaScript
{
    public static class CharExtensions
    {
        public static bool IsOperator(this char c)
        {
            return c == '+' || c == '-' || c == '/' || c == '*'
                || c == '^' || c == '%' || c == '←' || c == '['
                || c == '©'
                || c == '=' || c == 'ⱻ' || c == '˭'
                || c == '!' || c == '>' || c == '<' || c == '|' || c == '&'
                || c == '≠' || c == '≥' || c == '≤' || c == '‖' || c == 'Ѯ'
                || c == '.'
                || c == '○' || c == 'ᴥ' || c == 'є' || c == 'ᵜ'
                || c == 'ꝑ' || c == '₥' || c == '×' || c == '÷'
                || c == 'Ꞧ'
                || c == '¶' || c == '⌂'
                || c == 'ꜛ'
                || c == '□';
        }

        public static int GetOperatorPriority(this char c)
        {
            switch (c)
            {
                // special
                // note that all of these happen immediately because __their terms should always be in parens__
                case '○': // while operator
                    return 2; // occurs before assignment
                case 'ᴥ': // if operator
                    return 2;
                case 'є': // else operator
                    return 1; // must happen before if
                case 'ᵜ':
                    return 2; // if else operator
                case 'Ꞧ': 
                    return 300; // return op, should be last
                case '¶':
                    return 3; // class definition
                case '⌂':
                    return 3; // object definition
                case 'ꜛ':
                    return 3; // library import
                case '□':
                    return 300; // free operator, should occur after everyting else

                // assigment
                case '=': // =, equiv to setvar / setfunc
                    return 200; // should occur after everything, more or less
                case 'ⱻ': // :=, equiv. to newvar / newfunc
                    return 200;
                case 'ꝑ': // +=
                    return 200;
                case '₥': // -=
                    return 200;
                case '×': // *=
                    return 200;
                case '÷': // /=
                    return 200;

                // accessors
                case '.':
                    return 11; // occurs on same level as function calls 

                // logicals
                case '!':
                    return 19; // what priority? higher?
                case '>':
                    return 20;
                case '<':
                    return 20;
                case '|':
                    return 19; // unsure what priority | and & should have...
                case '&':
                    return 19;
                case '˭':
                    return 20;
                case '≠':
                    return 20;
                case '≥':
                    return 20;
                case '≤':
                    return 20;
                case '‖':
                    return 21; // should happen after other booleans
                case 'Ѯ':
                    return 21;
                    
                //
                case '+':
                    return 15;
                case '-':
                    return 15;
                case '*':
                    return 14;
                case '/':
                    return 14;
                case '^':
                    return 13;
                case '%':
                    return 13;
                case '←': // function op
                    return 11;
                case '©': // cast op
                    return 10; 
                case '[': // index array
                    return 11; // NOTE : promoting this from 12 to 11 because this should
                    // happen on the same plane as functions and accessors... right?
                    // obj[0].func should just proceed left to right in order
                default:
                    return 0;
            }
        }

        public static bool IsNumber(this char c)
        {
            return c == '0' || c == '1' || c == '2' || c == '3'
                || c == '4' || c == '5' || c == '6' || c == '7'
                || c == '8' || c == '9'
                || c == '.' || c == '-';
        }
    }
}
