using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class MediaRecord : GEDCommon
    {
        public static string Tag = "OBJE";

        private NoteHold _notes = new NoteHold();

        public List<Note> Notes { get { return _notes.Notes; } }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }

        public List<MediaFile> _files;

        public UnkRec Blob { get; set; } // GEDCOM 5.5 standard

        public List<MediaFile> Files { get { return _files ?? (_files = new List<MediaFile>()); } }

        public MediaRecord(GedRecord lines, string ident, string remain)
        {
            // Text = remain; // TODO what to do with extra?

            BegLine = lines.Beg;
            EndLine = lines.End;
            Ident = ident;

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
