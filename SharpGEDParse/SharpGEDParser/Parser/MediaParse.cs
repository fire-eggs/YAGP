using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parsing for Media Record [not Media link]
    // The 5.5 and 5.5.1 standards are a bit different.
    // 5.5: no FILE tag, make sure a MediaFile exists to assign FORM/TITL to
    // 5.5.1: multiple FILEs
    // 5.5.1: no BLOB tag
    // 5.5: possibly no SOUR tags TODO: confirm
    // 5.5: no TYPE tag
    // 5.5.1: OBJE chaining TODO punt?

    public class MediaParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN", RinProc);
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("SOUR", sourCitProc); // GEDCOM 5.5.1 ?
            _tagSet2.Add("FILE", fileProc); // GEDCOM 5.5.1
            _tagSet2.Add("FORM", formProc);
            _tagSet2.Add("TITL", titlProc);
            _tagSet2.Add("TYPE", typeProc); // GEDCOM 5.5.1
            _tagSet2.Add("BLOB", blobProc); // GEDCOM 5.5
            //_tagSet2.Add("OBJE", objeProc); // GEDCOM 5.5
        }

        private void blobProc(ParseContext2 context)
        {
            UnkRec foo = new UnkRec();
            LookAhead(context);
            foo.Beg = context.Begline;
            foo.End = context.Endline;
            (context.Parent as MediaRecord).Blob = foo;
        }

        private MediaFile GetFile(ParseContext2 context)
        {
            // 5.5: no FILE tag, make sure there is a MediaFile created
            var files = (context.Parent as MediaRecord).Files;
            if (files.Count == 0)
            {
                files.Add(new MediaFile());
            }
            MediaFile file = files[files.Count - 1];
            return file;
        }

        private void typeProc(ParseContext2 context)
        {
            // TODO 5.5: needs a warning
            var file = GetFile(context);
            file.Type = context.Remain;
        }

        private void titlProc(ParseContext2 context)
        {
            var file = GetFile(context);
            file.Title = context.Remain;
        }

        private void formProc(ParseContext2 context)
        {
            var file = GetFile(context);
            file.Form = context.Remain;
        }

        private void fileProc(ParseContext2 context)
        {
            MediaFile file = new MediaFile();
            file.FileRefn = context.Remain;
            (context.Parent as MediaRecord).Files.Add(file);
        }

        private void NoteProc(ParseContext2 ctx)
        {
            var note = NoteStructParse.NoteParser(ctx);
            (ctx.Parent as MediaRecord).Notes.Add(note);
        }

    }
}
