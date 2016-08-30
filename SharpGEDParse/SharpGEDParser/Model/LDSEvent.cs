using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class LDSEvent : StructCommon, NoteHold, SourceCitHold
    {
        public string Tag { get; set; } // which event is this

        public string Date { get; set; }

        public string Temple { get; set; }

        public string Place { get; set; }

        public string Status { get; set; }

        public string StatusDate { get; set; }

        public string FamilyXref { get; set; } // for SLGC

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }
}
