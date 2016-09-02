using System.Diagnostics.CodeAnalysis;

namespace SharpGEDParser
{
    public class KBRGedUnk : KBRGedRec
    {
        public KBRGedUnk(GedRecord lines, string ident, string tag)
            : base(lines)
        {
            Ident = ident;
            Tag = tag;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }
    }
}
