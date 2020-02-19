using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelaScript.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.Samples.UT
{
    [TestClass]
    public class IntroSamples
    {
        [TestMethod]
        public void LastTermReturned()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "1\r\n" +
                "2\r\n" +
                "3"
            },
            expected: new List<object>()
            {
                3.0
            });
        }

        [TestMethod]
        public void Comment()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "15 // 20"
            },
            expected: new List<object>()
            {
                15.0
            });
        }

        [TestMethod]
        public void BasicMath()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "1 + 1 // 2",
                "2 - 2 * 2 // -2",
                "3 ^ 2 // 9",
                "12 % 3 // 0",
                "10 / 2 // 5"
            },
            expected: new List<object>()
            {
                2.0,
                -2.0,
                9.0,
                0.0,
                5.0
            });
        }

        [TestMethod]
        public void DoubleByDefault()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "9 / 2 // 4.5"
            },
            expected: new List<object>()
            {
                4.5
            });
        }

        [TestMethod]
        public void Variables()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:treats := 1\r\n" +
                "v:treats = 2\r\n" +
                "v:treats + 1"
            },
            expected: new List<object>()
            {
                3.0
            });
        }

        [TestMethod]
        public void VariablesAssignments()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:snacks := 10\r\n" +
                "v:snacks + 5\r\n" +
                "v:snacks += 3\r\n" +
                "v:snacks + 0"
            },
            expected: new List<object>()
            {
                13.0
            });
        }

        [TestMethod]
        public void VariablesAssignmentResult()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:apples := 10" 
            },
            expected: new List<object>()
            {
                new InputVar("v:apples", 10.0)
            });
        }

        [TestMethod]
        public void VariablesAssignmentChainResult()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:apples := 10\r\n" + 
                "v:oranges := v:apples += 5"
            },
            expected: new List<object>()
            {
                new InputVar("v:oranges", 15.0)
            });
        }

        [TestMethod]
        public void Funcs()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:half := { a:0 / 2 }\r\n" +
                "f:half(10)"
            },
            expected: new List<object>()
            {
                5.0
            });
        }

        [TestMethod]
        public void FuncArgRename()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:value := {\r\n" +
                "\ta:quarters a:dimes\r\n" +
                "\ta:quarters * 25 + a:dimes * 10\r\n" +
                "}\r\n" +
                "f:value(3, 4)"
            },
            expected: new List<object>()
            {
                115.0
            });
        }

        [TestMethod]
        public void Scopes()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "{ v:i := 100 }\r\n " +
                "{ v:i := 50 }"
            },
            expected: new List<object>()
            {
                new InputVar("v:i", 50.0)
            });
        }

        [TestMethod]
        public void NonScopes()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "( v:i := 100 )\r\n " +
                "( v:i += 10 )"
            },
            expected: new List<object>()
            {
                new InputVar("v:i", 110.0)
            });
        }

        [TestMethod]
        public void FunctionsAffectParentScope()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:pies := 30\r\n" +
                "f:eat := { v:pies -= 1 }\r\n" +
                "f:eat()\r\n" +
                "v:pies"
            },
            expected: new List<object>()
            {
                new InputVar("v:pies", 29.0)
            });
        }

        [TestMethod]
        public void FunctionsInsideFunctions()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:apples := 0\r\n" +
                "f:gatherapples := {\r\n" +
                "\tf:pick := {\r\n" +
                "\t\tv:apples += v:apples + 1\r\n" +
                "\t}\r\n" +
                "\tf:pick()\r\n" +
                "\tf:pick()\r\n" +
                "\tf:pick()\r\n" +
                "}\r\n" +
                "f:gatherapples()"
            },
            expected: new List<object>()
            {
                new InputVar("v:apples", 7.0)
            });
        }

        [TestMethod]
        public void RedefineFunctions()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:three := { a:0 * 3 }\r\n" +
                "f:three(2) // 6\r\n" +
                "f:three = { a:0 + 3 }\r\n" +
                "f:three(2) // 5"
            },
            expected: new List<object>()
            {
                5.0
            });
        }
    }
}
