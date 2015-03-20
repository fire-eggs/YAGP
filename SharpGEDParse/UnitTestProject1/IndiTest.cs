using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// TODO my asserts are backward - 'expected' first

namespace UnitTestProject1
{
    [TestClass]
    public class IndiTest
    {
        // TODO refactor to common base
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
        }

        // TODO refactor to common base
        public static List<KBRGedRec> ReadIt(string testString)
        {
            FileRead fr = new FileRead();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                fr.ReadLines(stream);
            }
            return fr.Data;
        }

        [TestMethod]
        public void TestMethod1()
        {
            // NOTE extra trailing '0' record: testing kludge
            var simpleInd = "0 @I1@ INDI\n1 NAME One /Note/\n2 SURN Note\n2 GIVN One\n1 NOTE First line of a note.\n2 @IDENT@ CONT Second line of a note.\n2 CONT Third line of a note.\n0 TESTKLUDGE";
            var res = ReadIt(simpleInd);
            Assert.AreEqual(res.Count, 1);
            Assert.AreEqual(res[0].Tag, "INDI");

            var rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'U');
            Assert.AreEqual(rec.Names.Count, 1);
        }

        [TestMethod]
        public void TestSexU()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n0TestKludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX U\n0TestKludge";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Gibber\n0TestKludge";

            var res = ReadIt(indiU1);
            var rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'U');

            res = ReadIt(indiU2);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'U');

            res = ReadIt(indiU3);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual('U', rec.Sex); // TODO known issue: not validating
        }

        [TestMethod]
        public void TestSexM()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX M\n0TestKludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Masculine\n0TestKludge";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Male\n0TestKludge";

            var res = ReadIt(indiU1);
            var rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'M');

            res = ReadIt(indiU2);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'M');

            res = ReadIt(indiU3);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'M');
        }

        [TestMethod]
        public void TestSexF()
        {
            var indiU1 = "0 INDI\n1 NAME kludge\n1 SEX F\n0TestKludge";
            var indiU2 = "0 INDI\n1 NAME kludge\n1 SEX Feminine\n0TestKludge";
            var indiU3 = "0 INDI\n1 NAME kludge\n1 SEX Female\n0TestKludge";

            var res = ReadIt(indiU1);
            var rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'F');

            res = ReadIt(indiU2);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'F');

            res = ReadIt(indiU3);
            rec = res[0] as KBRGedIndi;
            Assert.AreNotEqual(rec, null);
            Assert.AreEqual(rec.Sex, 'F');
        }
    }
}
