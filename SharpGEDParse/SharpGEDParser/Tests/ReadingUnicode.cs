using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class ReadingUnicode : ReadingUtil
    {
        private static string[] lines =
        {
            "0 HEAD",
            "0 @ind00001@ INDI",
            "1 NAME Jiří /Sobotka/",
            "2 DISPLAY Jiří Sobotka",
            "2 GIVN Jiří",
            "0 TRLR"
        };
        private static string[] lines2 =
        {
            "0 HEAD",
            "1 CHAR UTF-8",
            "0 @ind00001@ INDI",
            "1 NAME Jiří /Sobotka/",
            "2 DISPLAY Jiří Sobotka",
            "2 GIVN Jiří",
            "0 TRLR"
        };

        [Test]
        public void check()
        {
            // With no BOM and no HEAD.CHAR, data is incorrect
            var r = BuildAndRead(lines, GedReader.LB.UNIX, false, true);

            // error: no HEAD.CHAR
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MissCharSet, r.Errors[0].Error);

            //Assert.AreEqual("None", r.BomEncoding);
            Assert.AreEqual(6, r.NumberLines);

            Assert.AreEqual(1,r.AllIndividuals.Count);
            var rec = r.AllIndividuals[0];
            Assert.AreEqual(1, rec.Names.Count);

            string name = rec.Names[0].Names;
            char[] chars = name.ToCharArray();
            Assert.AreEqual(197, chars[2]);
        }

        [Test]
        public void checkBom()
        {
            // BOM and no HEAD.CHAR: data is correct due to BOM
            var r = BuildAndRead(lines, GedReader.LB.UNIX, true, true);

            // error: no HEAD.CHAR
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.MissCharSet, r.Errors[0].Error);

            //Assert.AreEqual("UTF8", r.BomEncoding);
            Assert.AreEqual(6, r.NumberLines);

            Assert.AreEqual(1, r.AllIndividuals.Count);
            var rec = r.AllIndividuals[0];
            Assert.AreEqual(1, rec.Names.Count);

            string name = rec.Names[0].Names;
            char[] chars = name.ToCharArray();
            Assert.AreEqual(345, chars[2]);

            //string line = r.Lines[4];
            //char[] chars = line.ToCharArray();
            //Assert.AreEqual(345, chars[9]);
        }

        [Test]
        public void check2()
        {
            // BOM and encoding don't match; encoding is correct->data is correct
            var r = BuildAndRead(lines2, GedReader.LB.UNIX, false, true);

            // error: BOM/HEAD.CHAR mismatch
            Assert.AreEqual(1, r.Errors.Count);
            Assert.AreEqual(UnkRec.ErrorCode.BOMMismatch, r.Errors[0].Error);

            //Assert.AreEqual("None", r.BomEncoding);

            Assert.AreEqual(7, r.NumberLines);

            Assert.AreEqual(1, r.AllIndividuals.Count);
            var rec = r.AllIndividuals[0];
            Assert.AreEqual(1, rec.Names.Count);

            string name = rec.Names[0].Names;
            char[] chars = name.ToCharArray();
            Assert.AreEqual(345, chars[2]);

            //string line = r.Lines[5];
            //char[] chars = line.ToCharArray();
            //Assert.AreEqual(345, chars[9]);

        }

        [Test]
        public void checkBom2()
        {
            // BOM and encoding match, data is correct
            var r = BuildAndRead(lines2, GedReader.LB.UNIX, true, true);
            Assert.AreEqual(0, r.Errors.Count);
            Assert.AreEqual(7, r.NumberLines);

            Assert.AreEqual(1, r.AllIndividuals.Count);
            var rec = r.AllIndividuals[0];
            Assert.AreEqual(1, rec.Names.Count);

            string name = rec.Names[0].Names;
            char[] chars = name.ToCharArray();
            Assert.AreEqual(345, chars[2]);
        }
    }
}
