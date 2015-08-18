using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class SourTest : GedParseTest
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
    }
}
