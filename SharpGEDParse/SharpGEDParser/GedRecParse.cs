using System.Collections.Generic;
using System.Text;
using SharpGEDParser.Model;
using SharpGEDParser.Parser;

namespace SharpGEDParser
{
    public abstract class GedRecParse : GedParse
    {
        protected delegate void TagProc2(ParseContext2 context);
        protected readonly Dictionary<string, TagProc2> _tagSet2 = new Dictionary<string, TagProc2>();

        protected GedRecParse()
        {
            BuildTagSet();
        }

        protected abstract void BuildTagSet();

        public void Parse(GEDCommon rec, GedRecord Lines)
        {
            ParseContext2 ctx = new ParseContext2();
            ctx.Lines = Lines;
            ctx.Parent = rec;

            for (int i = 1; i < Lines.Max; i++)
            {
                string line = Lines.GetLine(i);
                ctx.Begline = i;
                ctx.Endline = i; // assume it is one line long, parser might change it

                LineUtil.LevelTagAndRemain(ctx, line); //, ref ctx.Level, ref ident, ref ctx.Tag, ref ctx.Remain);
                if (ctx.Tag != null && _tagSet2.ContainsKey(ctx.Tag))
                {
                    _tagSet2[ctx.Tag](ctx);
                }
                else
                {
                    // Custom and invalid treated as 'unknowns': let the consumer figure it out
                    // TODO gedr5419_blood_type_events.ged has garbage characters in SOUR/ABBR tags: incorrect line terminator, blank lines etc.
                    LookAhead(ctx);
                    rec.Unknowns.Add(new UnkRec(ctx.Tag, Lines.Beg + ctx.Begline, Lines.Beg + ctx.Endline));
                }
                i = ctx.Endline;
            }

            // TODO post parse error checking on sub-structures
            PostCheck(ctx.Parent); // post parse error checking

            ctx = null;
        }

        // Find the end of this 'record'.
        public static void LookAhead(ParseContextCommon ctx)
        {
            if (ctx.Begline == ctx.Lines.LineCount)
            {
                ctx.Endline = ctx.Begline;
                return; // Nothing to do: already at last line
            }
            int linedex = ctx.Begline;
            int sublinedex;
            while (ctx.Lines.GetLevel(linedex + 1, out sublinedex) > ctx.Level && 
                   linedex + 1 <= ctx.Lines.LineCount)
                linedex++;
            ctx.Endline = linedex;
        }

        protected UnkRec ErrorRec(ParseContext2 ctx, string reason)
        {
            var rec = new UnkRec(ctx.Tag, ctx.Begline + ctx.Lines.Beg, ctx.Endline + ctx.Lines.Beg);
            rec.Error = reason;
            ctx.Parent.Errors.Add(rec);
            return rec;
        }

        protected void RinProc(ParseContext2 ctx)
        {
            // Common RIN processing
            ctx.Parent.RIN = ctx.Remain;
        }

        protected void ChanProc(ParseContext2 ctx)
        {
            // Common CHAN processing
            ChanStructParse.ChanProc(ctx);
        }

        protected void SourCitProc(ParseContext2 ctx)
        {
            // Common source citation processing
            var cit = SourceCitParse.SourceCitParser(ctx);
            (ctx.Parent as SourceCitHold).Cits.Add(cit);
        }

        protected void NoteProc(ParseContext2 ctx)
        {
            // Common note processing
            var note = NoteStructParse.NoteParser(ctx);
            (ctx.Parent as NoteHold).Notes.Add(note);
        }

        protected static void ObjeProc(ParseContext2 ctx)
        {
            MediaLink mlink = MediaStructParse.MediaParser(ctx);
            (ctx.Parent as MediaHold).Media.Add(mlink);
        }

        protected void DataProc(ParseContext2 ctx, bool multipleOK)
        {
            // Common processing for UID, RFN, AFN
            bool isRefn = ctx.Tag.Equals("REFN");
            if (!isRefn && !multipleOK && ctx.Parent.Ids.HasId(ctx.Tag))
            {
                ErrorRec(ctx, string.Format("Multiple {0} encountered; keeping only the first", ctx.Tag));
                // TODO what to do: we're throwing away data!
                return;
            }
            var sp = new StringPlus();
            sp.Value = ctx.Remain;
            LookAhead(ctx);
            sp.Extra.Beg = ctx.Begline + 1;
            sp.Extra.End = ctx.Endline;
            if (isRefn)
                ctx.Parent.Ids.REFNs.Add(sp);
            else
                ctx.Parent.Ids.Add(ctx.Tag, sp);
        }

        protected void RefnProc(ParseContext2 ctx)
        {
            DataProc(ctx, true);
        }

        public virtual void PostCheck(GEDCommon rec)
        {
            // post parse checking; each record parser should overload
        }

        // Handle a sub-tag with possible CONC / CONT sub-sub-tags.
        public static string extendedText(ParseContextCommon ctx)
        {
            StringBuilder txt = new StringBuilder(ctx.Remain.TrimStart());
            int i = ctx.Begline + 1;
            LineUtil.LineData ld = new LineUtil.LineData();
            for (; i < ctx.Lines.Max; i++)
            {
                LineUtil.LevelTagAndRemain(ld, ctx.Lines.GetLine(i));
                if (ld.Level <= ctx.Level)
                    break; // end of sub-record
                if (ld.Tag == "CONC")
                {
                    txt.Append(ld.Remain); // must keep trailing space
                }
                else if (ld.Tag == "CONT")
                {
                    txt.Append("\n"); // NOTE: not appendline, which is \r\n
                    txt.Append(ld.Remain); // must keep trailing space
                }
                else
                    break; // non-CONC, non-CONT: stop!
            }
            ctx.Endline = i - 1;
            return txt.ToString();
        }

        public void CheckRestriction(GEDCommon rec, string restrict)
        {
            // Common post-processing restriction checking
            if (string.IsNullOrWhiteSpace(restrict)) // nothing specified, nothing to do
                return;
            switch (restrict.ToLowerInvariant())
            {
                case "confidential":
                case "locked":
                case "privacy":
                    break;
                default:
                    UnkRec err = new UnkRec();
                    err.Error = "Non-standard Restriction value";
                    err.Beg = err.End = rec.BegLine;
                    rec.Errors.Add(err);
                    break;
            }
        }

    }
}
