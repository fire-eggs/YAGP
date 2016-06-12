using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class KBRGedRec
    {
        public GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            // TODO keep null ptrs until used
            Unknowns = new List<UnkRec>();
            Errors = new List<UnkRec>();
            Sources = new List<GedSourCit>();
            Data = new List<DataRec>();
            Notes = new List<string>();
            Lines = lines;
        }

        public override string ToString()
        {
            return string.Format("KBRGedRec:{0}:{1}:{2}", Tag, Ident, Lines);
        }

        public string Ident { get; set; }
        public string Tag { get; set; }
        public List<UnkRec> Unknowns { get; set; }
        public List<UnkRec> Errors { get; set; }
        public List<GedSourCit> Sources { get; set; }
        public List<DataRec> Data { get; set; }
        public List<string> Notes { get; set; } // TODO I cannot remember: should original note pointer be preserved?
        public Tuple<int, int> Change { get; set; }
    }
}
