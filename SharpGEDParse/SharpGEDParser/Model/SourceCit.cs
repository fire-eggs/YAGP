using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a citation to a <b>GEDCOM</b> SOUR record.
    /// </summary>
    /// 
    /// The GEDCOM standard allows two variants of source citations:
    /// a pointer (cross-reference) to a SOUR record, or an embedded citation.
    /// 
    /// \warning Future library releases may convert "embedded" citations to cross-reference.
    public class SourceCit : StructCommon, NoteHold, MediaHold
    {
        /// <summary>
        /// The cross-reference to a SOUR record. 
        /// </summary>
        /// 
        /// This will be empty if this is an "embedded" citation.
        public string Xref { get; set; } // will be empty if an embedded citation

        /// <summary>
        /// The description of the source citation.
        /// </summary>
        /// 
        /// Will be empty if none, or this is a "pointer" citation.
        public string Desc { get; set; }

        /// <summary>
        /// The page within the source. 
        /// </summary>
        /// 
        /// This will be empty if this is an "embedded" citation.
        public string Page { get; set; }

        /// <summary>
        /// The type of event which was responsible for the source entry being recorded.
        /// </summary>
        ///
        /// E.g. citing a child's birth record as the source of this person's name        
        /// 
        /// This will be empty if this is an "embedded" citation.
        public string Event { get; set; }

        /// <summary>
        /// Indicates what role this person played in the event that is being cited in this context.
        /// </summary>
        ///
        /// E.g. citing a child's birth record as the source of this person's name, the value here
        /// would be "MOTH" or "FATH" as appropriate. Some GEDCOM-standard specified values include
        /// CHIL, HUSB, WIFE, MOTH, FATH, SPOU.
        public string Role { get; set; }

        /// <summary>
        /// A quantitative assessment of the credibility of this piece of information.
        /// </summary>
        /// 
        /// GEDCOM-standard values are a number from 0 to 3. Some genealogy programs use other values.
        public string Quay { get; set; }

        /// \cond PRIVATE
        public bool Data { get; set; } // was the DATA tag encountered
        /// \endcond

        // TODO additional parsing/validation for date
        /// <summary>
        /// The date this data was entered in the source document. 
        /// </summary>
        /// 
        /// This will be null if this is an embedded citation.
        public string Date { get; set; }

        private List<string> _text;
        /// <summary>
        /// The text from the source. 
        /// </summary>
        /// 
        /// Will be empty if there is none.
        /// 
        /// Contains the text from the SOUR.TEXT (embedded) or SOUR.DATA.TEXT (pointer) tag.
        public List<string> Text { get { return _text ?? (_text = new List<string>()); }}

        /// <summary>
        /// Is any text provided from the source? Will be true if there is none.
        /// </summary>
        public bool AnyText { get { return _text != null; } } // Don't force allocation of List during verify

        private List<Note> _notes;

        /// <summary>
        /// Any NOTEs associated with the record. 
        /// </summary>
        /// An empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<MediaLink> _media;
        /// <summary>
        /// Any Multimedia objects associated with the record.
        /// </summary>
        ///  
        /// An empty list if none.
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
