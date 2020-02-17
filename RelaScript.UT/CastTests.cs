using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class CastTests
    {
        [TestMethod]
        public void VarCastTest()
        {
            // something important to note here
            // vars unbox as object
            // you can't go straight from object<double> to (int) in csharp
            // so we can't do c:i(v:test), we have to do c:i(c:d(v:test))
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:o(v:test)",
                    "c:d(v:test)",
                    "c:i(c:d(v:test))",
                    "v:test",
                    "f:newvar(v:testint, c:i(3)); c:i(v:testint)",
                    "c:d(c:i(v:testint))"
                },
                expected: new List<object>()
                {
                    15.0,
                    15.0,
                    15,
                    new InputVar("v:test", 15.0),
                    3,
                    3.0
                },
                vars: new Dictionary<string, double>()
                {
                    { "v:test", 15.0 }
                }
            );
        }

        [TestMethod]
        public void IntCastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:i(-15.0)",
                    "c:i(2.5+2.5)",
                    "c:i(3.1)"
                },
                expected: new List<object>()
                {
                    -15,
                    5,
                    3
                }
            );
        }

        [TestMethod]
        public void DoubleCastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:d(15)",
                    "c:d(-5)",
                    "c:d(c:i(3.15))"
                },
                expected: new List<object>()
                {
                    15.0,
                    -5.0,
                    3.0
                }
            );
        }

        [TestMethod]
        public void StringCastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:s(15.5)",
                    "c:s(c:i(17))",
                    "c:s(-2 * 3)"
                },
                expected: new List<object>()
                {
                    "15.5",
                    "17",
                    "-6"
                }
            );
        }

        [TestMethod]
        public void StringToDoubleCastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:d('11.1')",
                    "c:d('-2.5')",
                    "c:d('31' + '13.3')"
                },
                expected: new List<object>()
                {
                    11.1,
                    -2.5,
                    3113.3
                }
            );
        }

        [TestMethod]
        public void StringToIntCastTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "c:i('15')",
                    "c:i('2' + '5')",
                    "c:i('-3')"
                },
                expected: new List<object>()
                {
                    15,
                    25,
                    -3
                }
            );
        }
    }
}
