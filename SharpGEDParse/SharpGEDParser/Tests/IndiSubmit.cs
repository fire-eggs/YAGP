using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// Exercise the various submitter tags under INDI
// TODO cannot exercise validation of Xref id until SUBM record supported

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class IndiSubmit : GedParseTest
    {
        [Test]
        public void TestAnci()
        {
            var txt = "0 @I1@ INDI \n1 ANCI @S1@";
            var rec = parse<IndiRecord>(txt);
            Assert.AreEqual(1, rec.Submitters.Count);
            Assert.AreEqual(Submitter.SubmitType.ANCI, rec.Submitters[0].SubmitterType);
            Assert.AreEqual("S1", rec.Submitters[0].Xref);
        }
        [Test]
        public void TestDesi()
        {
            var txt = "0 @I1@ INDI \n1 DESI @S1@";
            var rec = parse<IndiRecord>(txt);
            Assert.AreEqual(1, rec.Submitters.Count);
            Assert.AreEqual(Submitter.SubmitType.DESI, rec.Submitters[0].SubmitterType);
            Assert.AreEqual("S1", rec.Submitters[0].Xref);
        }
        [Test]
        public void TestSubm()
        {
            var txt = "0 @I1@ INDI \n1 SUBM @S1@";
            var rec = parse<IndiRecord>(txt);
            Assert.AreEqual(1, rec.Submitters.Count);
            Assert.AreEqual(Submitter.SubmitType.SUBM, rec.Submitters[0].SubmitterType);
            Assert.AreEqual("S1", rec.Submitters[0].Xref);
        }
        [Test]
        public void TestSubmErr()
        {
            var txt = "0 @I1@ INDI \n1 SUBM ";
            var rec = parse<IndiRecord>(txt);
            Assert.AreEqual(1, rec.Errors.Count); // TODO valid error details
        }
    }
}
