using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class BoolTests
    {
        [TestMethod]
        public void BoolShorthandTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "true",
                "false",
                "if(true) { 1 } else { 2 }",
                "if(false) { 1 } else { 2 }"
            },
            expected: new List<object>()
            {
                true,
                false,
                1.0,
                2.0
            });
        }

        [TestMethod]
        public void BoolTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "b:t",
                "b:f",
                "if(b:t) { 1 } else { 2 }",
                "if(b:f) { 1 } else { 2 }",
                "b:true",
                "b:false",
                "if(b:true) { 1 } else { 2 }",
                "if(b:false) { 1 } else { 2 }"
            },
            expected: new List<object>()
            {
                true,
                false,
                1.0,
                2.0,
                true,
                false,
                1.0,
                2.0
            });
        }
    }
}
