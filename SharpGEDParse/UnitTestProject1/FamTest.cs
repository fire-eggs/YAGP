using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class FamTest : GedParseTest
    {
        // TODO missing FAM ident?
        // TODO test SOUR
        // TODO RIN as supported FAM tag?
        // TODO more than 1 DAD record? Error/warn/take-the-last?
        // TODO more than 1 MOM record? Error/warn/take-the-last?
        // TODO more than 1 NOTE record? Error/warn/take-the-last?


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
            Assert.AreEqual(1, rec.Data.Count); // TODO RIN handling?
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

        [TestMethod]
        public void TestFamNote()
        {
            string fam = "0 @F1@ FAM\n1 HUSB @p1@\n1 NOTE @N123@";
            var rec = parse(fam);
            Assert.AreNotEqual(null, rec.Notes);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("@N123@", rec.Notes[0]);
        }

        [TestMethod]
        public void TestFamNote2()
        {
            string fam = "0 @F1@ FAM\n1 NOTE This is a family record note\n2 CONT blah blah";
            var rec = parse(fam);
            Assert.AreNotEqual(null, rec.Notes);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("This is a family record note\nblah blah", rec.Notes[0]);
        }

        [TestMethod]
        public void TestChange()
        {
            var indi = "0 @F1@ FAM\n1 CHAN";
            var rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(1, rec.Change.Item2);

            indi = "0 @F1@ FAM\n1 CHAN notes\n2 DATE blah";
            rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(2, rec.Change.Item2);

            // Only 1 change record allowed
            // Gedcom spec says take the FIRST one
            indi = "0 @F1@ FAM\n1 CHAN notes\n2 DATE blah\n1 CHAN notes2";
            rec = parse(indi);
            Assert.AreNotEqual(null, rec.Change);
            Assert.AreEqual(1, rec.Change.Item1);
            Assert.AreEqual(2, rec.Change.Item2);
            Assert.AreEqual(1, rec.Errors.Count);

            // TODO test actual details
        }

        [TestMethod]
        public void TestStat()
        {
            var fam = "0 @F1@ FAM\n1 _STAT MARRIED\n1 HUSB @I1@\n1 WIFE @I2@\n1 CHIL @I72@\n1 MARR\n2 DATE 6 JUN 1948\n2 PLAC Oak Park,Illinois";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Data.Count);
        }
    }
}

// TODO test _MREL, _FREL from 'AGES'
//0 @F17299@ FAM\n1 HUSB @I4235@\n1 WIFE @I4236@\n1 CHIL @I7431@\n2 _FREL step\n2 _MREL step\n1 CHIL @I7432@\n2 _FREL step\n2 _MREL step\n1 MARR
