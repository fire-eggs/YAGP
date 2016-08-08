using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class SourceRecord : GEDCommon, MediaHold, NoteHold
    {
        public static string Tag = "SOUR";

        public string Author { get; set; }

        public string Title { get; set; }

        public string Abbreviation { get; set; }

        public string Publication { get; set; }

        public string Text { get; set; }

        public SourceData Data { get; set; }

        private List<RepoCit> _cits;
        public List<RepoCit> Cits { get { return _cits ?? (_cits = new List<RepoCit>()); } }

        private List<MediaLink> _media;
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        public SourceRecord(GedRecord lines, string ident, string remain)
            : base(lines, ident)
        {
            if (!string.IsNullOrWhiteSpace(remain)) // TODO save as a NOTE
            {
                UnkRec err = new UnkRec();
                err.Beg = err.End = BegLine;
                err.Error = string.Format("Non-standard extra text with tag: '{0}'", remain);
                Errors.Add(err);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):[{2}:{3}]", Tag, Ident, BegLine, EndLine);
        }

    }
}
