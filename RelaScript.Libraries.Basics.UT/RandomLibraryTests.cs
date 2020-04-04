using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelaScript.UT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RelaScript;

namespace RelaScript.Libraries.Basics.UT
{
    [TestClass]
    public class RandomLibraryTests
    {
        [TestMethod]
        public void RandomTest()
        {
            IRandomProvider rp = new RandomBasic(0);
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:random','l:rand');0.0",
                    "l:rand.f:random(1.0, 2.0)",
                    "l:rand.f:random(-12.5, 125.0)",
                    "l:rand.f:random(-12.5, c:i(125))"
                },
                expected: new List<object>()
                {
                    0.0,
                    rp.RandomDouble(1.0,2.0),
                    rp.RandomDouble(-12.5,125.0),
                    rp.RandomDouble(-12.5,125.0)
                }
            );
        }

        [TestMethod]
        public void AnonRandomTest()
        {
            IRandomProvider rp = new RandomBasic(0);
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:random','anon');0.0",
                    "f:random(1.0, 2.0)",
                    "f:random(-12.5, 125.0)",
                    "f:random(-12.5, c:i(125))"
                },
                expected: new List<object>()
                {
                    0.0,
                    rp.RandomDouble(1.0,2.0),
                    rp.RandomDouble(-12.5,125.0),
                    rp.RandomDouble(-12.5,125.0)
                }
            );
        }

        [TestMethod]
        public void RandomIntTest()
        {
            IRandomProvider rp = new RandomBasic(0);
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:random','l:rand');0.0",
                    "l:rand.f:randomint(1, 4)",
                    "l:rand.f:randomint(-2, 8)",
                    "l:rand.f:randomint(-2, c:i(8))"
                },
                expected: new List<object>()
                {
                    0.0,
                    rp.RandomInt(1,4),
                    rp.RandomInt(-2,8),
                    rp.RandomInt(-2,8)
                }
            );
        }

        [TestMethod]
        public void RollTest()
        {
            IRandomProvider rp = new RandomBasic(0);
            RandomLibrary rl = new RandomLibrary();
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:random','l:rand');0.0",
                    "l:rand.f:roll(2, 6)",
                    "l:rand.f:roll(10, 10)"
                },
                expected: new List<object>()
                {
                    0.0,
                    rl.Roll(2, 6, rp),
                    rl.Roll(10, 10, rp)
                }
            );
        }
    }
}
