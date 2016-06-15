using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class GedSource : KBRGedRec
    {
        public GedSource(GedRecord lines) : base(lines)
        {
            Beg = lines.Beg; // TODO hack for unit tests ???
            End = lines.End; // TODO hack for unit tests ???

            Tag = "SOUR";
            UserReferences = new List<string>(); // TODO null until used?
            Citations = new List<GedRepository>(); // TODO null until used?
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

        public List<GedRepository> Citations { get; set; } 

        public List<string> UserReferences { get; set; }

        public int Beg { get; set; } // TODO having problems

        public int End { get; set; } // TODO having problems

        // Change in parent
        // Notes in parent
        // RIN is where?
    }
}
