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
            GEDSplitter gs = new GEDSplitter();

            ctx.Lines = Lines;
            ctx.Parent = rec;
            int max = Lines.Max;

            for (int i = 1; i < max; i++)
            {
                var line = Lines.GetLine(i);
                ctx.Begline = i;
                ctx.Endline = i; // assume it is one line long, parser might change it

                gs.LevelTagAndRemain(line, ctx);
                //LineUtil.LevelTagAndRemain(ctx, line); //, ref ctx.Level, ref ident, ref ctx.Tag, ref ctx.Remain);
                TagProc2 tagProc;
                if (ctx.Tag != null && _tagSet2.TryGetValue(ctx.Tag, out tagProc))
                    //_tagSet2.ContainsKey(ctx.Tag))
                {
                    tagProc(ctx);
                    //_tagSet2[ctx.Tag](ctx);
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
            gs = null;
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

        protected void DataProc(ParseContext2 ctx)
        {
            // Common processing for UID, RFN, AFN
            if (ctx.Parent.Ids.HasId(ctx.Tag))
            {
                var rec = new UnkRec(ctx.Tag, ctx.Begline + ctx.Lines.Beg, ctx.Endline + ctx.Lines.Beg);
                rec.Error = UnkRec.ErrorCode.MultId; // reason;
                ctx.Parent.Errors.Add(rec);

                //ErrorRec(ctx, string.Format("Multiple {0} encountered; keeping only the first", ctx.Tag));
                // TODO what to do: we're throwing away data!
                return;
            }
            var sp = new StringPlus();
            sp.Value = ctx.Remain;
            LookAhead(ctx);
            sp.Extra.Beg = ctx.Begline + 1;
            sp.Extra.End = ctx.Endline;
            ctx.Parent.Ids.Add(ctx.Tag, sp);
        }

        protected void RefnProc(ParseContext2 ctx)
        {
            var sp = new StringPlus();
            sp.Value = ctx.Remain;
            LookAhead(ctx);
            sp.Extra.Beg = ctx.Begline + 1;
            sp.Extra.End = ctx.Endline;
            ctx.Parent.Ids.REFNs.Add(sp);
        }

        public abstract void PostCheck(GEDCommon rec);

        //private static LineUtil.LineData LevelTagAndRemain(LineUtil.LineData data, string line)
        //{
        //    int max = line.Length;

        //    // Move past level
        //    int dex = LineUtil.FirstChar(line, 0, max);
        //    data.Level = line[dex];
        //    data.Tag = ""; // in case of error
        //    data.Remain = ""; // in case of error
        //    dex = LineUtil.AllCharsUntil(line, max, dex+1, ' ');

        //    dex = LineUtil.FirstChar(line, dex, max);
        //    // TODO assuming no ident here!
        //    int endTag = LineUtil.AllCharsUntil(line, max, dex + 1, ' ');
        //    data.Tag = line.Substring(dex, endTag - dex);
            
        //    if (endTag < max)
        //        data.Remain = line.Substring(endTag + 1);
        //    else
        //        data.Remain = "";
        //    return data;
        //}
        
        // Handle a sub-tag with possible CONC / CONT sub-sub-tags.
        public static string extendedText(ParseContextCommon ctx)
        {
            GEDSplitter eTextSplit = new GEDSplitter();
            LineUtil.LineData eTextLd = new LineUtil.LineData();

            StringBuilder txt = new StringBuilder(ctx.Remain.TrimStart(),1024);
            int i = ctx.Begline + 1;
            int max = ctx.Lines.Max;
            for (; i < max; i++)
            {
                eTextSplit.LevelTagAndRemain(ctx.Lines.GetLine(i), eTextLd);
                //LevelTagAndRemain(eTextLd, ctx.Lines.GetLine(i));
                if (eTextLd.Level <= ctx.Level)
                    break; // end of sub-record
                if (eTextLd.Tag == "CONC")
                {
                    txt.Append(eTextLd.Remain); // must keep trailing space
                }
                else if (eTextLd.Tag == "CONT")
                {
                    txt.Append("\n"); // NOTE: not appendline, which is \r\n
                    txt.Append(eTextLd.Remain); // must keep trailing space
                }
                else
                    break; // non-CONC, non-CONT: stop!
            }
            ctx.Endline = i - 1;
            eTextLd = null;
            eTextSplit = null;
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
                    err.Error = UnkRec.ErrorCode.InvRestrict;
                    //err.Error = "Non-standard Restriction value";
                    err.Beg = err.End = rec.BegLine;
                    rec.Errors.Add(err);
                    break;
            }
        }

        public static void NonStandardRemain(string remain, GEDCommon rec)
        {
            // Extra text on the record line (e.g. "0 @R1@ REPO blah blah blah") is not standard for
            // most record types. Preserve it as a note if possible.
            if (!string.IsNullOrWhiteSpace(remain))
            {
                UnkRec err = new UnkRec();
                err.Beg = err.End = rec.BegLine;
                err.Error = UnkRec.ErrorCode.InvExtra; // TODO string.Format("Non-standard extra text: '{0}'", remain);
                rec.Errors.Add(err);

                if (rec is NoteHold)
                {
                    Note not = new Note();
                    not.Text = remain;
                    (rec as NoteHold).Notes.Add(not);
                }
            }

        }

        protected static string parseForXref(ParseContext2 context, UnkRec.ErrorCode errVal = UnkRec.ErrorCode.MissIdent)
        {
            string xref;
            string extra;
            StructParser.parseXrefExtra(context.Remain, out xref, out extra);
            if (string.IsNullOrEmpty(xref))
            {
                UnkRec err = new UnkRec();
                err.Error = errVal;
                err.Beg = err.End = context.Begline + context.Parent.BegLine;
                context.Parent.Errors.Add(err);
            }
            return xref;
        }
    }
}
