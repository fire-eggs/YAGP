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

// TODO GEDCOM 5.5 syntax -> separate test file

// TODO exercise all sub-records with multiple FILE? mixed?
// TODO how thorough to test source citation? tested elsewhere?
// TODO exercise deep record - OBJE+SOUR+NOTE ; OBJE+NOTE+CHAN+NOTE; OBJE+SOUR+NOTE+CHAN+NOTE
// TODO consider refactoring large portions of copy-pasta test code?
// TODO error details

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class MediaTest : GedParseTest
    {
        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [Test]
        public void TestSimple1()
        {
            var txt = "0 @M1@ OBJE\n1 FILE reference\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
        }

        [Test]
        public void TestBasic1()
        {
            // file, form, title
            var txt = "0 @M1@ OBJE\n1 FILE reference\n2 FORM wav\n2 TITL this is sparta";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("wav", rec.Files[0].Form);
            Assert.AreEqual("this is sparta", rec.Files[0].Title);

            Assert.AreEqual(0, rec.Errors.Count);
        }

        [Test]
        public void TestMulti1()
        {
            // 3 files, form, title; interspersed with other tags
            var txt = "0 @M1@ OBJE\n1 FILE reference\n2 FORM jpg\n3 TYPE photo\n2 TITL this is sparta\n1 RIN foobar\n1 FILE file2\n2 TITL title2\n2 FORM bmp\n3 TYPE photo\n1 CHAN\n2 DATE 01 APR 2001\n1 FILE file number 3\n2 FORM wav\n2 TITL title3\n1 REFN 007";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("007", rec.Ids.REFNs[0].Value);

            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2001, 4, 1), res2.Date));

            Assert.AreEqual("foobar", rec.RIN);

            Assert.AreEqual(3, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("jpg", rec.Files[0].Form);
            Assert.AreEqual("this is sparta", rec.Files[0].Title);

            Assert.AreEqual("file2", rec.Files[1].FileRefn);
            Assert.AreEqual("bmp", rec.Files[1].Form);
            Assert.AreEqual("title2", rec.Files[1].Title);
            Assert.AreEqual("photo", rec.Files[1].Type);

            Assert.AreEqual("file number 3", rec.Files[2].FileRefn);
            Assert.AreEqual("wav", rec.Files[2].Form);
            Assert.AreEqual("title3", rec.Files[2].Title);
        }

        [Test]
        public void TestRin()
        {
            var txt = "0 @M1@ OBJE\n1 RIN foobar\n1 FILE reference\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("blah", rec.Files[0].Form);
            Assert.AreEqual("foobar", rec.RIN);
        }

        #region Errors
        [Test]
        public void TestMissingId1()
        {
            // empty record; missing id
            var txt = "0 OBJE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(2, (res.Data[0] as GEDCommon).Errors.Count); // TODO Missing FILE, what else? validate details
        }

        [Test]
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

        [Test]
        public void TestMissingFile()
        {
            var txt = "0 @M1@ OBJE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Errors.Count, "Missing FILE");
        }

        [Test]
        public void TestMissingForm()
        {
            var txt = "0 @M1@ OBJE\n1 FILE reference";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual(1, rec.Errors.Count, "Missing FORM");
        }

        [Test]
        public void InvalidForm()
        {
            var txt = "0 @M1@ OBJE\n1 FILE ref\n2 FORM blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("ref", rec.Files[0].FileRefn);
            Assert.AreEqual(1, rec.Errors.Count, "Invalid FORM");
        }

        [Test]
        public void InvalidType()
        {
            var txt = "0 @M1@ OBJE\n1 FILE ref\n2 FORM gif\n3 TYPE blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("ref", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);
            Assert.AreEqual(1, rec.Errors.Count, "Invalid TYPE");
        }

        #endregion

        #region Custom
        [Test]
        public void TestCust1()
        {
            var txt = "0 @M1@ OBJE\n1 _CUST foobar\n1 FILE reference\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
        }

        [Test]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @M1@ OBJE\n1 _CUST foobar\n2 CONC foobar2\n1 FILE reference\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].LineCount);
        }

        [Test]
        public void TestFileCust()
        {
            var txt = "0 @M1@ OBJE\n1 FILE reference\n2 _CUST foobar\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
        }

        [Test]
        public void TestFileCust2()
        {
            var txt = "0 @M1@ OBJE\n1 _CUST fumbar\n1 FILE reference\n2 _CUST foobar\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(2, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual(1, rec.Unknowns[1].LineCount);
        }
        #endregion

        #region REFN
        [Test]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n1 FILE reference\n2 FORM wav";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("wav", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
        }

        [Test]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @M1@ OBJE\n1 REFN 001\n1 FILE reference\n2 FORM tif\n1 REFN 002";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("tif", rec.Files[0].Form);

            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
        }

        [Test]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n2 TYPE blah\n1 FILE reference\n2 FORM ole";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("ole", rec.Files[0].Form);

            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
        }

        [Test]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @M1@ OBJE\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 FILE reference\n2 FORM pcx";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("pcx", rec.Files[0].Form);

            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
        }
        #endregion

        #region NOTE
        [Test]
        public void TestNote1()
        {
            // simple note
            var txt = "0 @M1@ OBJE\n1 NOTE @N1@\n1 REFN 001\n1 FILE reference\n2 FORM bmp";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("bmp", rec.Files[0].Form);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("N1", rec.Notes[0].Xref);
        }
        [Test]
        public void TestNote2()
        {
            // simple note
            var txt = "0 @M1@ OBJE\n1 NOTE blah blah blah\n1 REFN 001\n1 FILE reference\n2 FORM bmp";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("bmp", rec.Files[0].Form);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("blah blah blah", rec.Notes[0].Text);
        }

        [Test]
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

        [Test]
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

        [Test]
        public void TestSimpleSour()
        {
            string txt = "0 @M1@ OBJE\n1 SOUR @S1@\n1 FILE blah\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(0, res[0].Errors.Count, "No error");

            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("S1", rec.Cits[0].Xref);
        }

        [Test]
        public void TestMultSourCit()
        {
            var txt = "0 @M1@ OBJE\n1 SOUR out of bed\n2 TEXT fumbar ex\n2 CONC tended\n2 QUAY nope\n1 FILE reference\n2 FORM gif\n1 RIN rin_tin_tin\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual("rin_tin_tin", rec.RIN);
            Assert.AreEqual(2, rec.Cits.Count);

            Assert.AreEqual("out of bed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("fumbar extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("nope", rec.Cits[0].Quay);

            Assert.AreEqual("inbed", rec.Cits[1].Desc);
            Assert.AreEqual(1, rec.Cits[1].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[1].Text[0]);
            Assert.AreEqual("yup", rec.Cits[1].Quay);

        }

        [Test]
        public void ExtraText()
        {
            var txt = "0 @M1@ OBJE blah blah blah\n1 FILE reference\n2 FORM gif";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("reference", rec.Files[0].FileRefn);
            Assert.AreEqual("gif", rec.Files[0].Form);
        }

    }
}
