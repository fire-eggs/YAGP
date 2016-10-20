using NUnit.Framework;

// Testing for Media (OBJE) records - using 5.5 syntax
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
    [TestFixture]
    public class Media55Test : GedParseTest
    {
        [Test]
        public void TestSimple1()
        {
            var txt = "0 @M1@ OBJE\n1 FORM gif\n1 BLOB\n2 CONT not real";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count); // treat BLOB as unknown
        }

        [Test]
        public void TestSimple2()
        {
            var txt = "0 @M1@ OBJE\n1 FORM gif\n1 OBJE @xref@";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            MediaRecord rec = res[0] as MediaRecord;
            Assert.IsNotNull(rec);
            Assert.AreEqual("M1", rec.Ident);
            Assert.AreEqual(1, rec.Files.Count);
            Assert.AreEqual("gif", rec.Files[0].Form);

            Assert.AreEqual(0, rec.Errors.Count);
            Assert.AreEqual(1, rec.Unknowns.Count); // treat BLOB as unknown
        }

    }
}
