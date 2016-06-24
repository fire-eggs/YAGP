using System;
using System.Collections.Generic;
using System.Text;

namespace SharpGEDParser
{
    public abstract class GedRecParse : GedParse
    {
        protected KBRGedRec _rec;

        protected delegate void TagProc();

        protected readonly Dictionary<string, TagProc> _tagSet = new Dictionary<string, TagProc>();

        public class ParseContext
        {
            public string Line;
            public int Max; // length of Line
            public string Tag;
            public int Begline;
            public int Endline;
            public int Nextchar;
        }

        public GedRecParse()
        {
            BuildTagSet();
            _context = new ParseContext();
        }

        protected abstract void BuildTagSet();

        // TODO does this make parsing effectively single-threaded? need one context per thread?
        internal ParseContext _context;

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
                var err = new UnkRec("");
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

        protected KBRGedEvent CommonEventProcessing(GedRecord lines)
        {
            var eRec = KBRGedParser.EventParser.Parse0(_rec, _context);
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
            var rec = new UnkRec(_context.Tag);
            rec.Error = reason;
            rec.Beg = _context.Begline;
            rec.End = _context.Endline;
            _rec.Errors.Add(rec);
            return rec;
        }

        // Common Source Citation processing
        protected void SourCitProc(KBRGedRec _rec)
        {
            var scRec = KBRGedParser.SourceCitParseSingleton.Parse0(_rec, _context);
            if (scRec != null)
                _rec.Sources.Add(scRec as GedSourCit);
        }

        protected void CustomTag(string tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            _rec.Custom.Add(rec);
        }

        protected void UnknownTag(string tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            _rec.Unknowns.Add(rec);
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
                _context.Line = line;
                _context.Max = line.Length;
                _context.Tag = tag;
                _context.Begline = startLineDex;
                _context.Endline = maxLineDex;
                _context.Nextchar = nextChar;
                _rec = rec;

                _tagSet[tag]();
            }
            else
            {
                if (tag.StartsWith("_"))
                    CustomTag(tag, startLineDex, maxLineDex);
                else
                    UnknownTag(tag, startLineDex, maxLineDex);
            }
        }

        protected string Remainder()
        {
            return _context.Line.Substring(_context.Nextchar).Trim();
        }

        // Handle a sub-tag with possible CONC / CONT sub-sub-tags.
        protected string extendedText()
        {
            StringBuilder txt = new StringBuilder(_context.Line.Substring(_context.Nextchar).TrimStart());

            // NOTE: do NOT trim the end... trailing spaces are significant
            if (_context.Endline > _context.Begline)
            {
                for (int i = _context.Begline + 1; i <= _context.Endline; i++)
                {
                    string line = _rec.Lines.GetLine(i);
                    string ident = null;
                    string tag = null;

                    // TODO should be no ident! but should be allowed... see Tamura Jones
                    int nextChar = GedLineUtil.LevelIdentAndTag(line, ref ident, ref tag);
                    if (tag == "CONC")
                        txt.Append(line.Substring(nextChar + 1)); // must keep trailing space
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
    }
}
