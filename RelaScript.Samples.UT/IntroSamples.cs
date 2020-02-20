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

        [TestMethod]
        public void BooleanExample()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "1 == 2"
            },
            expected: new List<object>()
            {
                false
            });
        }

        [TestMethod]
        public void IfStatements()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:cakes := 5\r\n" +
                "if (v:cakes > 10) { v:cakes -= 1 }\r\n" +
                "if (v:cakes < 10) { v:cakes += 1 }\r\n" +
                "v:cakes"
            },
            expected: new List<object>()
            {
                new InputVar("v:cakes", 6.0)
            });
        }

        [TestMethod]
        public void IfStatementsReturn()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:pears := 3\r\n" +
                "v:tarts := if (v:pears > 1) { 10 }\r\n" +
                "v:tarts += if (v:pears > 100) { 30 }"
            },
            expected: new List<object>()
            {
                new InputVar("v:tarts", 10.0)
            });
        }

        [TestMethod]
        public void IfElseStatement()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "if (1 == 2) { 10 } else { 300 }"
            },
            expected: new List<object>()
            {
                300.0
            });
        }

        [TestMethod]
        public void WhileStatement()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:plums := 5\r\n" +
                "v:guilt := 0\r\n" +
                "while (v:plums > 0) {\r\n" +
                "    v:plums -= 1\r\n" +
                "    v:guilt += 2\r\n" +
                "}"
            },
            expected: new List<object>()
            {
                new InputVar("v:guilt", 10.0)
            });
        }

        [TestMethod]
        public void BlockInIfStatement()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "if { \r\n" +
                "    v:apples := 5\r\n" +
                "    f:eat := {\r\n" +
                "        v:apples -= if (v:apples > 2) { 2 } else { 1 }\r\n" +
                "    }\r\n" +
                "    f:eat() > 2\r\n" +
                "} { 100 }"
            },
            expected: new List<object>()
            {
                100.0
            });
        }

        [TestMethod]
        public void ArrayAccess()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:arr := (10, 20, 30)\r\n" +
                "v:arr[1]"
            },
            expected: new List<object>()
            {
                20.0
            });
        }

        [TestMethod]
        public void ArrayMake()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:arr := f:array(3, 9)\r\n" +
                "v:arr[0]",
                "v:arr[1]",
                "v:arr[2]"
            },
            expected: new List<object>()
            {
                9.0,
                9.0,
                9.0
            });
        }

        [TestMethod]
        public void ArrayAssignment()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:color := f:array(3, 0)\r\n" +
                "v:color[2] = 255\r\n" +
                "v:color[0]",
                "v:color[1]",
                "v:color[2]"
                
            },
            expected: new List<object>()
            {
                0.0,
                0.0,
                255.0
            });
        }

        [TestMethod]
        public void FunctionReturnArray()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:doubleandsquare := { a:0 * 2 , a:0 ^ 2 }\r\n" +
                "f:doubleandsquare(3)[0]",
                "f:doubleandsquare(3)[1]"
            },
            expected: new List<object>()
            {
                6.0,
                9.0
            });
        }

        [TestMethod]
        public void FunctionReturnArrayMakeStyle()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:das := {\r\n" +
                "    v:result := f:array(2, 0)\r\n" +
                "    v:result[0] = a:0 * 2\r\n" +
                "    v:result[1] = a:0 ^ 2\r\n" +
                "    v:result\r\n" +
                "}\r\n" +
                "f:das(3)[0]",
                "f:das(3)[1]"
            },
            expected: new List<object>()
            {
                6.0,
                9.0
            });
        }

        [TestMethod]
        public void MultidimensionalArray()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:grid := ((0, 0, 1), (0, 0, 0), (0, 0, 0))\r\n" +
                "v:grid[0][2]"
            },
            expected: new List<object>()
            {
                1.0
            });
        }

        [TestMethod]
        public void MultidimensionalArrayMake()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "v:grid := f:array((3,3), 0)\r\n" +
                "v:grid[2][1] = 1\r\n" +
                "v:grid[2][0]",
                "v:grid[2][1]",
                "v:grid[2][2]"
            },
            expected: new List<object>()
            {
                0.0,
                1.0,
                0.0
            });
        }
    }
}
