using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SharpGEDParser.Tests
{
	public class GedParseTest
	{
		public static Stream ToStream(string str)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(str ?? ""));
		}

		// For those tests which need to verify errors at the topmost level
		public static FileRead ReadItHigher(string testString)
		{
			// TODO this is a bug
			// TODO as implemented, trailing newline in original string will cause an "empty line" error record to be generated
			// Testing kludge required: parser won't terminate w/o trailing '0' record.
			if (!testString.EndsWith("\n0 KLUDGE") && !testString.EndsWith("\n0 TRLR"))
				testString += "\n0 KLUDGE";
			FileRead fr = new FileRead();
			using (var stream = new StreamReader(ToStream(testString)))
			{
				fr.ReadLines(stream);
			}
			return fr;
		}

		public static List<KBRGedRec> ReadIt(string testString)
		{
			var fr = ReadItHigher(testString);
			return fr.Data.Select(o => o as KBRGedRec).ToList();
		}

		public T parse<T>(string testString, string tagN) where T: class
		{
			var res = ReadIt(testString);
			Assert.AreEqual(1, res.Count, "record count");
			Assert.AreEqual(tagN, res[0].Tag, "Tag:"+tagN);
			var rec = res[0] as T;
			Assert.AreNotEqual(null, rec, "wrong record type:"+tagN);
			return rec;
		}

	}
}

