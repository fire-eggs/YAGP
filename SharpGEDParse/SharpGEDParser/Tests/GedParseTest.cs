using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SharpGEDParser.Model;

namespace SharpGEDParser.Tests
{
	public class GedParseTest
	{
		public static Stream ToStream(string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str));
		}

		// For those tests which need to verify errors at the topmost level
		public static FileRead ReadItHigher(string testString)
		{
			// TODO as implemented, trailing newline in original string will cause an "empty line" error record to be generated
			FileRead fr = new FileRead();
			using (var stream = new StreamReader(ToStream(testString)))
			{
				fr.ReadLines(stream);
			}
			return fr;
		}

        public static List<GEDCommon> ReadIt(string testString)
        {
            var fr = ReadItHigher(testString);
            return fr.Data.Select(o => o as GEDCommon).ToList();
        }

	    public static T parse<T>(string val) where T:class
	    {
            var res = ReadIt(val);
            Assert.AreEqual(1, res.Count);
            T rec = res[0] as T;
            Assert.IsNotNull(rec);
            return rec;       
	    }

        public static GEDCommon ReadOne(string teststring)
        {
            var res = ReadIt(teststring);
            Assert.AreEqual(1, res.Count);
            return res[0];
        }

	}
}

