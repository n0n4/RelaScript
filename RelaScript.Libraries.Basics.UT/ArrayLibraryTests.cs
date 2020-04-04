using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelaScript.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.Libraries.Basics.UT
{
    [TestClass]
    public class ArrayLibraryTests
    {
        [TestMethod]
        public void LengthTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "l:array.f:length((1, 2, 3, 4))",
                    "l:array.f:length((1))"
                },
                expected: new List<object>()
                {
                    0.0,
                    4,
                    1
                }
            );
        }

        [TestMethod]
        public void LengthVarTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := (1, 2, 3, 4) l:array.f:length(v:arr)",
                    "v:arr2 := (1) l:array.f:length(v:arr2)"
                },
                expected: new List<object>()
                {
                    0.0,
                    4,
                    1
                }
            );
        }

        [TestMethod]
        public void SubsetStartTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:subset((1, 2, 3, 4), 2)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]"
                },
                expected: new List<object>()
                {
                    0.0,
                    2,
                    3.0,
                    4.0
                }
            );
        }

        [TestMethod]
        public void SubsetStartCountTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:subset((1, 2, 3, 4), 1, 2)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]"
                },
                expected: new List<object>()
                {
                    0.0,
                    2,
                    2.0,
                    3.0
                }
            );
        }

        [TestMethod]
        public void AnonSubsetStartTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','anon');0.0",
                    "v:arr := f:subset((1, 2, 3, 4), 2)\r\n" +
                    "f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]"
                },
                expected: new List<object>()
                {
                    0.0,
                    2,
                    3.0,
                    4.0
                }
            );
        }

        [TestMethod]
        public void AnonSubsetStartCountTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','anon');0.0",
                    "v:arr := f:subset((1, 2, 3, 4), 1, 2)\r\n" +
                    "f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]"
                },
                expected: new List<object>()
                {
                    0.0,
                    2,
                    2.0,
                    3.0
                }
            );
        }

        [TestMethod]
        public void AppendTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:append((1, 2, 3, 4), 2)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    1.0,
                    2.0,
                    3.0,
                    4.0,
                    2.0
                }
            );
        }

        [TestMethod]
        public void AppendVarTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:basearr := (1,2,3,4) \r\n" +
                    "v:val := 2 \r\n" +
                    "v:arr := l:array.f:append(v:basearr, v:val)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]",
                    "v:val := 3 \r\n v:arr[4]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    1.0,
                    2.0,
                    3.0,
                    4.0,
                    2.0,
                    2.0
                }
            );
        }

        [TestMethod]
        public void InsertTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:insert((1, 2, 3, 4), 5, 1)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    1.0,
                    5.0,
                    2.0,
                    3.0,
                    4.0
                }
            );
        }

        [TestMethod]
        public void InsertVarTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:basearr := (1,2,3,4) \r\n" +
                    "v:val := 5 \r\n" +
                    "v:pos := 1 \r\n" +
                    "v:arr := l:array.f:insert(v:basearr, v:val, v:pos)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]",
                    "v:val := 3 \r\n v:arr[1]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    1.0,
                    5.0,
                    2.0,
                    3.0,
                    4.0,
                    5.0
                }
            );
        }

        [TestMethod]
        public void InsertFirstTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:insert((1, 2, 3, 4), 5, 0)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    5.0,
                    1.0,
                    2.0,
                    3.0,
                    4.0
                }
            );
        }

        [TestMethod]
        public void InsertLastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:insert((1, 2, 3, 4), 5, 4)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                    "v:arr[3]",
                    "v:arr[4]"
                },
                expected: new List<object>()
                {
                    0.0,
                    5,
                    1.0,
                    2.0,
                    3.0,
                    4.0,
                    5.0
                }
            );
        }

        [TestMethod]
        public void RemoveTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:remove((1, 2, 3, 4), 1)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                },
                expected: new List<object>()
                {
                    0.0,
                    3,
                    1.0,
                    3.0,
                    4.0,
                }
            );
        }

        [TestMethod]
        public void RemoveVarTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:basearr := (1,2,3,4) \r\n" +
                    "v:pos := 1 \r\n" +
                    "v:arr := l:array.f:remove(v:basearr, v:pos)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]"
                },
                expected: new List<object>()
                {
                    0.0,
                    3,
                    1.0,
                    3.0,
                    4.0,
                }
            );
        }

        [TestMethod]
        public void RemoveFirstTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:remove((1, 2, 3, 4), 0)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                },
                expected: new List<object>()
                {
                    0.0,
                    3,
                    2.0,
                    3.0,
                    4.0,
                }
            );
        }

        [TestMethod]
        public void RemoveLastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:array','l:array');0.0",
                    "v:arr := l:array.f:remove((1, 2, 3, 4), 3)\r\n" +
                    "l:array.f:length(v:arr)",
                    "v:arr[0]",
                    "v:arr[1]",
                    "v:arr[2]",
                },
                expected: new List<object>()
                {
                    0.0,
                    3,
                    1.0,
                    2.0,
                    3.0,
                }
            );
        }
    }
}
