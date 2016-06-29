using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser
{
    public class GedSource : KBRGedRec
    {
        public GedSource(GedRecord lines) : base(lines)
        {
            Beg = lines.Beg; // TODO hack for unit tests ???
            End = lines.End; // TODO hack for unit tests ???

            Tag = "SOUR";
        }

        // TODO XrefRec usage?
        public string XRef { get; set; }

        // Data or Events?
        // events
        // agency
        // notes

        public string Embed { get; set; } // embedded source text

        public string Author { get; set; }

        public string Title { get; set; }

        public string Abbreviation { get; set; }

        public string Publication { get; set; }

        public string Text { get; set; }

        public string RIN { get; set; }

        private List<GedRepository> _citations;
        public List<GedRepository> Citations { get { return _citations ?? (_citations = new List<GedRepository>()); }}

        private List<string> _userRefs;
        public List<string> UserReferences { get { return _userRefs ?? (_userRefs = new List<string>()); }}

        public int Beg { get; set; } // TODO having problems

        public int End { get; set; } // TODO having problems

        // Change in parent
        // Notes in parent
        // RIN is where?
    }
}
