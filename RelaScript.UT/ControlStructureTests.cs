
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class ControlStructureTests
    {
        [TestMethod]
        public void WhileTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test,10); f:while(v:test < 100, f:setvar(v:test, v:test + 10)); c:d(v:test)"
                },
                expected: new List<object>()
                {
                    100.0
                }
            );
        }

        [TestMethod]
        public void WhileShorthandTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:newvar(v:test,10); while (v:test < 100) {f:setvar(v:test, v:test + 10)}; c:d(v:test)",
                    "v:a := 0; while (v:a < 7) {v:a = v:a + 3}; v:a",
                    "f:newvar(v:test,10); while (v:test < 100){f:setvar(v:test, v:test + 10)}; c:d(v:test)",
                    "v:a := 0; while (v:a < 7){v:a = v:a + 3}; v:a"
                },
                expected: new List<object>()
                {
                    100.0,
                    new InputVar("v:a", 9.0),
                    100.0,
                    new InputVar("v:a", 9.0)
                }
            );
        }

        [TestMethod]
        public void IfTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:if(b:t, 10.0)",
                    "f:if(b:f, 10.0)",
                    "f:if(10.0 < 5.0, 10.0)",
                    "f:if(10.0 > 5.0, 10.0)"
                },
                expected: new List<object>()
                {
                    10.0,
                    0,
                    0,
                    10.0
                }
            );
        }

        [TestMethod]
        public void IfShorthandTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "if (b:t) {10.0}",
                    "if (b:f) {10.0}",
                    "if (10.0 < 5.0) {10.0}",
                    "if (10.0 > 5.0) {10.0}"
                },
                expected: new List<object>()
                {
                    10.0,
                    0,
                    0,
                    10.0
                }
            );
        }

        [TestMethod]
        public void IfOptionalElseTest()
        {
            // if if is passed 3 args, it acts like ifelse
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:if(b:t, 10.0, 5.0)",
                    "f:if(b:f, 10.0, 5.0)",
                    "f:if(10.0 < 5.0, 10.0, 5.0)",
                    "f:if(10.0 > 5.0, 10.0, 5.0)"
                },
                expected: new List<object>()
                {
                    10.0,
                    5.0,
                    5.0,
                    10.0
                }
            );
        }

        [TestMethod]
        public void IfElseTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:ifelse(b:t, 10.0, 5.0)",
                    "f:ifelse(b:f, 10.0, 5.0)",
                    "f:ifelse(10.0 < 5.0, 10.0, 5.0)",
                    "f:ifelse(10.0 > 5.0, 10.0, 5.0)"
                },
                expected: new List<object>()
                {
                    10.0,
                    5.0,
                    5.0,
                    10.0
                }
            );
        }

        [TestMethod]
        public void IfElseShorthandTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "if(b:t) {10.0} else {5.0}",
                    "if(b:f) {10.0} else {5.0}",
                    "if(10.0 < 5.0) {10.0} else {5.0}",
                    "if(10.0 > 5.0) {10.0} else {5.0}"
                },
                expected: new List<object>()
                {
                    10.0,
                    5.0,
                    5.0,
                    10.0
                }
            );
        }

        [TestMethod]
        public void IfElseShorthandAddTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "(if(b:t) {10.0} else {5.0}) + (if(b:f) {1.0} else {5.0})",
                    "(if(b:t) {10.0} else {5.0}) + (if(b:t) {1.0} else {5.0})",
                    "(if(b:f) {10.0} else {5.0}) + (if(b:f) {1.0} else {5.0})",
                    "(if(b:f) {10.0} else {5.0}) + (if(b:t) {1.0} else {5.0})",
                    "if(b:t) {10.0} else {5.0} + if(b:f) {1.0} else {5.0}",
                    "if(b:t) {10.0} else {5.0} + if(b:t) {1.0} else {5.0}",
                    "if(b:f) {10.0} else {5.0} + if(b:f) {1.0} else {5.0}",
                    "if(b:f) {10.0} else {5.0} + if(b:t) {1.0} else {5.0}",
                },
                expected: new List<object>()
                {
                    15.0,
                    11.0,
                    10.0,
                    6.0,
                    15.0,
                    11.0,
                    10.0,
                    6.0
                }
            );
        }

        [TestMethod]
        public void IfElseShorthandChainTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "if(b:t) {10.0} else {if (b:f) {5.0} else {2.0}}",
                    "if(b:f) {10.0} else {if (b:t) {5.0} else {2.0}}",
                    "if(b:f) {10.0} else {if (b:f) {5.0} else {2.0}}",
                    "if(b:t) {10.0} else if (b:f) {5.0} else {2.0}",
                    "if(b:f) {10.0} else if (b:t) {5.0} else {2.0}",
                    "if(b:f) {10.0} else if (b:f) {5.0} else {2.0}",
                    "if(b:f) {10.0} else if (b:f) {5.0} else if (b:f) {2.0} else {1.0}",
                    "if(b:f) {10.0} else if (b:f) {5.0} else if (b:f) {2.0} else if (b:t) {1.0}",
                    "if(b:f) {10.0} else if (b:t) {1.0}"
                },
                expected: new List<object>()
                {
                    10.0,
                    5.0,
                    2.0,
                    10.0,
                    5.0,
                    2.0,
                    1.0,
                    1.0,
                    1.0
                }
            );
        }

        [TestMethod]
        public void IfElseShorthandAssignmentTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "v:a := if(b:t) {10.0} else {5.0}",
                    "v:a := if(b:f) {10.0} else {5.0}",
                    "v:a := if(10.0 < 5.0) {10.0} else {5.0}",
                    "v:a := if(10.0 > 5.0) {10.0} else {5.0}"
                },
                expected: new List<object>()
                {
                    new InputVar("v:a", 10.0),
                    new InputVar("v:a", 5.0),
                    new InputVar("v:a", 5.0),
                    new InputVar("v:a", 10.0)
                }
            );
        }
    }
}
