using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class FamTest : GedParseTest
    {
        // TODO RIN as supported FAM tag?

        private KBRGedFam parse(string val)
        {
            return parse<KBRGedFam>(val, "FAM");
        }

        [TestMethod]
        public void TestFam()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 WIFE @p2@";
            var rec = parse(fam);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);
        }

        [TestMethod]
        public void TestFam2()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 WIFE @p2@\n1 CHIL @p3@\n1 CHIL @p4@\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(2, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            Assert.AreEqual("p4", rec.Childs[1]);
            Assert.AreEqual(1, rec.Unknowns.Count); // TODO RIN handling?
        }

        private KBRGedFam TestIdentErr(string dadIdent, string momIdent, string kidIdent, int expectedErrCount)
        {
            string fam = string.Format("0 @F1@ FAM\n1 HUSB{0}\n1 WIFE{1}\n1 CHIL{2}", dadIdent, momIdent, kidIdent);
            KBRGedFam rec = parse(fam);
            Assert.AreEqual(expectedErrCount, rec.Errors.Count);
            return rec;
        }

        [TestMethod]
        public void TestDadIdentErrs()
        {
            KBRGedFam rec = TestIdentErr("", " @p2@", " @p3@", 1);
            Assert.AreEqual(null, rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            KBRGedFam rec2 = TestIdentErr(" @", " @p2@", " @p3@", 1);
            KBRGedFam rec3 = TestIdentErr(" @p1", " @p2@", " @p3@", 0);
            Assert.AreEqual("p1", rec3.Dad); // TODO is this correct? unterminated ident?
            KBRGedFam rec4 = TestIdentErr(" ", " @p2@", " @p3@", 1);
        }

        [TestMethod]
        public void TestMomIdentErrs()
        {
            KBRGedFam rec = TestIdentErr(" @p1@", "", " @p3@", 1);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual(null, rec.Mom);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0]);
            KBRGedFam rec2 = TestIdentErr(" @p1@", " @", " @p3@", 1);
            KBRGedFam rec3 = TestIdentErr(" @p1", " @p2", " @p3@", 0);
            Assert.AreEqual("p2", rec3.Mom); // TODO is this correct? unterminated ident?
            KBRGedFam rec4 = TestIdentErr(" @p1@", " ", " @p3@", 1);
        }

        [TestMethod]
        public void TestKidIdentErrs()
        {
            KBRGedFam rec = TestIdentErr(" @p1@", " @p2@", "", 1);
            Assert.AreEqual("p1", rec.Dad);
            Assert.AreEqual("p2", rec.Mom);
            Assert.AreEqual(0, rec.Childs.Count);
            KBRGedFam rec2 = TestIdentErr(" @p1@", " @p2@", " @", 1);
            KBRGedFam rec3 = TestIdentErr(" @p1@", " @p2@", " @p3", 0);
            Assert.AreEqual("p3", rec3.Childs[0]); // TODO is this correct? unterminated ident?
            KBRGedFam rec4 = TestIdentErr(" @p1@", " @p2@", " ", 1);
        }

    }
}
