using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.Text;

// Common support / logic when parsing GED 'structures'. E.g. CHAN, NOTE-structure, Source-Citation, ADDR, etc

namespace SharpGEDParser.Parser
{
    public class StructParser
    {
        protected delegate void TagProc(StructParseContext context, int linedex, char level);

        protected static void StructParse(StructParseContext ctx, Dictionary<string, TagProc> tagSet)
        {
            LineUtil.LineData ld = new LineUtil.LineData();
            //GEDSplitter gs = new GEDSplitter();

            int i = ctx.Begline + 1;
            int max = ctx.Lines.Max;

            for (; i < max; i++)
            {
                try
                {
                    ctx.gs.LevelTagAndRemain(ctx.Lines.GetLine(i), ld);
                    //LineUtil.LevelTagAndRemain(ld, ctx.Lines.GetLine(i));
                }
                catch (Exception)
                {
                    UnkRec exc = new UnkRec();
                    exc.Beg = exc.End = i;
                    exc.Error = UnkRec.ErrorCode.Exception;
                    // TODO exc.Error = "Exception during parse, skipping line";
                    ctx.Parent.Errors.Add(exc);
                    continue;
                }

                if (ld.Level <= ctx.Level)
                    break; // end of sub-record
                ctx.Remain = ld.Remain;

                if (ld.Tag == null)
                {
                    UnkRec exc = new UnkRec();
                    exc.Beg = exc.End = i;
                    exc.Error = UnkRec.ErrorCode.Exception;
                    // TODO exc.Error = "Exception during parse, skipping line";
                    // TODO not exception - missing tag / invalid linebreak
                    ctx.Parent.Errors.Add(exc);
                    continue;
                }

                TagProc tagproc;
                if (tagSet.TryGetValue(ld.Tag, out tagproc))
                {
                    ctx.Begline = i;
                    ctx.Tag = ld.Tag;
                    tagproc(ctx, i, ld.Level);
                }
                else
                {
                    LineSet extra = new LineSet();
                    char oldLevel = ctx.Level;
                    ctx.Begline = i;
                    ctx.Level = ld.Level;
                    GedRecParse.LookAhead(ctx);
                    extra.Beg = ctx.Begline;
                    extra.End = ctx.Endline;
                    ctx.Parent.OtherLines.Add(extra);
                    ctx.Level = oldLevel;
                }
                i = Math.Max(ctx.Endline,i);
            }
            ctx.Endline = i - 1;
            ld = null;
            //gs = null;
        }

        // Common Note sub-structure parsing
        protected static void noteProc(StructParseContext ctx, int linedex, char level)
        {
            NoteHold dad = (ctx.Parent as NoteHold);
            var note = NoteStructParse.NoteParser(ctx, linedex, level);
            dad.Notes.Add(note);
        }

        protected static void addNote(NoteHold dad, string txt)
        {
            // save some unexpected text as a note
            Note note = new Note();
            note.Text = txt;
            dad.Notes.Add(note);
        }

        public static void parseXrefExtra(string txt, out string xref, out string extra)
        {
            // parse remainder text which ideally is of form "@R1@", but handle error cases:
            // "@R@1@"
            // "blah blah"
            // "@R1@ blah blah
            // TODO consider string.split() on '@' ?
            // Used by repo cit, sour cit
            // TODO xref is not permitted to start with '#'. Use of '!' and ':' are reserved?

            if (txt.Length < 1 || txt[0] != '@') // No xref specified
            {
                xref = null;
                extra = txt;
                return;
            }

            // find LAST instance of '@' sign
            int dex = txt.Length - 1;
            while (dex >= 0 && txt[dex] != '@')
                dex--;

            if (dex == 0) // TODO should this be treated as an unterminated xref?
            {
                xref = ""; // xref specified but empty?
                extra = txt;
                return;
            }

            xref = txt.Substring(1, dex - 1).Trim();
            extra = txt.Substring(dex + 1);
        }

        protected static void sourProc(StructParseContext context, int linedex, char level)
        {
            var cit = SourceCitParse.SourceCitParser(context, linedex, level);
            (context.Parent as SourceCitHold).Cits.Add(cit);
        }
    }
}
