using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using SharpGEDParser.Model;

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
            
            Assert.AreEqual(4, res.Count);
            Assert.IsNotNull(res[2] as IndiRecord);
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
            var fr = ReadItHigher(testString);
            var res = fr.Data;

            Assert.AreEqual(7, res.Count);
            //Assert.AreEqual("HEAD", (res[0] as KBRGedRec).Tag);
            // TODO GedCommon doesn't expose Tag property
            Assert.IsNotNull(res[2] as IndiRecord);
            Assert.IsNotNull(res[3] as IndiRecord);
            Assert.IsNotNull(res[4] as IndiRecord);
            Assert.IsNotNull(res[5] as FamRecord); 
        }

        [TestMethod]
        public void IllegalAdopLevel()
        {
            // ADOP as a sub-record of PLAC is an error
            string indi = "0 INDI\n1 BIRT Y\n2 FAMC @FAM99@\n2 PLAC Sands, Oldham, Lncshr, Eng\n3 ADOP pater";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Events.Count);
            //Assert.AreNotEqual(0, rec.Events[0].Errors.Count); // TODO no place to store error
            Assert.AreNotEqual("pater", rec.Events[0].FamcAdop); // TODO currently broken
        }

        // TODO ADOP as a sub-record of AGNC, RELI, RESN, CAUS, TYPE, DATE, AGE would be error

        [TestMethod]
        public void BogusText()
        {
            // NOTE the third line is missing the leading level #
            string indi = "0 INDI\n1 DSCR attrib_value\nCONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var fr = ReadItHigher(indi);
            Assert.AreEqual(1, fr.Data.Count);
            var rec = fr.Data[0] as IndiRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual(1, rec.Attribs.Count, "Attribute not parsed");
            //Assert.AreEqual(1, rec.Attribs[0].Errors.Count, "Error not recorded in attribute");
        }

        [TestMethod]
        public void BogusText2()
        {
            // NOTE the third line has an invalid leading level # (value below '0')
            // TODO which is changing how the upper level record is accumulated
            string indi = "0 INDI\n1 BIRT attrib_value\n+ CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Events.Count, "Event not parsed");
            // TODO no place to store errors
            //Assert.AreNotEqual(0, rec.Events[0].Errors.Count, "Error not recorded"); // TODO verify details
        }

        [TestMethod]
        public void BogusText3()
        {
            // NOTE the third line has an invalid leading level # (value above '9')
            string indi = "0 INDI\n1 BIRT attrib_value\nA CONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Events.Count, "Event not parsed");
            // TODO no place to store errors
            //Assert.AreNotEqual(0, rec.Events[0].Errors.Count, "Error not recorded");
        }

        [TestMethod]
        public void NotZero()
        {
            // No zero level at first line
            var indi = "INDI\n1 DSCR attrib_value\nCONC a big man\n2 CONT I don't know the\n2 CONT secret handshake\n2 DATE 1774\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 17\n2 TYPE suspicious";
            var fr = ReadItHigher(indi);
            Assert.AreEqual(0, fr.Data.Count);
            Assert.AreNotEqual(0, fr.Errors.Count); // no leading zero TODO verify details
        }

        [TestMethod]
        public void EmptyLines()
        {
            // multiple empty lines (incl. blanks)
            string indi = "0 INDI\n1 DSCR attrib_value\n\n2 DATE 1774\n   "; // TODO trailing record bug
            var fr = ReadItHigher(indi);
            Assert.AreEqual(1, fr.Data.Count);
            Assert.AreEqual(2, fr.Errors.Count); // blank lines as error "records"
        }

        [TestMethod]
        public void LeadingSpaces()
        {
            string indi = "0 INDI\n     1 DSCR attrib_value\n     2 DATE 1774";
            string indi2 = "     0 INDI\n     1 DSCR attrib_value\n     2 DATE 1774";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Attribs.Count);
            rec = parse<IndiRecord>(indi2, "INDI");
            Assert.AreEqual(1, rec.Attribs.Count);
        }

        [TestMethod]
        public void LeadingTabs()
        {
            string indi = "0 INDI\n\t\t1 DSCR attrib_value\n\t\t2 DATE 1774";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Attribs.Count);
        }

        [TestMethod]
        public void Malform()
        {
            // exercise infinite loop found when no '1' level line
            string indi = "0 INDI\n2 DATE 1774";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(0, rec.Attribs.Count);
            // TODO error? - INDI record with no data
        }

        [TestMethod]
        public void LineTooLong()
        {
            // TODO how to exercise for UTF-16 ?
            string indi = "0 INDI\n1 DSCR attrib_value"+
            "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890" +
            "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890" +
            "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890" +
                          "\n2 DATE 1774";
            var fr = ReadItHigher(indi);
            Assert.AreEqual(1, fr.Data.Count);
            Assert.AreEqual(1, fr.Errors.Count); // long lines as error "records"
        }

        [TestMethod]
        public void runOnConc()
        {
            // a later CONC tag was picked up by a tag which took a CONC
            var txt = "0 INDI\n1 NOTE notes \n2 CONC detail\n1 DSCR a big man\n2 CONT I don't";
            var rec = parse<IndiRecord>(txt, "INDI");
            Assert.AreEqual("notes detail", rec.Notes[0].Text); // i.e. do NOT pick up the CONT from DSCR
        }

        [TestMethod]
        public void LeadingSpacesCONT()
        {
            // CONC / CONT tags with leading spaces
            var indi = "0 INDI\n1 NOTE notes\n    2 CONT more detail";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes\nmore detail", rec.Notes[0].Text);
        }

        [TestMethod]
        public void LeadingSpacesCONC()
        {
            // CONC / CONT tags with leading spaces
            var indi = "0 INDI\n1 NOTE notes \n    2 CONC more detail";
            var rec = parse<IndiRecord>(indi, "INDI");
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("notes more detail", rec.Notes[0].Text);
        }

        // TODO where else might leading spaces be a problem? SOUR/DATA/DATE? SOUR/DATA/TEXT? SOUR/TEXT?

        [TestMethod]
        public void EmptyCONT()
        {
            // one GEDCOM had CONT lines with no text
            var txt = "0 INDI\n1 NOTE blah\n2 CONT";
            var rec = parse<IndiRecord>(txt, "INDI");
            Assert.AreEqual(1, rec.Notes.Count);
            Assert.AreEqual("blah\n", rec.Notes[0].Text);
        }
    }
}

// TODO custom tags
// TODO unknown tags

// TODO an integer level value > 9, e.g. "10 PLAC someplace"
// TODO leading spaces and an integer level value > 9, e.g. "   10 CONT continued"
// TODO an ident within a CONC/CONT tag, e.g. "2 @3@ CONT continued"
// TODO invalid level value NOT in sub-record, e.g. "0 INDI\nA bogus "???

// TODO xref parsing
// From Tamura Jones:
/*
Issue an error for non-ASCII characters in a GEDCOM identifier, and abort.
Upon encountering a C0 Control Character (0x00-0x1F) in a GEDCOM identifier, report a fatal error and abort.
A Horizontal Tab is invisible, assist the user by mentioning that control character by name.
Upon encountering a colon in a GEDCOM identifier, report a fatal error and abort.
Upon encountering an exclamation mark in a GEDCOM identifier, report a fatal error and abort.
Upon encountering an underscore in a GEDCOM identifier, report a fatal error and abort.
Upon encountering a number sign in a GEDCOM identifier; simply continue.
Upon encountering a space in a GEDCOM identifier, report a warning and continue.
For any other non-alphanumeric character in a GEDCOM identifier: simply continue.
*/