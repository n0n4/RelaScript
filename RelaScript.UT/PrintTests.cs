using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelaScript.Printers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class PrintTests
    {
        [TestMethod]
        public void PrintLogTest()
        {
            InputContext context = TestScaffold.ConstructContext();
            PrintReceiverLog logger = new PrintReceiverLog();
            context.PrintChannel.Register(logger);

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:print('this place is a message') \r\n" +
                    "f:print('... and part of a system of messages')"
                },
                expected: new List<object>()
                {
                    0,
                },
                context: context
            );

            Assert.IsTrue(logger.Logs.Count == 2);
            Assert.AreEqual(logger.Logs[0], "this place is a message");
            Assert.AreEqual(logger.Logs[1], "... and part of a system of messages");
        }

        [TestMethod]
        public void PrintLogVarTest()
        {
            InputContext context = TestScaffold.ConstructContext();
            PrintReceiverLog logger = new PrintReceiverLog();
            context.PrintChannel.Register(logger);

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "v:str := 'this place is not a place of honor' \r\n" +
                    "f:print(v:str)"
                },
                expected: new List<object>()
                {
                    0,
                },
                context: context
            );

            Assert.IsTrue(logger.Logs.Count == 1);
            Assert.AreEqual(logger.Logs[0], "this place is not a place of honor");
        }

        [TestMethod]
        public void PrintLogHigherScopeTest()
        {
            InputContext context = TestScaffold.ConstructContext();
            PrintReceiverLog logger = new PrintReceiverLog();
            context.PrintChannel.Register(logger);

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "{ f:print('... no highly esteemed deed is commemorated here') }"
                },
                expected: new List<object>()
                {
                    0,
                },
                context: context
            );

            Assert.IsTrue(logger.Logs.Count == 1);
            Assert.AreEqual(logger.Logs[0], "... no highly esteemed deed is commemorated here");
        }

        [TestMethod]
        public void PrintLogInObjectTest()
        {
            InputContext context = TestScaffold.ConstructContext();
            PrintReceiverLog logger = new PrintReceiverLog();
            context.PrintChannel.Register(logger);

            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "defn d:printer { f:init := { f:print('... nothing valued is here') } } \r\n" +
                    "v:p := object anon d:printer ()\r\n" +
                    "0.0"
                },
                expected: new List<object>()
                {
                    0.0,
                },
                context: context
            );

            Assert.IsTrue(logger.Logs.Count == 1);
            Assert.AreEqual(logger.Logs[0], "... nothing valued is here");
        }
    }
}
