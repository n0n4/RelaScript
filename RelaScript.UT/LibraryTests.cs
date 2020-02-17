using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class LibraryTests
    {
        [TestMethod]
        public void InputLibraryPremadeTest()
        {
            InputContext ic = TestScaffold.ConstructContext();

            // create a library
            InputLibrary il = new InputLibrary("l:testlibrary");
            Exline ex = new Exline("a:0 + 5");
            ic.CompileLine(ex);
            il.Funcs.Add("f:add5", ex);

            // add the library to the inputcontext
            ic.SetLibrary("l:test", il);

            // test using the library
            Exline t1 = new Exline("l:test.f:add5(10.0)");
            Exline t2 = new Exline("l:test.f:add5(1.3)");

            ic.CompileLine(t1);
            ic.CompileLine(t2);

            double r1 = (double)t1.Execute(new object[] { 0 });
            double r2 = (double)t2.Execute(new object[] { 0 });

            Assert.AreEqual(15.0, r1);
            Assert.AreEqual(6.3, r2);
        }
    }
}
