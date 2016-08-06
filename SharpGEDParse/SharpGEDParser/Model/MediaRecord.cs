using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // NOTE: the Gedcom 5.5 BLOB record is treated as an 'unknown'. 99% of
    // documented GED files don't use BLOB

    public class MediaRecord : GEDCommon, NoteHold, SourceCitHold
    {
        public static string Tag = "OBJE";

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }

        public List<MediaFile> _files;

        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); } }

        public MediaRecord(GedRecord lines, string ident, string remain)
            : base(lines, ident)
        {
            // Text = remain; // TODO what to do with extra?

            if (string.IsNullOrWhiteSpace(ident))
            {
                UnkRec err = new UnkRec();
                err.Error = "Missing identifier"; // TODO assign one?
                err.Beg = err.End = lines.Beg;
                err.Tag = Tag;
                Errors.Add(err);
            }
        }

        public override string ToString()
        {
            return Tag;
        }
    }
}
