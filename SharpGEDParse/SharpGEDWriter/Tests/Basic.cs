using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class Basic : GedWriteTest
    {
        [Test]
        public void TryIt()
        {
            var indi1 = "0 @I1@ INDI\n1 SEX M";
            var res = ParseAndWrite(indi1);
            Assert.AreEqual(indi1+"\n", res);
        }

        private string [] records =
        {
            "0 @I1@ INDI",
            "1 NAME Fred",
            "1 SEX M",
            "1 FAMS @F1@",
            "1 FAMC @F2@",
            "1 _UID 9876543210", // Note: not valid value TODO parse may fail
            "1 NOTE @N1@",
            "1 CHAN",
            "2 DATE 2 DEC 2017",
            "0 @F1@ FAM",
            "1 HUSB @I1@",
            "1 WIFE @I2@",
            "0 @F2@ FAM",
            "1 CHIL @I1@",
            "0 @N1@ NOTE"
        };

        [Test]
        public void MultRec()
        {
            string str = MakeInput(records);
            var fr = ReadItHigher(str);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            Assert.AreEqual(str, res);
        }


        [Test]
        public void HeadNote()
        {
            var txt = "0 HEAD\n1 NOTE this is a header note\n0 @I1@ INDI";
            var fr = ReadItHigher(txt);
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr, noHead:false);
            // TODO will fail when version change
            var exp = "﻿0 HEAD\n1 GEDC\n2 VERS 5.5.1\n"+
                      "2 FORM LINEAGE-LINKED\n1 CHAR UTF-8\n"+
                      "1 SOUR SharpGEDWriter\n2 VERS V0.2-Alpha\n"+
                      "1 NOTE this is a header note\n"+
                      "1 SUBM @S0@\n0 @S0@ SUBM\n0 @I1@ INDI\n0 TRLR\n";
            Assert.AreEqual(res, exp);
        }

        [Test]
        public void Repo()
        {
            var inp = "0 @R1@ REPO\n1 NAME Fort Knox\n1 REFN gold standard\n1 RIN auto_id";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void Fam()
        {
            var inp = "0 @F1@ FAM\n1 HUSB @I1@\n1 WIFE @I2@\n1 CHIL @I3@\n1 NCHI 1\n1 SUBM @S1@";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }

        [Test]
        public void Addr()
        {
            // exercise address struct for coverage
            var inp = "0 @R1@ REPO\n1 ADDR Olivier et Sophie LOREAU\n2 ADR1 18 RUE JULES VERNE\n2 CITY VILLEJUIF\n" +
                      "2 STAE VAL DE MARNE\n2 POST 94800\n2 CTRY FRANCE\n1 EMAIL olivier.loreau@hds.com\n1 EMAIL foo@bar.com\n" +
                      "1 WWW http://oloreau.free.fr\n1 WWW www.google.com";
            var res = ParseAndWrite(inp);
            Assert.AreEqual(inp + "\n", res);
        }
    }
}
