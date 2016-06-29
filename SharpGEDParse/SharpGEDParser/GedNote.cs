// Top-level note record "0 @R1@ NOTE"

namespace SharpGEDParser
{
    public class GedNote : KBRGedRec
    {
        public GedNote(GedRecord lines, string ident)
            : base(lines)
        {
            Ident = ident;
            Tag = "NOTE"; // TODO use enum
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }
    }
}
