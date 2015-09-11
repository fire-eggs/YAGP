using System;
using System.Collections.Generic;

namespace SharpGEDParser
{
    public class KBRGedRec
    {
        public GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            Unknowns = new List<UnkRec>();
            Errors = new List<UnkRec>();
            Sources = new List<GedSourCit>();
            Data = new List<DataRec>();
            Notes = new List<Tuple<int, int>>();

            if (lines.LineCount < 1)
                throw new Exception("Empty GedRecord!");
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
        public List<Tuple<int, int>> Notes { get; set; }
        public Tuple<int, int> Change { get; set; }
    }
}
