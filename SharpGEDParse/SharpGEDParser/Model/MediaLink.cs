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
    /// See #MediaRecord for an example GEDCOM record.
    /// </summary>
    public class MediaFile
    {
        /// <summary>
        /// A file reference to the media file. 
        /// 
        /// Typically a file path. The application should be prepared to 
        /// request an updated path from the user.
        /// </summary>
        public string FileRefn { get; set; }

        /// <summary>
        /// The file format of the media file. 
        /// 
        /// The GEDCOM standard suggests only a small number of possible values; the application
        /// should be prepared to handle any possible file format.
        /// </summary>
        public string Form { get; set; }

        /// <summary>
        /// Indicates the "type of material" for the media file. 
        /// 
        /// The GEDCOM standard suggests only a small number of possible values; the 
        /// application should be prepared to handle any possible type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// A descriptive title for the item.
        /// </summary>
        public string Title { get; set; }
    }

    public class MediaLink : StructCommon, NoteHold
    {
        public string Xref { get; set; }

        private List<MediaFile> _files;

        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); }}

        public string Title { get; set; }

        private List<Note> _notes; // Notes are possible with GEDCOM 5.5.
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }

    /// <summary>
    /// Represents OBJE references associated to a record.
    /// </summary>
    public interface MediaHold
    {
        List<MediaLink> Media { get; }
    }
}
