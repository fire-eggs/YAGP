using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class KBRGedEvent : KBRGedRec
    {
        public KBRGedEvent(GedRecord lines, string tag) : base(lines)
        {
            Tag = tag;

            Notes = new List<Tuple<int, int>>();
            Sources = new List<SourceRec>();
            Unknowns = new List<UnkRec>();
            Errors = new List<UnkRec>();
        }

        public List<SourceRec> Sources { get; set; } // TODO COMMON
        public List<Tuple<int, int>> Notes { get; set; }
        public Tuple<int, int> Change { get; set; } // TODO COMMON
        public List<UnkRec> Unknowns { get; set; } // TODO COMMON
        public List<UnkRec> Errors { get; set; }

        public string Detail { get; set; } // e.g. caste_name
        public string Date { get; set; }
        public string Place { get; set; }
        public string Age { get; set; }
        public string Type { get; set; } // detail, classification
        public string Agency { get; set; }
        public string Cause { get; set; }
        public string Religion { get; set; }
        public string Restriction { get; set; }

    }
}
