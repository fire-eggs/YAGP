// When testing, secondary structures such as CHAN, NOTE, etc need to be
// exercised when appearing before and after the record data proper. The
// record data needs to be validated as well. Doing so caught some issues
// where parsing of secondary structures made primary record data vanish.

using System;
using NUnit.Framework;
using SharpGEDParser.Model;
using System.Collections.Generic;
using System.Linq;

// Testing for Media (OBJE) Records
// Unless specifically mentioned, the syntax for these records is GEDCOM 5.5.1

// TODO a FILE is required
// TODO a FORM is required per FILE
// TODO an XREF is required
// TODO multiple FILE
// TODO exercise all sub-records with multiple FILE? mixed?
// TODO custom tag underneath FILE
// TODO RIN
// TODO source citation

// TODO consider refactoring large portions of copy-pasta test code?

namespace SharpGEDParser.Tests
{
    [TestFixture()]
    public class MediaTest : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public new static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [Test()]
        public void TestSimple1()
        {
            var txt = "0 @M1@ OBJE\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
        }

        [Test()]
        public void TestMissingId1()
        {
            // empty record; missing id
            var txt = "0 OBJE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test()]
        public void TestMissingId2()
        {
            // missing id
            var txt = "0 OBJE\n1 FILE reference\n2 FORM blah";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
        }

        [Test()]
        public void TestCust1()
        {
            var txt = "0 @M1@ OBJE\n1 _CUST foobar\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
        }

        [Test()]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @M1@ OBJE\n1 _CUST foobar\n2 CONC foobar2\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].LineCount);
        }

        #region REFN
        [Test()]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
        }

        [Test()]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @M1@ OBJE\n1 REFN 001\n1 FILE reference\n2 FORM blah\n1 REFN 002";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);

            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
        }

        [Test()]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n2 TYPE blah\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);

            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
        }

        [Test()]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);

            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
        }
        #endregion

        #region CHAN

        [Test()]
        public void TestChan()
        {
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 DATE 1 APR 2000";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
        }

        [Test()]
        public void TestChan2()
        {
            // no date for chan
            var txt = "0 @M1@ OBJE\n1 CHAN";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        [Test()]
        public void TestChan3()
        {
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 DATE 1 APR 2000\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
        }

        [Test()]
        public void TestChan4()
        {
            // no date value
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 DATE\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
        }

        [Test()]
        public void TestChan5()
        {
            // extra
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 CUSTOM foo\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
        }

        [Test()]
        public void TestChan6()
        {
            // multi line extra
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 CUSTOM foo\n3 _BLAH bar\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            ChangeRec chan = rec.CHAN;
            Assert.AreEqual(1, chan.OtherLines.Count);
        }

        [Test()]
        public void TestChan7()
        {
            // multiple CHAN
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 DATE 1 MAR 2000\n1 FILE reference\n2 FORM blah\n1 CHAN";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            Assert.IsTrue(Equals(new DateTime(2000, 3, 1), rec.CHAN.Date));
        }

        [Test()]
        public void TestChanNote()
        {
            var txt = "0 @M1@ OBJE\n1 CHAN\n2 NOTE @N1@\n2 DATE 1 APR 2000";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("N1", res2.Notes[0].Xref);

            txt = "0 @M1@ OBJE\n1 CHAN\n2 NOTE notes\n3 CONT more detail\n2 DATE 1 APR 2000";
            res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual(1, res2.Notes.Count);
            Assert.AreEqual("notes\nmore detail", res2.Notes[0].Text);
        }

        #endregion

        #region NOTE
        [Test()]
        public void TestNote1()
        {
            // simple note
            var txt = "0 @M1@ OBJE\n1 NOTE @N1@\n1 REFN 001\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("N1", rec.Notes[0].Xref);
        }
        [Test()]
        public void TestNote2()
        {
            // simple note
            var txt = "0 @M1@ OBJE\n1 NOTE blah blah blah\n1 REFN 001\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("blah blah blah", rec.Notes[0].Text);
        }

        [Test()]
        public void TestNote()
        {
            var indi = "0 @M1@ OBJE\n1 NOTE";
            var res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("", rec.Notes[0].Text);

            indi = "0 @M1@ OBJE\n1 NOTE notes\n2 CONT more detail";
            res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);

            indi = "0 @M1@ OBJE\n1 NOTE notes\n2 CONT more detail\n1 FILE foo\n2 FORM bar\n1 NOTE notes2";
            res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(2, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
            Assert.AreEqual("notes2", rec.Notes[1].Text);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("foo", rec.Files[0].FileRefn);
            Assert.AreEqual("bar", rec.Files[0].Form);

            // trailing space must be preserved
            indi = "0 @M1@ OBJE\n1 NOTE notes\n2 CONC more detail \n2 CONC yet more detail";
            res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notesmore detail yet more detail", rec.Notes[0].Text);

            indi = "0 @M1@ OBJE\n1 NOTE notes \n2 CONC more detail \n2 CONC yet more detail ";
            res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes more detail yet more detail ", rec.Notes[0].Text);
        }

        [Test()]
        public void TestNoteOther()
        {
            // exercise other lines
            var indi = "0 @M1@ OBJE\n1 NOTE\n2 OTHR gibber";
            var res = ReadIt(indi);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("", rec.Notes[0].Text);
            Assert.AreEqual(1, rec.Notes[0].OtherLines.Count);
        }

        #endregion

    }
}
