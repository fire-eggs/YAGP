using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SharpGEDParser.Model;

namespace SharpGEDParser
{
    public class Rec
    {
        public int Beg { get; set; }

        public int End { get; set; }

        // TODO enum?
        public string Tag { get; set; }
        //        public int LineCount { get { return End - Beg + 1; } }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format("{0}({1},{2})", Tag, Beg, End);
        }
    }

    // Currently used for _UID
    public class DataRec : Rec
    {
        public string Data { get; set; }

        public DataRec(string tag, string data)
        {
            Tag = tag;
            Data = data;
        }
    }

    public class NameRec : Rec
    {
        public string Names { get; set; }
        public string Surname { get; set; }
        public string Suffix { get; set; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format(" {0} /{1}/ {2}", Names, Surname, Suffix).Trim();
        }
    }

    public class KBRGedRec
    {
        public GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            Lines = lines;
        }

        //public override string ToString()
        //{
        //    return string.Format("KBRGedRec:{0}:{1}:{2}", Tag, Ident, Lines);
        //}

        public string Ident { get; set; }
        public string Tag { get; set; }

        // Unknown and custom tags
        // Keep null until required, saves memory
        private List<UnkRec> _unknowns;
        public List<UnkRec> Unknowns { get { return _unknowns ?? (_unknowns = new List<UnkRec>()); }}

        // Problems, other than 'unknown' tag
        // Keep null until required, saves memory
        private List<UnkRec> _errors;
        public List<UnkRec> Errors { get { return _errors ?? (_errors = new List<UnkRec>()); }}

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
