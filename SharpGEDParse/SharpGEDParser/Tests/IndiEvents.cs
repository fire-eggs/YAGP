using NUnit.Framework;
using SharpGEDParser.Model;

// TODO CHAN
// TODO OBJE
// TODO multiple NOTE, SOUR

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiEvents : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        // Dunno if this is "cheating" or not but perform common event testing for
        // a given tag.
        public IndiRecord TestEventTag1(string tag)
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

        public IndiRecord TestEventTag2(string tag)
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

        public IndiRecord TestEventTag3(string tag)
        {
            string indi3 = string.Format("0 INDI\n1 {0}\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreEqual(tag, rec.Events[0].Tag);
            Assert.AreEqual("", rec.Events[0].Descriptor);
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
            Assert.AreEqual("Y", rec.Events[0].Descriptor);
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
            Assert.AreEqual("Y", rec.Events[0].Descriptor);
            Assert.AreEqual("@FAM99@", rec.Events[0].Famc);
            Assert.AreEqual("pater", rec.Events[0].FamcAdop);
            Assert.AreEqual(null, rec.Events[0].Date);
            Assert.AreEqual(null, rec.Events[0].Age);
            Assert.AreEqual(null, rec.Events[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.Events[0].Place);
        }

        [Test]
        public void FamcBadSub()
        {
            var indi2 = "0 INDI\n1 BIRT Y\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 FAMC @FAM99@\n3 BOGUS pater";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            Assert.AreNotEqual(0, rec.Errors.Count);
        }

        [Test]
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

        [Test]
        public void TestCHR()
        {
            TestEventTag1("CHR");
            TestEventTag2("CHR");
            TestEventTag3("CHR");
            TestBirthExtra("CHR");
        }

        [Test]
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

        [Test]
        public void TestCrem()
        {
            var tag = "CREM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }

        [Test]
        public void TestBurial()
        {
            var tag = "BURI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestBAPM()
        {
            var tag = "BAPM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestBARM()
        {
            var tag = "BARM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestBASM()
        {
            var tag = "BASM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestBLES()
        {
            var tag = "BLES";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestCHRA()
        {
            var tag = "CHRA";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestCONF()
        {
            var tag = "CONF";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestFCOM()
        {
            var tag = "FCOM";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestORDN()
        {
            var tag = "ORDN";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestNATU()
        {
            var tag = "NATU";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestEMIG()
        {
            var tag = "EMIG";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestIMMI()
        {
            var tag = "IMMI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestPROB()
        {
            var tag = "PROB";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestWILL()
        {
            var tag = "WILL";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestGRAD()
        {
            var tag = "GRAD";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestRETI()
        {
            var tag = "RETI";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
        }
        [Test]
        public void TestADOP()
        {
            var tag = "ADOP";
            TestEventTag1(tag);
            TestEventTag2(tag);
            TestEventTag3(tag);
            TestBirthExtra(tag);
        }

        [Test]
        public void TestEventSource()
        {
            string indi2 = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR @S122@\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n2 SOUR @S124@";
            var rec = parse(indi2);
            Assert.AreEqual(1, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual(2, aEvent.Cits.Count);

            // TODO details
            //Assert.AreEqual(3, aEvent.Cits[0].Beg);
            //Assert.AreEqual(5, aEvent.Cits[0].End);
            //Assert.AreEqual(6, aEvent.Cits[1].Beg);
            //Assert.AreEqual(6, aEvent.Cits[1].End);

            Assert.AreEqual("S124", aEvent.Cits[1].Xref);
            Assert.AreEqual("S122", aEvent.Cits[0].Xref);
        }

        [Test]
        public void TestErrorSource()
        {
            string indi = "0 INDI\n1 BURI\n2 PLAC Corinth Cemt. Barry Co., Missouri.\n2 SOUR\n3 DATA\n4 TEXT Date of Import: 17 Jun 2000\n1 SOUR";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [Test]
        public void TestMultEvent()
        {
            string txt = "0 INDI\n1 BIRT\n2 DATE 1774\n2 NOTE this is a note\n2 PLAC Sands, Oldham, Lncshr, Eng\n1 BURI\n2 RESN locked\n2 DATE 1774\n2 PLAC who knows";
            var rec = parse(txt);
            Assert.AreEqual(2, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual(1, aEvent.Notes.Count);
            Assert.AreEqual("this is a note", aEvent.Notes[0].Text);
            aEvent = rec.Events[1];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual("locked", aEvent.Restriction);
        }

        [Test]
        public void TestMultEvent2()
        {
            string txt = "0 INDI\n1 BIRT\n2 NOTE this is a note\n2 DATE 1774\n2 CAUS fun\n2 PLAC Sands, Oldham, Lncshr, Eng\n1 BURI\n2 RELI cthulhu\n2 AGNC blunt instrument\n2 DATE 1774\n2 PLAC who knows";
            var rec = parse(txt);
            Assert.AreEqual(2, rec.Events.Count);
            var aEvent = rec.Events[0];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual(1, aEvent.Notes.Count);
            Assert.AreEqual("this is a note", aEvent.Notes[0].Text);
            Assert.AreEqual("fun", aEvent.Cause);
            aEvent = rec.Events[1];
            Assert.AreEqual("1774", aEvent.Date);
            Assert.AreEqual("cthulhu", aEvent.Religion);
            Assert.AreEqual("blunt instrument", aEvent.Agency);
        }

    }
}
