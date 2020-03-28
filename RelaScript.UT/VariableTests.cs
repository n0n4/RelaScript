using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void BasicVarTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "v:alpha",
                    "v:alpha * 2.0",
                    "v:alpha + v:beta",
                    "v:alpha + (v:beta)",
                    "v:alpha * f:sin(v:beta)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:alpha",5.0),
                    10.0,
                    7.0,
                    7.0,
                    5.0 * Math.Sin(2.0)
                },
                vars: new Dictionary<string, double>()
                {
                    { "v:alpha", 5.0 },
                    { "v:beta", 2.0 }
                }
            );
        }

        [TestMethod]
        public void SetVarTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:setvar(v:alpha, 15.0)",
                    "2.0 * f:setvar(v:beta, 4.0)",
                    "f:setvar(v:alpha, v:alpha + 15.0)",
                    "f:setvar(v:beta, v:beta + v:alpha + 4.0)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:alpha",15.0),
                    8.0,
                    new InputVar("v:alpha",30.0),
                    new InputVar("v:beta",38.0)
                },
                vars: new Dictionary<string, double>()
                {
                    { "v:alpha", 5.0 },
                    { "v:beta", 2.0 }
                }
            );
        }

        [TestMethod]
        public void NewVarTest()
        {
            /*TestScaffold.TestLinesSequential(
                lines: new List<string>()
                {
                    "f:newvar(v:alpha, 15.0)",
                    "2.0 * f:newvar(v:beta, 4.0)",
                    "f:setvar(v:alpha, v:alpha + 15.0)",
                    "f:setvar(v:beta, v:beta + v:alpha + 4.0)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:alpha",15.0),
                    8.0,
                    new InputVar("v:alpha",30.0),
                    new InputVar("v:beta",38.0)
                },
                vars: new Dictionary<string, double>()
                {
                    
                }
            );*/

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:alpha, 15.0)",
                    "2.0 * f:newvar(v:beta, 4.0)",
                    "f:setvar(v:alpha, v:alpha + 15.0)",
                    "f:setvar(v:beta, v:beta + v:alpha + 4.0)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:alpha",15.0),
                    8.0,
                    new InputVar("v:alpha",30.0),
                    new InputVar("v:beta",38.0)
                },
                vars: new Dictionary<string, double>()
                {

                }
            );
        }

        [TestMethod]
        public void DynamicVarLookupTest()
        {
            TestScaffold.TestLinesSequential(
                lines: new List<string>()
                {
                    "f:newvar(v:name, 'v:two')",
                    "2.0 + f:newvar(f:getvar(v:name), 5.0)",
                    "v:two",
                    "2.0 + f:setvar('v:' + 'two', 10.0)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:name","v:two"),
                    7.0,
                    new InputVar("v:two", 5.0),
                    12.0
                },
                vars: new Dictionary<string, double>()
                {

                }
            );

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:name, 'v:two')",
                    "2.0 + f:newvar(f:getvar(v:name), 5.0)",
                    "v:two",
                    "2.0 + f:setvar('v:' + 'two', 10.0)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:name","v:two"),
                    7.0,
                    new InputVar("v:two", 5.0),
                    12.0
                },
                vars: new Dictionary<string, double>()
                {

                }
            );
        }

        [TestMethod]
        public void VarToVarAssignmentTest()
        {
            // POSIT: assigning a variable equal to an existing variable
            //        should give the VALUE of the variable, NOT a pointer
            //        to the var itself. Changing the var used in the assignment
            //        SHOULD NOT change the var that was assigned 
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := 1; v:b := v:a; v:a += 1; v:b + 0",
                "v:a + v:b",
                "v:c := 1; v:d := v:c; v:d"
            },
            expected: new List<object>()
            {
                1.0,
                3.0,
                new InputVar("v:d", 1.0)
            });
        }

        [TestMethod]
        public void ArrayVarTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math\r\n" +
                "l:math.f:max((1, 2, 3))",
                "v:a := (1, 2, 3)\r\n" +
                "l:math.f:max(v:a)",
            },
            expected: new List<object>()
            {
                3.0,
                3.0,
            });
        }

        [TestMethod]
        public void ArrayStringVarTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:string l:str\r\n" +
                "l:str.f:commaand(('a','b','c'))",
                "v:s := ('a','b','c')\r\n" +
                "l:str.f:commaand(v:s)"
            },
            expected: new List<object>()
            {
                "a, b, and c",
                "a, b, and c",
            });
        }
    }
}
