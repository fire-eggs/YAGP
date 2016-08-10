using System;
using System.Collections.Generic;
using System.Text;
using SharpGEDParser.Model;
using SharpGEDParser.Parser;

namespace SharpGEDParser
{
    public abstract class GedRecParse : GedParse
    {
        protected KBRGedRec _rec;

        protected delegate void TagProc();

        protected readonly Dictionary<string, TagProc> _tagSet = new Dictionary<string, TagProc>();

        protected delegate void TagProc2(ParseContext2 context);
        protected readonly Dictionary<string, TagProc2> _tagSet2 = new Dictionary<string, TagProc2>();

        public class ParseContext
        {
            public string Line;
            public int Max; // length of Line
            public string Tag;
            public int Begline;
            public int Endline;
            public int Nextchar;
        }

        public class ParseContext2
        {
            public GedRecord Lines;
            public GEDCommon Parent;
            public string Remain;
            public char Level;
            public int Begline; // index of first line for this 'record'
            public int Endline; // index of last line FOUND for this 'record'
        }

        public GedRecParse()
        {
            BuildTagSet();
            ctx = new ParseContext();
        }

        protected abstract void BuildTagSet();

        // TODO does this make parsing effectively single-threaded? need one context per thread?
        internal ParseContext ctx;

        // Common parsing logic for all record types
        public void Parse(KBRGedRec rec)
        {
            _rec = rec;

            // At this point we know the record 'type' and its ident.
            // TODO any trailing data after the keyword?

            var Lines = rec.Lines;

            int sublinedex;
            int linedex = 1;
            while (Lines.GetLevel(linedex, out sublinedex) != '1' && linedex <= Lines.Max)
                linedex++;
            if (linedex > Lines.Max)
                return;

            while (true)
            {
                int startrec = linedex;
                int startSubLine = sublinedex;
                linedex++;
                if (linedex > Lines.Max)
                    break;
                while (Lines.GetLevel(linedex, out sublinedex) > '1')
                    linedex++;
                ParseSubRec(rec, startrec, linedex - 1, startSubLine);
                if (linedex >= Lines.Max)
                    break;
            }
        }

        public virtual KBRGedRec Parse0(KBRGedRec rec, ParseContext context)
        {
            throw new NotImplementedException();
        }

        public void Parse(KBRGedRec rec, ParseContext context)
        {
            _rec = rec;
            var Lines = rec.Lines;

            int linedex = context.Begline+1;
            if (linedex > context.Endline) // TODO empty record error?
                return;

            // TODO context is blown after first ParseSubRec call
            int maxLinedex = context.Endline;

            int sublinedex;
            char startLevel = Lines.GetLevel(linedex, out sublinedex);
            if (startLevel < '0' || startLevel > '9')
            {
                var err = new UnkRec();
                err.Beg = linedex;
                err.End = linedex;
                err.Error = "Invalid or missing level; record processing stopped";
                rec.Errors.Add(err);
                return;
            }

            while (true)
            {
                if (linedex > maxLinedex)
                    break;
                int startrec = linedex;
                int startsubdex = sublinedex;
                while (Lines.GetLevel(linedex+1, out sublinedex) > startLevel && linedex+1 <= maxLinedex)
                    linedex++;
                ParseSubRec(rec, startrec, linedex, startsubdex);
                linedex++;
            }
        }

        public void Parse(GEDCommon rec, GedRecord Lines)
        {
            ParseContext2 ctx = new ParseContext2();
            ctx.Lines = Lines;
            ctx.Parent = rec;

            for (int i = 1; i < Lines.Max; i++)
            {
                string line = Lines.GetLine(i);
                string tag = null;
                string ident = null;
                ctx.Begline = i;
                ctx.Endline = i; // assume it is one line long, parser might change it
                GedLineUtil.LevelTagAndRemain(line, ref ctx.Level, ref ident, ref tag, ref ctx.Remain);
                if (tag != null && _tagSet2.ContainsKey(tag))
                {
                    _tagSet2[tag](ctx);
                }
                else
                {
                    // Custom and invalid treated as 'unknowns': let the consumer figure it out
                    // TODO gedr5419_blood_type_events.ged has garbage characters in SOUR/ABBR tags: incorrect line terminator, blank lines etc.
                    LookAhead(ctx);
                    rec.Unknowns.Add(new UnkRec(tag, Lines.Beg + ctx.Begline, Lines.Beg + ctx.Endline));
                }
                i = ctx.Endline;
            }

            // TODO post parse error checking on sub-structures
            PostCheck(ctx.Parent); // post parse error checking
        }

        // Find the end of this 'record'.
        public static void LookAhead(ParseContext2 ctx)
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

