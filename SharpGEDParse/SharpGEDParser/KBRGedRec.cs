using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser
{
    public class KBRGedRec
    {
        public GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            Lines = lines;
        }

        public override string ToString()
        {
            return string.Format("KBRGedRec:{0}:{1}:{2}", Tag, Ident, Lines);
        }

        public string Ident { get; set; }
        public string Tag { get; set; }

        // Unknown (i.e. not custom) tags
        // Keep null until required, saves memory
        private List<UnkRec> _unknowns;
        public List<UnkRec> Unknowns { get { return _unknowns ?? (_unknowns = new List<UnkRec>()); }}

        // Problems, other than 'unknown' tag
        // Keep null until required, saves memory
        private List<UnkRec> _errors;
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); }}

        // Custom tags as defined by other genealogical applications
        // Keep null until required, saves memory
        private List<UnkRec> _custom;
        public List<UnkRec> Custom { get { return _custom ?? (_custom = new List<UnkRec>()); }}

        // Keep null until required, saves memory
        private List<GedSourCit> _sources;
        public List<GedSourCit> Sources { get { return _sources ?? (_sources = new List<GedSourCit>()); } }

        // Keep null until required, saves memory
        private List<DataRec> _data;
        public List<DataRec> Data { get { return _data ?? (_data = new List<DataRec>()); } }

        // TODO I cannot remember: should original note pointer be preserved?
        private List<string> _notes;
        public List<string> Notes { get { return _notes ?? (_notes = new List<string>()); }}

        public Tuple<int, int> Change { get; set; }
    }
}
