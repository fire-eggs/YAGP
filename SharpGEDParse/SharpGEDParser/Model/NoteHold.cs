using System.Collections.Generic;
using System.Text;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a NOTE reference within a record.
    /// 
    /// The GEDCOM standards specify two variants of NOTE references: a cross-reference
    /// to a NOTE record, or an "embedded" note.
    /// 
    /// The GEDCOM 5.5 standard permits source citations for a NOTE reference.
    /// 
    /// \warning Future library releases may convert "embedded" notes to cross-reference.
    /// \warning Future library releases may move note source citations to a note record.
    /// </summary>
    public class Note : StructCommon, SourceCitHold
    {
        /// <summary>
        /// A NOTE record cross-reference.
        /// 
        /// Will be empty if this is an embedded note.
        /// </summary>
        public string Xref { get; set; }

        /// <summary>
        /// All text associated to an "embedded" note.
        /// 
        /// Will typically be empty if this is a cross-reference, barring non-standard
        /// GEDCOM files.
        /// </summary>
        public string Text { get; set; }

        internal StringBuilder Builder { get; set; } // Accumulate text during parse

        private List<SourceCit> _cits;

        // GEDCOM 5.5 standard allows source citations on NOTE structure
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }

    /// <summary>
    /// Represents a record which may contain NOTE references.
    /// </summary>
    public interface NoteHold
    {
        /// <summary>
        /// Any NOTE references associated with a record.
        /// </summary>
        List<Note> Notes { get; }
    }
}
