using System;
using System.Collections.Generic;
using GedTag = SharpGEDParser.Model.Tag.GedTag;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents an individual's name.
    /// </summary>
    /// A NAME entry in the GEDCOM is of the form:
    /// \code
    /// 1 NAME given1 given2 /surname/ suffix
    /// \endcode
    /// The Names property will contain "given1 given2" (any text before the first slash).
    /// The Surname property will contain "surname" (any text within the slashes).
    /// The Suffix property will contain "suffix" (any text after the slashes).
    /// 
    /// Additional name data is contained in the Parts structure.
    public class NameRec : StructCommon, SourceCitHold, NoteHold
    {
        /// <summary>
        /// The person's given name(s) - from the NAME line.
        /// </summary>
        public string Names { get; set; }
        /// <summary>
        /// The person's surname - from the NAME line.
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Any name suffix for the person - from the NAME line.
        /// </summary>
        public string Suffix { get; set; }

        // TODO do something better/complete with NAME sub-records
        private List<Tuple<GedTag, string>> _parts;
        /// <summary>
        /// The person's name data, as represented with extra sub-records.
        /// </summary>
        /// \todo More details
        public List<Tuple<GedTag, string>> Parts
        {
            get { return _parts ?? (_parts = new List<Tuple<GedTag,string>>()); }             
        }

        private List<Note> _notes;
        private List<SourceCit> _cits;

        /// <summary>
        /// Any NOTEs associated with the record. 
        /// </summary>
        /// An empty list if none.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        /// <summary>
        /// Any Source citations associated with the record. 
        /// </summary>
        /// An empty list if none.
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }
}
