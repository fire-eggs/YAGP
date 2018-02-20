using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Container for DATA within a source (SOUR) record.
    /// </summary>
    public class SourceData : StructCommon, NoteHold
    {
        private List<SourEvent> _events;

        /// <summary>
        /// Details regarding the events recorded in the source.
        /// </summary>
        /// See SourData for an example.
        /// Will be an empty list if none.
        public List<SourEvent> Events { get { return _events ?? (_events = new List<SourEvent>()); } }

        /// <summary>
        /// Details about those responsible for the source.
        /// </summary>
        /// E.g. the person or institution which created the source.
        public string Agency { get; set; }

        private List<Note> _notes;

        /// <summary>
        /// Any notes associated with the source.
        /// </summary>
        /// Will be an empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}