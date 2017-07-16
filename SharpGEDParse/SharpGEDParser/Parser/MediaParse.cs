using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // Parsing for Media Record [not Media link]
    // NOTE: the use of the OBJE *record* is extremely rare. Few of the various
    // programs actually export OBJE records. As a result, I'm not attempting
    // to fully parse OBJE tags.

    // The 5.5 and 5.5.1 standards are a bit different.
    // 5.5: no FILE tag, make sure a MediaFile exists to assign FORM/TITL to
    // 5.5.1: multiple FILEs
    // 5.5.1: no BLOB tag
    // 5.5: no SOUR tags
    // 5.5: no TYPE tag
    // 5.5: OBJE chaining - treating as unknown
    // 5.5: BLOB tag - treating as unknown

    // NOTE: no validation of the media FORMAT or TYPE: the standard lists only a very small subset of possible values

    public class MediaParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            _tagSet2.Add("REFN", RefnProc);
            _tagSet2.Add("RIN",  RinProc);
            _tagSet2.Add("CHAN", ChanProc);
            _tagSet2.Add("NOTE", NoteProc);
            _tagSet2.Add("SOUR", SourCitProc); // GEDCOM 5.5.1
            _tagSet2.Add("FILE", fileProc); // GEDCOM 5.5.1
            _tagSet2.Add("FORM", formProc);
            _tagSet2.Add("TITL", titlProc);
            _tagSet2.Add("TYPE", typeProc); // GEDCOM 5.5.1
            //_tagSet2.Add("BLOB", blobProc); // GEDCOM 5.5 - intentionally treated as unknown
            //_tagSet2.Add("OBJE", objeProc); // GEDCOM 5.5 - intentionally treated as unknown

            _tagSet2.Add("_UID", DataProc);
            _tagSet2.Add("UID", DataProc);
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

        public override void PostCheck(GEDCommon rec)
        {
            MediaRecord me = rec as MediaRecord;

            if (string.IsNullOrWhiteSpace(me.Ident))
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissIdent; // TODO "Missing identifier"; // TODO assign one?
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // A FILE record is required
            if (me.Files.Count < 1)
            {
                UnkRec err = new UnkRec();
                err.Error = UnkRec.ErrorCode.MissFile; // "Missing FILE";
                err.Beg = err.End = me.BegLine;
                me.Errors.Add(err);
            }

            // Each FILE record must have a FORM
            foreach (var mediaFile in me.Files)
            {
                if (string.IsNullOrWhiteSpace(mediaFile.Form))
                {
                    UnkRec err = new UnkRec();
                    err.Error = UnkRec.ErrorCode.MissForm; // "Missing FORM";
                    err.Beg = err.End = me.BegLine;
                    me.Errors.Add(err);
                }
            }
        }
    }
}
