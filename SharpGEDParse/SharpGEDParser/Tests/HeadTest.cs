using NUnit.Framework;
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class HeadTest : GedParseTest
    {
        [Test]
        public void Simple()
        {
            var txt = "0 HEAD\n";
            var rec = ReadOne(txt);
            Assert.AreEqual("HEAD", rec.Tag);
        }
        [Test]
        public void BadDate()
        {
            var txt = "0 HEAD\n1 DATE";
            var rec = ReadOne(txt);
            Assert.AreEqual("HEAD", rec.Tag);
        }
        [Test]
        public void BadSubm()
        {
            var txt = "0 HEAD\n1 SUBM";
            var rec = ReadOne(txt);
            Assert.AreEqual("HEAD", rec.Tag);
            Assert.AreEqual(1, rec.Errors.Count);
        }
        [Test]
        public void ExtraSubm()
        {
            // TODO should this be an error?
            var txt = "0 HEAD\n1 SUBM @I5@ blah";
            var rec = ReadOne(txt);
            Assert.AreEqual("HEAD", rec.Tag);
            Assert.AreEqual(0, rec.Errors.Count);
        }
    }
}
