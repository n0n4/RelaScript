using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class OperatorTests
    {
        [TestMethod]
        public void OrderOfOperationsTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("2 + 3 * 5"));
            rs.Add(17.0);

            es.Add(new Exline("5 * 3 + 2"));
            rs.Add(17.0);

            es.Add(new Exline("10 / 2 - 1"));
            rs.Add(4.0);

            es.Add(new Exline("-1 + 10 / 2"));
            rs.Add(4.0);

            es.Add(new Exline("2 ^ 2 - 1"));
            rs.Add(3.0);

            es.Add(new Exline("-1 + 2 ^ 2"));
            rs.Add(3.0);

            es.Add(new Exline("2 ^ 2 ^ 2 + 1"));
            rs.Add(17.0);

            es.Add(new Exline("10 % 4 + 7"));
            rs.Add(9.0);

            es.Add(new Exline("7 + 10 % 4"));
            rs.Add(9.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void NegativeTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("-2 + 3"));
            rs.Add(1.0);

            es.Add(new Exline("-2 + -3"));
            rs.Add(-5.0);

            es.Add(new Exline("1 - 1"));
            rs.Add(0.0);

            es.Add(new Exline("1 - -1"));
            rs.Add(2.0);

            es.Add(new Exline("-1 - -1"));
            rs.Add(0.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void ModuloTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("10 % 3"));
            rs.Add(1.0);

            es.Add(new Exline("10 % 4"));
            rs.Add(2.0);

            es.Add(new Exline("10 % 2"));
            rs.Add(0.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void AddTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("10 + 3"));
            rs.Add(13.0);

            es.Add(new Exline("1 + 1 + 1 + 1"));
            rs.Add(4.0);

            es.Add(new Exline("1 + 2 + 3 + 4"));
            rs.Add(10.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void SubtractionTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("10 - 3"));
            rs.Add(7.0);

            es.Add(new Exline("10 - 4"));
            rs.Add(6.0);

            es.Add(new Exline("2 - 6"));
            rs.Add(-4.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void MultiplyTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("2 * 3"));
            rs.Add(6.0);

            es.Add(new Exline("100 * 10"));
            rs.Add(1000.0);

            es.Add(new Exline("1 * 1"));
            rs.Add(1.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void DivideTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("10 / 2"));
            rs.Add(5.0);

            es.Add(new Exline("10 / 3"));
            rs.Add(10.0 / 3.0);

            es.Add(new Exline("1 / 4"));
            rs.Add(0.25);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void PowerTest()
        {
            InputContext context = new InputContext();

            List<Exline> es = new List<Exline>();
            List<double> rs = new List<double>();

            es.Add(new Exline("2 ^ 3"));
            rs.Add(8.0);

            es.Add(new Exline("10 ^ 1"));
            rs.Add(10.0);

            es.Add(new Exline("10 ^ 0"));
            rs.Add(1.0);

            foreach (Exline e in es)
                context.CompileLine(e);

            for (int i = 0; i < es.Count; i++)
                Assert.AreEqual(rs[i], es[i].Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void EqualsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "b:t == b:f",
                    "b:t == b:t",
                    "b:f == b:f",
                    "15.0 == 15.0",
                    "14.9 == 15.0",
                    "3 == 2",
                    "-2 == -2"
                },
                expected: new List<object>()
                {
                    false,
                    true,
                    true,
                    true,
                    false,
                    false,
                    true
                }
            );
        }

        [TestMethod]
        public void NotEqualsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "b:t != b:f",
                    "b:t != b:t",
                    "b:f != b:f",
                    "15.0 != 15.0",
                    "14.9 != 15.0",
                    "3 != 2",
                    "-2 != -2"
                },
                expected: new List<object>()
                {
                    true,
                    false,
                    false,
                    false,
                    true,
                    true,
                    false
                }
            );
        }

        [TestMethod]
        public void GreaterThanTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "15.0 > 10.0",
                    "3 > 2",
                    "25.0 > 30.0",
                    "4 > 8",
                    "15.3 > 15.3",
                    "4 > 4"
                },
                expected: new List<object>()
                {
                    true,
                    true,
                    false,
                    false,
                    false,
                    false
                }
            );
        }

        [TestMethod]
        public void GreaterThanOrEqualTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "15.0 >= 10.0",
                    "3 >= 2",
                    "25.0 >= 30.0",
                    "4 >= 8",
                    "15.3 >= 15.3",
                    "4 >= 4"
                },
                expected: new List<object>()
                {
                    true,
                    true,
                    false,
                    false,
                    true,
                    true
                }
            );
        }

        [TestMethod]
        public void LessThanTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "15.0 < 10.0",
                    "3 < 2",
                    "25.0 < 30.0",
                    "4 < 8",
                    "15.3 < 15.3",
                    "4 < 4"
                },
                expected: new List<object>()
                {
                    false,
                    false,
                    true,
                    true,
                    false,
                    false
                }
            );
        }

        [TestMethod]
        public void LessThanOrEqualTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "15.0 <= 10.0",
                    "3 <= 2",
                    "25.0 <= 30.0",
                    "4 <= 8",
                    "15.3 <= 15.3",
                    "4 <= 4"
                },
                expected: new List<object>()
                {
                    false,
                    false,
                    true,
                    true,
                    true,
                    true
                }
            );
        }

        [TestMethod]
        public void OrTest()
        {
            
        }

        [TestMethod]
        public void AndTest()
        {

        }

        [TestMethod]
        public void OrElseTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "b:t || b:f",
                    "b:f || b:f",
                    "b:t || b:t",
                    "(1 < 2) || b:f"
                },
                expected: new List<object>()
                {
                    true,
                    false,
                    true,
                    true
                }
            );
        }

        [TestMethod]
        public void AndAlsoTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "b:t && b:f",
                    "b:f && b:f",
                    "b:t && b:t",
                    "(1 < 2) && b:f"
                },
                expected: new List<object>()
                {
                    false,
                    false,
                    true,
                    false
                }
            );
        }

        [TestMethod]
        public void AssignVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 0.0); v:test = 5.0",
                    "f:newvar(v:a, b:f); v:a = b:t",
                    "f:newvar(v:alpha, 0.0); v:alpha = 3 - 4 * (1 + 3)",
                    "f:newvar(v:beta, 0.0); 3 + (v:beta = 2)",
                    "f:newvar(v:red, 0.0); v:red = 255; v:red"
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", 5.0),
                    new InputVar("v:a", true),
                    new InputVar("v:alpha", -13.0),
                    5.0,
                    new InputVar("v:red", 255.0)
                }
            );
        }

        [TestMethod]
        public void AssignNewVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "v:test := 5.0",
                    "v:a := b:t",
                    "v:alpha := 3 - 4 * (1 + 3)",
                    "3 + (v:beta := 2)",
                    "v:red := 255; v:red"
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", 5.0),
                    new InputVar("v:a", true),
                    new InputVar("v:alpha", -13.0),
                    5.0,
                    new InputVar("v:red", 255.0)
                }
            );
        }

        [TestMethod]
        public void AssignFuncsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newfunc(f:half, 0); f:half = {a:0/2}; f:half(10.0)",
                    "f:newfunc(f:abc, 0); f:abc = {(a:0 - a:1) * a:2}; f:abc(2,3,5)",
                    "f:newfunc(f:addah, 0); f:newvar(v:test, 'ah'); f:addah = {c:s(a:0) + v:test}; f:addah('boat')"
                },
                expected: new List<object>()
                {
                    5.0,
                    -5.0,
                    "boatah"
                }
            );
        }

        [TestMethod]
        public void AssignNewFuncsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:half := {a:0/2}; f:half(10.0)",
                    "f:abc := {(a:0 - a:1) * a:2}; f:abc(2,3,5)",
                    "f:newvar(v:test, 'ah'); f:addah := {c:s(a:0) + v:test}; f:addah('boat')"
                },
                expected: new List<object>()
                {
                    5.0,
                    -5.0,
                    "boatah"
                }
            );
        }


        [TestMethod]
        public void AddAssignVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 1.0); v:test += 5.0",
                    "f:newvar(v:alpha, 1.0); v:alpha += 3 - 4 * (1 + 3)",
                    "f:newvar(v:beta, 1.0); 3 + (v:beta += 2)",
                    "f:newvar(v:red, 1.0); v:red += 255; v:red"
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", 6.0),
                    new InputVar("v:alpha", -12.0),
                    6.0,
                    new InputVar("v:red", 256.0)
                }
            );
        }

        [TestMethod]
        public void AddAssignStringVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 'boat'); v:test += 'ah'",
                    "f:newvar(v:alpha, 'test '); v:alpha += c:s(3)",
                    "f:newvar(v:beta, 'a'); 'c' + (v:beta += 'b')",
                    "f:newvar(v:red, 'r'); v:red += 'e'; v:red += 'd'",
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", "boatah"),
                    new InputVar("v:alpha", "test 3"),
                    "cab",
                    new InputVar("v:red", "red")
                }
            );
        }

        [TestMethod]
        public void SubtractAssignVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 0.0); v:test -= 5.0",
                    "f:newvar(v:alpha, 0.0); v:alpha -= 3 - 4 * (1 + 3)",
                    "f:newvar(v:beta, 1.0); 3 + (v:beta -= 2)",
                    "f:newvar(v:red, 0.0); v:red -= 255; v:red",
                    "f:newvar(v:alpha, 3.0); v:alpha -= 3 - 4 * (1 + 3)",
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", -5.0),
                    new InputVar("v:alpha", 13.0),
                    2.0,
                    new InputVar("v:red", -255.0),
                    new InputVar("v:alpha", 16.0),
                }
            );
        }

        [TestMethod]
        public void MultiplyAssignVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 0.0); v:test *= 5.0",
                    "f:newvar(v:alpha, 1.0); v:alpha *= 3 - 4 * (1 + 3)",
                    "f:newvar(v:beta, 2.0); 3 + (v:beta *= 2)",
                    "f:newvar(v:red, 10.0); v:red *= 255; v:red"
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", 0.0),
                    new InputVar("v:alpha", -13.0),
                    7.0,
                    new InputVar("v:red", 2550.0)
                }
            );
        }

        [TestMethod]
        public void DivideAssignVarsTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 25.0); v:test /= 5.0",
                    "f:newvar(v:alpha, 0.0); v:alpha /= 3 - 4 * (1 + 3)",
                    "f:newvar(v:beta, 4.0); 3 + (v:beta /= 2)",
                    "f:newvar(v:red, 13.0); v:red /= 255; v:red"
                },
                expected: new List<object>()
                {
                    new InputVar("v:test", 5.0),
                    new InputVar("v:alpha", 0.0),
                    5.0,
                    new InputVar("v:red", 13.0/255.0)
                }
            );
        }

        [TestMethod]
        public void NotTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "!true",
                    "!false",
                    "if (!(3 > 1)) { 1 } else { 2 }",
                    "if (!(3 < 1)) { 1 } else { 2 }",
                    "!(13 >= 7 * 2)",
                    "v:a := true; !v:a"
                },
                expected: new List<object>()
                {
                    false,
                    true,
                    2.0,
                    1.0,
                    true,
                    false
                }
            );
        }
    }
}
