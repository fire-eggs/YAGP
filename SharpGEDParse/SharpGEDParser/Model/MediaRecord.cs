using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents a GEDCOM Media (OBJE) record.
    /// 
    /// NOTE:
    /// The GEDCOM 5.5.1 standard changed the OBJE format from the 5.5
    /// version. 5.5 standard OBJE records have been converted to 5.5.1
    /// format by the parser.
    /// 
    /// NOTE:
    /// The Gedcom 5.5 BLOB record is treated as an 'unknown'. BLOB is
    /// extremely obscure and not supported by the majority of genealogy
    /// applications.
    /// 
    /// Example OBJE record:
    /// \code
    /// 0 @M2@ OBJE
    /// 1 FILE Moreton Bay Assisted Immig 1848-59.jpg
    /// 2 FORM jpeg
    /// 2 TITL Extract from New South Wales Assisted Immigrants database
    /// \endcode
    /// </summary>
    public class MediaRecord : GEDCommon, NoteHold, SourceCitHold
    {
        /// <summary>
        /// GEDCOM Tag value.
        /// </summary>
        public override string Tag { get { return "OBJE"; } }

        private List<Note> _notes;

        /// <summary>
        /// Any NOTEs associated with the record. An empty list if none.
        /// </summary>
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
        /// <summary>
        /// Any SOUR citations associated with the record. An empty list if none.
        /// </summary>
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }

        private List<MediaFile> _files;

        /// <summary>
        /// The media files referenced by the record.
        /// </summary>
        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); } }

        internal MediaRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            GedRecParse.NonStandardRemain(remain, this);
        }

        //[ExcludeFromCodeCoverage]
        //public override string ToString()
        //{
        //    return Tag;
        //}
    }
}
