using System.IO;
using System.Text;

namespace GEDWrap.Tests
{
    class TestUtil
    {
        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        protected Forest LoadGEDFromStream(string testString)
        {
            Forest f = new Forest();
            using (var stream = new StreamReader(ToStream(testString)))
            {
                f.LoadFromStream(stream);
            }
            return f;
        }
    }
}
