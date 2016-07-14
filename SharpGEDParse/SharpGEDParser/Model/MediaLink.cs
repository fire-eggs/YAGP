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
    }

    public class MediaLink : StructCommon
    {
        public string Xref { get; set; }

        private List<MediaFile> _files;

        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); }}

        public string Title { get; set; }
    }

    public class MediaHold
    {
        private List<MediaLink> _media;

        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
    }
}
