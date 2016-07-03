using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Note
    {
        public string Xref { get; set; }

        public string Text { get; set; }

        // All other lines (typically custom/unknown)
        private List<LineSet> _other;
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }
    }

    public class NoteHold
    {
        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
