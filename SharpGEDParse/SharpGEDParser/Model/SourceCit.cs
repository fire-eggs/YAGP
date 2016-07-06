using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Source Citation record
    public class SourceCit : StructCommon
    {
        public string Xref { get; set; }

        public string Desc { get; set; }

        public string Page { get; set; }

        public string Event { get; set; }

        public string Role { get; set; }

        public string Quay { get; set; }

        public DateTime? Date { get; set; }

        public string Text { get; set; } // TODO this can be multiple

        private NoteHold _noteHold = new NoteHold();
        public List<Note> Notes { get { return _noteHold.Notes; } }

        private MediaHold _mediaHold = new MediaHold();
        public List<MediaLink> Media { get { return _mediaHold.Media; } }
    }
}
