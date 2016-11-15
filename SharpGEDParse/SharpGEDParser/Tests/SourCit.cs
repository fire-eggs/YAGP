using NUnit.Framework;
using SharpGEDParser.Model;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

// TODO generalize for all record types

// Expanded Source Citation testing (from mutation testing)

namespace SharpGEDParser.Tests
{
    [TestFixture]
    class SourCit : GedParseTest
    {
        [Test]
        public void RefSourceInv()
        {
            // invalid reference source id
            string txt = "0 @N1@ NOTE\n1 SOUR @@";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.IsNotNullOrEmpty(res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void RefSourceText()
        {
            // TEXT tag for reference source is error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 TEXT this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.IsNotNullOrEmpty(res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void EmbSourcePage()
        {
            // PAGE tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR embed\n2 PAGE this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.IsNotNullOrEmpty(rec.Cits[0].Page);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.IsNotNullOrEmpty(res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void EmbSourceNoPage()
        {
            // Embedded source - no PAGE tag
            string txt = "0 @N1@ NOTE\n1 SOUR embed";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(0, res[0].Errors.Count, "error");
            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual(null, rec.Cits[0].Xref);
            Assert.AreEqual(null, rec.Cits[0].Page);
        }

        [Test]
        public void EmbSourceEven()
        {
            // EVEN tag for embedded source is error
            string txt = "0 @N1@ NOTE\n1 SOUR inbed\n2 EVEN this is error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.IsNull(rec.Cits[0].Xref);
            Assert.IsNotNullOrEmpty(rec.Cits[0].Event);
            Assert.AreEqual(1, res[0].Errors.Count, "No error");
            Assert.IsNotNullOrEmpty(res[0].Errors[0].Error); // TODO verify details?
        }

        [Test]
        public void RefSourceEven()
        {
            // EVEN tag for ref source is not error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 EVEN this is no error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.IsNotNullOrEmpty(rec.Cits[0].Event);
            Assert.IsNullOrEmpty(rec.Cits[0].Page);
            Assert.AreEqual(0, res[0].Errors.Count, "No error");
        }

        [Test]
        public void RefSourcePage()
        {
            // PAGE tag for ref source is not error
            string txt = "0 @N1@ NOTE\n1 SOUR @p1@\n2 PAGE this is no error";
            var res = ReadIt(txt);
            Assert.AreEqual(1, res.Count);
            var rec = res[0] as NoteRecord;
            Assert.IsNotNull(rec);

            Assert.AreEqual(1, rec.Cits.Count);
            Assert.AreEqual("p1", rec.Cits[0].Xref);
            Assert.IsNullOrEmpty(rec.Cits[0].Event);
            Assert.IsNotNullOrEmpty(rec.Cits[0].Page);
            Assert.AreEqual(0, res[0].Errors.Count, "No error");
        }

    }
}
