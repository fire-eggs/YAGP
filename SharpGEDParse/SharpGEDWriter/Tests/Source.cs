using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [TestFixture]
    class Source : GedWriteTest
    {
        [Test]
        public void SourRepo()
        {
            // SOUR.REPO was missing '@'s
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO @R1@\n1 RIN rin-chan";
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n");
        }

        [Test]
        public void SourRepo2()
        {
            // SOUR.REPO with no xref
            var txt = "0 @S1@ SOUR\n1 AUTH Fred\n1 REPO\n2 CALN blah\n1 RIN rin-chan";
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n");
        }

        private void ShortTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a single-line blah";
            var txt = string.Format(format, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag + "Sh");
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n", tag + "Sh");
        }

        private void ShortConcTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a single\n2 CONC -line blah";
            var expform = "0 @S1@ SOUR\n1 {0} this is a single-line blah\n";
            var txt = string.Format(format, tag);
            var exp = string.Format(expform, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag+"ShCo");
            var res = Write(fr);
            Assert.AreEqual(res, exp, tag + "ShCo");
        }

        private void ShortContTest(string tag)
        {
            var format = "0 @S1@ SOUR\n1 {0} this is a multiple\n2 CONT -line blah";
            var txt = string.Format(format, tag);
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count, tag + "ShCt");
            var res = Write(fr);
            Assert.AreEqual(res, txt + "\n", tag + "ShCt");
        }

        [Test]
        public void Auth()
        {
            ShortTest("AUTH");
            ShortConcTest("AUTH");
            ShortContTest("AUTH");
        }

        [Test]
        public void Titl()
        {
            ShortTest("TITL");
            ShortConcTest("TITL");
            ShortContTest("TITL");
        }

        [Test]
        public void Publ()
        {
            ShortTest("PUBL");
            ShortConcTest("PUBL");
            ShortContTest("PUBL");
        }

        [Test]
        public void Text()
        {
            ShortTest("TEXT");
            ShortConcTest("TEXT");
            ShortContTest("TEXT");
        }

        [Test]
        public void SourCitRec()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitRec2()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is multi-\n4 CONT line text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitRec3()
        {
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short conc\n4 CONC atenated text";
            var res = ParseAndWrite(inp);
            var exp = "0 @I1@ INDI\n1 SOUR @S1@\n2 DATA\n3 TEXT This is short concatenated text\n";
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void SourCit()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is short text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCit2()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is multi-\n2 CONT line text";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCit3()
        {
            var inp = "0 @I1@ INDI\n1 SOUR This is short conc\n2 CONC atenated text";
            var res = ParseAndWrite(inp);
            var exp = "0 @I1@ INDI\n1 SOUR This is short concatenated text\n";
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void SourCitQuay()
        {
            // I observed that Quay was done wrong [src record variant]
            var inp = "0 @I1@ INDI\n1 SOUR @S1@\n2 QUAY quack";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void SourCitQuay2()
        {
            // I observed that Quay was done wrong [no src record variant]
            var inp = "0 @I1@ INDI\n1 SOUR\n2 QUAY quack";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
    }
}
