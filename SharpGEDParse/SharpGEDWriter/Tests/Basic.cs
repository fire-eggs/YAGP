using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class Basic : GedWriteTest
    {
        [Test]
        public void TryIt()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1+"\n", res);
        }
    }
}
