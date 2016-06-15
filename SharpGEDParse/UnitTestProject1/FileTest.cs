using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpGEDParser;

// Exercise file behavior.
// Create a temp file, write out a known string, read and parse the file.

// TODO encodings
// TODO valid INDI record

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
            return fr.Data;
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
            // The smallest valid GED
            var txt = "0 HEAD\n1 SOUR 0\n1 SUBM @U_A@\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR ASCII\n0 @U_A@ SUBM\n1 NAME X\n0 TRLR";
            Encoding fileEnc = Encoding.Default;
            var results = CommonBasic(txt, fileEnc);
            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void TestUTF8()
        {
            // The smallest valid GED
            var txt = "0 HEAD\n1 SOUR 0\n1 SUBM @U_A@\n1 GEDC\n2 VERS 5.5.1\n2 FORM LINEAGE-LINKED\n1 CHAR ASCII\n0 @U_A@ SUBM\n1 NAME X\n0 TRLR";
            Encoding fileEnc = Encoding.UTF8;
            var results = CommonBasic(txt, fileEnc);
            Assert.AreEqual(2, results.Count);

            // TODO verify UTF8 characters
        }

    }
}
