using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelaScript.Libraries.Basics;

namespace RelaScript.UT
{
    [TestClass]
    public class WorkflowTests
    {
        [TestMethod]
        public void BasicTest()
        {
            InputContext context = new InputContext();

            context.DefineVar("v:test", 0);

            Exline e1 = new Exline("2 + 3 * 5");
            Exline e2 = new Exline("v:test * 2");
            Exline e3 = new Exline("a:0 * 5");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            context.SetVar("v:test", (double)e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(17.0, context.GetVar("v:test").Value);
            context.SetVar("v:test", (double)e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(34.0, context.GetVar("v:test").Value);
            context.SetVar("v:test", (double)e3.Execute(new object[] { 50.0 }));
            Assert.AreEqual(250.0, context.GetVar("v:test").Value);
        }

        [TestMethod]
        public void WithoutSpacingTest()
        {
            InputContext context = new InputContext();

            context.DefineVar("v:test", 0);

            Exline e1 = new Exline("2+3*5");
            Exline e2 = new Exline("v:test*2");
            Exline e3 = new Exline("a:0*5");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            context.SetVar("v:test", (double)e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(17.0, context.GetVar("v:test").Value);
            context.SetVar("v:test", (double)e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(34.0, context.GetVar("v:test").Value);
            context.SetVar("v:test", (double)e3.Execute(new object[] { 50.0 }));
            Assert.AreEqual(250.0, context.GetVar("v:test").Value);
        }

        [TestMethod]
        public void ParensTest()
        {
            InputContext context = new InputContext();

            Exline e1 = new Exline("2 * (3 - 1)");
            Exline e2 = new Exline("2 * (3 - (2 - 1))");

            context.CompileLine(e1);
            context.CompileLine(e2);

            Assert.AreEqual(4.0, e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(4.0, e2.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void EmptyParensTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "()"
                },
                expected: new List<object>()
                {
                    0.0
                }
            );
        }

        [TestMethod]
        public void ZeroParensTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "(0)"
                },
                expected: new List<object>()
                {
                    0.0
                }
            );
        }

        [TestMethod]
        public void FunctionTest()
        {
            InputContext context = new InputContext();
            context.LibraryProviders.Add(new LibraryProviderBasic());

            Exline e1 = new Exline("import basic:math anon \r\n 2 * f:sin(10)");
            Exline e2 = new Exline("f:sin(2)");
            Exline e3 = new Exline("2 * f:sin(1 - (3 * 4)) - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            Assert.AreEqual(2.0 * Math.Sin(10.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(Math.Sin(2.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * Math.Sin(1.0 - (3.0 * 4.0)) - 1.0, e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void CustomFunctionTest()
        {
            InputContext context = new InputContext();

            context.DefineFunc("f:half", "0.5 * a:0");
            double half(double input)
            {
                return input / 2.0;
            }

            Exline e1 = new Exline("2 * f:half(10)");
            Exline e2 = new Exline("f:half(2)");
            Exline e3 = new Exline("2 * f:half(1 - (3 * 4)) - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            Assert.AreEqual(2.0 * half(10.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(half(2.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * half(1.0 - (3.0 * 4.0)) - 1.0, e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void CustomFunctionWithVarTest()
        {
            InputContext context = new InputContext();

            double alpha = 15.0;
            context.DefineVar("v:alpha", alpha);
            context.DefineFunc("f:addalpha", "v:alpha + a:0");
            double addalpha(double input)
            {
                return alpha + input;
            }

            Exline e1 = new Exline("2 * f:addalpha(10)");
            Exline e2 = new Exline("f:addalpha(2)");
            Exline e3 = new Exline("2 * f:addalpha(1 - (3 * 4)) - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            Assert.AreEqual(2.0 * addalpha(10.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(addalpha(2.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * addalpha(1.0 - (3.0 * 4.0)) - 1.0, e3.Execute(new object[] { 0.0 }));

            alpha = 3.0;
            context.SetVar("v:alpha", alpha);

            Assert.AreEqual(2.0 * addalpha(10.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(addalpha(2.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * addalpha(1.0 - (3.0 * 4.0)) - 1.0, e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void CustomFunctionMultipleInputsTest()
        {
            InputContext context = new InputContext();

            context.DefineFunc("f:abc", "0.5 * a:0 + a:1 / a:2");
            double abc(double a, double b, double c)
            {
                return (a / 2.0) + (b / c);
            }

            Exline e1 = new Exline("2 * f:abc(10,5,3)");
            Exline e2 = new Exline("f:abc(8,4,1)");
            Exline e3 = new Exline("2 * f:abc(1 - (3 * 4), 2, 7) - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            Assert.AreEqual(2.0 * abc(10.0, 5.0, 3.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(abc(8.0, 4.0, 1.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * abc(1.0 - (3.0 * 4.0), 2.0, 7.0) - 1.0, e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void CustomFunctionChainMultipleInputsTest()
        {
            InputContext context = new InputContext();

            context.DefineFunc("f:twobytwo", "0.5 * a:0 + 2.0 * a:1, 2.0 * a:0 + 0.5 * a:1");
            double[] twobytwo(double input1, double input2)
            {
                return new double[] { input1 / 2.0 + input2 * 2.0, input1 * 2.0 + input2 / 2.0 };
            }

            Exline e1 = new Exline("2 * f:twobytwo(10,4)[0]");
            Exline e2 = new Exline("2 * f:twobytwo(f:twobytwo(10,4))[0]");
            Exline e3 = new Exline("f:twobytwo(f:twobytwo(f:twobytwo(10,4)))");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            double[] first = twobytwo(10.0, 4.0);
            double[] second = twobytwo(first[0], first[1]);
            double[] third = twobytwo(second[0], second[1]);

            Assert.AreEqual(2.0 * twobytwo(10.0, 4.0)[0], e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * second[0], e2.Execute(new object[] { 0.0 }));
            object e3result = e3.Execute(new object[] { 0.0 });
            Assert.AreEqual(third[0], ((object[])(e3result))[0]);
            Assert.AreEqual(third[1], ((object[])(e3result))[1]);
        }

        [TestMethod]
        public void CustomFunctionTooManyInputsTest()
        {
            InputContext context = new InputContext();

            context.DefineFunc("f:half", "0.5 * a:0");
            double half(double input)
            {
                return input / 2.0;
            }

            Exline e1 = new Exline("2 * f:half(10,5)");
            Exline e2 = new Exline("f:half(2,1,3,15)");
            Exline e3 = new Exline("2 * f:half(1 - (3 * 4),f:half(2)) - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);

            Assert.AreEqual(2.0 * half(10.0), e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(half(2.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * half(1.0 - (3.0 * 4.0)) - 1.0, e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void ArrayIndexTest()
        {
            InputContext context = new InputContext();

            Exline e1 = new Exline("(1,2,3)[0]");
            Exline e2 = new Exline("(1,2,3)[1]");
            Exline e3 = new Exline("2 + (1,2,3)[1]");
            Exline e4 = new Exline("2 + ((1,2,3)[1])");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);
            context.CompileLine(e4);

            Assert.AreEqual(1.0, e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0, e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(4.0, e3.Execute(new object[] { 0.0 }));
            Assert.AreEqual(4.0, e4.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void Array2dIndexTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := ((1,2,3),(4,5,6))" +
                "v:a[0][0]",
                "v:a[1][1]",
                "v:a[1][2]",
            },
            expected: new List<object>()
            {
                1.0,
                5.0,
                6.0,
            });
        }

        [TestMethod]
        public void Array3dIndexTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (((1,2,3),(4,5,6)),((7,8,9),(10,11,12)))" +
                "v:a[0][0][0]",
                "v:a[1][1][1]",
                "v:a[1][0][2]",
            },
            expected: new List<object>()
            {
                1.0,
                11.0,
                9.0,
            });
        }

        [TestMethod]
        public void ArrayVarIndexTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (1,2,3) v:b := 1 " +
                "v:a[v:b]",
            },
            expected: new List<object>()
            {
                2.0,
            });
        }

        [TestMethod]
        public void ArrayAssignTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (1,2,3)\r\n" +
                "v:a[1] = 5\r\n" +
                "v:a[2] = 6\r\n" +
                "v:a[0]",
                "v:a[1]",
                "v:a[2]",
            },
            expected: new List<object>()
            {
                1.0,
                5.0,
                6.0,
            });
        }

        [TestMethod]
        public void ArrayVarAssignTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := (1,2,3) v:b := 1 " +
                "v:a[v:b] = 5 " +
                "v:a[1]",
            },
            expected: new List<object>()
            {
                5.0,
            });
        }

        [TestMethod]
        public void Array2dAssignTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:a := ((1,2,3),(0,0,0))\r\n" +
                "v:a[1][1] = 5\r\n" +
                "v:a[1][2] = 6\r\n" +
                "v:a[0][0]",
                "v:a[1][1]",
                "v:a[1][2]",
            },
            expected: new List<object>()
            {
                1.0,
                5.0,
                6.0,
            });
        }

        [TestMethod]
        public void FunctionArrayIndexTest()
        {
            InputContext context = new InputContext();

            context.DefineFunc("f:two", "0.5 * a:0, 2.0 * a:0");
            double[] two(double input)
            {
                return new double[] { input / 2.0, input * 2.0 };
            }

            Exline e1 = new Exline("2 * f:two(10)[0]");
            Exline e2 = new Exline("2 * f:two(10)[1]");
            Exline e3 = new Exline("f:two(2)");
            Exline e4 = new Exline("2 * f:two(1 - (3 * 4))[0] - 1");
            Exline e5 = new Exline("2 * f:two(1 - (3 * 4))[1] - 1");

            context.CompileLine(e1);
            context.CompileLine(e2);
            context.CompileLine(e3);
            context.CompileLine(e4);
            context.CompileLine(e5);

            Assert.AreEqual(2.0 * two(10.0)[0], e1.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * two(10.0)[1], e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(two(2.0)[0], (double)((object[])(e3.Execute(new object[] { 0.0 })))[0]);
            Assert.AreEqual(two(2.0)[1], (double)((object[])(e3.Execute(new object[] { 0.0 })))[1]);
            Assert.AreEqual(2.0 * two(1.0 - (3.0 * 4.0))[0] - 1.0, e4.Execute(new object[] { 0.0 }));
            Assert.AreEqual(2.0 * two(1.0 - (3.0 * 4.0))[1] - 1.0, e5.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void IsThisTheIssueTest()
        {
            // no, it is not
            Expression exp = Expression.NewArrayInit(typeof(double), Expression.Constant(15.0));
            Expression exp2 = Expression.Convert(exp, typeof(object));
            var lam = Expression.Lambda<Func<object>>(exp2);
            Func<object> func = lam.Compile();
            var result = func();
        }

        [TestMethod]
        public void WhichCastsAreInvalidTest()
        {
            object[] args = new object[] { 3.0, 5.0 };
            ParameterExpression p = Expression.Parameter(typeof(object[]));
            Expression e1 = Expression.Convert(Expression.ArrayIndex(p, Expression.Constant(0)), typeof(double));
            Expression e2 = Expression.Convert(Expression.Multiply(Expression.Constant(0.5), e1), typeof(object));
            Expression e3 = Expression.NewArrayInit(typeof(object), e2);
            Expression e4 = Expression.Convert(e3, typeof(object));

            Func<object[], double> r1 = Expression.Lambda<Func<object[], double>>(e1, p).Compile();
            Func<object[], object> r2 = Expression.Lambda<Func<object[], object>>(e2, p).Compile();
            Func<object[], object[]> r3 = Expression.Lambda<Func<object[], object[]>>(e3, p).Compile();
            Func<object[], object> r4 = Expression.Lambda<Func<object[], object>>(e4, p).Compile();

            r1(args);
            r2(args);
            r3(args);
            r4(args);
        }

        [TestMethod]
        public void SemiColonTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "1.0;2.0;3.0",
                    "1.0;2.0;3.0;",
                    "f:newvar(v:test, 10.0); f:setvar(v:test, v:test + 5.0); v:test * 2.0",
                    "10.0 + 5.0; 'test'",
                    "f:newvar(v:alpha, 5.0); v:alpha"
                },
                expected: new List<object>()
                {
                    3.0,
                    3.0,
                    30.0,
                    "test",
                    new InputVar("v:alpha", 5.0)
                }
            );
        }

        [TestMethod]
        public void ElideSemicolonTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "1.0 2.0 3.0",
                    "1.0 2.0 3.0;",
                    "f:newvar(v:test, 10.0) f:setvar(v:test, v:test + 5.0) v:test * 2.0",
                    "10.0 + 5.0 'test'",
                    "f:newvar(v:alpha, 5.0) v:alpha",
                    "v:i := 0; while(v:i < 10) {v:i = v:i + 3} v:i",
                    "v:i := 0 while(v:i < 10) {v:i = v:i + 3} v:i",
                    "v:a := 3; if(v:a > 2) {v:a = 5} else {v:a = 1} v:a",
                    "v:a := 3 if(v:a > 2) {v:a = 5} else {v:a = 1} v:a"
                },
                expected: new List<object>()
                {
                    3.0,
                    3.0,
                    30.0,
                    "test",
                    new InputVar("v:alpha", 5.0),
                    new InputVar("v:i", 12.0),
                    new InputVar("v:i", 12.0),
                    new InputVar("v:a", 5.0),
                    new InputVar("v:a", 5.0)
                }
            );
        }

