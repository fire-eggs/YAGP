using NUnit.Framework;
using System.Linq;

// Exercise both child and spouse connections

namespace GEDWrap.Tests
{
    [TestFixture]
    class AllConnect : TestUtil
    {
        [Test]
        public void CorrectChilSpouse()
        {
            // Correctly matching FAMC/CHIL + FAMS/HUSB pair
            var txt = "0 @I1@ INDI\n1 FAMC @F1@\n1 FAMS @F2@\n0 @F1@ FAM\n1 CHIL @I1@\n0 @F2@ FAM\n1 HUSB @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.NumberOfTrees);
            Assert.AreEqual(1, f.Indi.Count);
            Assert.AreEqual(2, f.Fams.Count);
            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.AreEqual(1, p.ChildIn.Count);
            Assert.AreEqual("F1", p.ChildIn.First().Id);
            Assert.AreEqual(1, p.SpouseIn.Count);
            Assert.AreEqual("F2", p.SpouseIn.First().Id);
        }
    }
}
