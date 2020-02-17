using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class SoloBenchmarks
    {
        [TestMethod]
        public void AllRounder()
        {
            int runcount = 100000;
            List<string> testlines = new List<string>();
            List<string> descs = new List<string>();

            descs.Add("Simple Math 1");
            testlines.Add("1 + 1 * 2 - 3 / 4");

            descs.Add("Simple Math 2 - Multiline");
            testlines.Add("1 - 3; 4 * 4; 1 + 1 + 8 - 3");

            descs.Add("Simple Math 3 - Parens");
            testlines.Add("1 + (2 - 5 * (8 + 3) + (1 - 3))");

            descs.Add("Simple Math 4 - Scopes");
            testlines.Add("10 + {3 - 4 * {2 + 1}}");

            descs.Add("Var 1");
            testlines.Add("v:alpha := 10; v:alpha += 3; v:alpha");

            descs.Add("Var 2 - Many Vars");
            testlines.Add("v:r := 0.8; v:g := 0.3; v:b := 0.1; v:r + (v:g - v:b) - (v:b * v:r)");

            descs.Add("Func 1");
            testlines.Add("f:test := {5}; f:test()");

            descs.Add("Func 2 - Arg");
            testlines.Add("f:test := {a:0 + 2}; f:test(10)");

            descs.Add("Func 3 - Args");
            testlines.Add("f:test := {a:0 - a:1 * a:2}; f:test(1,2,3)");

            descs.Add("Func 4 - Var");
            testlines.Add("v:red := 1; f:test := {v:red += 10}; f:test()");

            descs.Add("Func 5 - Builtin");
            testlines.Add("f:random(1,2)");

            descs.Add("Lib 1");
            testlines.Add("import basic:math l:math; l:math.f:abs(-1)");

            descs.Add("Control 1 - If");
            testlines.Add("if (b:t) {1} else {2}");

            descs.Add("Control 2 - While");
            testlines.Add("v:i := 0; while (v:i < 10) { v:i += 1 }; v:i");

            // TODO: defn tests can't be performed because redefining in the same context 
            // throws an exception. Need a separate test for these which discards the 
            // list of defns after each iteration
            /*descs.Add("Defn 1 - Compile Empty");
            testlines.Add("defn d:empty {}");

            descs.Add("Defn 1 - Compile Simple");
            testlines.Add("defn d:simple {f:half := {a:0 / 2}}");

            descs.Add("Defn 1 - Compile Counter");
            testlines.Add("defn d:counter {v:c := 0; f:init := {a:startvalue v:c = a:startvalue} f:add := {v:c += 1}}");
            */

            double[] compiletimes = new double[testlines.Count];
            double[] runtimes = new double[testlines.Count];
            Stopwatch watch = new Stopwatch();
            object[] args = new object[] { 0 };
            for (int i = 0; i < testlines.Count; i++)
            {
                watch.Start();
                InputContext context = TestScaffold.ConstructContext();
                Exline ex = new Exline(testlines[i]);
                context.CompileLine(ex);
                watch.Stop();
                compiletimes[i] = (double)watch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
                watch.Reset();
                
                watch.Start();
                for (int o = 0; o < runcount; o++)
                {
                    ex.Execute(args);
                }
                watch.Stop();
                runtimes[i] = (double)watch.ElapsedTicks / (double)TimeSpan.TicksPerMillisecond;
            }

            // make the prettyprint
            string pretty = "NO\tCOMPILE\tRUN\t(per 1)\tDESC\r\n";
            for(int i = 0; i < testlines.Count; i++)
            {
                pretty += "" + i + "\t" + compiletimes[i] + "\t" + runtimes[i] + "\t" + (runtimes[i] / (double)runcount) + "\t" + descs[i] + "\r\n";
            }

            pretty += "DONE";
        }
    }
}
