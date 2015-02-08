
namespace SharpGEDParser
{
    public class KBRGedHead : KBRGedRec
    {
        public KBRGedHead(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "HEAD"; // TODO use enum
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }
    }
}
