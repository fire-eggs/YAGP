using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local

namespace GEDWrap.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class UnionProps : TestUtil
    {
        private Union LoadUnion(string txt)
        {
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllUnions.Count());
            var u = f.AllUnions.First();
            return u;
        }

        [Test]
        public void MDate1()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR";
            var u = LoadUnion(txt);

            Assert.IsNull(u.MarriageDate);
        }

        [Test]
        public void MDate2()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 DATE";
            var u = LoadUnion(txt);

            var d = u.MarriageDate;
            Assert.IsFalse(d.Initialized);
        }

        [Test]
        public void MDate3()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 DATE garbage";
            var u = LoadUnion(txt);

            var d = u.MarriageDate;
            Assert.IsFalse(d.Initialized);
        }

        [Test]
        public void MDate4()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 DATE 1 APR 1990";
            var u = LoadUnion(txt);

            var d = u.MarriageDate;
            Assert.IsTrue(d.Initialized);
        }

        [Test]
        public void Spouse1()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 DATE 1 APR 1990";
            var u = LoadUnion(txt);

            var s = u.Spouse(u.Husband);
            Assert.IsNull(s);
            s = u.Spouse(u.Wife);
            Assert.AreEqual(s, u.Husband);
        }

        [Test]
        public void Spouse2()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @I2@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 DATE 1 APR 1990\n1 WIFE @I2@";
            var u = LoadUnion(txt);

            var s = u.Spouse(u.Husband);
            Assert.AreEqual(s, u.Wife);
            s = u.Spouse(u.Wife);
            Assert.AreEqual(s, u.Husband);
        }

        [Test]
        public void MPlace1()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n2 PLAC las vegas";
            var u = LoadUnion(txt);

            Assert.AreEqual("las vegas", u.MarriagePlace);
        }

        [Test]
        public void MPlace2()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR";
            var u = LoadUnion(txt);

            Assert.IsNullOrEmpty(u.MarriagePlace);
        }

        [Test]
        public void MEvent1()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR";
            var u = LoadUnion(txt);

            var e = u.GetEvent("RESI");
            Assert.IsNull(e);
        }

        [Test]
        public void MEvent2()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n1 RESI";
            var u = LoadUnion(txt);

            var e = u.GetEvent("RESI");
            Assert.IsNotNull(e);
            Assert.IsNullOrEmpty(e.Descriptor);
        }

        [Test]
        public void MEvent3()
        {
            var txt = "0 @I1@ INDI\n1 FAMS @F1@\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n1 RESI slum";
            var u = LoadUnion(txt);

            var e = u.GetEvent("RESI");
            Assert.IsNotNull(e);
            Assert.AreEqual("slum", e.Descriptor);
        }

    }
}
