using NUnit.Framework;

namespace SharpGEDWriter.Tests
{
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
    }
}
