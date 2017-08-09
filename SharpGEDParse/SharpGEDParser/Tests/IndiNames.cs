using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// TODO name sub-records

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class IndiNames : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        [Test]
        public void TestBasicName()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME  kludge  ";
            var rec = parse(indi1);
            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual(null, rec.Names[0].Surname);
            Assert.AreEqual(0, rec.Names[0].Errors.Count);
        }

        [Test]
        public void TestBasicSurname()
        {
            var indi1 = "0 INDI\n1 NAME kludge /clan/";
            var indi2 = "0 INDI\n1 NAME /clan2/";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("", rec.Names[0].Names);
            Assert.AreEqual("clan2", rec.Names[0].Surname);
        }

        [Test]
        public void TestBasicSurname2()
        {
            // real person from a GED, extra spaces
            var indi1 = "0 INDI\n1    NAME     Marjorie    Lee     /Smith/   \n2 GIVN Marjorie Lee\n2 SURN Smith";

            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("Marjorie Lee", rec.Names[0].Names, "Failure to remove extra spaces"); // TODO failure to strip extra spaces
            Assert.AreEqual("Smith", rec.Names[0].Surname);
        }

        [Test]
        public void TestMultiName()
        {
            var indi = "0 INDI\n1 NAME John /Smith/\n1 NAME Eric /Jones/";
            var indi2 = "0 INDI\n1 NAME John /Smith/\n1 SEX M\n2 NOTE blah blah\n1 NAME Eric /Jones/";
            var rec = parse(indi);
            Assert.AreEqual(2, rec.Names.Count);
            Assert.AreEqual("Smith", rec.Names[0].Surname);
            Assert.AreEqual("Jones", rec.Names[1].Surname);
            rec = parse(indi2);
            Assert.AreEqual(2, rec.Names.Count);
            Assert.AreEqual("Smith", rec.Names[0].Surname);
            Assert.AreEqual("Jones", rec.Names[1].Surname);
        }

        [Test]
        public void TestSuffix()
        {
            var indi = "0 INDI\n1 NAME Given Name /Smith/ jr ";
            var indi2 = "0 INDI\n1 NAME Given Name /Smith/esq";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("jr", rec.Names[0].Suffix);
            rec = parse(indi2);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("esq", rec.Names[0].Suffix);
        }

        [Test]
        public void UntermSurname()
        {
            var indi = "0 @I1@ INDI\n1 NAME Given Name /Smith";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("Smith", rec.Names[0].Surname);
            Assert.AreEqual(1, rec.Names[0].Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.UntermSurname, rec.Names[0].Errors[0].Error);
        }
    }
}
