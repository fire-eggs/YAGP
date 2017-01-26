using NUnit.Framework;
using SharpGEDParser;
using System.IO;
using System.Linq;
using System.Text;

// Exercise child connections INDI.FAMC <> FAM.CHIL

namespace GEDWrap.Tests
{
    [TestFixture]
    class ChilConnect
    {
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

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
        public void CorrectChil()
        {
            // Correctly matching FAMC/CHIL pair
            var txt = "0 @I1@ INDI\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.ChildIn.Count);
            Assert.AreEqual("F1", p.ChildIn.First().Id);
        }

        [Test]
        public void NoChil()
        {
            // INDI.FAMC and no matching FAM.CHIL
            var txt = "0 @I1@ INDI\n1 FAMC @F1@\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.FAMC_UNM, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.ChildIn.Count);
            Assert.AreEqual("F1", p.ChildIn.First().Id);
        }

        [Test]
        public void NoFamc()
        {
            // FAM.CHIL and no matching INDI.FAMC
            var txt = "0 @I1@ INDI\n0 @F1@ FAM\n1 CHIL @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.CHIL_NOTMATCH, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.ChildIn.Count);
            Assert.AreEqual("F1", p.ChildIn.First().Id);
        }

        [Test]
        public void NoFam()
        {
            // INDI.FAMC and no FAM
            var txt = "0 @I1@ INDI\n1 FAMC @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.FAMC_MISSING, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(0, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(0, p.ChildIn.Count);
        }

        [Test]
        public void NoIndi()
        {
            // FAM.CHIL and no INDI
            var txt = "0 @F1@ FAM\n1 CHIL @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.CHIL_MISS, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(0, f.NumberOfTrees);
            Assert.AreEqual(0, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(0, f.AllPeople.Count());
        }

    }
}
