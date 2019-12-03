using System.Diagnostics.CodeAnalysis;
using System.Text;
using NUnit.Framework;

// Example records pulled from real-world GED files

namespace SharpGEDWriter.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class RealWorld : GedWriteTest
    {
        // INDI expected order is:
        // ID, NAME, SEX, EVENTS, FAMS, FAMC, NOTE, SOUR, OBJE, CHAN

        private string[] record1 = // From Blades.ged
        {
        "0 @I4@ INDI", // 0
        "1 NAME Frances /Unknown/",
        "2 GIVN Frances",
        "2 SURN Unknown",
        "1 SEX F",
        "1 _UID 5D1461CFCB53DF439EA8F56D8146A6D4CA50", // 5
        "1 CHAN",
        "2 DATE 13 APR 2008",
        "1 SOUR @S2@",
        "2 PAGE n/a",
        "2 _TMPLT", // 10
        "3 FIELD",
        "4 NAME Page",
        "4 VALUE n/a",
        "1 BIRT",
        "2 DATE ABT 1660", // 15
        "2 _SDATE 1 JUL 1660",
        "2 PLAC prob Talbot County, Maryland",
        "1 DEAT",
        "2 DATE 1698",
        "2 _SDATE 1 JUL 1698", // 20
        "1 FAMS @F2@",
        };

        [Test,Ignore("Custom tag writing NYI")]
        public void BladesReal()
        {
            int[] exp = {0,1,2,3,4,14,15,16,17,18,19,20,21,5,8,9,10,11,12,13,6,7};
            Assert.AreEqual(exp.Length,record1.Length);
            var fr = ReadItHigher(MakeInput(record1));
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            var ideal = MakeInput(record1, exp);
            Assert.AreEqual(ideal,res); // TODO failing on custom tags
        }

        private string[] record2 = // From Blades.ged
        {
        "0 @I4@ INDI", // 0
        "1 NAME Frances /Unknown/",
        "1 SEX F",
        "1 _UID 5D1461CFCB53DF439EA8F56D8146A6D4CA50",
        "1 CHAN",
        "2 DATE 13 APR 2008", // 5
        "1 SOUR @S2@",
        "2 PAGE n/a",
        "1 BIRT",
        "2 DATE ABT 1660", 
        "2 PLAC prob Talbot County, Maryland", // 10
        "1 DEAT",
        "2 DATE 1698", 
        "1 FAMS @F2@", // 13
        };

        // INDI expected order is:
        // ID, NAME, SEX, EVENTS, FAMS, FAMC, _UID, NOTE, SOUR, OBJE, CHAN

        [Test]
        public void BladesSimple()
        {
            // No custom tags
            int[] exp = {0, 1, -1, -2, 2, 8, 9, 10, 11, 12, 13, 3, 6, 7, 4, 5};
            string[] extra =
            {
                "",
                "2 GIVN Frances",
                "2 SURN Unknown"
            };

            //Assert.AreEqual(exp.Length, record2.Length);
            var fr = ReadItHigher(MakeInput(record2));
            Assert.AreEqual(0, fr.AllErrors.Count);
            var res = Write(fr);
            var ideal = MakeInput(record2, exp, extra);
            Assert.AreEqual(ideal, res);
        }

        public string MakeInput(string[] recs, int[] order, string [] extra)
        {
            StringBuilder inp = new StringBuilder();
            foreach (var dex in order)
            {
                if (dex < 0)
                    inp.Append(extra[-dex]);
                else
                    inp.Append(recs[dex]);
                inp.Append("\n");
            }
            return inp.ToString();
        }

        // From Gene.Genie tests - nominally for Spouse Sealing
        private readonly string[] record3 =
        {
            "0 HEAD", // 0
            "1 SOUR FTM",
            "2 VERS Family Tree Maker (22.0.0.1404)",
            "2 NAME Family Tree Maker for Windows",
            "2 CORP Ancestry.com",
            "3 ADDR 360 W 4800 N",
            "4 CONT Provo, UT 84604",
            "3 PHON (801) 705-7000",
            "1 DEST GED55",
            "1 DATE 21 OCT 2016",
            "1 CHAR UTF-8", // 10
            @"1 FILE C:\Users\r\Desktop\Sealing test_2016-10-21.ged",
            "1 SUBM @SUBM@",
            "1 GEDC",
            "2 VERS 5.5",
            "2 FORM LINEAGE-LINKED",
            "0 @SUBM@ SUBM",
            "0 @I1@ INDI",
            "1 NAME /Child/",
            "1 SEX U",
            "1 FAMC @F1@", // 20
            "0 @I2@ INDI",
            "1 NAME /Father/",
            "1 SEX M",
            "1 TITL Father",
            "1 FAMS @F1@",
            "0 @I3@ INDI",
            "1 NAME /Mother/",
            "1 SEX F",
            "1 FAMS @F1@",
            "0 @F1@ FAM", // 30
            "1 HUSB @I2@",
            "1 WIFE @I3@",
            "1 CHIL @I1@",
            "1 SLGS Sealing description",
            "2 DATE 10 JAN 2011",
            "2 PLAC Timbuktu",
            "2 TEMP Temple Code",
            "2 STAT DNS/CAN",
            "3 DATE 11 FEB 2012",
            "2 SOUR @SRC1@", // 40
            "2 NOTE An example note",
            "",
            "0 @SRC1@ SOUR",
            "1 REPO @R150224420@",
            "1 RFN Source 1 reference",
            "1 TITL Source 1",
            "1 AUTH Source 1 author",
            "1 PUBL Source 1 publisher",
            "1 CALN 11111111111",
            "1 NOTE Note for source", // 50
            "",
            "0 @R150224420@ REPO",
            "1 RFN Reference",
            "1 NAME Repository 1",
            "1 PHON 12112121212",
            "",
            "1 ADDR Bedfordshire",
            "1 CALN 1",
            "1 NOTE Note",
            "", // 60
            "0 TRLR",
        };

        [Test,Ignore("Echo-ing unknown tags is NYI")]
        public void Genie()
        {
            var inp = MakeInput(record3);
            var fr = ReadItHigher(inp);
            // The blank lines are an error [only one reported]
            Assert.AreEqual(1, fr.AllErrors.Count);
            Assert.AreEqual(SharpGEDParser.Model.UnkRec.ErrorCode.EmptyLine, fr.AllErrors[0].Error);
            var res = Write(fr, noHead: false);

            Assert.AreEqual(inp,res);

            // TODO output header is different
            // TODO SOUR.CALN is not valid ... SOUR.REPO.CALN is valid [but both PAF and FTM do SOUR.CALN]
        }
    }
}
