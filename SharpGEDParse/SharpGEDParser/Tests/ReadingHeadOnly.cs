using NUnit.Framework;

// ReSharper disable InconsistentNaming

// variations on a file with only "0 HEAD".

// TODO FileRead.LineBreaks as an exposed property?

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class ReadingHeadOnly : ReadingUtil
    {
        [Test]
        public void HeadNoBom()
        {
            var r = ReadFile("0 HEAD");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadLFNoBom()
        {
            var r = ReadFile("0 HEAD\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadCRLFNoBom()
        {
            var r = ReadFile("0 HEAD\r\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadBom()
        {
            var r = ReadFile("0 HEAD", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadLFBom()
        {
            var r = ReadFile("0 HEAD\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadCRLFBom()
        {
            var r = ReadFile("0 HEAD\r\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusNoBom()
        {
            var r = ReadFile("0 HEAD extra");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusLFNoBom()
        {
            var r = ReadFile("0 HEAD extra\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusCRLFNoBom()
        {
            var r = ReadFile("0 HEAD extra\r\n");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusBom()
        {
            var r = ReadFile("0 HEAD extra", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusLFBom()
        {
            var r = ReadFile("0 HEAD extra\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

        [Test]
        public void HeadPlusCRLFBom()
        {
            var r = ReadFile("0 HEAD extra\r\n", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
            //Assert.AreEqual("ERR", r.LineBreaks);
        }

    }

}
