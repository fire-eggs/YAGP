using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // An OBJE link
    // There are two variants: a simple Xref, and a non-Xref. 
    // The OBJE *record* is not used by almost all applications, so the simple Xref variant isn't useful?

    /// <summary>
    /// Represents a media file referenced by a Media (OBJE) record or
    /// by a multimedia link.
    /// 
    /// Contains information pertaining to a media file on-disk.
    /// 
    /// See MediaRecord for an example GEDCOM record.
    /// </summary>
    public class MediaFile
    {
        /// <summary>
        /// A file reference to the media file. 
        /// </summary>
        /// 
        /// Typically a file path. The application should be prepared to 
        /// request an updated path from the user.
        public string FileRefn { get; set; }

        /// <summary>
        /// The file format of the media file. 
        /// </summary>
        /// 
        /// The GEDCOM standard suggests only a small number of possible values; the application
        /// should be prepared to handle any possible file format.
        public string Form { get; set; }

        /// <summary>
        /// Indicates the "type of material" for the media file. 
        /// </summary>
        /// 
        /// The GEDCOM standard suggests only a small number of possible values; the 
        /// application should be prepared to handle any possible type.
        public string Type { get; set; }

        /// <summary>
        /// A descriptive title for the item.
        /// </summary>
        public string Title { get; set; }
    }

    /// <summary>
    /// Represents a link to a multimedia object.
    /// </summary>
    /// 
    /// The GEDCOM standard allows two variants of multimedia links: 'pointer' (cross-reference)
    /// and 'embedded'.
    /// 
    /// \note Future releases may convert 'embedded' to 'pointer'.
    public class MediaLink : StructCommon, NoteHold
    {
        /// <summary>
        /// A cross-reference to an OBJE (multimedia) record.
        /// </summary>
        /// 
        /// Will be empty if an 'embedded' link.
        public string Xref { get; set; }

        private List<MediaFile> _files;

        /// <summary>
        /// Any multimedia files as part of an 'embedded' link.
        /// </summary>
        /// 
        /// Will be empty if a 'pointer' link.
        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); }}

        /// <summary>
        /// A descriptive title for the multimedia link.
        /// </summary>
        /// 
        /// Will be empty if a 'pointer' link.
        public string Title { get; set; }

        private List<Note> _notes; // Notes are possible with GEDCOM 5.5.
        /// <summary>
        /// Any NOTEs associated with the multimedia link.
        /// </summary>
        /// 
        /// NOTEs are only allowed here under the GEDCOM 5.5 standard.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }

    /// <summary>
    /// Represents OBJE references associated to a record.
    /// </summary>
    public interface MediaHold
    {
        /// <summary>
        /// Any media records associated to the record.
        /// </summary>
        List<MediaLink> Media { get; }
    }
}
