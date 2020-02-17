using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class RandomProviderTests
    {
        private void RandomIntConsistencyTest(IRandomProvider r)
        {
            int mintries = 1000;
            int maxtries = 10000;
            int min = 1;
            int max = 6;
            bool foundmin = false;
            bool foundmax = false;
            for (int i = 0; i < maxtries; i++)
            {
                int next = r.RandomInt(min, max);
                Assert.IsTrue(next >= min && next <= max);
                if (next == max)
                    foundmax = true;
                if (next == min)
                    foundmin = true;
                if (foundmax && foundmin && i > mintries)
                    break;
            }
            Assert.IsTrue(foundmin);
            Assert.IsTrue(foundmax);
        }

        private void RandomDoubleConsistencyTest(IRandomProvider r)
        {
            int maxtries = 1000;
            double min = 1.2;
            double max = 6.4;
            for (int i = 0; i < maxtries; i++)
            {
                double next = r.RandomDouble(min, max);
                Assert.IsTrue(next >= min && next <= max);
            }
        }

        [TestMethod]
        public void RandomBasicTest()
        {
            RandomBasic r = new RandomBasic(0);
            RandomIntConsistencyTest(r);
            RandomDoubleConsistencyTest(r);
        }
    }
}
