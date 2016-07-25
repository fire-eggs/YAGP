// Top-level note record "0 @R1@ NOTE"

// TODO refactor common logic in ctor

using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class NoteRecord : GEDCommon
    {
        public static string Tag = "NOTE";

        // Submitter text
        public string Text { get; set; }

        private List<SourceCit> _cits;
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); }}

        public NoteRecord(GedRecord lines, string ident, string remain)
        {
            Text = remain;

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
    }
}
