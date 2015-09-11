using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using System.Diagnostics;

// ReSharper disable ConvertToConstant.Local

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1 : GedParseTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string testString = "0 HEAD\n1 SOUR TJ\n2 NAME Tamura Jones\n2 VERS 1.0\n1 DATE 9 Sep 2013\n1 FILE IdentCONT.GED\n1 NOTE Test File: CONT line with identifier.\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR UTF-8\n1 LANG English\n1 SUBM @U1@\n0 @U1@ SUBM\n1 NAME Name\n0 @I1@ INDI\n1 NAME One /Note/\n2 SURN Note\n2 GIVN One\n1 NOTE First line of a note.\n2 @IDENT@ CONT Second line of a note.\n2 CONT Third line of a note.\n0 TRLR";
            var res = ReadIt(testString);
            
            // examine results in data
            Debug.Assert(res.Count == 3);
            Debug.Assert(res[0].Tag == "HEAD");
            Debug.Assert(res[2].Tag == "INDI");
        }

        [TestMethod]
        public void TestMethod2()
        {
            string testString =
                "0 HEAD\n1 CHAR ASCII\n1 SOUR ID_OF_CREATING_FILE\n1 GEDC\n2 VERS 5.5\n2 FORM Lineage-Linked\n1 SUBM @SUBMITTER@\n" +
                "0 @SUBMITTER@ SUBM\n1 NAME /Submitter/\n1 ADDR Submitters address\n2 CONT address continued here\n0 @FATHER@ INDI\n" +
                "1 NAME /Father/\n1 SEX M\n1 BIRT\n2 PLAC birth place\n2 DATE 1 JAN 1899\n1 DEAT\n2 PLAC death place\n2 DATE 31 DEC 1990\n" +
                "1 FAMS @FAMILY@\n0 @MOTHER@ INDI\n1 NAME /Mother/\n1 SEX F\n1 BIRT\n2 PLAC birth place\n2 DATE 1 JAN 1899\n1 DEAT\n" +
                "2 PLAC death place\n2 DATE 31 DEC 1990\n1 FAMS @FAMILY@\n0 @CHILD@ INDI\n1 NAME /Child/\n1 BIRT\n2 PLAC birth place\n" +
                "2 DATE 31 JUL 1950\n1 DEAT\n2 PLAC death place\n2 DATE 29 FEB 2000\n1 FAMC @FAMILY@\n0 @FAMILY@ FAM\n1 MARR\n" +
                "2 PLAC marriage place\n2 DATE 1 APR 1950\n1 HUSB @FATHER@\n1 WIFE @MOTHER@\n1 CHIL @CHILD@\n0 TRLR";
            var res = ReadIt(testString);

            Debug.Assert(res.Count == 6);
            Debug.Assert(res[0].Tag == "HEAD");
            Debug.Assert(res[2].Tag == "INDI");
            Debug.Assert(res[3].Tag == "INDI");
            Debug.Assert(res[4].Tag == "INDI");
            Debug.Assert(res[5].Tag == "FAM");
        }

        [TestMethod]
        public void IllegalAdopLevel()
        {
            // ADOP as a sub-record of PLAC is an error
            string indi = "0 INDI\n1 BIRT Y\n2 FAMC @FAM99@\n2 PLAC Sands, Oldham, Lncshr, Eng\n3 ADOP pater";
            var rec = parse<KBRGedIndi>(indi, "INDI");
            Assert.AreNotEqual(0, rec.Errors.Count);
        }

        [TestMethod]
        public void BogusText()
        {
            // NOTE the third line is missing the leading level #
            string indi = "0 INDI\n1 DSCR attrib_value\nCONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse<KBRGedIndi>(indi, "INDI");

            Assert.AreEqual(1, rec.Attribs.Count);

            // TODO where/how is the error recorded?
            Assert.AreEqual(1, rec.Attribs[0].Errors.Count);
            Assert.AreEqual(1, rec.Errors.Count);
        }

    }
}
