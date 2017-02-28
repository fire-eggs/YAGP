using NUnit.Framework;
using SharpGEDParser.Parser;

// TODO valid symbol


namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class DateToken
    {
        [Test]
        public void Test()
        {
            string val = "13 Apr 1964";
            var tokenize = new DateTokens();
            var toks = tokenize.Tokenize(val);
            Assert.AreEqual(3, toks.Count);
            Assert.AreEqual(0, toks[0].offset);
            Assert.AreEqual(2, toks[0].length);
            Assert.AreEqual(TokType.NUM, toks[0].type);
            Assert.AreEqual(3, toks[1].offset);
            Assert.AreEqual(3, toks[1].length);
            Assert.AreEqual(TokType.WORD, toks[1].type);
            Assert.AreEqual(7, toks[2].offset);
            Assert.AreEqual(4, toks[2].length);
            Assert.AreEqual(TokType.NUM, toks[2].type);
        }

        [Test]
        public void Test2()
        {
            string val = "@#DGREGORIAN@  1964B.C.";
            var tokenize = new DateTokens();
            var toks = tokenize.Tokenize(val);
            Assert.AreEqual(3, toks.Count);
            CheckToken(toks[0], TokType.CALEN, 1, 11);
            CheckToken(toks[1], TokType.NUM, 15, 4);
            CheckToken(toks[2], TokType.WORD, 19, 4);
        }

        [Test]
        public void Test3()
        {
            // improperly terminate calendar escape
            string val = "@#DGREGORIAN  1964B.C.";
            var tokenize = new DateTokens();
            var toks = tokenize.Tokenize(val);
            Assert.AreEqual(3, toks.Count);
            CheckToken(toks[0], TokType.CALEN, 1, 11);
            CheckToken(toks[1], TokType.NUM, 14, 4);
            CheckToken(toks[2], TokType.WORD, 18, 4);
        }

        [Test]
        public void TestPhrase()
        {
            string val = "INT 1964 (tombstone calculated)";
            var toks = new DateTokens().Tokenize(val);
            Assert.AreEqual(3, toks.Count);
            CheckToken(toks[0], TokType.WORD, 0, 3);
            CheckToken(toks[1], TokType.NUM, 4, 4);
            CheckToken(toks[2], TokType.PHRASE, 10, 20);
        }

        private void CheckToken(Token tok, TokType targetType, int off = -1, int len = -1)
        {
            Assert.AreEqual(targetType, tok.type);
            if (len != -1)
                Assert.AreEqual(len, tok.length, "length");
            if (off != -1)
                Assert.AreEqual(off, tok.offset, "offset");
        }

        [Test]
        public void TestMix()
        {
            string val = "BET Jun 1910 AND 1 Aug 1911";
            var toks = new DateTokens().Tokenize(val);
            Assert.AreEqual(7, toks.Count);
            CheckToken(toks[0], TokType.WORD);
            CheckToken(toks[1], TokType.WORD);
            CheckToken(toks[3], TokType.WORD);
            CheckToken(toks[5], TokType.WORD);
            CheckToken(toks[2], TokType.NUM);
            CheckToken(toks[4], TokType.NUM);
            CheckToken(toks[6], TokType.NUM);
        }

        [Test]
        public void TestUnk()
        {
            string val = "gibber!";
            var toks = new DateTokens().Tokenize(val);
            Assert.AreEqual(2, toks.Count);
            CheckToken(toks[0], TokType.WORD, 0, 6);
            CheckToken(toks[1], TokType.UNK, 6, 1);
        }

        [Test]
        public void TestSym()
        {
            string val = "1860/61";
            var toks = new DateTokens().Tokenize(val);
            Assert.AreEqual(3, toks.Count);
            CheckToken(toks[0], TokType.NUM, 0, 4);
            CheckToken(toks[1], TokType.SYMB, 4, 1);
            CheckToken(toks[2], TokType.NUM, 5, 2);

        }
    }
}
