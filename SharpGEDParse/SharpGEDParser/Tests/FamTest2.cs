using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using SharpGEDParser.Model;

// Exercise _FREL/_MREL (e.g. FAM.CHIL._FREL)

namespace SharpGEDParser.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class FamTest2 : GedParseTest
    {
        private FamRecord parse(string val)
        {
            return parse<FamRecord>(val);
        }

        [Test]
        public void Basic()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@";
            var rec = parse(fam);
            Assert.AreEqual("FAM", rec.Tag);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("I1", rec.Childs[0].Xref);
            Assert.IsNull(rec.Childs[0].FatherRelation);
            Assert.IsNull(rec.Childs[0].MotherRelation);
        }

        [Test]
        public void Basic2()
        {
            // verify CHIL parsing didn't break follow-on records
            string fam = "0 @F1@ FAM\n1 CHIL @p3@\n1 CHIL @p4@\n1 HUSB @p1@\n1 WIFE @p2@\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual(1, rec.Moms.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            Assert.AreEqual("p2", rec.Moms[0]);
            Assert.AreEqual(2, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0].Xref);
            Assert.AreEqual("p4", rec.Childs[1].Xref);
            Assert.AreEqual("2", rec.RIN);
        }

        // TODO no mother/father relation specified

        [Test]
        public void BasicRel()
        {
            // Added _MREL, _FREL
            string fam = "0 @F1@ FAM\n1 CHIL @p3@\n2 _MREL Natural\n2 _FREL Natural\n1 CHIL @p4@\n2 _MREL Adopted\n2 _FREL Step\n1 HUSB @p1@\n1 WIFE @p2@\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual(1, rec.Moms.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            Assert.AreEqual("p2", rec.Moms[0]);
            Assert.AreEqual(2, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0].Xref);
            Assert.AreEqual("Natural", rec.Childs[0].FatherRelation);
            Assert.AreEqual("Natural", rec.Childs[0].MotherRelation);
            Assert.AreEqual("p4", rec.Childs[1].Xref);
            Assert.AreEqual("Step", rec.Childs[1].FatherRelation);
            Assert.AreEqual("Adopted", rec.Childs[1].MotherRelation);
            Assert.AreEqual("2", rec.RIN);
        }

        [Test]
        public void DefaultRelation()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 _FREL Natural\n2 _MREL Natural";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("I1", rec.Childs[0].Xref);
            Assert.AreEqual("Natural", rec.Childs[0].FatherRelation);
            Assert.AreEqual("Natural", rec.Childs[0].MotherRelation);
        }

        [Test]
        public void DefaultRelation2()
        {
            // verify CHIL parsing didn't break follow-on records
            string fam = "0 @F1@ FAM\n1 CHIL @p3@\n2 _FREL Natural\n2 _MREL Natural\n1 CHIL @p4@\n1 HUSB @p1@\n1 WIFE @p2@\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Dads.Count);
            Assert.AreEqual(1, rec.Moms.Count);
            Assert.AreEqual("p1", rec.Dads[0]);
            Assert.AreEqual("p2", rec.Moms[0]);
            Assert.AreEqual(2, rec.Childs.Count);
            Assert.AreEqual("p3", rec.Childs[0].Xref);
            Assert.AreEqual("Natural", rec.Childs[0].FatherRelation);
            Assert.AreEqual("Natural", rec.Childs[0].MotherRelation);
            Assert.AreEqual("p4", rec.Childs[1].Xref);
            Assert.AreEqual("2", rec.RIN);
        }

        [Test]
        public void NotDefault1()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 _FREL Adopted\n2 _MREL Natural";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("I1", rec.Childs[0].Xref);
            Assert.AreEqual("Adopted", rec.Childs[0].FatherRelation);
            Assert.AreEqual("Natural", rec.Childs[0].MotherRelation);
        }

        [Test]
        public void NotDefault2()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 _MREL Adopted\n2 _FREL Natural";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("I1", rec.Childs[0].Xref);
            Assert.AreEqual("Adopted", rec.Childs[0].MotherRelation);
            Assert.AreEqual("Natural", rec.Childs[0].FatherRelation);
        }

        [Test]
        public void NotDefault3()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 _MREL Adopted\n2 _FREL Step";
            var rec = parse(fam);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual("I1", rec.Childs[0].Xref);
            Assert.AreEqual("Adopted", rec.Childs[0].MotherRelation);
            Assert.AreEqual("Step", rec.Childs[0].FatherRelation);
        }

        [Test]
        public void NoId()
        {
            string fam = "0 @F1@ FAM\n1 CHIL     \n2 _MREL Adopted\n2 _FREL Step\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("2", rec.RIN);
            Assert.AreEqual(0, rec.Childs.Count);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(2, rec.Errors[0].Beg);
            Assert.AreEqual(4, rec.Errors[0].End);
        }

        [Test]
        public void DupId()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 _MREL Adopted\n2 _FREL Step\n1 CHIL @I1@\n2 _MREL Adopted\n2 _FREL Step\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("2", rec.RIN);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual(1, rec.Errors.Count);
            Assert.AreEqual(5, rec.Errors[0].Beg);
            Assert.AreEqual(7, rec.Errors[0].End);
        }

        [Test]
        public void UnkSubRec()
        {
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 BLAH Adopted\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("2", rec.RIN);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(3, rec.Unknowns[0].Beg);
            Assert.AreEqual(3, rec.Unknowns[0].End);
        }

        [Test,Ignore("sub-records not rolled up into unknowns")]
        public void UnkSubRec2()
        {
            // TODO ideally the entire sub-record should be one 'unknown' - currently 2 unknowns
            // TODO a legit sub-tag under an unknown tag would be incorrectly handled (e.g. 1 CHIL\2 BLAH\3 _MREL)
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 BLAH Adopted\n3 BLAHS more\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("2", rec.RIN);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(2, rec.Unknowns[0].Beg);
            Assert.AreEqual(3, rec.Unknowns[0].End);
        }

        [Test]
        public void Combined()
        {
            // _MREL and other
            string fam = "0 @F1@ FAM\n1 CHIL @I1@\n2 BLAH Adopted\n2 _MREL Step\n1 RIN 2";
            var rec = parse(fam);
            Assert.AreEqual("2", rec.RIN);
            Assert.AreEqual(1, rec.Childs.Count);
            Assert.AreEqual(1, rec.Unknowns.Count);
            Assert.AreEqual(3, rec.Unknowns[0].Beg);
            Assert.AreEqual(3, rec.Unknowns[0].End);
            Assert.AreEqual("Step", rec.Childs[0].MotherRelation);
        }
    }
}
