using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class ScopeTests
    {
        [TestMethod]
        public void ScopeAsParensTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "10 * {3 + 1}",
                    "{2 * 3}",
                    "3 * {2 * {1 + 1}}"
                },
                expected: new List<object>()
                {
                    40.0,
                    6.0,
                    12.0
                }
            );
        }

        [TestMethod]
        public void VarScopeParentTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 5); {v:test * 2}"
                },
                expected: new List<object>()
                {
                    10.0
                }
            );
        }

        [TestMethod]
        public void VarScopeTest()
        {
            InputContext context = new InputContext();

            Exline e1 = new Exline("{f:newvar(v:test, 5)}");

            context.CompileLine(e1);

            e1.Execute(new object[] { 0.0 });

            // using originator false here because I may change what originator true returns
            // in the future to be something other than a null var
            Assert.IsTrue(context.GetVar("v:test", false) == null);
        }

        [TestMethod]
        public void WhileScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:sum, 0); f:while(v:sum < 100, {v:i := 0; f:while(v:i <= 5, {v:sum = v:sum + v:i; v:i = v:i + 1;});}); v:sum"
                },
                expected: new List<object>()
                {
                    new InputVar("v:sum", 105.0)
                }
            );
        }

        [TestMethod]
        public void FuncScopeParentTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newfunc(f:test, a:0 / 2.0); {f:test(10.0)}"
                },
                expected: new List<object>()
                {
                    5.0
                }
            );
        }

        [TestMethod]
        public void FuncScopeTest()
        {
            InputContext context = new InputContext();

            Exline e1 = new Exline("{f:newfunc(f:test, a:0 / 2.0)}");

            context.CompileLine(e1);

            e1.Execute(new object[] { 0.0 });

            // using originator false here because I may change what originator true returns
            // in the future to be something other than a null var
            Assert.IsTrue(context.GetFunc("f:test", false) == null);
        }

        [TestMethod]
        public void ZeroScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "{0}"
                },
                expected: new List<object>()
                {
                    0.0
                }
            );
        }

        [TestMethod]
        public void EmptyScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "{}"
                },
                expected: new List<object>()
                {
                    0.0
                }
            );
        }

        [TestMethod]
        public void BackToBackScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "{0}{1}"
                },
                expected: new List<object>()
                {
                    1.0
                }
            );
        }

        // Note on following tests:
        // current deciding that functions must be accessed via ()
        // and not supporting e.g. f:sin{3} (would be f:sin({3})
        // these tests reflect that decision and would need to be
        // updated if we decide against it
        // (currently I don't see any value in it)
        [TestMethod]
        public void FuncInputScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:test := {a:0 - 1} f:test{5}"
                },
                expected: new List<object>()
                {
                    "f:test{5}"
                }
            );
        }

        [TestMethod]
        public void FuncInputEmptyScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:test := {3} f:test{}"
                },
                expected: new List<object>()
                {
                    "f:test{}"
                }
            );
        }
        // NOTE: this behavior is no longer supported
        // (that being redefining something defined in a higher scope)
        /*[TestMethod]
        public void MultipleDefinitionScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test, 1.0); {f:newvar(v:i, 2.0); f:setvar(v:test, v:test + v:i)}; " 
                    + "{f:newvar(v:i, 5.0); f:setvar(v:test, v:test + v:i)}; v:test + 1.0"
                },
                expected: new List<object>()
                {
                    9.0
                }
            );
        }

        [TestMethod]
        public void FuncDefinedInScopeTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:i, 2.0); f:newfunc(f:test, {f:newvar(v:i, 3.0); a:0 / v:i}); f:test(9.0)"
                },
                expected: new List<object>()
                {
                    3.0
                }
            );
        }*/
    }
}
