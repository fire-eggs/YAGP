using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser.Model;

// TODO note text CONC
// TODO note text CONT
// TODO source citation
// TODO CHAN-NOTE _and_ CONC/CONT

namespace UnitTestProject1
{
    [TestClass]
    public class NoteRecordTest : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public new static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [TestMethod]
        public void TestSimple1()
        {
            var txt = "0 @N1@ NOTE blah blah blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("blah blah blah", rec.Text);
            Assert.AreEqual("N1", rec.Ident);
        }

        [TestMethod]
        public void TestSimple2()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("N1", rec.Ident);
        }

        [TestMethod]
        public void TestCust1()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 _CUST foobar\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [TestMethod]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @N1@ NOTE\n1 _CUST foobar\n2 CONC foobar2";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [TestMethod]
        public void TestCust3()
        {
            // custom tag at the end of the record
            var txt = "0 @N1@ NOTE\n1 CONC foobar\n1 _CUST foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
            Assert.AreEqual("foobar", rec.Text);
        }

        [TestMethod]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n1 CONC fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("fumbar", rec.Text);
        }
        [TestMethod]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @N1@ NOTE\n1 REFN 001\n1 CONC fumbar\n1 REFN 002\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n2 TYPE blah\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestMissingId()
        {
            // empty record; missing id
            var txt = "0 NOTE\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [TestMethod]
        public void TestMissingId2()
        {
            // missing id
            var txt = "0 NOTE\n1 CONC foobar\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as GedNote;
            Assert.IsNotNull(rec);
        }

        [TestMethod]
        public void TestChan()
        {
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 DATE 1 APR 2000\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
        }

        [TestMethod]
        public void TestChan2()
        {
            // no date for chan
            var txt = "0 @N1@ NOTE\n1 CHAN";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [TestMethod]
        public void TestChan3()
        {
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 DATE 1 APR 2000\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestChan4()
        {
            // no date value
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 DATE\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestChan5()
        {
            // extra
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 CUSTOM foo\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestChan6()
        {
            // multi line extra
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 CUSTOM foo\n3 _BLAH bar\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestChan7()
        {
            // multiple CHAN
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 DATE 1 MAR 2000\n1 CONC fumbar\n1 CHAN";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
            Assert.AreEqual("fumbar", rec.Text);
        }

        [TestMethod]
        public void TestChanNote()
        {
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 NOTE @N1@\n2 DATE 1 APR 2000";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("N1", res2.Notes[0].Xref);

            txt = "0 NOTE @N1@\n1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000";
            res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text);

        }

    }
}
