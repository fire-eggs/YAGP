using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// TODO missing relation text
// TODO missing relation tag
// TODO notes

// TODO why 'unterm' ident instead of 'missing' ident?

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
            Assert.AreEqual(0, rec.Assocs[0].Cits.Count);
        }
        [Test]
        public void MissingIdent()
        {
            var indi = "0 @I1@ INDI\n1 ASSO @@\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.UntermIdent, rec.Errors[0].Error);
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        }
        [Test]
        public void MissingIdent2()
        {
            var indi = "0 @I1@ INDI\n1 ASSO\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.UntermIdent, rec.Errors[0].Error);
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

        [Test]
        public void AssoSource()
        {
            var indi = "0 @I1@ INDI\n1 ASSO @I2@\n2 SOUR @S1@\n3 QUAY quack\n2 RELA godfather";
            var rec = parse(indi);
            Assert.AreEqual("I1", rec.Ident);
            Assert.AreEqual(1, rec.Assocs.Count);
            Assert.AreEqual("I2", rec.Assocs[0].Ident);
            Assert.AreEqual("godfather", rec.Assocs[0].Relation);
            Assert.AreEqual(1, rec.Assocs[0].Cits.Count);
            var cit = rec.Assocs[0].Cits[0];
            Assert.AreEqual("quack", cit.Quay);
            Assert.AreEqual("S1", cit.Xref);
        }

        // TODO parsing for 'extra' text after ident but not saving it - in any code path
        //[Test]
        //public void AssoExtra()
        //{
        //    var indi = "0 @I1@ INDI\n1 ASSO @I2@1@ extra\n2 RELA godfather";
        //    var rec = parse(indi);
        //    Assert.AreEqual("I1", rec.Ident);
        //    Assert.AreEqual(1, rec.Assocs.Count);
        //    Assert.AreEqual("I2", rec.Assocs[0].Ident);
        //    Assert.AreEqual("godfather", rec.Assocs[0].Relation);
        //    Assert.AreEqual(0, rec.Assocs[0].Cits.Count);
        //    Assert.AreEqual("extra", rec.Assocs[0].???);
        //}
    }
}
