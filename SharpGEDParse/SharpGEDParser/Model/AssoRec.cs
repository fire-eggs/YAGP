using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class AssoRec : StructCommon, NoteHold, SourceCitHold
    {
        public string Relation { get; set; }

        public string Ident { get; set; }

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }
}
