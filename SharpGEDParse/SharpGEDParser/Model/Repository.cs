
/*
0 @REPO24@ REPO
0 @REPO25@ REPO
1 NAME County Clerk's Office, Greene County, NY
1 ADDR Hudson, NY

0 @R29@ REPO
1 NAME Superintendent Registrar (York)
1 ADDR York Register Office
2 CONT 56 Bootham
2 CONT York,,  YO30 7DA
2 CONT England (UK)
2 ADR1 York Register Office
2 ADR2 56 Bootham
2 CITY York,
2 POST YO30 7DA
2 CTRY England (UK)

*/

using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Repository : GEDCommon, NoteHold
    {
        public static string Tag = "REPO";

        public string Name { get; set; }

        public Address Addr { get; set; }

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        public Repository(GedRecord lines, string ident)
        {
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
