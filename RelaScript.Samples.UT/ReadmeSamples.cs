using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RelaScript.Libraries.Basics;

namespace RelaScript.Samples.UT
{
    [TestClass]
    public class ReadmeSamples
    {
        [TestMethod]
        public void GettingStarted()
        {
            InputContext context = new InputContext();

            // random must be provided if you want to use functions that require random numbers
            context.Random = new RandomBasic(0); // replace 0 with your random seed

            // load library providers-- this controls what libraries may be imported into scripts
            // you may write your own libraries to allow RelaScript to interface with your project
            context.LibraryProviders.Add(new LibraryProviderBasic());

            string scriptText = "a:0 * 2"; // load your script

            // an Exline represents a script file
            Exline exscript = new Exline(scriptText);

            // the Exline must be compiled before it can be run
            context.CompileLine(exscript);

            // args are passed into the script upon execution
            object[] args = new object[] { 2.0 };

            object result = exscript.Execute(args);
            // result --> 4.0 (double)

            Assert.AreEqual(4.0, result);
        }
    }
}
