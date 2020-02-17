using RelaScript.UT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using RelaScript;
using System;

namespace RelaScript.Libraries.Basics.UT
{
    [TestClass]
    public class MathLibraryTests
    {
        private void MathTestSingle(string fname, Func<double, double> func)
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0)",
                "l:math." + fname + "(-3)",
                "l:math." + fname + "(-3.9)",
                "l:math." + fname + "(13.3)",
                "v:test := 15.1; l:math." + fname + "(v:test)",
            },
            expected: new List<object>()
            {
                0.0,
                func(0),
                func(-3.0),
                func(-3.9),
                func(13.3),
                func(15.1)
            }
            );
        }

        private void MathTestDouble(string fname, Func<double, double, double> func)
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0, 0)",
                "l:math." + fname + "(-3, 1)",
                "l:math." + fname + "(13.3, 0.75)",
                "v:test := 15.1; l:math." + fname + "(v:test, 3.1)",
            },
            expected: new List<object>()
            {
                0.0,
                func(0, 0),
                func(-3.0, 1.0),
                func(13.3, 0.75),
                func(15.1, 3.1)
            }
            );
        }

        private void MathTestTriple(string fname, Func<double, double, double, double> func)
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0, 0, 0)",
                "l:math." + fname + "(-3, 1, 10)",
                "l:math." + fname + "(13.3, 0.75, 14.0)",
                "v:test := 15.1; l:math." + fname + "(v:test, 3.1, 12.1)",
            },
            expected: new List<object>()
            {
                0.0,
                func(0, 0, 0),
                func(-3.0, 1.0, 10.0),
                func(13.3, 0.75, 14.0),
                func(15.1, 3.1, 12.1)
            }
            );
        }

        private void MathTestTripleArray(string fname, Func<double, double, double, double[]> func, int expectedLength)
        {
            for (int i = 0; i < expectedLength; i++)
            {
                TestScaffold.TestLines(
                lines: new List<string>()
                {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0, 0, 0)[" + i +"]",
                "l:math." + fname + "(-3, 1, 10)[" + i +"]",
                "l:math." + fname + "(13.3, 0.75, 14.0)[" + i +"]",
                "v:test := 15.1; l:math." + fname + "(v:test, 3.1, 12.1)[" + i +"]",
                },
                expected: new List<object>()
                {
                0.0,
                func(0, 0, 0)[i],
                func(-3.0, 1.0, 10.0)[i],
                func(13.3, 0.75, 14.0)[i],
                func(15.1, 3.1, 12.1)[i]
                }
                );
            }
        }

        private void MathTestQuad(string fname, Func<double, double, double, double, double> func)
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0, 0, 0, 0)",
                "l:math." + fname + "(-3, 1, 10, 3.1)",
                "l:math." + fname + "(13.3, 0.75, 14.0, -0.5)",
                "v:test := 15.1; l:math." + fname + "(v:test, 3.1, 12.1, 0.75)",
            },
            expected: new List<object>()
            {
                0.0,
                func(0, 0, 0, 0),
                func(-3.0, 1.0, 10.0, 3.1),
                func(13.3, 0.75, 14.0, -0.5),
                func(15.1, 3.1, 12.1, 0.75)
            }
            );
        }

        private void MathTestQuadArray(string fname, Func<double, double, double, double, double[]> func, int expectedLength)
        {
            for (int i = 0; i < expectedLength; i++)
            {
                TestScaffold.TestLines(
                lines: new List<string>()
                {
                "import basic:math l:math;0.0",
                "l:math." + fname + "(0, 0, 0, 0)[" + i +"]",
                "l:math." + fname + "(-3, 1, 10, 3.1)[" + i +"]",
                "l:math." + fname + "(13.3, 0.75, 14.0, -0.5)[" + i +"]",
                "v:test := 15.1; l:math." + fname + "(v:test, 3.1, 12.1, 0.75)[" + i +"]",
                },
                expected: new List<object>()
                {
                0.0,
                func(0, 0, 0, 0)[i],
                func(-3.0, 1.0, 10.0, 3.1)[i],
                func(13.3, 0.75, 14.0, -0.5)[i],
                func(15.1, 3.1, 12.1, 0.75)[i]
                }
                );
            }
        }


        [TestMethod]
        public void SinTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:sin(1.0)",
                    "l:math.f:sin(12.5)",
                    "l:math.f:sin(c:i(8))"
                },
                expected: new List<object>()
                {
                    0.0,
                    Math.Sin(1.0),
                    Math.Sin(12.5),
                    Math.Sin(8)
                }
            );
        }

        [TestMethod]
        public void CosTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:cos(1.0)",
                    "l:math.f:cos(12.5)",
                    "l:math.f:cos(c:i(8))"
                },
                expected: new List<object>()
                {
                    0.0,
                    Math.Cos(1.0),
                    Math.Cos(12.5),
                    Math.Cos(8)
                }
            );
        }

        [TestMethod]
        public void TanTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:tan(1.0)",
                    "l:math.f:tan(12.5)",
                    "l:math.f:tan(c:i(8))"
                },
                expected: new List<object>()
                {
                    0.0,
                    Math.Tan(1.0),
                    Math.Tan(12.5),
                    Math.Tan(8)
                }
            );
        }

        [TestMethod]
        public void AbsTest()
        {
            TestScaffold.TestLinesOrderAndSequential(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:abs(-1.0)",
                    "l:math.f:abs(-12.5)",
                    "l:math.f:abs(c:i(-8))",
                    "l:math.f:abs(1.0)",
                    "l:math.f:abs(12.5)",
                    "l:math.f:abs(c:i(8))"
                },
                expected: new List<object>()
                {
                    0.0,
                    1.0,
                    12.5,
                    8.0,
                    1.0,
                    12.5,
                    8.0
                }
            );
        }

        [TestMethod]
        public void MinTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:min(-1.0)",
                    "l:math.f:min(-1.0, 5.0)",
                    "l:math.f:min(7.0, 3.0, 1.0)",
                    "l:math.f:min(10, 15, 300, 6)",
                    "v:test := 5; v:a := 3; l:math.f:min(12.5, v:test, v:a)",
                },
                expected: new List<object>()
                {
                    0.0,
                    -1.0,
                    -1.0,
                    1.0,
                    6.0,
                    3.0
                }
            );
        }

        [TestMethod]
        public void MaxTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:max(-1.0)",
                    "l:math.f:max(-1.0, 5.0)",
                    "l:math.f:max(7.0, 3.0, 1.0)",
                    "l:math.f:max(10, 15, 300, 6)",
                    "v:test := 15; v:a := 3; l:math.f:max(12.5, v:test, v:a)",
                },
                expected: new List<object>()
                {
                    0.0,
                    -1.0,
                    5.0,
                    7.0,
                    300.0,
                    15.0
                }
            );
        }

        [TestMethod]
        public void AvgTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "f:import('basic:math','l:math');0.0",
                    "l:math.f:avg(-1.0)",
                    "l:math.f:avg(-1.0, 5.0)",
                    "l:math.f:avg(7.0, 3.0, 1.0)",
                    "l:math.f:avg(10, 15, 300, 6)",
                    "v:test := 15; v:a := 3; l:math.f:avg(12.5, v:test, v:a)",
                },
                expected: new List<object>()
                {
                    0.0,
                    -1.0,
                    (-1.0 + 5.0) / 2.0,
                    (7.0 + 3.0 + 1.0) / 3.0,
                    (10.0 + 15.0 + 300.0 + 6.0) / 4.0,
                    (12.5 + 15.0 + 3.0) / 3.0
                }
            );
        }

        [TestMethod]
        public void AvgShorthandImportTest()
        {
            TestScaffold.TestLines(
                lines: new List<string>()
                {
                    "import basic:math l:math;0.0",
                    "l:math.f:avg(-1.0)",
                    "l:math.f:avg(-1.0, 5.0)",
                    "l:math.f:avg(7.0, 3.0, 1.0)",
                    "l:math.f:avg(10, 15, 300, 6)",
                    "v:test := 15; v:a := 3; l:math.f:avg(12.5, v:test, v:a)",
                },
                expected: new List<object>()
                {
                    0.0,
                    -1.0,
                    (-1.0 + 5.0) / 2.0,
                    (7.0 + 3.0 + 1.0) / 3.0,
                    (10.0 + 15.0 + 300.0 + 6.0) / 4.0,
                    (12.5 + 15.0 + 3.0) / 3.0
                }
            );
        }

        [TestMethod]
        public void ClausenTest()
        {
            MathLibrary ml = new MathLibrary();
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math.f:clausen(-1.0)",
                "l:math.f:clausen(-1.0, 5.0)",
                "l:math.f:clausen(7.0, 3.0, 1.0)",
                "l:math.f:clausen(10, 15, 300)",
                "v:test := 15; v:a := 3; l:math.f:clausen(12.5, v:test, v:a)",
            },
            expected: new List<object>()
            {
                0.0,
                ml.Clausen(new double[]{ -1.0 }),
                ml.Clausen(new double[]{ -1.0, 5.0 }),
                ml.Clausen(new double[]{ 7.0, 3.0, 1.0 }),
                ml.Clausen(new double[]{ 10.0, 15.0, 300.0 }),
                ml.Clausen(new double[]{ 12.5, 15.0, 3.0 }),
            }
            );
        }

        [TestMethod]
        public void CycloidTest()
        {
            MathLibrary ml = new MathLibrary();
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math.f:cycloid(-1.0, 0.75)",
                "l:math.f:cycloid(-1.0, 5.0)",
                "l:math.f:cycloid(7.0, 3.0)",
                "l:math.f:cycloid(10, 15)",
                "v:test := 15; v:a := 3; l:math.f:cycloid(v:test, v:a)",
            },
            expected: new List<object>()
            {
                0.0,
                ml.Cycloid(-1.0, 0.75),
                ml.Cycloid(-1.0, 5.0),
                ml.Cycloid(7.0, 3.0),
                ml.Cycloid(10.0, 15.0),
                ml.Cycloid(15.0, 3.0)
            }
            );
        }

        [TestMethod]
        public void SquareWaveTest()
        {
            MathLibrary ml = new MathLibrary();
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math.f:squarewave(0)",
                "l:math.f:squarewave(-3)",
                "l:math.f:squarewave(13.3)",
                "v:test := 15.1; l:math.f:squarewave(v:test)",
            },
            expected: new List<object>()
            {
                0.0,
                ml.SquareWave(0),
                ml.SquareWave(-3.0),
                ml.SquareWave(13.3),
                ml.SquareWave(15.1)
            }
            );
        }

        [TestMethod]
        public void TriangleWaveTest()
        {
            MathLibrary ml = new MathLibrary();
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math.f:trianglewave(0)",
                "l:math.f:trianglewave(-3)",
                "l:math.f:trianglewave(13.3)",
                "v:test := 15.1; l:math.f:trianglewave(v:test)",
            },
            expected: new List<object>()
            {
                0.0,
                ml.TriangleWave(0),
                ml.TriangleWave(-3.0),
                ml.TriangleWave(13.3),
                ml.TriangleWave(15.1)
            }
            );
        }

        [TestMethod]
        public void SawtoothWaveTest()
        {
            MathLibrary ml = new MathLibrary();
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "import basic:math l:math;0.0",
                "l:math.f:sawtoothwave(0)",
                "l:math.f:sawtoothwave(-3)",
                "l:math.f:sawtoothwave(13.3)",
                "v:test := 15.1; l:math.f:sawtoothwave(v:test)",
            },
            expected: new List<object>()
            {
                0.0,
                ml.SawtoothWave(0),
                ml.SawtoothWave(-3.0),
                ml.SawtoothWave(13.3),
                ml.SawtoothWave(15.1)
            }
            );
        }

        [TestMethod]
        public void SignTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:sign", x => { return ml.Sign(x); });
        }

        [TestMethod]
        public void PowTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestDouble("f:pow", (x, y) => { return ml.Pow(x, y); });
        }

        [TestMethod]
        public void SqrtTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:sqrt", x => { return ml.Sqrt(x); });
        }

        [TestMethod]
        public void LnTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:ln", x => { return ml.Ln(x); });
        }

        [TestMethod]
        public void Log10Test()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:log10", x => { return ml.Log10(x); });
        }

        [TestMethod]
        public void LogTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestDouble("f:log", (x, y) => { return ml.Log(x, y); });
        }

        [TestMethod]
        public void RoundTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:round", x => { return ml.Round(x); });
        }

        [TestMethod]
        public void FloorTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:floor", x => { return ml.Floor(x); });
        }

        [TestMethod]
        public void CeilingTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:ceiling", x => { return ml.Ceiling(x); });
        }

        [TestMethod]
        public void AcosTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:acos", x => { return ml.Acos(x); });
        }

        [TestMethod]
        public void AsinTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:asin", x => { return ml.Asin(x); });
        }

        [TestMethod]
        public void AtanTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:atan", x => { return ml.Atan(x); });
        }

        [TestMethod]
        public void Atan2Test()
        {
            MathLibrary ml = new MathLibrary();
            MathTestDouble("f:atan2", (x, y) => { return ml.Atan2(x, y); });
        }

        [TestMethod]
        public void CoshTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:cosh", x => { return ml.Cosh(x); });
        }

        [TestMethod]
        public void SinhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:sinh", x => { return ml.Sinh(x); });
        }

        [TestMethod]
        public void TanhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:tanh", x => { return ml.Tanh(x); });
        }

        [TestMethod]
        public void AsinhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:asinh", x => { return ml.Asinh(x); });
        }

        [TestMethod]
        public void AcoshTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:acosh", x => { return ml.Acosh(x); });
        }

        [TestMethod]
        public void AtanhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:atanh", x => { return ml.Atanh(x); });
        }

        [TestMethod]
        public void AcothTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:acoth", x => { return ml.Acoth(x); });
        }

        [TestMethod]
        public void AsechTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:asech", x => { return ml.Asech(x); });
        }

        [TestMethod]
        public void AcschTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:acsch", x => { return ml.Acsch(x); });
        }

        [TestMethod]
        public void DasinhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:dasinh", x => { return ml.Dasinh(x); });
        }

        [TestMethod]
        public void DacoshTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:dacosh", x => { return ml.Dacosh(x); });
        }

        [TestMethod]
        public void DatanhTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:datanh", x => { return ml.Datanh(x); });
        }

        [TestMethod]
        public void DacothTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:dacoth", x => { return ml.Dacoth(x); });
        }

        [TestMethod]
        public void DasechTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:dasech", x => { return ml.Dasech(x); });
        }

        [TestMethod]
        public void DacschTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:dacsch", x => { return ml.Dacsch(x); });
        }

        [TestMethod]
        public void CotTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:cot", x => { return ml.Cot(x); });
        }

        [TestMethod]
        public void SecTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:sec", x => { return ml.Sec(x); });
        }

        [TestMethod]
        public void CscTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:csc", x => { return ml.Csc(x); });
        }

        [TestMethod]
        public void CasTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:cas", x => { return ml.Cas(x); });
        }

        [TestMethod]
        public void ExpTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestSingle("f:exp", x => { return ml.Exp(x); });
        }

        [TestMethod]
        public void ClampTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestTriple("f:clamp", (x, y, z) => { return ml.Clamp(x, y, z); });
        }

        [TestMethod]
        public void BlendTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestTriple("f:blend", (x, y, z) => { return ml.Blend(x, y, z); });
        }

        [TestMethod]
        public void TrochoidTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestTripleArray("f:trochoid", (x, y, z) => { return ml.Trochoid(x, y, z); }, 2);
            MathTestTriple("f:trochoidx", (x, y, z) => { return ml.TrochoidX(x, y, z); });
            MathTestTriple("f:trochoidy", (x, y, z) => { return ml.TrochoidY(x, y, z); });
        }

        [TestMethod]
        public void HypotrochoidTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestQuadArray("f:hypotrochoid", (x, y, z, a) => { return ml.Hypotrochoid(x, y, z, a); }, 2);
            MathTestQuad("f:hypotrochoidx", (x, y, z, a) => { return ml.HypotrochoidX(x, y, z, a); });
            MathTestQuad("f:hypotrochoidy", (x, y, z, a) => { return ml.HypotrochoidY(x, y, z, a); });
        }

        [TestMethod]
        public void EpitrochoidTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestQuadArray("f:epitrochoid", (x, y, z, a) => { return ml.Epitrochoid(x, y, z, a); }, 2);
            MathTestQuad("f:epitrochoidx", (x, y, z, a) => { return ml.EpitrochoidX(x, y, z, a); });
            MathTestQuad("f:epitrochoidy", (x, y, z, a) => { return ml.EpitrochoidY(x, y, z, a); });
        }

        [TestMethod]
        public void EpicycloidTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestTripleArray("f:epicycloid", (x, y, z) => { return ml.Epicycloid(x, y, z); }, 2);
            MathTestTriple("f:epicycloidx", (x, y, z) => { return ml.EpicycloidX(x, y, z); });
            MathTestTriple("f:epicycloidy", (x, y, z) => { return ml.EpicycloidY(x, y, z); });
        }

        [TestMethod]
        public void HypocycloidTest()
        {
            MathLibrary ml = new MathLibrary();
            MathTestTripleArray("f:hypocycloid", (x, y, z) => { return ml.Hypocycloid(x, y, z); }, 2);
            MathTestTriple("f:hypocycloidx", (x, y, z) => { return ml.HypocycloidX(x, y, z); });
            MathTestTriple("f:hypocycloidy", (x, y, z) => { return ml.HypocycloidY(x, y, z); });
        }
    }
}
