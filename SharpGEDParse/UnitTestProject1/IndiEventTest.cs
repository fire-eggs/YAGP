using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// ReSharper disable InconsistentNaming

namespace UnitTestProject1
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class IndiEventTest : GedParseTest
    {
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
            Assert.AreEqual("", rec.Events[0].Detail);
            Assert.AreEqual(null, rec.Events[0].Famc);
            Assert.AreEqual(null, rec.Events[0].FamcAdop);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
            return rec;
        }

        public void TestBirthExtra(string tag)
        {
            string indi = string.Format("0 INDI\n1 {0} Y\n2 FAMC @FAM99@\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("Y", rec.Events[0].Detail);
            Assert.AreEqual("@FAM99@", rec.Events[0].Famc);
            Assert.AreEqual(null, rec.Events[0].FamcAdop);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);

            indi = string.Format("0 INDI\n1 {0} Y\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 FAMC @FAM99@\n3 ADOP pater", tag);
            rec = parse(indi);

            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("Y", rec.Events[0].Detail);
            Assert.AreEqual("@FAM99@", rec.Events[0].Famc);
            Assert.AreEqual("pater", rec.Events[0].FamcAdop);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
        }

        [TestMethod]
        public void FamcBadSub()
        {
            var indi2 = "0 INDI\n1 BIRT Y\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 FAMC @FAM99@\n3 BOGUS pater";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreNotEqual(0, rec.Events[0].Errors.Count);
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

            TestBirthExtra("BIRT");
        }

        [TestMethod]
        public void TestCHR()
        {
            TestEventTag1("CHR");
            TestEventTag2("CHR");
            TestEventTag3("CHR");
            TestBirthExtra("CHR");
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
            TestBirthExtra(tag);
        }

        [TestMethod]
        public void TestEventSource()
        {
            string indi2 = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR @S122@\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n2 SOUR @S124@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual(2, aEvent.Sources.Count);
            Assert.AreEqual(3, aEvent.Sources[0].Beg);
            Assert.AreEqual(5, aEvent.Sources[0].End);
            Assert.AreEqual(6, aEvent.Sources[1].Beg);
            Assert.AreEqual(6, aEvent.Sources[1].End);

            Assert.AreEqual("S124", aEvent.Sources[1].XRef);
            Assert.AreEqual("S122", aEvent.Sources[0].XRef);
        }

        [TestMethod]
        public void TestErrorSource()
        {
            string indi = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n1 SOUR";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(1, rec.Events[0].Errors.Count);
        }

        [TestMethod]
        public void TestMultEvent()
        {
            string txt = "0 INDI\n1 BIRT\n2 DATE 1774\n2 NOTE this is a note\n2 PLAC Sands, Oldham, Lncshr, Eng\n1 BURI\n2 RESN locked\n2 DATE 1774\n2 PLAC who knows";
            var rec = parse(txt);
            Assert.AreEqual(2, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual(1, aEvent.Notes.Count);
            Assert.AreEqual("this is a note", aEvent.Notes[0]);
            aEvent = rec.Events[1];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual("locked", aEvent.Restriction);
        }
        [TestMethod]
        public void TestMultEvent2()
        {
            string txt = "0 INDI\n1 BIRT\n2 NOTE this is a note\n2 DATE 1774\n2 CAUS fun\n2 PLAC Sands, Oldham, Lncshr, Eng\n1 BURI\n2 RELI cthulhu\n2 AGNC blunt instrument\n2 DATE 1774\n2 PLAC who knows";
            var rec = parse(txt);
            Assert.AreEqual(2, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual(1, aEvent.Notes.Count);
            Assert.AreEqual("this is a note", aEvent.Notes[0]);
            Assert.AreEqual("fun", aEvent.Cause);
            aEvent = rec.Events[1];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual("cthulhu", aEvent.Religion);
            Assert.AreEqual("blunt instrument", aEvent.Agency);
        }

    }
}
