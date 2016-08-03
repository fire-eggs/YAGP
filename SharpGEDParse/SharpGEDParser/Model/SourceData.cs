using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Container for DATA within a SOURCE record
    public class SourceData : StructCommon, NoteHold
    {
        private List<SourEvent> _events;

        public List<SourEvent> Events { get { return _events ?? (_events = new List<SourEvent>()); } }

        public string Agency { get; set; }

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        // TODO unknowns
    }
}
