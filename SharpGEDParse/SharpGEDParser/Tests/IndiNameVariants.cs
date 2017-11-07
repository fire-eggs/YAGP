using NUnit.Framework;
using SharpGEDParser.Model;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable ConvertToConstant.Local

// Variations on name formats, as per standard

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class IndiNameVariants : GedParseTest
    {
        private IndiRecord parse(string val)
        {
            return parse<IndiRecord>(val);
        }

        #region fornames /surname/
        [Test]
        public void fn_sn_1()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME  kludge  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_2()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME  kludge/clan/  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_2a()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME  kludge /clan/  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("kludge", rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_3()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME / clan /  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_4()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME /von Neumann/  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("von Neumann", rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_5()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME john damm /von Neumann/  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("von Neumann", rec.Names[0].Surname);
            Assert.AreEqual("john damm", rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void fn_sn_5a()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME john damm/von Neumann/  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("von Neumann", rec.Names[0].Surname);
            Assert.AreEqual("john damm", rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        #endregion

        #region /surname/fornames

        [Test]
        public void sn_fn_2()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME /clan/ kludge  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.AreEqual("kludge", rec.Names[0].Suffix);
            Assert.AreEqual("clan", rec.Names[0].Surname);
        }

        [Test]
        public void sn_fn_2a()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME /clan/kludge  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.AreEqual("kludge", rec.Names[0].Suffix);
        }

        [Test]
        public void sn_fn_3()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME / clan /  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Suffix);
        }

        [Test]
        public void sn_fn_4()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME /von Neumann/ john damm ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("von Neumann", rec.Names[0].Surname);
            Assert.AreEqual("john damm", rec.Names[0].Suffix);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
        }
        #endregion

        #region name/surname/name
        [Test]
        public void n_sn_n_1()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME john /clan/ damm  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("clan", rec.Names[0].Surname);
            Assert.AreEqual("john", rec.Names[0].Names);
            Assert.AreEqual("damm", rec.Names[0].Suffix);
        }

        [Test]
        public void n_sn_n_2()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME john /von Neumann/dammit  jim  ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("von Neumann", rec.Names[0].Surname);
            Assert.AreEqual("john", rec.Names[0].Names);
            Assert.AreEqual("dammit jim", rec.Names[0].Suffix);
        }

        #endregion

        // BOULDER_CEM_02212009b.GED had a "1 NAME" with nothing else
        [Test]
        public void Empty()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME   ";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual(1, rec.Names[0].Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.EmptyName, rec.Names[0].Errors[0].Error);
            Assert.IsNullOrEmpty(rec.Names[0].Names);
            Assert.IsNullOrEmpty(rec.Names[0].Surname);
        }

        [Test]
        public void Living()
        {
            var indi1 = "0 @I1@ INDI\n1 NAME LIVING";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("LIVING", rec.Names[0].Names);
        }

        [Test]
        public void BadSlash()
        {
            // kartei.ged had this
            var indi1 = "0 @I1@ INDI\n1 NAME John /Doe/Roe/";
            var rec = parse(indi1);
            Assert.AreEqual(1, rec.Names.Count);
            Assert.AreEqual("John", rec.Names[0].Names);
            Assert.AreEqual("Doe/Roe", rec.Names[0].Surname);
            Assert.AreEqual(1, rec.Names[0].Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.SlashInName, rec.Names[0].Errors[0].Error);
        }

    }
}
