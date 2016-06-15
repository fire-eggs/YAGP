using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    // Source Record parse testing

    [TestClass]
    public class SourceTest : GedParseTest
    {
        private GedSource parse(string val)
        {
            return parse<GedSource>(val, "SOUR");
        }

        [TestMethod]
        public void TestBasic()
        {
            var txt = "0 @S1@ SOUR\n1 AUTH Fred";
            var rec = parse(txt);
            Assert.AreEqual("S1", rec.XRef);
            Assert.AreEqual("Fred", rec.Author);
        }

        [TestMethod]
        public void TestRefn()
        {
            var txt = "0 @S1@ SOUR\n1 REFN 123";
            var rec = parse(txt);
            Assert.AreEqual(1, rec.UserReferences.Count);
            Assert.AreEqual("123", rec.UserReferences[0]);
            txt = "0 @S1@ SOUR\n1 REFN 123\n1 REFN 456";
            rec = parse(txt);
            Assert.AreEqual(2, rec.UserReferences.Count);
        }
    }
}
