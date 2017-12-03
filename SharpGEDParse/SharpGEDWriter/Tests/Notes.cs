using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class Notes : GedWriteTest
    {
        [Test]
        public void Simple()
        {
            var txt = "0 @I1@ INDI\n1 NOTE @N1@";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, txt+"\n");
        }

        [Test]
        public void Short()
        {
            var txt = "0 @I1@ INDI\n1 NOTE this is a note";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void ShortCont()
        {
            var txt = "0 @I1@ INDI\n1 NOTE this is a\n2 CONT note";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void ShortConc()
        {
            var txt = "0 @I1@ INDI\n1 NOTE this is a n\n2 CONC ote";
            var exp = "0 @I1@ INDI\n1 NOTE this is a note\n";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, exp);
        }

        [Test]
        public void ShortRec()
        {
            var txt = "0 @N1@ NOTE this is a note";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void ShortContRec()
        {
            var txt = "0 @N1@ NOTE this is a longer\n1 CONT note";
            var res = ParseAndWrite(txt);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void ShortConcRec()
        {
            var txt = "0 @N1@ NOTE this is a long\n1 CONC er note";
            var res = ParseAndWrite(txt);
            var exp = "0 @N1@ NOTE this is a longer note\n";
            Assert.AreEqual(exp, res);
        }
    }
}
