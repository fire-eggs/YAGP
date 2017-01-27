using NUnit.Framework;
using SharpGEDParser;
using System.IO;
using System.Linq;
using System.Text;

namespace GEDWrap.Tests
{
    [TestFixture]
    class MiscTests
    {
        // TODO move to lower class
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        // TODO move to lower class
        private Forest LoadGEDFromStream(string testString)
        {
            Forest f = new Forest();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                f.LoadFromStream(stream);
            }
            return f;
        }

        [Test]
        public void DuplIndi()
        {
            // Duplicated INDI
            var txt = "0 @I1@ INDI\n0 @I1@ INDI";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.DUPL_INDI, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);
        }
        [Test]
        public void DuplFam()
        {
            // Duplicated INDI
            var txt = "0 @I1@ FAM\n0 @I1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.DUPL_FAM, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);
        }

        [Test]
        public void BadIndiLink()
        {
            // Error in INDI link out
            var txt = "0 @I1@ INDI\n1 FAMC\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.MISS_XREFID, f.Issues.First().IssueId);
            Assert.AreEqual(1, f.Errors.Count); // TODO verify details
        }

        [Test]
        public void BadFamLink()
        {
            // Error in FAM link out
            var txt = "0 @I1@ INDI\n0 @F1@ FAM\n1 CHIL";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(1, f.Errors.Count); // TODO verify error details
        }

        [Test]
        public void MultiTree()
        {
            var txt = "0 @I1@ INDI\n0 @I2@ INDI";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(0, f.ErrorsCount);
                Assert.AreEqual(0, f.Errors.Count);
                Assert.AreEqual(2, f.NumberOfTrees);
            }
        }

        [Test]
        public void MultiTree2()
        {
            // A specific code coverage case: treenum != -1
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @I3@ INDI\n0 @F1@ FAM\n1 CHIL @I1@\n1 CHIL @I3@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(2, f.NumberOfTrees);
            }
        }

        [Test]
        public void AmbDad()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @I2@ INDI\n0 @I3@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(1, f.ErrorsCount);
                Assert.AreEqual(Issue.IssueCode.AMB_CONN, f.Issues.First().IssueId);
            }
        }

        [Test]
        public void MissFamId()
        {
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @ @ FAM\n1 CHIL @I1@";
            using (Forest f = LoadGEDFromStream(txt))
            {
                Assert.AreEqual(1, f.ErrorsCount);
                Assert.AreEqual(Issue.IssueCode.MISS_FAMID, f.Issues.First().IssueId);
                Assert.AreEqual(1, f.Errors.Count); // TODO verify error details
            }
        }

    }
}
