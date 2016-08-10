using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Source Citation record
    public class SourceCit : StructCommon, NoteHold, MediaHold
    {
        public string Xref { get; set; } // will be empty if an embedded citation

        public string Desc { get; set; }

        public string Page { get; set; }

        public string Event { get; set; }

        public string Role { get; set; }

        public string Quay { get; set; }

        public bool Data { get; set; } // was the DATA tag encountered

        // TODO additional parsing/validation for date
        public string Date { get; set; } // will be null if an embedded citation

        private List<string> _text;
        public List<string> Text { get { return _text ?? (_text = new List<string>()); }}

        public bool AnyText { get { return _text != null; } } // Don't force allocation of List during verify

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<MediaLink> _media;
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
    }

    public interface SourceCitHold
    {
        List<SourceCit> Cits { get; }
    }
}
