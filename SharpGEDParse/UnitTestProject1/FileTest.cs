using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Exercise file behavior.
// Create a temp file, write out a known string, read and parse the file.

// TODO encodings
// TODO valid INDI record

// TODO how to exercise charset/encoding variants, i.e.  different BOM
 
namespace UnitTestProject1
{
    [TestClass]
    public class FileTest : GedParseTest
    {
        public List<KBRGedRec> CommonBasic(string txt, Encoding fileEnc)
        {
            // Exercise a file encoding
            
            var tmppath = Path.GetTempFileName();
            using (FileStream fStream = new FileStream(tmppath, FileMode.Create))
            {
                using (StreamWriter stream = new StreamWriter(fStream, fileEnc))
                {
                    stream.Write(txt);
                }
            }

            FileRead fr = new FileRead();
            fr.ReadGed(tmppath);
            File.Delete(tmppath);
            return fr.Data.Select(o => o as KBRGedRec).ToList();
        }

        [TestMethod]
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

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void TestDefault()
        {
            var results = CommonEnc(Encoding.Default);
        }

        [TestMethod]
        public void TestUTF8()
        {
            var results = CommonEnc(Encoding.UTF8);
            // TODO verify UTF8 characters
        }

        [TestMethod]
        public void TestUTF16LE()
        {
            var results = CommonEnc(Encoding.BigEndianUnicode);
            // TODO verify characters
        }

        [TestMethod]
        public void TestUTF32()
        {
            var results = CommonEnc(Encoding.UTF32);
            // TODO verify characters
        }

        [TestMethod]
        public void TestUnicode()
        {
            var results = CommonEnc(Encoding.Unicode);
            // TODO verify characters
        }

        private List<KBRGedRec> CommonEnc(Encoding fileEnc)
        {
            var txt = "0 HEAD\n1 SOUR 0\n1 SUBM @U_A@\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR ASCII\n0 @U_A@ SUBM\n1 NAME X\n0 TRLR";
            var results = CommonBasic(txt, fileEnc);
            Assert.AreEqual(2, results.Count);
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
                if ((result as KBRGedIndi) != null)
                    indi++;
                if ((result as KBRGedFam) != null)
                    fam++;
            }

            Assert.AreNotEqual(0, indi);
            Assert.AreNotEqual(0, fam);
        }

        [TestMethod]
        public void AllGed()
        {
            var path = @"E:\projects\YAGP\Sample GED\allged.ged"; // TODO project-relative path
            DoFile(path);
        }

        [TestMethod]
        public void TGC55()
        {
            var path = @"E:\projects\YAGP\Sample GED\tgc55c.ged"; // TODO project-relative path
            DoFile(path);
        }
    }
}
