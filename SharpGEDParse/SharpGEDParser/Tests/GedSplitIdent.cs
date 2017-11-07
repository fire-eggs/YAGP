using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local

namespace SharpGEDParser.Tests
{
    // Exercise the GedSplitter when parsing for level/ident/tag/remain

    [ExcludeFromCodeCoverage]
    [TestFixture]
    class GedSplitIdent
    {
        private void SplitIt(string txt, char expLevel, string expIdent, string expTag, string rem= null)
        {
            var txt2 = txt.ToCharArray();
            GEDSplitter gs = new GEDSplitter();

            char[] tag;
            char level;
            string ident;
            char[] remain;
            gs.LevelIdentTagRemain(txt2, out level, out tag, out ident, out remain);

            Assert.AreEqual(expLevel, level);
            Assert.AreEqual(expTag, new string(tag));
            Assert.AreEqual(expIdent, ident);
            if (rem != null)
                Assert.AreEqual(rem, new string(remain));
        }

        [Test]
        public void Partial1()
        {
            var txt = "0 @I1@";
            SplitIt(txt, '0', "I1", "");
        }

        [Test]
        public void Partial2()
        {
            var txt = "0";
            SplitIt(txt, '0', "", "");
        }

        [Test]
        public void Partial3()
        {
            var txt = "0 @I1@ ";
            SplitIt(txt, '0', "I1", "");
        }

        [Test]
        public void Partial4()
        {
            var txt = " 0 @I1@ ";
            SplitIt(txt, '0', "I1", "");
        }

        [Test]
        public void ExtraSpaces()
        {
            var txt = "0 @I1@  INDI";
            SplitIt(txt, '0', "I1", "INDI");
        }

        [Test]
        public void ExtraSpaceInIdent()
        {
            var txt = "0      @ I1@         INDI";
            SplitIt(txt, '0', " I1", "INDI", "");
        }

        [Test]
        public void ExtraSpaceInIdent2()
        {
            var txt = "0    @I1 @   FAM";
            SplitIt(txt, '0', "I1 ", "FAM");
        }

        [Test]
        public void ExtraSpaceInIdent3()
        {
            var txt = "0    @I1 @   FAM ";
            SplitIt(txt, '0', "I1 ", "FAM", "");
        }

        [Test]
        public void UntermIdent()
        {
            // TODO should this be split properly?
            var txt = "0    @I1 FAM";
            SplitIt(txt, '0', "", "I1 FAM");
        }

        [Test]
        public void EmptyId()
        {
            var txt = "0 @ @ FAM";
            SplitIt(txt, '0', " ", "FAM");
        }

        [Test]
        public void Remain1()
        {
            var txt = "0 @I1@  INDI  junk junk";
            SplitIt(txt, '0', "I1", "INDI", " junk junk");
        }

        //[Test]
        //public void TabSplit()
        //{
        //    // TODO should this be handled?
        //    var txt = "0\t@I1@\tINDI";
        //    SplitIt(txt, '0', "I1", "INDI");
        //}

        [Test]
        public void MissIdent()
        {
            var txt = "0 INDI";
            SplitIt(txt, '0', "", "INDI");
        }
    }
}
