using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class GedSourCit : KBRGedRec
    {
        // A Source Citation
        public GedSourCit(GedRecord lines) : base(lines)
        {
        }

        public string XRef { get; set; }

        public string Quay { get; set; }

        public string Text { get; set; }

        public string Event { get; set; }

        public string Date { get; set; }

        public string Role { get; set; }

        public string Page { get; set; }

        public string RIN { get; set; } // Non-standard

        // TODO embedded source needs to be converted to a SOURCE record and 'normal' citation
        public string Embed { get; set; }

        // TODO for testing
        public int Beg { get; set; }
        public int End { get; set; }
    }
}
