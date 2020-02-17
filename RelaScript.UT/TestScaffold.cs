using RelaScript.Libraries.Basics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    public static class TestScaffold
    {
        public static InputContext ConstructContext()
        {
            InputContext context = new InputContext();
            context.Random = new RandomBasic(0); // we always use randombasic with 0 seed

            // load library providers
            context.LibraryProviders.Add(new LibraryProviderBasic());

            return context;
        }

        public static List<double> TestDoubleLines(List<string> lines, 
            List<object[]> args = null, // if non null, pass these args to respective lines
            List<double> expected = null, // if non null, compare results to these
            Dictionary<string,double> vars = null, Dictionary<string,string> funcs = null)
        {
            // construct a context
            InputContext context = ConstructContext();

            // load vars if they exist
            if (vars != null)
                foreach (var kvp in vars)
                    context.DefineVar(kvp.Key, kvp.Value);

            // load funcs if they exist
            if (funcs != null)
                foreach (var kvp in funcs)
                    context.DefineFunc(kvp.Key, kvp.Value);

            // build and compile the lines
            List<Exline> es = new List<Exline>();
            foreach (string s in lines)
            {
                Exline e = new Exline(s);
                es.Add(e);
                context.CompileLine(e);
            }

            // run the lines
            List<double> rs = new List<double>();
            for(int i = 0; i < lines.Count; i++)
            {
                object[] arg = new object[] { 0.0 };
                if (args != null)
                    arg = args[i];
                double r = (double)es[i].Execute(arg);
                rs.Add(r);
                // compare to expected, if expected values are provided
                if(expected != null)
                    Assert.AreEqual(expected[i], r);
            }

            return rs;
        }

        public static List<double> TestDoubleLineRepeat(string line, int count,
            object[] args = null,
            Dictionary<string, double> vars = null, Dictionary<string, string> funcs = null)
        {
            // construct a context
            InputContext context = ConstructContext();

            // load vars if they exist
            if (vars != null)
                foreach (var kvp in vars)
                    context.DefineVar(kvp.Key, kvp.Value);

            // load funcs if they exist
            if (funcs != null)
                foreach (var kvp in funcs)
                    context.DefineFunc(kvp.Key, kvp.Value);

            // build and compile the line
            Exline e = new Exline(line);
            context.CompileLine(e);

            // run the line
            object[] arg = new object[] { 0.0 };
            if (args != null)
                arg = args;
            List<double> rs = new List<double>();
            for (int i = 0; i < count; i++)
            {
                double r = (double)e.Execute(arg);
                rs.Add(r);
            }

            return rs;
        }

        public static List<object> TestLines(List<string> lines,
            List<object[]> args = null, // if non null, pass these args to respective lines
            List<object> expected = null, // if non null, compare results to these
            Dictionary<string, double> vars = null, Dictionary<string, string> funcs = null)
        {
            // construct a context
            InputContext context = ConstructContext();

            // load vars if they exist
            if (vars != null)
                foreach (var kvp in vars)
                    context.DefineVar(kvp.Key, kvp.Value);

            // load funcs if they exist
            if (funcs != null)
                foreach (var kvp in funcs)
                    context.DefineFunc(kvp.Key, kvp.Value);

            // build and compile the lines
            List<Exline> es = new List<Exline>();
            foreach (string s in lines)
            {
                Exline e = new Exline(s);
                es.Add(e);
                context.CompileLine(e);
            }

            // run the lines
            List<object> rs = new List<object>();
            for (int i = 0; i < lines.Count; i++)
            {
                object[] arg = new object[] { 0.0 };
                if (args != null)
                    arg = args[i];
                object r = es[i].Execute(arg);
                rs.Add(r);
                // compare to expected, if expected values are provided
                if (expected != null)
                    Assert.AreEqual(expected[i], r);
            }

            return rs;
        }

        public static List<object> TestLinesAndElideSemicolons(List<string> lines,
            List<object[]> args = null, // if non null, pass these args to respective lines
            List<object> expected = null, // if non null, compare results to these
            Dictionary<string, double> vars = null, Dictionary<string, string> funcs = null)
        {
            List<object> o1 = TestLines(lines, args, expected, vars, funcs);
            for (int i = 0; i < lines.Count; i++)
                lines[i] = lines[i].Replace(";", " ");
            List<object> o2 = TestLines(lines, args, expected, vars, funcs);
            for (int i = 0; i < o1.Count; i++)
                Assert.AreEqual(o1[i], o2[i]);
            return o1;
        }

        public static List<object> TestLinesSequential(List<string> lines,
            List<object[]> args = null, // if non null, pass these args to respective lines
            List<object> expected = null, // if non null, compare results to these
            Dictionary<string, double> vars = null, Dictionary<string, string> funcs = null)
        {
            // construct a context
            InputContext context = ConstructContext();

            // load vars if they exist
            if (vars != null)
                foreach (var kvp in vars)
                    context.DefineVar(kvp.Key, kvp.Value);

            // load funcs if they exist
            if (funcs != null)
                foreach (var kvp in funcs)
                    context.DefineFunc(kvp.Key, kvp.Value);

            List<object> rs = new List<object>();
            // run all lines in order
            for (int i = 0; i < lines.Count; i++)
            {
                Exline e = new Exline(lines[i]);
                context.CompileLine(e);
                object[] arg = new object[] { 0.0 };
                if (args != null)
                    arg = args[i];
                object r = e.Execute(arg);
                rs.Add(r);
                // compare to expected, if expected values are provided
                if (expected != null)
                    Assert.AreEqual(expected[i], r);
            }
            
            return rs;
        }

        public static List<object> TestLinesOrderAndSequential(List<string> lines,
            List<object[]> args = null, // if non null, pass these args to respective lines
            List<object> expected = null, // if non null, compare results to these
            Dictionary<string, double> vars = null, Dictionary<string, string> funcs = null)
        {
            List<object> o1 = TestLinesSequential(lines, args, expected, vars, funcs);
            List<object> o2 = TestLines(lines, args, expected, vars, funcs);
            for (int i = 0; i < o1.Count; i++)
                Assert.AreEqual(o1[i], o2[i]);
            return o1;
        }
    }
}
