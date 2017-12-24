using System.Collections.Generic;
using System.Text;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a NOTE reference within a record.
    /// 
    /// The GEDCOM standard specifies two variants of NOTE references: a cross-reference
    /// to a NOTE record, or an "embedded" note.
    /// 
    /// \warning Future library releases may convert "embedded" notes to cross-reference.
    /// </summary>
    public class Note : StructCommon
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
    }

    /// <summary>
    /// Represents a record which may contain NOTE references.
    /// </summary>
    public interface NoteHold
    {
        // Any NOTE references associated with a record.
        List<Note> Notes { get; }
    }
}
