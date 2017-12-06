using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Media : GedWriteTest
    {
        [Test]
        public void Obje()
        {
            var inp = "0 @M1@ OBJE\n1 FILE blah\n2 TITL anim\n2 FORM fie\n3 TYPE gif\n1 REFN blah\n1 RIN auto_id";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SimpleXref()
        {
            var inp = "0 @I1@ INDI\n1 OBJE @M1@";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp+"\n", res);
        }

        [Test]
        public void EmbedVar1()
        {
            var inp = "0 @I1@ INDI\n1 OBJE\n2 TITL title\n2 FILE file_refn\n3 FORM gif\n4 MEDI what is this?";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void EmbedVar2()
        {
            var inp = "0 @I1@ INDI\n1 OBJE\n2 FILE file_refn\n2 FORM gif\n3 MEDI what is this?";
            var exp = "0 @I1@ INDI\n1 OBJE\n2 FILE file_refn\n3 FORM gif\n4 MEDI what is this?\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }
    }
}
