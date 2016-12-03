using NUnit.Framework;

// sub-tags off INDI-FAMC
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiFamc : GedParseTest
    {
        [Test]
        public void FamcValid()
        {
            // 5.5.1 standard sub-tags
            var indi = "0 @I1@ INDI\n1 FAMC @F1@\n2 PEDI foster\n2 STAT disproven\n2 NOTE @N1@\n1 RIN blah";
            var rec = parse<IndiRecord>(indi);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual("blah",rec.RIN);

            Assert.AreEqual(1, rec.ChildLinks.Count);
            Assert.AreEqual("F1", rec.ChildLinks[0]);

            Assert.AreEqual(0, rec.Unknowns.Count); // PEDI/STAT first treated as unknown
            Assert.AreEqual(0, rec.Notes.Count);    // FAMC-NOTE treated as INDI-NOTE

            // TODO validate PEDI, STAT values
        }

        [Test]
        public void FamcCustom()
        {
            // _PREF, _MREL, _FREL are 'common' custom sub-tags
            var indi = "0 @I1@ INDI\n1 FAMC @F1@\n2 _PREF Y\n2 _MREL blah\n2 NOTE @N1@\n1 RIN blah";
            var rec = parse<IndiRecord>(indi);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual("blah", rec.RIN);

            Assert.AreEqual(1, rec.ChildLinks.Count);
            Assert.AreEqual("F1", rec.ChildLinks[0]);

            Assert.AreEqual(0, rec.Unknowns.Count); // PEDI/STAT first treated as unknown
            Assert.AreEqual(0, rec.Notes.Count);    // FAMC-NOTE treated as INDI-NOTE

            // TODO validate values
        }

        [Test]
        public void FamsValid()
        {
            // The only 5.5.1 sub-tag which is valid for FAMS is NOTE
            var indi = "0 @I1@ INDI\n1 FAMS @F1@\n2 NOTE @N1@\n1 RIN blah";
            var rec = parse<IndiRecord>(indi);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual("blah", rec.RIN);

            Assert.AreEqual(1, rec.FamLinks.Count);
            Assert.AreEqual("F1", rec.FamLinks[0]);

            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(0, rec.Notes.Count);    // FAMS-NOTE treated as INDI-NOTE
        }

        [Test]
        public void FamsCustom()
        {
            // There are a small number of non-standard sub-tags out there;
            // the 'most common' (0.000019) is RFN
            var indi = "0 @I1@ INDI\n1 FAMS @F1@\n2 RFN blah\n2 NOTE @N1@\n1 RIN blah";
            var rec = parse<IndiRecord>(indi);

            Assert.AreEqual(0, rec.Errors.Count);

            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual("blah", rec.RIN);

            Assert.AreEqual(1, rec.FamLinks.Count);
            Assert.AreEqual("F1", rec.FamLinks[0]);

            Assert.AreEqual(0, rec.Unknowns.Count);
            Assert.AreEqual(0, rec.Notes.Count);    // FAMS-NOTE treated as INDI-NOTE

        }
    }
}
