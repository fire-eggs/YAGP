using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Note : StructCommon
    {
        public string Xref { get; set; }

        public string Text { get; set; }
    }

    public class NoteHold
    {
        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
