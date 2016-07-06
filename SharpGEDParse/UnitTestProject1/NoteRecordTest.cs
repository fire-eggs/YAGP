using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser.Model;

// TODO note text CONC
// TODO note text CONT
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
            // CHAN with NOTE
            var txt = "0 @N1@ NOTE\n1 CHAN\n2 NOTE @N1@\n2 DATE 1 APR 2000";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("N1", res2.Notes[0].Xref);
        }

        [TestMethod]
        public void TestChanNote2()
        {
            // CHAN with multi-line note
            var txt = "0 NOTE @N1@\n1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text);
        }

        [TestMethod]
        public void TestNoteSrc()
        {
            // simple reference source cit
            var txt = "0 @N1@ NOTE blah blah\n1 SOUR @S1@\n2 PAGE 42\n2 QUAY wha?\n1 CHAN\n2 DATE 1 APR 2000\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("N1", rec.Ident);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.AreEqual("S1", rec.Cits.Xref); // TODO should be multiple
            Assert.AreEqual("42", rec.Cits.Page); // TODO should be multiple
        }

        [TestMethod]
        public void TestNoteSrc2()
        {
            // embedded source cit; changed order of lines
            var txt = "0 @N1@ NOTE blah blah\n1 CHAN\n2 DATE 1 APR 2000\n1 RIN foobar\n1 SOUR description\n2 PAGE 42\n2 QUAY wha?";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("N1", rec.Ident);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.AreEqual("description", rec.Cits.Desc); // TODO should be multiple
            Assert.AreEqual("42", rec.Cits.Page); // TODO should be multiple
            Assert.AreEqual("wha?",rec.Cits.Quay);
        }

        [TestMethod]
        public void TestInvalidXref()
        {
            string txt = "0 @N1@ NOTE\n1 SOUR @ @";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);

            txt = "0 INDI\n1 SOUR @@@";
            res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [TestMethod]
        public void TestEmbSour2()
        {
            // Embedded SOUR cit with CONC/CONT
            var txt = "0 @N1@ NOTE\n1 SOUR this is a source \n2 CONC with extension";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("this is a source with extension", rec.Cits.Desc);
            Assert.IsNull(rec.Cits.Xref);
        }

        [TestMethod]
        public void TestEmbSour3()
        {
            // Embedded SOUR cit with CONC/CONT; multi source cit
            var txt = "0 @N1@ NOTE\n1 SOUR this is a source\n2 CONT extended to next line\n1 SOUR this is another";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);
            Assert.AreEqual("this is a source\nextended to next line", rec.Cits.Desc);
            Assert.IsNull(rec.Cits.Xref);
        }

        [TestMethod]
        public void TestEmbSourText()
        {
            var txt = "0 @N1@ NOTE\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);

            Assert.AreEqual(null, rec.Cits.Xref);
            Assert.AreEqual("embedded source", rec.Cits.Desc);
            Assert.AreEqual("this is text", rec.Cits.Text);
        }

        [TestMethod]
        public void TestEmbSourText2()
        {
            var txt = "0 @N1@ NOTE\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text ex\n3 CONC tended";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);

            Assert.AreEqual(null, rec.Cits.Xref);
            Assert.AreEqual("embedded source", rec.Cits.Desc);
            Assert.AreEqual("this is text extended", rec.Cits.Text);
        }

        [TestMethod]
        public void TestSourCitErr()
        {
            // TEXT tag for reference source is error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 TEXT this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);

            Assert.AreEqual("p1", rec.Cits.Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        [TestMethod]
        public void TestSourCitErr2()
        {
            // PAGE tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR inbed\n2 PAGE this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);

            Assert.AreEqual("p1", rec.Cits.Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        [TestMethod]
        public void TestSourCitErr3()
        {
            // EVEN tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR inbed\n2 EVEN this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as GedNote;
            Assert.IsNotNull(rec);

            Assert.IsNull(rec.Cits.Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        // TODO NOTE+SOUR+EVEN+ROLE
        // TODO NOTE+SOUR+DATA+DATE+TEXT
        // TODO NOTE+SOUR+OBJE
        // TODO NOTE+SOUR+NOTE+...


        // TODO test complex nested structs  0INDI+1ASSOC+2SOUR+3TEXT; 0NOTE+1SOUR+2DATA+3TEXT
    }
}
