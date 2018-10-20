using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// SOURCE_CITATION testing

// TODO source citation on all valid structures; both embedded and reference
// PERSONAL_NAME_PIECES
// LDS_SPOUSE_SEALING
// LDS_INDIVIDUAL_ORDINANCE
// EVENT_DETAIL
// ASSOCIATION_STRUCTURE
// MULTIMEDIA_RECORD
// INDIVIDUAL_RECORD
// FAM_RECORD

// TODO all sub-tags

// TODO generalize for all record types

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class SourCit : GedParseTest
    {
        private IndiRecord parseInd(string val)
        {
            return parse<IndiRecord>(val);
        }

        private FamRecord parseFam(string val)
        {
            return parse<FamRecord>(val);
        }

        [Test]
        public void RefSourceInv()
        {
            // invalid reference source id
            string txt = "0 @N1@ NOTE\n1 SOUR @@";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.AreNotEqual(0, (int)res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void RefSourceText()
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
            Assert.AreNotEqual(0, (int)res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void EmbSourcePage()
        {
            // PAGE tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR embed\n2 PAGE this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.IsNotEmpty(rec.Cits[0].Page);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.AreNotEqual(0, (int)res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void EmbSourceNoPage()
        {
            // Embedded source - no PAGE tag
            string txt = "0 @N1@ NOTE\n1 SOUR embed";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(0, res[0].Errors.Count, "error");
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(null, rec.Cits[0].Page);
        }

        [Test]
        public void EmbSourceEven()
        {
            // EVEN tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR inbed\n2 EVEN this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.IsNull(rec.Cits[0].Xref);
            Assert.IsNotEmpty(rec.Cits[0].Event);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.AreNotEqual(0, (int)res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void RefSourceEven()
        {
            // EVEN tag for ref source is not error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 EVEN this is no error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.IsNotEmpty(rec.Cits[0].Event);
            Assert.IsEmpty(rec.Cits[0].Page);
            Assert.AreEqual(0, res[0].Errors.Count, "No error");
        }

        [Test]
        public void RefSourcePage()
        {
            // PAGE tag for ref source is not error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 PAGE this is no error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.IsEmpty(rec.Cits[0].Event);
            Assert.IsNotEmpty(rec.Cits[0].Page);
            Assert.AreEqual(0, res[0].Errors.Count, "No error");
        }

        [Test]
        public void TestIndiSour()
        {
            // SOUR record on the INDI
            var indi1 = "0 @I1@ INDI\n1 SOUR @p1@";
            IndiRecord rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            var indi2 = "0 @I1@ INDI\n1 SOUR @p1@\n1 SOUR @p2@";
            IndiRecord rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Cits.Count);
            Assert.AreEqual("p1", rec2.Cits[0].Xref);
            Assert.AreEqual("p2", rec2.Cits[1].Xref);
        }

        [Test]
        public void TestIndiEvent()
        {
            // SOUR record on the event
            string indi1 = "0 INDI\n1 BIRT\n2 DATE 1774\n2 SOUR @p1@";
            var rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Events[0].Cits.Count);
            Assert.AreEqual("p1", rec.Events[0].Cits[0].Xref);

            string indi2 = "0 INDI\n1 BIRT\n2 SOUR @p1@\n2 DATE 1774\n2 SOUR @p2@";
            var rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Events[0].Cits.Count);
            Assert.AreEqual("p1", rec2.Events[0].Cits[0].Xref);
            Assert.AreEqual("p2", rec2.Events[0].Cits[1].Xref);
        }

        [Test]
        public void TestFamSour()
        {
            // SOUR record on the FAM
            string fam = "0 @F1@ FAM\n1 SOUR @p1@";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            string fam2 = "0 @F1@ FAM\n1 SOUR @p1@\n1 SOUR @p2@";
            rec = parseFam(fam2);
            Assert.AreEqual(2, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.AreEqual("p2", rec.Cits[1].Xref);
        }

        [Test]
        public void TestIndiEmbSour()
        {
            // Embedded SOUR record on the INDI
            var indi1 = "0 INDI\n1 SOUR this is a source";
            var rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("this is a source", rec.Cits[0].Desc);
            var indi2 = "0 INDI\n1 SOUR this is a source\n1 SOUR this is another";
            var rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Cits.Count);
            Assert.AreEqual(null, rec2.Cits[0].Xref);
            Assert.AreEqual(null, rec2.Cits[1].Xref);
            Assert.AreEqual("this is a source", rec2.Cits[0].Desc);
            Assert.AreEqual("this is another", rec2.Cits[1].Desc);
        }

        [Test]
        public void TestIndiEventEmb()
        {
            // Embedded SOUR record on the INDI event
            var indi1 = "0 INDI\n1 BIRT\n2 SOUR this is a source";
            var rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Events[0].Cits.Count);
            Assert.AreEqual(null, rec.Events[0].Cits[0].Xref);
            Assert.AreEqual("this is a source", rec.Events[0].Cits[0].Desc);
            var indi2 = "0 INDI\n1 BIRT\n2 SOUR this is a source\n2 SOUR this is another";
            var rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Events[0].Cits.Count);
            Assert.AreEqual(null, rec2.Events[0].Cits[0].Xref);
            Assert.AreEqual(null, rec2.Events[0].Cits[1].Xref);
            Assert.AreEqual("this is a source", rec2.Events[0].Cits[0].Desc);
            Assert.AreEqual("this is another", rec2.Events[0].Cits[1].Desc);
        }

        [Test]
        public void TestFamEmbSour()
        {
            // Embedded SOUR record on the FAM
            string fam = "0 @F1@ FAM\n1 SOUR this is a source";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("this is a source", rec.Cits[0].Desc);
            string fam2 = "0 @F1@ FAM\n1 SOUR this is one source\n1 SOUR this is another";
            var rec2 = parseFam(fam2);
            Assert.AreEqual(2, rec2.Cits.Count);
            Assert.AreEqual(null, rec2.Cits[0].Xref);
            Assert.AreEqual(null, rec2.Cits[1].Xref);
            Assert.AreEqual("this is one source", rec2.Cits[0].Desc);
            Assert.AreEqual("this is another", rec2.Cits[1].Desc);
        }

        [Test]
        public void TestInvalidXref()
        {
            string txt = "0 @I1@ INDI\n1 SOUR @ @";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(1, rec.Cits.Count);
            txt = "0 @I1@ INDI\n1 SOUR @@@";
            rec = parseInd(txt);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(1, rec.Cits.Count);
        }

        [Test]
        public void TestIndiEmbSour2()
        {
            // Embedded SOUR record on the INDI with CONC/CONT
            var indi1 = "0 INDI\n1 SOUR this is a source \n2 CONC with extension";
            var rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("this is a source with extension", rec.Cits[0].Desc);
            var indi2 = "0 INDI\n1 SOUR this is a source\n2 CONT extended to next line\n1 SOUR this is another";
            var rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Cits.Count);
            Assert.AreEqual(null, rec2.Cits[0].Xref);
            Assert.AreEqual(null, rec2.Cits[1].Xref);
            Assert.AreEqual("this is a source\nextended to next line", rec2.Cits[0].Desc);
            Assert.AreEqual("this is another", rec2.Cits[1].Desc);
        }

        [Test]
        public void TestEmbSourText()
        {
            var txt = "0 INDI\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("embedded source", rec.Cits[0].Desc);
            Assert.AreEqual("this is text", rec.Cits[0].Text[0]);
        }
        [Test]
        public void TestEmbSourText2()
        {
            var txt = "0 INDI\n1 SOUR embedded source\n2 TEXT this is text ex\n3 CONC tended\n2 NOTE a note";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(1, rec.Cits[0].Notes.Count);
            Assert.AreEqual("a note", rec.Cits[0].Notes[0].Text);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual("embedded source", rec.Cits[0].Desc);
            Assert.AreEqual("this is text extended", rec.Cits[0].Text[0]);
        }

        [Test]
        public void TestSourCitErr()
        {
            // TEXT tag for reference source is error
            string fam = "0 @F1@ FAM\n1 SOUR @p1@\n2 TEXT this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Errors.Count, "No error"); // TODO validate details
        }

        [Test]
        public void TestSourCitErr2()
        {
            // PAGE tag for embedded source is error
            string fam = "0 @F1@ FAM\n1 SOUR inbed\n2 PAGE this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Errors.Count, "No error");
        }

        [Test]
        public void TestSourCitErr3()
        {
            // EVEN tag for embedded source is error
            string fam = "0 @F1@ FAM\n1 SOUR inbed\n2 EVEN this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(1, rec.Errors.Count, "No error");
        }

    }
}
