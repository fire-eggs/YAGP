using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace GEDReadTest.Tests
{
    [TestFixture]
    public class Unicode : TestUtil
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
            var r = BuildAndRead(lines, LB.LF, false, true);
            Assert.AreEqual("None", r.BomEncoding);
            Assert.AreEqual(6, r.LineCount);
            string line = r.Lines[4];
            char[] chars = line.ToCharArray();
            Assert.AreEqual(197, chars[9]);

            // error: no HEAD.CHAR
            Assert.AreEqual(1, r.Errors.Count); // TODO validate contents
        }

        [Test]
        public void checkBom()
        {
            // BOM and no HEAD.CHAR: data is correct due to BOM
            var r = BuildAndRead(lines, LB.LF, true, true);
            Assert.AreEqual("UTF8", r.BomEncoding);
            Assert.AreEqual(6, r.LineCount);
            string line = r.Lines[4];
            char[] chars = line.ToCharArray();
            Assert.AreEqual(345, chars[9]);

            // error: no HEAD.CHAR
            Assert.AreEqual(1, r.Errors.Count); // TODO validate contents
        }
        [Test]
        public void check2()
        {
            // BOM and encoding don't match; encoding is correct->data is correct
            var r = BuildAndRead(lines2, LB.LF, false, true);
            Assert.AreEqual("None", r.BomEncoding);
            Assert.AreEqual(7, r.LineCount);
            string line = r.Lines[5];
            char[] chars = line.ToCharArray();
            Assert.AreEqual(345, chars[9]);

            // error: BOM/HEAD.CHAR mismatch
            Assert.AreEqual(1, r.Errors.Count); // TODO validate contents
        }

        [Test]
        public void checkBom2()
        {
            // BOM and encoding match, data is correct
            var r = BuildAndRead(lines2, LB.LF, true, true);
            Assert.AreEqual("UTF8", r.BomEncoding);
            Assert.AreEqual(7, r.LineCount);
            string line = r.Lines[5];
            char[] chars = line.ToCharArray();
            Assert.AreEqual(345, chars[9]);
            Assert.AreEqual(0, r.Errors.Count);
        }
    }
}
