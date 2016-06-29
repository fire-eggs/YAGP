// Top level submitter record "0 @S1@ SUBM"
namespace SharpGEDParser
{
    public class GedSubm : KBRGedRec
    {
        public GedSubm(GedRecord lines, string ident)
            : base(lines)
        {
            Ident = ident;
            Tag = "SUBM"; // TODO use enum
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }
    }
}
