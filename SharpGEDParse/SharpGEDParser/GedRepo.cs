// Top-level repository record "0 @R1@ REPO"
// TODO GedRepository is confusing, being a repository reference

namespace SharpGEDParser
{
    public class GedRepo : KBRGedRec
    {
        public GedRepo(GedRecord lines, string ident)
            : base(lines)
        {
            Ident = ident;
            Tag = "REPO"; // TODO use enum
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }
    }
}
