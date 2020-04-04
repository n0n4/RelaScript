using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class FunctionTests
    {
        [TestMethod]
        public void SinTest()
        {
            TestScaffold.TestDoubleLines(
                lines: new List<string>()
                {
                    "import basic:math anon\r\n" +
                    "f:sin(10)",
                    "2 * f:sin(3)",
                    "f:sin(f:sin(5))"
                },
                expected: new List<double>()
                {
                    Math.Sin(10.0),
                    Math.Sin(3.0) * 2.0,
                    Math.Sin(Math.Sin(5.0))
                });
        }

        [TestMethod]
        public void FunctionSpaceParensTest()
        {
            TestScaffold.TestDoubleLines(
                lines: new List<string>()
                {
                    "import basic:math anon\r\n" +
                    "f:sin (10)",
                    "2 * f:sin (3)",
                    "f:sin (f:sin (5))"
                },
                expected: new List<double>()
                {
                    Math.Sin(10.0),
                    Math.Sin(3.0) * 2.0,
                    Math.Sin(Math.Sin(5.0))
                });
        }

        [TestMethod]
        public void CustomFuncNoArgsTest()
        {
            // if func is followed by (), treat it as argumentless func call
            // if func is not followed by (), treat it as a string
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:test()",
                    "f:test",
                    "f:test(3)"
                },
                expected: new List<object>()
                {
                    5.0,
                    "f:test",
                    5.0
                },
                funcs: new Dictionary<string, string>()
                {
                    { "f:test", "5.0" }
                });
        }

        [TestMethod]
        public void CustomFuncDefineTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:newfunc(f:test,{5.0})",
                    "f:test",
                    "f:test()"
                },
                expected: new List<object>()
                {
                    0,
                    "f:test",
                    5.0
                });
        }

        [TestMethod]
        public void CustomFuncChangeTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:newfunc(f:test,{5.0})",
                    "f:test",
                    "f:test()",
                    "f:setfunc(f:test,{7.0*a:0})",
                    "f:test(2)"
                },
                expected: new List<object>()
                {
                    0,
                    "f:test",
                    5.0,
                    0,
                    14.0
                });
        }

        [TestMethod]
        public void CustomFuncDefineUnquotedTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:newfunc(f:test,5.0)",
                    "f:test",
                    "f:test()"
                },
                expected: new List<object>()
                {
                    0,
                    "f:test",
                    5.0
                });
        }

        [TestMethod]
        public void CustomFuncDefineComplexTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:newfunc(f:test,{(a:0 - 3.0) * 12.0})",
                    "f:test",
                    "f:test(5)"
                },
                expected: new List<object>()
                {
                    0,
                    "f:test",
                    24.0
                });
        }

        [TestMethod]
        public void CustomFuncDefineUnquotedComplexTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:newfunc(f:test,(a:0 - 3.0) * 12.0)",
                    "f:test",
                    "f:test(5)"
                },
                expected: new List<object>()
                {
                    0,
                    "f:test",
                    24.0
                });
        }

        [TestMethod]
        public void FuncAcceptVarTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := 10; f:test := {a:0 + 1}; f:test(v:a)"
            },
            expected: new List<object>()
            {
                11.0
            });
        }

        [TestMethod]
        public void FuncAcceptArrayTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (2,3); f:test := {a:0 + a:1}; f:test(v:a)"
            },
            expected: new List<object>()
            {
                5.0
            });
        }

        [TestMethod]
        public void FuncAcceptArrayAndOtherInputTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (2,3); f:test := {(a:0[0] + a:0[1]) * a:1}; f:test(v:a, 2)"
            },
            expected: new List<object>()
            {
                10.0
            });
        }

        [TestMethod]
        public void FuncAllArgsTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (1,2,3); f:test := {a:all[0] + a:all[1] + a:all[2]}; f:test(v:a)"
            },
            expected: new List<object>()
            {
                6.0
            });
        }

        [TestMethod]
        public void FuncNameArgsTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:health := 100; f:damage := { a:power a:armor a:bonus " +
                "v:health -= (a:power - a:armor) + (a:bonus - a:armor)} f:damage(10, 1, 4)"
            },
            expected: new List<object>()
            {
                new InputVar("v:health", 88.0)
            });
        }

        [TestMethod]
        public void EmptyFuncTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:test := {}; f:test() "
            },
            expected: new List<object>()
            {
                0.0
            });
        }
    }
}
