using RelaScript.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelaScript.UT
{
    [TestClass]
    public class ClassTests
    {
        [TestMethod]
        public void ManualObjectTest()
        {
            InputContext context = new InputContext();

            Exline e1 = new Exline("v:hp := 100; f:damage := {v:hp -= a:0};");
            context.CompileLine(e1);
            e1.Execute(new object[] { 0.0 });

            InputClass testClass = new InputClass("test", context);

            InputObject testObj = testClass.PrintObject("a");

            // see that obj has its own var and func implementations and scoping works
            Exline e2 = new Exline("f:damage(15)");
            testObj.Implementation.CompileLine(e2);
            Assert.AreEqual(new InputVar("v:hp", 85.0), e2.Execute(new object[] { 0.0 }));
            Assert.AreEqual(new InputVar("v:hp", 70.0), e2.Execute(new object[] { 0.0 }));

            // but also see that the original class is unaffected by the lines we just executed
            Exline e3 = new Exline("f:damage(15)");
            testClass.Definition.CompileLine(e3);
            Assert.AreEqual(new InputVar("v:hp", 85.0), e3.Execute(new object[] { 0.0 }));
        }

        [TestMethod]
        public void ObjectFuncTest()
        {
            TestScaffold.TestLines(lines: new List<string>()
            {
                "defn d:test {v:hp := 0; f:init := {v:hp = a:0}; f:damage := {v:hp -= a:0};}" +
                "object o:testa d:test (100);" +
                "o:testa . f:damage(20)",
                "o:testa . f:damage (20)"
            },
            expected: new List<object>()
            {
                new InputVar("v:hp", 80.0),
                new InputVar("v:hp", 60.0)
            });
        }

        [TestMethod]
        public void ObjectVarTest()
        {
            TestScaffold.TestLines(lines: new List<string>()
            {
                "defn d:test {v:hp := 0; v:mana := 0; f:init := {v:hp = a:0; v:mana = a:1 * 2};}" +
                "object o:testa d:test (100, 20);" +
                "o:testa . v:hp + 1",
                "o:testa . v:mana + 1",
                "o:testa . v:mana += 10",
                "o:testa . v:hp * o:testa . v:mana",
                "o:testa.v:hp * o:testa.v:mana"
            },
            expected: new List<object>()
            {
                101.0,
                41.0,
                new InputVar("v:mana", 50.0),
                5000.0,
                5000.0
            });
        }

        [TestMethod]
        public void FullClassTestMonster()
        {
            TestScaffold.TestLines(lines: new List<string>()
            {
                // a defn is an object type definition\r\n" + 
                "defn d:monster { \r\n" +
                "\tv:maxhp := 0;\r\n" +
                "\tv:hp := 0;\r\n" +
                "\tv:armor := 0;\r\n" +
                "\tv:power := 0;\r\n" +
                "\tv:alive := b:t;\r\n" +
                "\r\n" +
                "\t// init is called when an object of this type is made\r\n" +
                "\tf:init := { \r\n" +
                "\t\tv:maxhp = a:0;\r\n" +
                "\t\tv:hp = a:0;\r\n" +
                "\t\tv:armor = a:1;\r\n" +
                "\t\tv:power = a:2;\r\n" +
                "\t}\r\n" +
                "\r\n" +
                "\tf:heal := {\r\n" +
                "\t\tv:hp += a:0;\r\n" +
                "\t\tif v:hp > v:maxhp {\r\n" +
                "\t\t\tv:hp = v:maxhp;\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n" +
                "\r\n" +
                "\tf:damage := {\r\n" +
                "\t\tv:hp -= if (v:armor >= a:0) { 1 } else { a:0 - v:armor };\r\n" +
                "\t\tif (v:hp <= 0) {\r\n" +
                "\t\t\tv:hp = 0;\r\n" +
                "\t\t\tf:die();\r\n" +
                "\t\t}\r\n" +
                "\t}\r\n" +
                "\r\n" +
                "\tf:die := {\r\n" +
                "\t\tv:alive = b:f;\r\n" +
                "\t}\r\n" +
                "}\r\n" +
                "\r\n" +
                "f:testMonsters := {\r\n" +
                "\t// define some objects\r\n" +
                "\t// the terms at the end are passed as args to f:init\r\n" +
                "\tobject o:mon1 d:monster (80, 2, 10);\r\n" +
                "\tobject o:mon2 d:monster (50, 5, 20);\r\n" +
                "\r\n" +
                "\t// fight until one (or both!) is dead\r\n" +
                "\twhile (o:mon1.v:alive && o:mon2.v:alive) {\r\n" +
                "\t\to:mon1.f:damage(o:mon2.v:power);\r\n" +
                "\t\to:mon2.f:damage(o:mon1.v:power);\r\n" +
                "\t}\r\n" +
                "\t\r\n" +
                "\t// return a string stating who won the battle\r\n" +
                "\tif (o:mon1.v:alive == b:f && o:mon2.v:alive == b:f) {\r\n" +
                "\t\t'the fight was a stalemate'\r\n" +
                "\t} else if(o:mon1.v:alive) {\r\n" +
                "\t\t'monster 1 was the victor'\r\n" +
                "\t} else {\r\n" +
                "\t\t'monster 2 was the victor'\r\n" +
                "\t}\r\n" +
                "}\r\n" +
                "f:testMonsters() // --> 'monster 2 was the victor'"
            },
            expected: new List<object>()
            {
                "monster 2 was the victor",
            });
        }

        [TestMethod]
        public void FullClassTestColor()
        {
            TestScaffold.TestLines(lines: new List<string>()
            {
                "defn d:color {\r\n" +
                "    import basic:math l:math\r\n" +
                "    v:red := 0\r\n" +
                "    v:green := 0\r\n" +
                "    v:blue := 0\r\n" +
                "    \r\n" +
                "    f:init := {\r\n" +
                "        a:red a:green a:blue\r\n" +
                "        v:red = a:red\r\n" +
                "        v:green = a:green\r\n" +
                "        v:blue = a:blue\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:asarray := {\r\n" +
                "        (v:red, v:green, v:blue)\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:invert := {\r\n" +
                "        v:red = 1 - v:red\r\n" +
                "        v:green = 1 - v:green\r\n" +
                "        v:blue = 1 - v:blue\r\n" +
                "        f:asarray()\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:brighten := {\r\n" +
                "        a:amount\r\n" +
                "        v:red += a:amount\r\n" +
                "        v:green += a:amount\r\n" +
                "        v:blue += a:amount\r\n" +
                "        if (v:red > 1) { v:red = 1 }\r\n" +
                "        if (v:green > 1) { v:green = 1 }\r\n" +
                "        if (v:blue > 1) { v:blue = 1 }\r\n" +
                "        f:asarray()\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:avg := {\r\n" +
                "        // returns are implicit\r\n" +
                "        l:math.f:avg(v:red, v:green, v:blue)\r\n" +
                "    }\r\n" +
                "}\r\n" +
                "\r\n" +
                "object o:red d:color (1, 0.2, 0.1)\r\n" +
                "\r\n" +
                "o:red.f:avg() // --> 0.433",
                "v:res1 := o:red.f:invert(); v:res1[0] // --> (0, 0.8, 0.9)",
                "v:res1[1]",
                "v:res1[2]",
                "v:res2 := o:red.f:brighten(0.1); v:res2[0] // --> (0.1, 0.9, 1.0)",
                "v:res2[1]",
                "v:res2[2]",
                "v:res3 := o:red.f:brighten(0.1); v:res3[0] // --> (0.2, 1.0, 1.0)",
                "v:res3[1]",
                "v:res3[2]"
            },
            expected: new List<object>()
            {
                (1.0 + 0.2 + 0.1) / 3.0,
                new InputVar("v:red", 0.0),
                new InputVar("v:green", 0.8),
                new InputVar("v:blue", 0.9),
                new InputVar("v:red", 0.1),
                new InputVar("v:green", 0.9),
                new InputVar("v:blue", 1.0),
                new InputVar("v:red", 0.2),
                new InputVar("v:green", 1.0),
                new InputVar("v:blue", 1.0)
            });
        }

        [TestMethod]
        public void StoreObjectInVarTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "defn d:dog {\r\n" +
                "    v:size := 0\r\n" +
                "    f:init := {\r\n" +
                "        a:size\r\n" +
                "        v:size = a:size\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:barkpower := {\r\n" +
                "        v:size ^ 2\r\n" +
                "    }\r\n" +
                "}\r\n" +
                "\r\n" +
                "v:dog1var := object o:dog1 d:dog (3)\r\n" +
                "v:dog2var := object o:dog2 d:dog (5)\r\n" +
                "o:dog1.f:barkpower() //--> 9\r\n",
                "v:dog1var.f:barkpower() //--> 9\r\n",
                "o:dog2.f:barkpower() //--> 25\r\n",
                "v:dog2var.f:barkpower() //--> 25"
            },
            expected: new List<object>()
            {
                9.0,
                9.0,
                25.0,
                25.0
            });
        }

        [TestMethod]
        public void AnonObjectTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "defn d:dog {\r\n" +
                "    v:size := 0\r\n" +
                "    f:init := {\r\n" +
                "        a:size\r\n" +
                "        v:size = a:size\r\n" +
                "    }\r\n" +
                "\r\n" +
                "    f:barkpower := {\r\n" +
                "        v:size ^ 2\r\n" +
                "    }\r\n" +
                "}\r\n" +
                "\r\n" +
                "v:doganon := object anon d:dog (6)\r\n" +
                "v:doganon.f:barkpower() //--> 36",
                "v:doganon2 = object anon d:dog (2)\r\n" +
                "v:doganon2.f:barkpower() //--> 4",
                "v:doganon = v:doganon2\r\n" +
                "v:doganon.f:barkpower() //--> 4",
            },
            expected: new List<object>()
            {
                36.0,
                4.0,
                4.0
            });
        }

        [TestMethod]
        public void FreeObjectTest()
        {
            throw new Exception("TODO");
        }

        [TestMethod]
        public void EmptyDefnTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "defn d:empty {}; 0",
                "v:emptyanon := object anon d:empty (); 0"
            },
            expected: new List<object>()
            {
                0.0,
                0.0
            });
        }

        [TestMethod]
        public void ZeroDefnTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "defn d:zero {0}; 0",
                "v:emptyanon := object anon d:zero (); 0"
            },
            expected: new List<object>()
            {
                0.0,
                0.0
            });
        }

        // NOTE:
        // this test exists because of a peculiar bug
        // if any scope came before a defn, it would prevent
        // the defn from being linked to the correct scope
        // (i.e. what it was doing was always linking the defn to currentscope + 1
        //  but that was only valid if the defn was exactly the next scope)
        [TestMethod]
        public void DefnPriorScopeTest()
        {
            TestScaffold.TestLines(
            lines: new List<string>()
            {
                "f:aaa := { 0 } \r\n" +
                "defn d:point {\r\n" +
                "    v:x := 0\r\n" +
                "    v:y := 0\r\n" +
                "    f:init := {\r\n" +
                "        a:x a:y\r\n" +
                "        v:x = a:x\r\n" +
                "        v:y = a:y\r\n" +
                "    }\r\n" +
                "    f:array := { v:x, v:y }\r\n" +
                "}\r\n" +
                "\r\n" +
                "v:point1 := object anon d:point (5, 10)\r\n" +
                "v:point1.f:array()[0]",
                "v:point1.f:array()[1]"
            },
            expected: new List<object>()
            {
                new InputVar("v:x", 5.0),
                new InputVar("v:y", 10.0)
            });
        }
    }
}
