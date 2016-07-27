using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    // Testing for NOTE records
    [TestFixture]
    class NoteTest : GedParseTest
    {

// TODO note text CONC
// TODO note text CONT
// TODO CHAN-NOTE _and_ CONC/CONT

        // TODO move all source citation tests elsewhere?
        // TODO more than one source citation text

        // TODO this is temporary until GEDCommon replaces KBRGedRec
        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [Test]
        public void TestSimple1()
        {
            var txt = "0 @N1@ NOTE blah blah blah";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("blah blah blah", rec.Text);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestSimple2()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestNoteConc()
        {
            var txt = "0 @N1@ NOTE text\n1 CONC continued\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("textcontinued", rec.Text);
        }

        [Test]
        public void TestNoteCont()
        {
            var txt = "0 @N1@ NOTE text\n1 CONT continued\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("text\ncontinued", rec.Text);
        }

        #region Custom
        [Test]
        public void TestCust1()
        {
            var txt = "0 @N1@ NOTE blah blah blah\n1 _CUST foobar\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestCust2()
        {
            // multi-line custom tag
            var txt = "0 @N1@ NOTE\n1 _CUST foobar\n2 CONC foobar2";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
        }

        [Test]
        public void TestCust3()
        {
            // custom tag at the end of the record
            var txt = "0 @N1@ NOTE\n1 CONC foobar\n1 _CUST foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Unknowns[0].LineCount);
            Assert.AreEqual("N1", rec.Ident);
            Assert.AreEqual("foobar", rec.Text);
        }
        #endregion

        #region REFN
        [Test]
        public void TestREFN()
        {
            // single REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n1 CONC fumbar\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("fumbar", rec.Text);
        }
        [Test]
        public void TestREFNs()
        {
            // multiple REFNs
            var txt = "0 @N1@ NOTE\n1 REFN 001\n1 CONC fumbar\n1 REFN 002\n0 KLUDGE";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual("002", rec.Ids.REFNs[1].Value);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [Test]
        public void TestREFNExtra()
        {
            // extra on REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n2 TYPE blah\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(1, rec.Ids.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }

        [Test]
        public void TestREFNExtra2()
        {
            // multi-line extra on REFN
            var txt = "0 @N1@ NOTE\n1 REFN 001\n2 TYPE blah\n3 _CUST foo\n1 CONC fumbar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(1, rec.Ids.REFNs.Count);
            Assert.AreEqual("001", rec.Ids.REFNs[0].Value);
            Assert.AreEqual(2, rec.Ids.REFNs[0].Extra.LineCount);
            Assert.AreEqual("fumbar", rec.Text);
        }
        #endregion REFN

        [Test]
        public void TestMissingId()
        {
            // empty record; missing id
            var txt = "0 NOTE\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(1, res.Errors.Count); // TODO validate error details
            Assert.AreEqual(1, res.Data.Count);
            Assert.AreEqual(1, (res.Data[0] as GEDCommon).Errors.Count);
        }

        [Test]
        public void TestMissingId2()
        {
            // missing id
            var txt = "0 NOTE\n1 CONC foobar\n0 KLUDGE";
            var res = ReadItHigher(txt);
            Assert.AreEqual(0, res.Errors.Count);
            Assert.AreEqual(1, res.Data.Count);
            var rec = res.Data[0] as NoteRecord;
            Assert.IsNotNull(rec);
        }

        [Test]
        public void TestInvalidXref()
        {
            string txt = "0 @N1@ NOTE\n1 SOUR @ @";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);

            txt = "0 @N1@ NOTE\n1 SOUR @@@";
            res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Errors.Count);
        }

        #region Source Citation
        [Test]
        public void TestNoteSrc()
        {
            // simple reference source cit
            var txt = "0 @N1@ NOTE blah blah\n1 SOUR @S1@\n2 PAGE 42\n2 QUAY wha?\n1 CHAN\n2 DATE 1 APR 2000\n1 RIN foobar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("N1", rec.Ident);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("blah blah", rec.Text);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("S1", rec.Cits[0].Xref);
            Assert.AreEqual("42", rec.Cits[0].Page);
        }

        [Test]
        public void TestNoteSrc2()
        {
            // embedded source cit; changed order of lines
            var txt = "0 @N1@ NOTE blah blah\n1 CHAN\n2 DATE 1 APR 2000\n1 RIN foobar\n1 SOUR description\n2 PAGE 42\n2 QUAY wha?";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("N1", rec.Ident);
            var res2 = rec.CHAN;
            Assert.IsTrue(Equals(new DateTime(2000, 4, 1), res2.Date));
            Assert.AreEqual("foobar", rec.RIN);
            Assert.AreEqual("blah blah", rec.Text);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("description", rec.Cits[0].Desc);
            Assert.AreEqual("42", rec.Cits[0].Page);
            Assert.AreEqual("wha?", rec.Cits[0].Quay);
        }

        [Test]
        public void TestSimpleSour()
        {
            string txt = "0 @N1@ NOTE\n1 SOUR ";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);
//            Assert.AreEqual(1, rec.Errors.Count);
            // TODO should there be an error? e.g. "no description"?
        }

        [Test]
        public void TestEmbSour2()
        {
            // Embedded SOUR cit with CONC/CONT
            var txt = "0 @N1@ NOTE\n1 SOUR this is a source \n2 CONC with extension";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("this is a source with extension", rec.Cits[0].Desc);
            Assert.IsNull(rec.Cits[0].Xref);
        }

        [Test]
        public void TestEmbSour3()
        {
            // Embedded SOUR cit with CONC/CONT; multi source cit
            var txt = "0 @N1@ NOTE\n1 SOUR this is a source\n2 CONT extended to next line\n1 SOUR this is another";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("this is a source\nextended to next line", rec.Cits[0].Desc);
            Assert.IsNull(rec.Cits[0].Xref);
            Assert.AreEqual("this is another", rec.Cits[1].Desc);
            Assert.IsNull(rec.Cits[1].Xref);
        }

        [Test]
        public void TestEmbSourText()
        {
            var txt = "0 @N1@ NOTE\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("embedded source", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("this is text", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestEmbSourText2()
        {
            // A valid tag following a NOTE within the SOUR
            var txt = "0 @N1@ NOTE\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text ex\n3 CONC tended";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual("a note", rec.Cits[0].Notes[0].Text);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("this is text extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("embedded source", rec.Cits[0].Desc);
        }

        [Test]
        public void TestEmbSourText2A()
        {
            // A valid tag following a NOTE within the SOUR
            var txt = "0 @N1@ NOTE\n1 SOUR embedded source\n2 NOTE\n2 TEXT this is text ex\n3 CONC tended";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual("", rec.Cits[0].Notes[0].Text);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("this is text extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("embedded source", rec.Cits[0].Desc);
        }

        [Test]
        public void TestSourCitErr()
        {
            // TEXT tag for reference source is error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 TEXT this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        [Test]
        public void TestSourCitErrA()
        {
            // TEXT tag with DATA for reference source is valid
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 DATA\n3 TEXT this is valid";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.AreEqual(0, res[0].Errors.Count, "error");
        }

        [Test]
        public void TestSourCitErr2()
        {
            // PAGE tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR embed\n2 PAGE this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        [Test]
        public void TestSourCitErr3()
        {
            // EVEN tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR inbed\n2 EVEN this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.IsNull(rec.Cits[0].Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
        }

        [Test]
        public void TestSourCitText()
        {
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText2()
        {
            // empty CONC
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar\n2 CONC";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText2A()
        {
            // empty CONC
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar\n2 CONC\n2 CONC ex";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebarex", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText3()
        {
            // empty CONT
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar\n2 CONT";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar\n", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText4()
        {
            // no space
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar ex\n2 CONC tended";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText5()
        {
            // keep trailing space
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitText6()
        {
            // valid tag after TEXT
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("inbed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("yup", rec.Cits[0].Quay);
        }

        [Test]
        public void TestMultSourCit()
        {
            // more than one source citation
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR out of bed\n2 TEXT fumbar ex\n2 CONC tended\n2 QUAY nope\n1 RIN rin_tin_tin\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

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
        #endregion

        [Test]
        public void TestSourCitObje1()
        {
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR out of bed\n2 OBJE\n3 FILE fileref1\n3 FILE fileref2\n4 FORM format1\n2 TEXT fumbar ex\n2 CONC tended\n2 QUAY nope\n1 RIN rin_tin_tin\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 OBJE @mref2@\n2 OBJE\n3 FILE filerefn\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("rin_tin_tin", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);

            Assert.AreEqual("out of bed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("fumbar extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("nope", rec.Cits[0].Quay);
            Assert.AreEqual(1, rec.Cits[0].Media.Count);
            var media = rec.Cits[0].Media[0];
            Assert.AreEqual(2, media.Files.Count);
            Assert.AreEqual("fileref1", media.Files[0].FileRefn);
            Assert.AreEqual("fileref2", media.Files[1].FileRefn);

            Assert.AreEqual("inbed", rec.Cits[1].Desc);
            Assert.AreEqual(1, rec.Cits[1].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[1].Text[0]);
            Assert.AreEqual("yup", rec.Cits[1].Quay);
            Assert.AreEqual(2, rec.Cits[1].Media.Count);
            media = rec.Cits[1].Media[0];
            Assert.AreEqual("mref2", media.Xref);
            media = rec.Cits[1].Media[1];
            Assert.AreEqual("filerefn", media.Files[0].FileRefn);

        }

        [Test]
        public void TestSourCitObje2()
        {
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR out of bed\n\n2 OBJE\n3 TITL title\n2 OBJE\n3 FILE\n3 FORM format2\n3 FILE fileref2\n4 FORM format1\n2 TEXT fumbar ex\n2 CONC tended\n2 QUAY nope\n1 RIN rin_tin_tin\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 OBJE @mref2@\n2 OBJE\n3 FILE filerefn\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual("rin_tin_tin", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);

            Assert.AreEqual("out of bed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("fumbar extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("nope", rec.Cits[0].Quay);
            Assert.AreEqual(2, rec.Cits[0].Media.Count);

            var media = rec.Cits[0].Media[0];
            Assert.AreEqual(0, media.Files.Count);
            Assert.AreEqual("title", media.Title);

            media = rec.Cits[0].Media[1];
            Assert.AreEqual(2, media.Files.Count);
            Assert.AreEqual("", media.Files[0].FileRefn);
            Assert.AreEqual("format2", media.Files[0].Form);
            Assert.AreEqual("fileref2", media.Files[1].FileRefn);
            Assert.AreEqual("format1", media.Files[1].Form);

            Assert.AreEqual("inbed", rec.Cits[1].Desc);
            Assert.AreEqual(1, rec.Cits[1].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[1].Text[0]);
            Assert.AreEqual("yup", rec.Cits[1].Quay);
            Assert.AreEqual(2, rec.Cits[1].Media.Count);
            media = rec.Cits[1].Media[0];
            Assert.AreEqual("mref2", media.Xref);
            media = rec.Cits[1].Media[1];
            Assert.AreEqual("filerefn", media.Files[0].FileRefn);
        }

        [Test]
        public void TestSourCitObje3()
        {
            // Make sure OBJE\FILE\FORM\MEDI variant is exercised
            string txt = "0 @N1@ NOTE fiebar\n1 SOUR out of bed\n\n2 OBJE\n3 TITL title\n2 OBJE\n3 FILE\n3 FORM format2\n4 MEDI type\n3 FILE fileref2\n4 FORM format1\n2 TEXT fumbar ex\n2 CONC tended\n2 QUAY nope\n1 RIN rin_tin_tin\n1 SOUR inbed\n2 TEXT foebar \n2 CONC extended\n2 OBJE @mref2@\n2 OBJE\n3 FILE filerefn\n2 QUAY yup";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(0, rec.Unknowns.Count);

            Assert.AreEqual("rin_tin_tin", rec.RIN);

            Assert.AreEqual(2, rec.Cits.Count);

            Assert.AreEqual("out of bed", rec.Cits[0].Desc);
            Assert.AreEqual(1, rec.Cits[0].Text.Count);
            Assert.AreEqual("fumbar extended", rec.Cits[0].Text[0]);
            Assert.AreEqual("nope", rec.Cits[0].Quay);
            Assert.AreEqual(2, rec.Cits[0].Media.Count);

            var media = rec.Cits[0].Media[0];
            Assert.AreEqual(0, media.Files.Count);
            Assert.AreEqual("title", media.Title);

            media = rec.Cits[0].Media[1];
            Assert.AreEqual(2, media.Files.Count);
            Assert.AreEqual("", media.Files[0].FileRefn);
            Assert.AreEqual("format2", media.Files[0].Form);
            Assert.AreEqual("type", media.Files[0].Type);

            Assert.AreEqual("fileref2", media.Files[1].FileRefn);
            Assert.AreEqual("format1", media.Files[1].Form);

            Assert.AreEqual("inbed", rec.Cits[1].Desc);
            Assert.AreEqual(1, rec.Cits[1].Text.Count);
            Assert.AreEqual("foebar extended", rec.Cits[1].Text[0]);
            Assert.AreEqual("yup", rec.Cits[1].Quay);
            Assert.AreEqual(2, rec.Cits[1].Media.Count);
            media = rec.Cits[1].Media[0];
            Assert.AreEqual("mref2", media.Xref);
            media = rec.Cits[1].Media[1];
            Assert.AreEqual("filerefn", media.Files[0].FileRefn);
        }

        // TODO NOTE+SOUR+EVEN+ROLE
        // TODO NOTE+SOUR+DATA+DATE+TEXT
        // TODO NOTE+SOUR+OBJE - other lines, error scenarios
        // TODO NOTE+SOUR+NOTE+...

        /*
        2 SOUR @SOURCE1@
        3 PAGE 42
        3 DATA
        4 DATE BEF 1 JAN 1900
        4 TEXT a sample text
        5 CONT Sample text continued here. The word TE
        5 CONC ST should not be broken!
        3 QUAY 0
        3 NOTE A note
        4 CONT Note continued here. The word TE
        4 CONC ST should not be broken!
        */
        /*
2 SOUR @S2459518462@
3 NOTE http://trees.ancestry.com/rd?f=sse&db=wwiienlist&h=2022022&ti=0&indiv=try
3 NOTE
3 DATA
4 TEXT Name:  Cleo VanselBirth Date:  1923Birth Place:  Residence Date:  Residence Place:  Missouri
2 SOUR @S3096303497@
3 NOTE http://trees.ancestry.com/rd?f=sse&db=momarriages&h=500765573&ti=0&indiv=try
3 NOTE
3 DATA
4 TEXT Name:  Wilma Joan JacksonBirth Date:  abt 1934Birth Place:  Sedalia, Pettis, Missouri
2 SOUR @S2459518276@
3 PAGE Number: 499-16-6654; Issue State: Missouri; Issue Date: Before 1951.
3 NOTE http://trees.ancestry.com/rd?f=sse&db=ssdi&h=64159593&ti=0&indiv=try
3 NOTE http://search.ancestry.com/cgi-bin/sse.dll?db=ssdi&h=64159593&ti=0&indiv=try
3 DATA
4 TEXT Name:  Cleo VanselBirth Date:  9 Jul 1923Birth Place:  Death Date:  17 Apr 1991Death Place:  Sedalia, Pettis, Missouri, United States of America
        */
        // TODO test complex nested structs  0INDI+1ASSOC+2SOUR+3TEXT; 0NOTE+1SOUR+2DATA+3TEXT
    }

}
