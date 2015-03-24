using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [TestClass]
    public class IndiEventTest : GedParseTest
    {
        // TODO ambiguous tag : EVEN
        // TODO ambiguous tag : CENS

        // TODO AGNC
        // TODO RELI
        // TODO CAUS
        // TODO RESN
        // TODO CHAN
        // TODO OBJE
        // TODO multiple NOTE, SOUR

        private KBRGedIndi parse(string val)
        {
            return parse<KBRGedIndi>(val, "INDI");
        }

        // Dunno if this is "cheating" or not but perform common event testing for
        // a given tag.
        public KBRGedIndi TestEventTag1(string tag)
        {
            string indi =
                string.Format("0 INDI\n1 {0}\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("1774", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);

            return rec;
        }
        public KBRGedIndi TestEventTag2(string tag)
        {
            string indi = string.Format("0 INDI\n1 {0}\n2 TYPE suspicious\n2 DATE 1776\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("1776", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
            Assert.AreEqual("suspicious", rec.Events[0].Type);

            return rec;
        }

        public KBRGedIndi TestEventTag3(string tag)
        {
            string indi3 = string.Format("0 INDI\n1 {0}\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
            return rec;
        }

        [TestMethod]
        public void TestBirth()
        {
            string indi = "0 INDI\n1 BIRT\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng";
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("BIRT", rec.Events[0].Tag);
            Assert.AreEqual("1774", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);

            string indi2 = "0 INDI\n1 BIRT\n2 TYPE suspicious\n2 DATE 1776\n2 PLAC Sands, Oldham, Lncshr, Eng";
            rec = parse(indi2);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("BIRT", rec.Events[0].Tag);
            Assert.AreEqual("1776", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
            Assert.AreEqual("suspicious", rec.Events[0].Type);

            string indi3 = "0 INDI\n1 BIRT\n2 PLAC Sands, Oldham, Lncshr, Eng";
            rec = parse(indi3);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual("BIRT", rec.Events[0].Tag);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);

            // TODO - optional 'Y' argument?
            // TODO - FAMC testing
        }

        [TestMethod]
        public void TestCHR()
        {
            TestEventTag1("CHR");
            TestEventTag2("CHR");
            TestEventTag3("CHR");

            // TODO - optional 'Y' argument?
            // TODO - FAMC testing
        }

        [TestMethod]
        public void TestDeath()
        {
            string tag = "DEAT";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);

            // TODO - optional 'Y' argument?
            // TODO test AGE with all events

            string indi = string.Format("0 INDI\n1 {0}\n2 AGE 17\n2 DATE 1776\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("17", rec.Events[0].Age);
            Assert.AreEqual("1776", rec.Events[0].Date);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
        }

        [TestMethod]
        public void TestCrem()
        {
            var tag = "CREM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }

        [TestMethod]
        public void TestBurial()
        {
            var tag = "BURI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestBAPM()
        {
            var tag = "BAPM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestBARM()
        {
            var tag = "BARM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestBASM()
        {
            var tag = "BASM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestBLES()
        {
            var tag = "BLES";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestCHRA()
        {
            var tag = "CHRA";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestCONF()
        {
            var tag = "CONF";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestFCOM()
        {
            var tag = "FCOM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestORDN()
        {
            var tag = "ORDN";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestNATU()
        {
            var tag = "NATU";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestEMIG()
        {
            var tag = "EMIG";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestIMMI()
        {
            var tag = "IMMI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestPROB()
        {
            var tag = "PROB";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestWILL()
        {
            var tag = "WILL";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestGRAD()
        {
            var tag = "GRAD";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestRETI()
        {
            var tag = "RETI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [TestMethod]
        public void TestADOP()
        {
            var tag = "ADOP";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
            // TODO FAMC testing
            // TODO FAMC + ADOP
        }

        [TestMethod]
        public void TestEventSource()
        {
            string indi2 = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR @S122@\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n2 SOUR @S124@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(3, rec.Events[0].Source.Item1);
            Assert.AreEqual(5, rec.Events[0].Source.Item2);
            // TODO - more than one source!
        }

    }
}
