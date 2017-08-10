using NUnit.Framework;

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class Empty : TestUtil
    {
        [Test]
        public void EmptyNoBom()
        {
            GedReader r = ReadFile("");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }

        [Test]
        public void EmptyBom()
        {
            GedReader r = ReadFile("", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }

        [Test]
        public void EmptyGibberNoBom()
        {
            GedReader r = ReadFile("garbage");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }
        [Test]
        public void EmptyGibberBom()
        {
            GedReader r = ReadFile("garbage", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }
    }
}
