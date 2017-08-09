using NUnit.Framework;

// ReSharper disable InconsistentNaming

// variations on an empty file
namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class ReadingEmpty : ReadingUtil
    {
        [Test]
        public void EmptyNoBom()
        {
            var r = ReadFile("");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }

        [Test]
        public void EmptyBom()
        {
            var r = ReadFile("", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }

        [Test]
        public void EmptyGibberNoBom()
        {
            var r = ReadFile("garbage");
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }

        [Test]
        public void EmptyGibberBom()
        {
            var r = ReadFile("garbage", true);
            var errs = r.Errors;
            Assert.AreEqual(1, errs.Count);
        }
    }
}
