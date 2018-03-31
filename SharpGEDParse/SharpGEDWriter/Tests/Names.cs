using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Names : GedWriteTest
    {
        [Test]
        public void Basic()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred";
            var exp = "0 @I1@ INDI\n1 NAME Fred\n2 GIVN Fred\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void Suffix()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/ Jr.";
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/ Jr.\n2 GIVN Fred\n2 SURN Flintstone\n2 NSFX Jr.\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void Parts()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 SURN Flintstone\n2 GIVN Fred\n2 NICK Yabba dabba do\n2 NPFX blah\n2 SPFX blah\n2 NSFX blah";
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NICK Yabba dabba do\n2 NPFX blah\n2 SPFX blah\n2 NSFX blah\n2 GIVN Fred\n2 SURN Flintstone\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }
        [Test]
        public void Note()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NOTE This is a note";
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 GIVN Fred\n2 SURN Flintstone\n2 NOTE This is a note\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }
        [Test]
        public void SourCit()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 SOUR @S3@";
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 GIVN Fred\n2 SURN Flintstone\n2 SOUR @S3@\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void Suffix2()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NSFX Jr.";
            // TODO note ordering issue - parts output in specific order despite how they came in
            // TODO should suffix have been appended to the "1 NAME" line???
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NSFX Jr.\n2 GIVN Fred\n2 SURN Flintstone\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void Prefix()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NPFX Prof.";
            // TODO note ordering issue - parts output in specific order despite how they came in
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NPFX Prof.\n2 GIVN Fred\n2 SURN Flintstone\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void SurPrefix()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 SPFX van";
            // TODO note ordering issue - parts output in specific order despite how they came in
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 SPFX van\n2 GIVN Fred\n2 SURN Flintstone\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

        [Test]
        public void Nick()
        {
            var inp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NICK yabba dabba doo";
            // TODO note ordering issue - parts output in specific order despite how they came in
            var exp = "0 @I1@ INDI\n1 NAME Fred /Flintstone/\n2 NICK yabba dabba doo\n2 GIVN Fred\n2 SURN Flintstone\n";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(exp, res);
        }

    }
}
