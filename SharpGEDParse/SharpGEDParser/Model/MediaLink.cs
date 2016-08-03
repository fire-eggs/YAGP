using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // An OBJE link
    // There are two variants: a simple Xref, and a non-Xref. TODO: consider forcing to simple Xref

    public class MediaFile
    {
        public string FileRefn { get; set; }

        public string Form { get; set; }

        public string Type { get; set; }

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

    public interface MediaHold
    {
        List<MediaLink> Media { get; }
    }
}
