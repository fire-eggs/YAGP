using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

namespace UnitTestProject1
{
    [TestClass]
    public class FamEventsTest : GedParseTest
    {
        private KBRGedFam parse(string val)
        {
            return parse<KBRGedFam>(val, "FAM");
        }

        // TODO address structure
        // TODO object
        // TODO CHAN

        /*
1 MARR
2 DATE 31 DEC 1997
2 PLAC The place
2 TYPE Man and Wife
2 ADDR A Church3 CONT Main Street, USA
2 CAUS Love
2 AGNC Catholic Church
2 HUSB
3 AGE 42y
2 WIFE
3 AGE 42y 6m
2 OBJE
3 FORM jpeg
3 TITL Multimedia link about the marriage event
3 FILE ImgFile.JPG
2 SOUR @SOURCE1@
3 PAGE 42
3 DATA
4 DATE 31 DEC 1900
4 TEXT Text from marriage source.
3 QUAY 3
3 NOTE A note about the marriage source.
2 NOTE Marriage event note (a legal, common-law, or customary event of creating a family 
3 CONC unit of a man and a woman as husband and wife).        
         */


        public KBRGedFam TestEventTag(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0}\n2 PLAC Sands, Oldham, Lncshr, Eng", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual(null, rec.FamEvents[0].Date);
            Assert.AreEqual(null, rec.FamEvents[0].Age);
            Assert.AreEqual(null, rec.FamEvents[0].Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", rec.FamEvents[0].Place);
            return rec;
        }

        public KBRGedFam TestEventTag2(string tag)
        {
            string indi3 = string.Format("0 FAM\n1 {0} Y\n2 DATE 1774\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked", tag);
            var rec = parse(indi3);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual(tag, rec.FamEvents[0].Tag);
            Assert.AreEqual("Y", rec.FamEvents[0].Detail); // TODO 'MARR' specific? NOTE also occurs with ENGA in "ged complete"
            Assert.AreEqual("1774", rec.FamEvents[0].Date);
            Assert.AreEqual("suspicious", rec.FamEvents[0].Type);
            Assert.AreEqual(null, rec.FamEvents[0].Place);
            Assert.AreEqual("church", rec.FamEvents[0].Agency);
            Assert.AreEqual("pregnancy", rec.FamEvents[0].Cause);
            Assert.AreEqual("atheist", rec.FamEvents[0].Religion);
            Assert.AreEqual("locked", rec.FamEvents[0].Restriction);
            return rec;
        }

        public KBRGedFam TestEventAge(string tag, string spouse)
        {
            string fam = string.Format("0 FAM\n1 {0}\n2 {1}\n3 AGE 42\n2 PLAC Sands, Oldham, Lncshr, Eng\n2 AGE 99", tag, spouse);
            var rec = parse(fam);
            Assert.AreEqual(1, rec.FamEvents.Count);
            KBRGedEvent famEvent = rec.FamEvents[0];
            Assert.AreEqual(tag, famEvent.Tag);
            Assert.AreEqual(null, famEvent.Date);
            Assert.AreEqual("99", famEvent.Age);
            Assert.AreEqual(null, famEvent.Type);
            Assert.AreEqual("Sands, Oldham, Lncshr, Eng", famEvent.Place);
            if (spouse == "HUSB")
            {
                Assert.AreEqual("42", famEvent.HusbAge);
                Assert.AreEqual(null, famEvent.WifeAge);
            }
            else
            {
                Assert.AreEqual(null, famEvent.HusbAge);
                Assert.AreEqual("42", famEvent.WifeAge);
            }
            return rec;
        }

        [TestMethod]
        public void TestMarr()
        {
            string tag = "MARR";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestAnul()
        {
            string tag = "ANUL";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestMarl()
        {
            string tag = "MARL";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestMars()
        {
            string tag = "MARS";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestMarb()
        {
            string tag = "MARB";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestMarc()
        {
            string tag = "MARC";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestDiv()
        {
            string tag = "DIV";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestDivf()
        {
            string tag = "DIVF";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestEnga()
        {
            string tag = "ENGA";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }
        [TestMethod]
        public void TestEven()
        {
            string tag = "EVEN";
            TestEventTag(tag);
            TestEventTag2(tag);
            TestEventAge(tag, "HUSB");
            TestEventAge(tag, "WIFE");
        }

        [TestMethod]
        public void TestSpouseDetail()
        {
            string indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 HUSB blah blah\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            var rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].HusbDetail);
            Assert.AreEqual(null, rec.FamEvents[0].HusbAge);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 WIFE blah blah\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].WifeDetail);
            Assert.AreEqual(null, rec.FamEvents[0].WifeAge);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 HUSB blah blah\n3 AGE 87\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("blah blah", rec.FamEvents[0].HusbDetail);
            Assert.AreEqual("87", rec.FamEvents[0].HusbAge);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 WIFE bloh bloh\n3 AGE 23\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("bloh bloh", rec.FamEvents[0].WifeDetail);
            Assert.AreEqual("23", rec.FamEvents[0].WifeAge);

            indi = "0 FAM\n1 MARR Y\n2 DATE 1774\n2 TYPE suspicious\n2 AGNC church\n2 CAUS pregnancy\n2 RELI atheist\n2 RESN locked\n2 WIFE bloh bloh\n3 AGE 23";
            rec = parse(indi);
            Assert.AreEqual(1, rec.FamEvents.Count);
            Assert.AreEqual("bloh bloh", rec.FamEvents[0].WifeDetail);
            Assert.AreEqual("23", rec.FamEvents[0].WifeAge);
            
        }
    }
}
