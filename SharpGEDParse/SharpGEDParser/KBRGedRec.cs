using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class KBRGedRec
    {
        public GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            // TODO keep null ptrs until used?
            Custom = new List<UnkRec>();
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

        // Unknown (i.e. not custom) tags
        public List<UnkRec> Unknowns { get; set; }

        // Problems, other than 'unknown' tag
        public List<UnkRec> Errors { get; set; }

        // Custom tags as defined by other genealogical applications
        public List<UnkRec> Custom { get; set; }
        public List<GedSourCit> Sources { get; set; }
        public List<DataRec> Data { get; set; }
        public List<string> Notes { get; set; } // TODO I cannot remember: should original note pointer be preserved?
        public Tuple<int, int> Change { get; set; }
    }
}
