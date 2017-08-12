using NUnit.Framework;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// ReSharper disable ConvertToConstant.Local

namespace GEDWrap.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    class PersonProps : TestUtil
    {
        [Test]
        public void Date()
        {
            var txt = "0 @I1@ INDI\n1 EMIG\n2 DATE 1 APR 1990";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            var d = p.GetDate("EMIG");
            Assert.AreEqual("1-Apr-1990", d);
            var p2 = p.GetPlace("EMIG");
            Assert.IsNullOrEmpty(p2);

        }

        [Test]
        public void BadDate()
        {
            var txt = "0 @I1@ INDI\n1 EMIG\n2 DATE garbage";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            var e = p.GetEvent("EMIG");
            Assert.IsFalse(e.GedDate.Initialized);
            var d = p.GetDate("EMIG");
            Assert.AreEqual("garbage", d); // TODO is this what we want?
            var p2 = p.GetPlace("EMIG");
            Assert.IsNullOrEmpty(p2);
        }

        [Test]
        public void Place()
        {
            var txt = "0 @I1@ INDI\n1 EMIG\n2 PLAC Logan Airport";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            var d = p.GetDate("EMIG");
            Assert.IsNullOrEmpty(d);
            var p2 = p.GetPlace("EMIG");
            Assert.AreEqual("Logan Airport", p2);

        }
        [Test]
        public void What()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            var w = p.GetWhat("EMIG");
            Assert.AreEqual("deported", w);
            var d = p.GetDate("EMIG");
            Assert.IsNullOrEmpty(d);
            var p2 = p.GetPlace("EMIG");
            Assert.IsNullOrEmpty(p2);
        }

        [Test]
        public void NoEvent()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            var w = p.GetWhat("MILT");
            Assert.IsNullOrEmpty(w);
            var d = p.GetDate("MILT");
            Assert.IsNullOrEmpty(d);
            var p2 = p.GetPlace("MILT");
            Assert.IsNullOrEmpty(p2);
            var e = p.GetEvent("MILT");
            Assert.IsNull(e);
        }

        [Test]
        public void NoChild()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.IsNull(p.GetParent(true));
            Assert.IsNull(p.GetParent(false));
        }

        [Test]
        public void ChildNoParents()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            Assert.IsNull(p.GetParent(true));
            Assert.IsNull(p.GetParent(false));
        }

        [Test]
        public void GetDadNoName()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@\n1 HUSB @I2@\n0 @I2@ INDI\n1 FAMS @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(2, f.AllPeople.Count());
            var p = f.PersonById("I1"); // TODO: rename -> GetPersonById
            Assert.IsNotNull(p);
            Assert.IsNull(p.GetParent(false));
            var d = p.GetParent(true);
            Assert.IsNullOrEmpty(d);
        }

        [Test]
        public void GetDad()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@\n1 HUSB @I2@\n0 @I2@ INDI\n1 NAME Fred /Kruger/\n1 FAMS @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(2, f.AllPeople.Count());
            var p = f.PersonById("I1"); // TODO: rename -> GetPersonById
            Assert.IsNotNull(p);
            Assert.IsNull(p.GetParent(false));
            var d = p.GetParent(true);
            Assert.AreEqual("Fred Kruger", d);
        }

        [Test]
        public void GetMomNoName()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@\n1 WIFE @I2@\n0 @I2@ INDI\n1 FAMS @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(2, f.AllPeople.Count());
            var p = f.PersonById("I1"); // TODO: rename -> GetPersonById
            Assert.IsNotNull(p);
            Assert.IsNull(p.GetParent(true));
            var d = p.GetParent(false);
            Assert.IsNullOrEmpty(d);
        }

        [Test]
        public void GetMom()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@\n1 WIFE @I2@\n0 @I2@ INDI\n1 NAME Jane /Kruger/\n1 FAMS @F1@";
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(2, f.AllPeople.Count());
            var p = f.PersonById("I1"); // TODO: rename -> GetPersonById
            Assert.IsNotNull(p);
            Assert.IsNull(p.GetParent(true));
            var d = p.GetParent(false);
            Assert.AreEqual("Jane Kruger", d);
        }

        // TODO getting mom or dad when ambiguous spouse
    }
}
