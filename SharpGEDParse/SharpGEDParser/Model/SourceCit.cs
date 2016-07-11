using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Source Citation record
    public class SourceCit : StructCommon
    {
        public string Xref { get; set; } // will be empty if an embedded citation

        public string Desc { get; set; }

        public string Page { get; set; }

        public string Event { get; set; }

        public string Role { get; set; }

        public string Quay { get; set; }

        public bool Data { get; set; } // was the DATA tag encountered

        public DateTime? Date { get; set; } // will be null if an embedded citation

        private List<string> _text;
        public List<string> Text { get { return _text ?? (_text = new List<string>()); }}

        public bool AnyText { get { return _text != null; } } // Don't force allocation of List during verify

        private NoteHold _noteHold = new NoteHold();
        public List<Note> Notes { get { return _noteHold.Notes; } }

        private MediaHold _mediaHold = new MediaHold();
        public List<MediaLink> Media { get { return _mediaHold.Media; } }
    }
}
