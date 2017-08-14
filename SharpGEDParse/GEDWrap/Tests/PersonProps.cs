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
        private Person LoadPerson(string txt)
        {
            Forest f = LoadGEDFromStream(txt);
            Assert.AreEqual(0, f.ErrorsCount);
            Assert.AreEqual(0, f.Errors.Count);

            Assert.AreEqual(1, f.AllPeople.Count());
            var p = f.AllPeople.First();
            return p;
        }

        [Test]
        public void Date()
        {
            var txt = "0 @I1@ INDI\n1 EMIG\n2 DATE 1 APR 1990";
            var p = LoadPerson(txt);

            var d = p.GetDate("EMIG");
            Assert.AreEqual("1-Apr-1990", d);
            var p2 = p.GetPlace("EMIG");
            Assert.IsNullOrEmpty(p2);
        }

        [Test]
        public void BadDate()
        {
            var txt = "0 @I1@ INDI\n1 EMIG\n2 DATE garbage";
            var p = LoadPerson(txt);

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
            var p = LoadPerson(txt);

            var d = p.GetDate("EMIG");
            Assert.IsNullOrEmpty(d);
            var p2 = p.GetPlace("EMIG");
            Assert.AreEqual("Logan Airport", p2);
        }

        [Test]
        public void What()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported";
            var p = LoadPerson(txt);

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
            var p = LoadPerson(txt);

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
            var p = LoadPerson(txt);

            Assert.IsNull(p.GetParent(true));
            Assert.IsNull(p.GetParent(false));
        }

        [Test]
        public void ChildNoParents()
        {
            var txt = "0 @I1@ INDI\n1 EMIG deported\n1 FAMC @F1@\n0 @F1@ FAM\n1 CHIL @I1@";
            var p = LoadPerson(txt);

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

        [Test]
        public void Attrib()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school";
            var p = LoadPerson(txt);

            var a = p.GetAttrib("EDUC");
            Assert.AreEqual("high school", a.Descriptor);
        }

        [Test]
        public void NoAttrib()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school";
            var p = LoadPerson(txt);

            var a = p.GetAttrib("OCCU");
            Assert.IsNull(a);
        }

        [Test]
        public void Given1()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school";
            var p = LoadPerson(txt);

            Assert.IsNullOrEmpty(p.Given);
        }
        [Test]
        public void Given2()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school\n1 NAME /kruger/";
            var p = LoadPerson(txt);

            Assert.IsNullOrEmpty(p.Given);
        }
        [Test]
        public void Given3()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school\n1 NAME fred /kruger/";
            var p = LoadPerson(txt);

            Assert.AreEqual("fred", p.Given);
        }
        [Test]
        public void Last1()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school";
            var p = LoadPerson(txt);

            Assert.IsNullOrEmpty(p.Surname);
        }
        [Test]
        public void Last2()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school\n1 NAME /kruger/";
            var p = LoadPerson(txt);

            Assert.AreEqual("kruger", p.Surname);
        }
        [Test]
        public void Last3()
        {
            var txt = "0 @I1@ INDI\n1 EDUC high school\n1 NAME fred";
            var p = LoadPerson(txt);

            Assert.IsNullOrEmpty(p.Surname);
        }
    }
}
