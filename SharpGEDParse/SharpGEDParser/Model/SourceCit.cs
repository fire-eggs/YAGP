using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a citation to a <b>GEDCOM</b> SOUR record.
    /// 
    /// The GEDCOM standard allows two variants of source citations:
    /// a cross-reference to a SOUR record, or an "embedded" citation.
    /// 
    /// \warning Future library releases may convert "embedded" citations to cross-reference.
    /// </summary>
    public class SourceCit : StructCommon, NoteHold, MediaHold
    {
        /// <summary>
        /// The cross-reference to a SOUR record. This will be empty if
        /// this is an "embedded" citation.
        /// </summary>
        public string Xref { get; set; } // will be empty if an embedded citation

        public string Desc { get; set; }

        public string Page { get; set; }

        public string Event { get; set; }

        public string Role { get; set; }

        public string Quay { get; set; }

        public bool Data { get; set; } // was the DATA tag encountered

        // TODO additional parsing/validation for date
        /// <summary>
        /// The date this data was entered in the source document. This
        /// will be null if this is an embedded citation.
        /// </summary>
        public string Date { get; set; }

        private List<string> _text;
        /// <summary>
        /// The text from the source. Will be empty if there is none.
        /// </summary>
        public List<string> Text { get { return _text ?? (_text = new List<string>()); }}

        /// <summary>
        /// Is any text provided from the source? Will be true if there is none.
        /// </summary>
        public bool AnyText { get { return _text != null; } } // Don't force allocation of List during verify

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<MediaLink> _media;
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
    }

    /// <summary>
    /// Represents records which may contain source citations.
    /// </summary>
    public interface SourceCitHold
    {
        /// <summary>
        /// Any source citations associated to a record.
        /// </summary>
        List<SourceCit> Cits { get; }
    }
}
