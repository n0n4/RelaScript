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
    public class BasicLibraryTests
    {
        [TestMethod]
        public void LibraryMethodTakeAllArgsTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math')\r\n" +
                    "f:mpow := { l:math.f:pow(a:all) * 2 }\r\n" +
                    "f:mpow(10, 2)",
                },
                expected: new List<object>()
                {
                    200.0,
                }
            );
        }

        [TestMethod]
        public void AnonLibraryOptionalParameterFuncTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "import basic:string anon\r\n" +
                    "f:substring('boathouse', 2, 4)",
                    "f:substring('boathouse', 2)",
                },
                expected: new List<object>()
                {
                    "atho",
                    "athouse"
                }
            );
        }
    }
}
