using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class KBRGedFam : KBRGedRec
    {
        public KBRGedFam(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "FAM";

            Sources = new List<SourceRec>();
            Childs = new List<string>();

            Unknowns = new List<UnkRec>();
            FamEvents = new List<EventRec>();
            Data = new List<DataRec>();
        }

        public List<UnkRec> Unknowns { get; set; } // TODO COMMON

        public List<string> Childs { get; set; }

        public List<SourceRec> Sources { get; set; } // TODO COMMON

        public List<DataRec> Data { get; set; } // TODO COMMON

        public string Dad { get; set; } // identity string for Father
        public string Mom { get; set; } // identity string for Mother

        public Tuple<int, int> Note { get; set; } // TODO COMMON
        public Tuple<int, int> Change { get; set; } // TODO COMMON

        public List<EventRec> FamEvents { get; set; } // TODO COMMON
    }
}
