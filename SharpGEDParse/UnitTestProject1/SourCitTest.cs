using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

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

namespace UnitTestProject1
{
    [TestClass]
    public class SourCitTest : GedParseTest
    {
        private KBRGedFam parseFam(string val)
        {
            return parse<KBRGedFam>(val, "FAM");
        }

        private KBRGedIndi parseInd(string val)
        {
            return parse<KBRGedIndi>(val, "INDI");
        }

        [TestMethod]
        public void TestIndiSour()
        {
            // SOUR record on the INDI
            var indi1 = "0 INDI\n1 SOUR @p1@";
            KBRGedIndi rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual("p1", rec.Sources[0].XRef);
            var indi2 = "0 INDI\n1 SOUR @p1@\n1 SOUR @p2@";
            KBRGedIndi rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Sources.Count);
            Assert.AreEqual("p1", rec2.Sources[0].XRef);
            Assert.AreEqual("p2", rec2.Sources[1].XRef);
        }

        [TestMethod]
        public void TestIndiEvent()
        {
            // SOUR record on the event
            string indi1 = "0 INDI\n1 BIRT\n2 DATE 1774\n2 SOUR @p1@";
            KBRGedIndi rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Events[0].Sources.Count);
            Assert.AreEqual("p1", rec.Events[0].Sources[0].XRef);

            string indi2 = "0 INDI\n1 BIRT\n2 SOUR @p1@\n2 DATE 1774\n2 SOUR @p2@";
            KBRGedIndi rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Events[0].Sources.Count);
            Assert.AreEqual("p1", rec2.Events[0].Sources[0].XRef);
            Assert.AreEqual("p2", rec2.Events[0].Sources[1].XRef);
        }

        [TestMethod]
        public void TestFamSour()
        {
            // SOUR record on the FAM
            string fam = "0 @F1@ FAM\n1 SOUR @p1@";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual("p1", rec.Sources[0].XRef);
            string fam2 = "0 @F1@ FAM\n1 SOUR @p1@\n1 SOUR @p2@";
            var rec2 = parseFam(fam2);
            Assert.AreEqual(2, rec2.Sources.Count);
            Assert.AreEqual("p1", rec2.Sources[0].XRef);
            Assert.AreEqual("p2", rec2.Sources[1].XRef);
        }

        [TestMethod]
        public void TestIndiEmbSour()
        {
            // Embedded SOUR record on the INDI
            var indi1 = "0 INDI\n1 SOUR this is a source";
            KBRGedIndi rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual("this is a source", rec.Sources[0].Embed);
            var indi2 = "0 INDI\n1 SOUR this is a source\n1 SOUR this is another";
            KBRGedIndi rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Sources.Count);
            Assert.AreEqual(null, rec2.Sources[0].XRef);
            Assert.AreEqual(null, rec2.Sources[1].XRef);
            Assert.AreEqual("this is a source", rec2.Sources[0].Embed);
            Assert.AreEqual("this is another", rec2.Sources[1].Embed);
        }

        [TestMethod]
        public void TestIndiEventEmb()
        {
            // Embedded SOUR record on the INDI event
            var indi1 = "0 INDI\n1 BIRT\n2 SOUR this is a source";
            KBRGedIndi rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Events[0].Sources.Count);
            Assert.AreEqual(null, rec.Events[0].Sources[0].XRef);
            Assert.AreEqual("this is a source", rec.Events[0].Sources[0].Embed);
            var indi2 = "0 INDI\n1 BIRT\n2 SOUR this is a source\n2 SOUR this is another";
            KBRGedIndi rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Events[0].Sources.Count);
            Assert.AreEqual(null, rec2.Events[0].Sources[0].XRef);
            Assert.AreEqual(null, rec2.Events[0].Sources[1].XRef);
            Assert.AreEqual("this is a source", rec2.Events[0].Sources[0].Embed);
            Assert.AreEqual("this is another", rec2.Events[0].Sources[1].Embed);
        }

        [TestMethod]
        public void TestFamEmbSour()
        {
            // Embedded SOUR record on the FAM
            string fam = "0 @F1@ FAM\n1 SOUR this is a source";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual("this is a source", rec.Sources[0].Embed);
            string fam2 = "0 @F1@ FAM\n1 SOUR this is one source\n1 SOUR this is another";
            var rec2 = parseFam(fam2);
            Assert.AreEqual(2, rec2.Sources.Count);
            Assert.AreEqual(null, rec2.Sources[0].XRef);
            Assert.AreEqual(null, rec2.Sources[1].XRef);
            Assert.AreEqual("this is one source", rec2.Sources[0].Embed);
            Assert.AreEqual("this is another", rec2.Sources[1].Embed);
        }

        [TestMethod]
        public void TestInvalidXref()
        {
            string txt = "0 INDI\n1 SOUR @ @";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Sources.Count);
            txt = "0 INDI\n1 SOUR @@@";
            rec = parseInd(txt);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(0, rec.Sources.Count);
        }

        [TestMethod]
        public void TestIndiEmbSour2()
        {
            // Embedded SOUR record on the INDI with CONC/CONT
            var indi1 = "0 INDI\n1 SOUR this is a source \n2 CONC with extension";
            KBRGedIndi rec = parseInd(indi1);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual("this is a source with extension", rec.Sources[0].Embed);
            var indi2 = "0 INDI\n1 SOUR this is a source\n2 CONT extended to next line\n1 SOUR this is another";
            KBRGedIndi rec2 = parseInd(indi2);
            Assert.AreEqual(2, rec2.Sources.Count);
            Assert.AreEqual(null, rec2.Sources[0].XRef);
            Assert.AreEqual(null, rec2.Sources[1].XRef);
            Assert.AreEqual("this is a source\nextended to next line", rec2.Sources[0].Embed);
            Assert.AreEqual("this is another", rec2.Sources[1].Embed);
        }

        [TestMethod]
        public void TestEmbSourText()
        {
            var txt = "0 INDI\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(1, rec.Sources[0].Notes.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual("embedded source", rec.Sources[0].Embed);
            Assert.AreEqual("this is text", rec.Sources[0].Text);
        }
        [TestMethod]
        public void TestEmbSourText2()
        {
            var txt = "0 INDI\n1 SOUR embedded source\n2 NOTE a note\n2 TEXT this is text ex\n3 CONC tended";
            var rec = parseInd(txt);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(1, rec.Sources[0].Notes.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual("embedded source", rec.Sources[0].Embed);
            Assert.AreEqual("this is text extended", rec.Sources[0].Text);
        }

        [TestMethod]
        public void TestSourCitErr()
        {
            // TEXT tag for reference source is error
            string fam = "0 @F1@ FAM\n1 SOUR @p1@\n2 TEXT this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual("p1", rec.Sources[0].XRef);
            Assert.AreEqual(1, rec.Sources[0].Errors.Count, "No error");
        }

        [TestMethod]
        public void TestSourCitErr2()
        {
            // PAGE tag for embedded source is error
            string fam = "0 @F1@ FAM\n1 SOUR inbed\n2 PAGE this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual(1, rec.Sources[0].Errors.Count, "No error");
        }

        [TestMethod]
        public void TestSourCitErr3()
        {
            // EVEN tag for embedded source is error
            string fam = "0 @F1@ FAM\n1 SOUR inbed\n2 EVEN this is error";
            var rec = parseFam(fam);
            Assert.AreEqual(1, rec.Sources.Count);
            Assert.AreEqual(null, rec.Sources[0].XRef);
            Assert.AreEqual(1, rec.Sources[0].Errors.Count, "No error");
        }
    }
}