        protected KBRGedEvent CommonEventProcessing(GedRecord lines)
        {
            var eRec = KBRGedParser.EventParser.Parse0(_rec, ctx);
            return eRec as KBRGedEvent;
        }

        protected void NoteProc()
        {
            // Common note processing
            // TODO I cannot remember if it is important to store the original NOTE context or not
            var txt = extendedText();
            _rec.Notes.Add(txt);
        }

        protected UnkRec ErrorRec(string reason)
        {
            var rec = new UnkRec(ctx.Tag, ctx.Begline, ctx.Endline);
            rec.Error = reason;
            _rec.Errors.Add(rec);
            return rec;
        }

        // Common Source Citation processing
        protected void SourCitProc(KBRGedRec _rec)
        {
            var scRec = KBRGedParser.SourceCitParseSingleton.Parse0(_rec, ctx);
            if (scRec != null)
                _rec.Sources.Add(scRec as GedSourCit);
        }

        protected void UnknownTag(string tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(tag, startLineDex, maxLineDex);
            _rec.Unknowns.Add(rec);

            Console.WriteLine("Uknown:{0}[{1}:{2}]", tag, startLineDex, maxLineDex);
        }

        protected void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex, int startSubDex)
        {
            string line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string tag = "";

            int nextChar = GedLineUtil.IdentAndTag(line, startSubDex+1, ref ident, ref tag);
            if (_tagSet.ContainsKey(tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                ctx.Line = line;
                ctx.Max = line.Length;
                ctx.Tag = tag;
                ctx.Begline = startLineDex;
                ctx.Endline = maxLineDex;
                ctx.Nextchar = nextChar;
                _rec = rec;

                _tagSet[tag]();
            }
            else
            {
                int start = rec.Lines.Beg + startLineDex;
                int end = rec.Lines.Beg + maxLineDex;
                UnknownTag(tag, start, end);
            }
        }

        protected string Remainder()
        {
            return ctx.Line.Substring(ctx.Nextchar).Trim();
        }

        // Handle a sub-tag with possible CONC / CONT sub-sub-tags.
        protected string extendedText()
        {
            StringBuilder txt = new StringBuilder(ctx.Line.Substring(ctx.Nextchar).TrimStart());

            // NOTE: do NOT trim the end... trailing spaces are significant
            if (ctx.Endline > ctx.Begline)
            {
                for (int i = ctx.Begline + 1; i <= ctx.Endline; i++)
                {
                    string line = _rec.Lines.GetLine(i);
                    string ident = null;
                    string tag = null;

                    // TODO should be no ident! but should be allowed... see Tamura Jones
                    int nextChar = GedLineUtil.LevelIdentAndTag(line, ref ident, ref tag);
                    if (tag == "CONC")
                    {
                        if (line.Length > nextChar) // encountered empty CONC in real GED
                            txt.Append(line.Substring(nextChar + 1)); // must keep trailing space
                    }
                    else if (tag == "CONT")
                    {
                        txt.Append("\n"); // NOTE: not appendline, which is \r\n
                        if (line.Length > nextChar)
                            txt.Append(line.Substring(nextChar + 1)); // must keep trailing space
                    }
                    else
                        break; // non-CONC, non-CONT: stop!
                }
            }
            return txt.ToString();
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

        protected void RefnProc(ParseContext2 ctx)
        {
            // Common REFN processing
            var sp = new StringPlus();
            sp.Value = ctx.Remain;
            LookAhead(ctx);
            sp.Extra.Beg = ctx.Begline + 1;
            sp.Extra.End = ctx.Endline;

            ctx.Parent.Ids.REFNs.Add(sp);
        }

        public virtual void PostCheck(GEDCommon rec)
        {
            // post parse checking; each record parser should overload
        }

        // TODO copy-pasta from StructParser
        protected static string extendedText(ParseContext2 ctx)
        {
            StringBuilder txt = new StringBuilder(ctx.Remain.TrimStart());
            int i = ctx.Begline + 1;
            char level = ' ';
            string ident = null;
            string tag = null;
            string remain = null;
            for (; i < ctx.Lines.Max; i++)
            {
                GedLineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i), ref level, ref ident, ref tag, ref remain);
                if (level <= ctx.Level)
                    break; // end of sub-record
                if (tag == "CONC")
                {
                    txt.Append(remain); // must keep trailing space
                }
                else if (tag == "CONT")
                {
                    txt.Append("\n"); // NOTE: not appendline, which is \r\n
                    txt.Append(remain); // must keep trailing space
                }
                else
                    break; // non-CONC, non-CONT: stop!
            }
            ctx.Endline = i - 1;
            return txt.ToString();
        }

    }
}
