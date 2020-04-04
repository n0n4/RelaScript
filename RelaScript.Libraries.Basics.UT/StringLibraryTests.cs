using RelaScript.UT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.Libraries.Basics.UT
{
    [TestClass]
    public class StringLibraryTests
    {
        [TestMethod]
        public void LengthTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','l:string');0.0",
                    "l:string.f:length('rare')",
                    "l:string.f:length('boat' + 'house')",
                    "l:string.f:length(c:s(c:i(12)))"
                },
                expected: new List<object>()
                {
                    0.0,
                    4,
                    9,
                    2
                }
            );
        }

        [TestMethod]
        public void SubstringTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','l:string');0.0",
                    "l:string.f:substring('rare',c:i(1))",
                    "l:string.f:substring('boat' + 'house',2,4)",
                    "l:string.f:substring(c:s(c:i(12)),1)"
                },
                expected: new List<object>()
                {
                    0.0,
                    "are",
                    "atho",
                    "2"
                }
            );
        }

        [TestMethod]
        public void AnonSubstringTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','anon');0.0",
                    "f:substring('rare',c:i(1))",
                    "f:substring('boat' + 'house',2,4)",
                    "f:substring(c:s(c:i(12)),1)"
                },
                expected: new List<object>()
                {
                    0.0,
                    "are",
                    "atho",
                    "2"
                }
            );
        }

        [TestMethod]
        public void CommaTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','l:string');0.0",
                    "l:string.f:comma('rare','boat','house')",
                    "l:string.f:comma('boat' + 'house')",
                    "l:string.f:comma(c:s(c:i(12)),'0')",
                    "l:string.f:comma(('a','b','c'))"
                },
                expected: new List<object>()
                {
                    0.0,
                    "rare, boat, house",
                    "boathouse",
                    "12, 0",
                    "a, b, c"
                }
            );
        }

        [TestMethod]
        public void CommaAndTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','l:string');0.0",
                    "l:string.f:commaand('rare','boat','house')",
                    "l:string.f:commaand('boat' + 'house')",
                    "l:string.f:commaand(c:s(c:i(12)),'0')",
                    "l:string.f:commaand(('a','b','c'))"
                },
                expected: new List<object>()
                {
                    0.0,
                    "rare, boat, and house",
                    "boathouse",
                    "12, and 0",
                    "a, b, and c"
                }
            );
        }

        [TestMethod]
        public void ReplaceTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:string','l:string');0.0",
                    "l:string.f:replace('rareboat','boat','house')",
                    "l:string.f:replace('ababa' + 'hat','a','A')",
                    "l:string.f:replace(c:s(c:i(12)),'1','2')",
                    "l:string.f:replace('who is not right',' not','')"
                },
                expected: new List<object>()
                {
                    0.0,
                    "rarehouse",
                    "AbAbAhAt",
                    "22",
                    "who is right"
                }
            );
        }
    }
}
