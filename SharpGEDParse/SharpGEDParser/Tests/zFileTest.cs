using NUnit.Framework;
using SharpGEDParser.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable ConvertToConstant.Local
// ReSharper disable InconsistentNaming

namespace SharpGEDParser.Tests
{
    // Exercise file behavior.
    // Create a temp file, write out a known string, read and parse the file.

    // TODO encodings
    // TODO valid INDI record

    // TODO how to exercise charset/encoding variants, i.e.  different BOM

    [TestFixture]
    public class zFileTest : GedParseTest
    {
        public List<GEDCommon> CommonBasic(string txt, Encoding fileEnc)
        {
            // Exercise a file encoding

            var tmppath = Path.GetTempFileName();
            FileStream fStream = null;
            try
            {
                fStream = new FileStream(tmppath, FileMode.Create); // Code analysis claims fStream will be disposed twice if 'using'
                using (StreamWriter stream = new StreamWriter(fStream, fileEnc))
                {
                    stream.Write(txt);
                }
            }
            finally
            {
                if (fStream != null)                
                    fStream.Dispose();
            }

            FileRead fr = new FileRead();
            fr.ReadGed(tmppath);
            File.Delete(tmppath);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

        [Test]
        public void TestBasic()
        {
            // The smallest valid GED
            var txt = "0 HEAD\n1 SOUR 0\n1 SUBM @U_A@\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR ASCII\n0 @U_A@ SUBM\n1 NAME X\n0 TRLR";

            var tmppath = Path.GetTempFileName();
            using (StreamWriter stream = new StreamWriter(tmppath))
            {
                stream.Write(txt);
            }

            FileRead fr = new FileRead();
            fr.ReadGed(tmppath);
            var results = fr.Data;

            File.Delete(tmppath);

            Assert.AreEqual(3, results.Count);
        }

        [Test]
        public void TestDefault()
        {
            var results = CommonEnc(Encoding.Default);
        }

        [Test]
        public void TestUTF8()
        {
            var results = CommonEnc(Encoding.UTF8);
            // TODO verify UTF8 characters
        }

        [Test]
        public void TestUTF16LE()
        {
            var results = CommonEnc(Encoding.BigEndianUnicode);
            // TODO verify characters
        }

        [Test]
        public void TestUTF32()
        {
            var results = CommonEnc(Encoding.UTF32);
            // TODO verify characters
        }

        [Test]
        public void TestUnicode()
        {
            var results = CommonEnc(Encoding.Unicode);
            // TODO verify characters
        }

        private List<GEDCommon> CommonEnc(Encoding fileEnc)
        {
            var txt = "0 HEAD\n1 SOUR 0\n1 SUBM @U_A@\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR ASCII\n0 @U_A@ SUBM\n1 NAME X\n0 TRLR";
            var results = CommonBasic(txt, fileEnc);
            Assert.AreEqual(3, results.Count);
            return results;
        }

        public void DoFile(string path)
        {
            FileRead fr = new FileRead();
            fr.ReadGed(path);
            var results = fr.Data;

            int indi = 0;
            int fam = 0;
            foreach (var result in results)
            {
                if ((result as IndiRecord) != null)
                    indi++;
                if ((result as FamRecord) != null)
                    fam++;
            }

            Assert.AreNotEqual(0, indi, path);
            Assert.AreNotEqual(0, fam, path);
        }

        [Test]
        public void AllGed()
        {
            var path = @"E:\projects\YAGP\Sample GED\allged.ged"; // TODO project-relative path
            DoFile(path);
        }

        [Test]
        public void TGC55()
        {
            var path = @"E:\projects\YAGP\Sample GED\tgc55c.ged"; // TODO project-relative path
            DoFile(path);
        }

        [Test]
        public void DoSpecial()
        {
            string path = @"Z:\HOST_E\projects\GED\GED files\5.5.1\index7_kbr.ged";
            DoFile(path);
        }

        [Test]
        public void zDoAll551()
        {
            var path = @"Z:\HOST_E\projects\GED\GED files\5.5.1";
            foreach (var file in Directory.GetFiles(path))
            {
                DoFile(file);
            }
        }

        [Test]
        public void SimpleGed()
        {
            var path = @"E:\projects\YAGP\Sample GED\export_ged_919.ged";// TODO project-relative path

            FileRead fr = new FileRead();
            fr.ReadGed(path);
            var results = fr.Data;

            Assert.AreEqual(0, fr.Errors.Count);

            var indis = fr.Indi;
            Assert.AreEqual(2, indis.Count);

            var indi = fr.FindIndi("I919");
            Assert.IsNotNull(indi);

            Assert.AreEqual('M', indi.Sex);
            Assert.IsNotNull(indi.CHAN.Date);
            Assert.AreEqual(1, indi.Names.Count);

        }

        [Test]
        public void SimpleGed2()
        {
            var path = @"E:\projects\YAGP\Sample GED\ege.ged"; // TODO project-relative path
            FileRead fr = new FileRead();
            fr.ReadGed(path);
            var results = fr.Data;

            Assert.AreEqual(0, fr.Errors.Count);

            var indi = fr.FindIndi("I26");
            Assert.IsNotNull(indi);

        }
        [Test]
        public void SimpleGed3()
        {
            var path = @"Z:\HOST_E\projects\GED\GED files\01small\pallanezf.ged"; // TODO project-relative path
            FileRead fr = new FileRead();
            fr.ReadGed(path);
            var results = fr.Data;

            Assert.AreEqual(0, fr.Errors.Count);

            var indis = fr.Indi;
            var indi = fr.FindIndi("I30");

        }
    }
}
