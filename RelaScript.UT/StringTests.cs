using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        public void BasicStringTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "\"some text\"",
                    "(\"yup\")",
                    "\"2 * f:sin(1 - (3 * 4)) - 1\""
                }, 
                expected: new List<object>()
                {
                    "some text",
                    "yup",
                    "2 * f:sin(1 - (3 * 4)) - 1"
                }
            );
        }

        [TestMethod]
        public void ConcatTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "\"some text\" + \" some more\"",
                    "(\"yup\") + \" you got it\"",
                    "\"c\" + (\"a\" + \"b\")",
                    "\"1\" + \"2\" + \"3\" + \"4\""
                },
                expected: new List<object>()
                {
                    "some text some more",
                    "yup you got it",
                    "cab",
                    "1234"
                }
            );
        }

        [TestMethod]
        public void ParseTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "c:i('33')",
                    "c:d('15.5')",
                    "c:i('-5')",
                    "c:d('-11.11')",
                    "c:i('0')",
                    "c:d('0')",
                    "c:i('10' + '3')",
                    "c:d('5' + '13.' +  '75')"
                },
                expected: new List<object>()
                {
                    33,
                    15.5,
                    -5,
                    -11.11,
                    0,
                    0.0,
                    103,
                    513.75
                }
            );
        }

        [TestMethod]
        public void SingleQuoteTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "'local hero'",
                    "('yes')",
                    "'2 * f:sin(1 - (3 * 4)) - 1'",
                    "'show me the \" right here'",
                    "'a' + 'b'",
                    "'a' + 'b' + \"c\"",
                    "\"some singles '''\""
                },
                expected: new List<object>()
                {
                    "local hero",
                    "yes",
                    "2 * f:sin(1 - (3 * 4)) - 1",
                    "show me the \" right here",
                    "ab",
                    "abc",
                    "some singles '''"
                }
            );
        }

        [TestMethod]
        public void ParenQuoteTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "'a' + ('b)' + 'c')"
                },
                expected: new List<object>()
                {
                    "ab)c"
                }
            );
        }

        [TestMethod]
        public void BraceQuoteTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "'a' + {'b}' + 'c'}"
                },
                expected: new List<object>()
                {
                    "ab}c"
                }
            );
        }
    }
}
