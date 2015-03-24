using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class FamEventsTest : GedParseTest
    {
        private KBRGedFam parse(string val)
        {
            return parse<KBRGedFam>(val, "FAM");
        }

        // TODO address structure
        // TODO object
        // TODO CHAN

        // TODO HUSB + age
        // TODO WIFE + age

        public KBRGedFam TestEventTag(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0}\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date);
            Assert.AreEqual(null, rec.FamEvents[0].Age);
            Assert.AreEqual(null, rec.FamEvents[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place);
            return rec;
        }

        public KBRGedFam TestEventTag2(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0} Y\n2 DATE 1774\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual("Y", rec.FamEvents[0].Detail); // TODO 'MARR' specific?
            Assert.AreEqual("1774", rec.FamEvents[0].Date);
            Assert.AreEqual("suspicious", rec.FamEvents[0].Type);
            Assert.AreEqual(null, rec.FamEvents[0].Place);
            Assert.AreEqual("church", rec.FamEvents[0].Agency);
            Assert.AreEqual("pregnancy", rec.FamEvents[0].Cause);
            Assert.AreEqual("atheist", rec.FamEvents[0].Religion);
            Assert.AreEqual("locked", rec.FamEvents[0].Restriction);
            return rec;
        }

        [TestMethod]
        public void TestMarr()
        {
            string tag = "MARR";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestAnul()
        {
            string tag = "ANUL";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestMarl()
        {
            string tag = "MARL";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestMars()
        {
            string tag = "MARS";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestMarb()
        {
            string tag = "MARB";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestMarc()
        {
            string tag = "MARC";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestDiv()
        {
            string tag = "DIV";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestDivf()
        {
            string tag = "DIVF";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestEnga()
        {
            string tag = "ENGA";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
        [TestMethod]
        public void TestEven()
        {
            string tag = "EVEN";
            TestEventTag(tag);
            TestEventTag2(tag);
        }
    }
}
