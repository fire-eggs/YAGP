using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// TODO missing relation text
// TODO missing relation tag
// TODO sources
// TODO notes
// TODO extra text after ident e.g. "1 ASSO @I2@ foobar"
// TODO implication elsewhere: an ident of "@ @" will be considered missing. check usages of parseXrefExtra

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class IndiAssocs : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        [Test]
        public void BasicAsso()
        {
            var indi = "0 @I1@ INDI\n1 ASSO @I2@\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual("I2", rec.Assocs[0].Ident);
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        }
        [Test]
        public void MissingIdent()
        {
            var indi = "0 @I1@ INDI\n1 ASSO @@\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual(1, rec.Errors.Count); // TODO verify details
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        }
        [Test]
        public void MissingIdent2()
        {
            var indi = "0 @I1@ INDI\n1 ASSO\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual(1, rec.Errors.Count); // TODO verify details
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        }
        [Test]
        public void MissingIdent3()
        {
            var indi = "0 @I1@ INDI\n1 ASSO @ @\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual(1, rec.Errors.Count); // TODO verify details
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        }
    }
}
