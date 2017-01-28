using NUnit.Framework;
using SharpGEDParser;
using System.Linq;

// TODO verify HUSB is *not* set in these tests

namespace GEDWrap.Tests
{
    [TestFixture]
    class WifeConnect : TestUtil
    {
        [Test]
        public void CorrectWife()
        {
            // Correctly matching FAMS/WIFE pair
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 WIFE @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.SpouseIn.Count);
            Assert.AreEqual("F1", p.SpouseIn.First().Id);
            var fam = f.AllUnions.First();
            Assert.AreEqual("I1", fam.Wife.Id);

            Assert.IsNullOrEmpty(fam.DadId);
        }

        [Test]
        public void NoFam()
        {
            // INDI.FAMS and no FAM
            var txt = "0 @I1@ INDI\n1 FAMS @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.FAMS_MISSING, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(0, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(0, p.ChildIn.Count);
        }

        [Test]
        public void NoWife()
        {
            // INDI.FAMS and no matching FAM.WIFE
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.FAMS_UNM, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.SpouseIn.Count);
            Assert.AreEqual("F1", p.SpouseIn.First().Id);
            var fam = f.AllUnions.First();
            Assert.AreEqual(1, fam.Spouses.Count);
            Assert.AreEqual("I1", fam.Spouses.First().Id);
        }

        [Test]
        public void NoIndi()
        {
            // FAM.HUSB and no INDI
            var txt = "0 @F1@ FAM\n1 WIFE @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_MISS, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(0, f.NumberOfTrees);
            Assert.AreEqual(0, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(0, f.AllPeople.Count());

            /* TODO what should happen here? keep the INDI id even though the INDI doesn't exist?
            var fam = f.AllUnions.First();
            Assert.AreEqual("I1", fam.Wife.Id);
            Assert.AreEqual(1, fam.Spouses.Count);
            Assert.AreEqual("I1", fam.Spouses.First().Id);
            */
        }

        [Test]
        public void NoFams()
        {
            // FAM.WIFE and no matching INDI.FAMS
            var txt = "0 @I1@ INDI\n0 @F1@ FAM\n1 WIFE @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_UNM, f.Issues.First().IssueId);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.SpouseIn.Count);
            Assert.AreEqual("F1", p.SpouseIn.First().Id);
        }
    }
}
