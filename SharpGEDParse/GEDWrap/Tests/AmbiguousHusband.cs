using NUnit.Framework;
using SharpGEDParser;
using System.Linq;

// TODO replicate for WIFE
// TODO replicate for mixed HUSB/WIFE

namespace GEDWrap.Tests
{
    // Exercise scenarios where husband connection is ambiguous.
    // Ideally, the INDI.FAMS/FAM.HUSB connection is 1-to-1.
    // Here I attempt to exercise the 0-to-1, 0-to-many, 1-to-many,
    // 1-to-0, many-to-many, many-to-1, many-to-0 cases.
    [TestFixture]
    class AmbiguousHusband : TestUtil
    {
        [Test]
        public void ManyToOne()
        {
            // two FAMS, one HUSB : is second FAMS HUSB or WIFE?
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I2@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(3, f.ErrorsCount);

            var allIss = f.Issues.ToArray(); // TODO order sensitive
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[0].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[1].IssueId);
            Assert.AreEqual(Issue.IssueCode.FAMS_UNM, allIss[2].IssueId);
        }

        [Test]
        public void ManyToMany()
        {
            // two FAMS, two HUSB : ambiguous HUSB
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I2@\n1 HUSB @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(2, f.ErrorsCount);

            var allIss = f.Issues.ToArray(); // TODO order sensitive
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[0].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[1].IssueId);
        }

        [Test]
        public void ManyToZero()
        {
            // two FAMS, no HUSB : are FAMS HUSB or WIFE? 
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(5, f.ErrorsCount);

            var allIss = f.Issues.ToArray(); // TODO order sensitive
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[0].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[1].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[2].IssueId);
            Assert.AreEqual(Issue.IssueCode.FAMS_UNM, allIss[3].IssueId);
            Assert.AreEqual(Issue.IssueCode.FAMS_UNM, allIss[4].IssueId);
        }

        [Test]
        public void OneToMany()
        {
            // One FAMS, two HUSB : ambiguous HUSB
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I2@\n1 HUSB @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(3, f.ErrorsCount);

            var allIss = f.Issues.ToArray(); // TODO order sensitive
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[0].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[1].IssueId);
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_UNM, allIss[2].IssueId);
        }

        [Test]
        public void OneToZero()
        {
            // One FAMS, no HUSB : is FAMS HUSB or WIFE?
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(3, f.ErrorsCount);

            var allIss = f.Issues.ToArray();
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN, allIss[0].IssueId); // TODO duplicate error?
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[1].IssueId);
            Assert.AreEqual(Issue.IssueCode.FAMS_UNM, allIss[2].IssueId);

            Assert.AreEqual(2, f.NumberOfTrees);
            var indis = f.Indi;
            Assert.AreEqual(2, indis.Count);
            Assert.AreEqual("I1", indis[0].Ident); // TODO order sensitive?
            Assert.AreEqual("I2", indis[1].Ident);
            Assert.AreEqual(1, f.Fams.Count);
            Assert.AreEqual(2, f.AllPeople.Count());
            var peeps = f.AllPeople.ToArray();
            Assert.AreEqual(1, peeps[1].SpouseIn.Count);
            Assert.AreEqual("F1", peeps[1].SpouseIn.First().Id);
            var fam = f.AllUnions.First();
            Assert.AreEqual(1, fam.Spouses.Count);
            Assert.AreEqual("I2", fam.Spouses.First().Id);
        }

        [Test]
        public void ZeroToMany()
        {
            // No FAMS, two HUSB : ambiguous HUSB
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @F1@ FAM\n1 HUSB @I2@\n1 HUSB @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(3, f.ErrorsCount);

            var allIss = f.Issues.ToArray();
            Assert.AreEqual(Issue.IssueCode.AMB_CONN, allIss[0].IssueId); // TODO order-sensitive
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_UNM, allIss[1].IssueId);
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_UNM, allIss[2].IssueId);
        }

        [Test]
        public void ZeroToOne()
        {
            // No FAMS, one HUSB : self-correcting
            var txt = "0 @I1@ INDI\n0 @I2@ INDI\n0 @F1@ FAM\n1 HUSB @I2@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(1, f.ErrorsCount);

            var allIss = f.Issues.ToArray();
            Assert.AreEqual(Issue.IssueCode.SPOUSE_CONN_UNM, allIss[0].IssueId);

        }
    }
}
