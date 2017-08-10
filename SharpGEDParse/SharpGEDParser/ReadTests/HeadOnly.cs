using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class HeadOnly : TestUtil
    {
        [Test]
        public void HeadNoBom()
        {
            GedReader r = ReadFile("0 HEAD");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadLFNoBom()
        {
            GedReader r = ReadFile("0 HEAD\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadCRLFNoBom()
        {
            GedReader r = ReadFile("0 HEAD\r\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadBom()
        {
            GedReader r = ReadFile("0 HEAD", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadLFBom()
        {
            GedReader r = ReadFile("0 HEAD\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadCRLFBom()
        {
            GedReader r = ReadFile("0 HEAD\r\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusNoBom()
        {
            GedReader r = ReadFile("0 HEAD extra");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusLFNoBom()
        {
            GedReader r = ReadFile("0 HEAD extra\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusCRLFNoBom()
        {
            GedReader r = ReadFile("0 HEAD extra\r\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusBom()
        {
            GedReader r = ReadFile("0 HEAD extra", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusLFBom()
        {
            GedReader r = ReadFile("0 HEAD extra\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusCRLFBom()
        {
            GedReader r = ReadFile("0 HEAD extra\r\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            Assert.AreEqual("ERR", r.LineBreaks);
        }

    }
}
