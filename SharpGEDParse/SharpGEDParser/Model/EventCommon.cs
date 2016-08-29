using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class EventCommon : StructCommon, NoteHold, SourceCitHold, MediaHold
    {
        public string Tag { get; set; }

        public string Descriptor { get; set; }

        public string Type { get; set; } // detail, classification
        public string Agency { get; set; }
        public string Cause { get; set; }
        public string Religion { get; set; }
        public string Restriction { get; set; }

        public Address Address { get; set; }

        public string Place { get; set; } // TODO temporary - need full PLACE_STRUCTURE support

        public string Date { get; set; }

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }

        private List<MediaLink> _media;
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }

    }
}