        [TestMethod]
        public void SimpleWhileTest()
        {
            string line = "f:newvar(v:iterator, 1);"
                + "\r\nf:newvar(v:alpha, 1.0);"
                + "\r\nf:while(v:iterator <= 100, ("
                + "\r\n\tf:setvar(v:alpha, v:alpha * 1.0135);"
                + "\r\n\tf:setvar(v:iterator, v:iterator + 1);"
                + "\r\n));"
                + "\r\nc:d(v:alpha)";

            double expected = 1.0;
            for (int i = 1; i <= 100; i++)
                expected *= 1.0135;

            TestScaffold.TestLines(lines: new List<string>() { line },
                expected: new List<object>() { expected });
        }

        [TestMethod]
        public void FizzBuzzTest()
        {
            string line = "f:newfunc(f:fizzbuzz, ("
                + "\r\n\tf:if(a:0 % 3 == 0 && a:0 % 5 == 0, ("
                + "\r\n\t'FIZZBUZZ'"
                + "\r\n\t),("
                + "\r\n\t\tf:if(a:0 % 3 == 0, ("
                + "\r\n\t\t\t'FIZZ'"
                + "\r\n\t\t),("
                + "\r\n\t\t\tf:if(a:0 % 5 == 0, ("
                + "\r\n\t\t\t\t'BUZZ'"
                + "\r\n\t\t\t),("
                + "\r\n\t\t\t\ta:0"
                + "\r\n\t\t\t))"
                + "\r\n\t\t))"
                + "\r\n\t))"
                + "\r\n)); 0";

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    line,
                    "f:fizzbuzz(1)",
                    "f:fizzbuzz(3)",
                    "f:fizzbuzz(5)",
                    "f:fizzbuzz(11)",
                    "f:fizzbuzz(15)"
                },
                expected: new List<object>()
                {
                    0.0,
                    1.0,
                    "FIZZ",
                    "BUZZ",
                    11.0,
                    "FIZZBUZZ"
                });
        }

        [TestMethod]
        public void AssignmentTests()
        {
            TestScaffold.TestLinesAndElideSemicolons(
                lines: new List<string>()
                {
                    "f:hptest := {\r\n" +
                    "\tv:health := 100;\r\n" +
                    "\tf:damaged:= {\r\n" +
                    "\t\tv:health = v:health - a:0\r\n" +
                    "\t};\r\n" +
                    "\tf:damaged(10);\r\n" +
                    "\tf:damaged(20)\r\n" +
                    "};\r\n" +
                    "f:hptest()",
                    "f:reassigntest := {\r\n" +
                    "\tf:test := {a:0 + 10};\r\n" +
                    "\tv:a := f:test(3);\r\n" +
                    "\tf:test = {a:0 / 2};\r\n" +
                    "\tv:a = v:a + f:test(10)\r\n" +
                    "};\r\n" +
                    "f:reassigntest()"
                },
                expected: new List<object>()
                {
                    new InputVar("v:health", 70.0),
                    new InputVar("v:a", 18.0)
                });
        }

        [TestMethod]
        public void IfShorthandTests()
        {
            TestScaffold.TestLinesAndElideSemicolons(
                lines: new List<string>()
                {
                    "f:shorthandif := {\r\n" +
                    "\tif (a:0 == 1) {\r\n" +
                    "\t\t3\r\n" +
                    "\t} else if (a:0 == 2) {\r\n" +
                    "\t\t5\r\n" +
                    "\t} else {\r\n" +
                    "\t\t7\r\n" +
                    "\t}\r\n" +
                    "};\r\n" +
                    "f:shorthandif(0)",
                    "f:shorthandif(1)",
                    "f:shorthandif(2)",
                    "f:assigntoif := {\r\n" +
                    "\tv:i := if (a:0 == 1) {\r\n" +
                    "\t\t5\r\n" +
                    "\t} else {\r\n" +
                    "\t\t10\r\n" +
                    "\t};\r\n" +
                    "\tv:i + 3\r\n" +
                    "};\r\n" +
                    "f:assigntoif(1)",
                    "f:assigntoif(2)",
                    "f:addif := {\r\n" +
                    "\tv:i := if (a:0 > 1) { 5 } else { 10 } + if (a:0 < 4) { 5 } else { 10 };\r\n" +
                    "\tv:i + 3\r\n" +
                    "};\r\n" +
                    "f:addif(0)",
                    "f:addif(2)",
                    "f:addif(5)"
                },
                expected: new List<object>()
                {
                    7.0,
                    3.0,
                    5.0,
                    8.0,
                    13.0,
                    18.0,
                    13.0,
                    18.0
                });
        }

        [TestMethod]
        public void WhileShorthandTests()
        {
            TestScaffold.TestLinesAndElideSemicolons(
                lines: new List<string>()
                {
                    "f:shorthandwhile := {\r\n" +
                    "\tv:i := 0;\r\n" +
                    "\tv:sum := 0;\r\n" +
                    "\twhile (v:i < a:0) {\r\n" +
                    "\t\tv:sum = v:sum + 5;\r\n" +
                    "\t\tv:i = v:i + 1;\r\n" +
                    "\t};\r\n" +
                    "\tv:sum\r\n" +
                    "};\r\n" +
                    "f:shorthandwhile(3)",
                    "f:shorthandwhile(5)"
                },
                expected: new List<object>()
                {
                    new InputVar("v:sum", 15.0),
                    new InputVar("v:sum", 25.0)
                });
        }

        [TestMethod]
        public void CommentTests()
        {
            TestScaffold.TestLinesAndElideSemicolons(
            lines: new List<string>()
            {
                "v:a := 10 // end of line comment",
                "v:b := 20 // setting b to 20\r\n" +
                "v:b += 10",
                "v:c := 30 // setting c to 30\r\n" +
                "// whole line comment\r\n" +
                "v:c += 5"
            },
            expected: new List<object>()
            {
                new InputVar("v:a", 10.0),
                new InputVar("v:b", 30.0),
                new InputVar("v:c", 35.0)
            });
        }

        [TestMethod]
        public void TwoTiminTest()
        {
            TestScaffold.TestLinesAndElideSemicolons(
            lines: new List<string>()
            {
                "f:twotimin := {\r\n" +
                "    a:0 * a:0\r\n" +
                "    ,\r\n" +
                "    if (a:0 < 10) { a:0 * 2 } else { a:0 / 2 }\r\n" +
                "}\r\n" +
                "\r\n" +
                "f:twotimin(3)[0]//-- > (9, 6)",
                "f:twotimin(3)[1]",
                "f:twotimin(12)[0]//-- > (144, 6)",
                "f:twotimin(12)[1]"
            },
            expected: new List<object>()
            {
                9.0,
                6.0,
                144.0,
                6.0
            });
        }
    }
}
