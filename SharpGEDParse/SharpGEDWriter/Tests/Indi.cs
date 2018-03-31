using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Indi : GedWriteTest
    {

        [Test]
        public void AFN()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 AFN XYZ-ABC";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }
        [Test]
        public void RFN()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 RFN XYZ-ABC";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }

        [Test]
        public void Asso()
        {
            var indi = "0 @I1@ INDI\n1 SEX M\n1 ASSO @I2@\n2 RELA godfather\n2 SOUR @S1@\n3 QUAY quack";
            var res = ParseAndWrite(indi);
            Assert.AreEqual(indi + "\n", res);
        }

        [Test]
        public void Subm()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 SUBM @I1@";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }
        [Test]
        public void Alia()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 ALIA @I1@";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }
        [Test]
        public void Anci()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 ANCI @I1@";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }
        [Test]
        public void Desi()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 DESI @I1@";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1 + "\n", res);
        }
        [Test]
        public void EmptyDesi()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M\n1 DESI @ @";
            var exp = "0 @I1@ INDI\n1 SEX M\n";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(exp, res);
        }

    }
}
