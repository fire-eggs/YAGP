using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class Events : GedWriteTest
    {
        [Test]
        public void Text()
        {
            var inp = "0 @I1@ INDI\n1 DEAT Y";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void Dscr()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then?";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void DscrCont()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then?\n2 CONT I don't know the secret handshake";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void DscrConc()
        {
            var inp = "0 @I1@ INDI\n1 DSCR He's a big man then\n2 CONC ? I don't know the secret handshake";
            var exp = "0 @I1@ INDI\n1 DSCR He's a big man then? I don't know the secret handshake\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void BFamc()
        {
            var inp = "0 @I1@ INDI\n1 BIRT Y\n2 FAMC @F1@";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void AFamc()
        {
            var inp = "0 @I1@ INDI\n1 ADOP Y\n2 FAMC @F1@";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
        [Test]
        public void FamcAdop()
        {
            var inp = "0 @I1@ INDI\n1 ADOP Y\n2 FAMC @F1@\n3 ADOP Father";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
    }
}
