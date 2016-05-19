
using System.Collections.Generic;

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

        public void Parse(KBRGedRec rec, ParseContext context)
        {
            _rec = rec;
            var Lines = rec.Lines;

            int linedex = context.Begline+1;
            if (linedex > context.Endline) // TODO empty record error?
                return;

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
                if (linedex > context.Endline)
                    break;
                int startrec = linedex;
                int startsubdex = sublinedex;
                while (Lines.GetLevel(linedex+1, out sublinedex) > startLevel && linedex+1 <= context.Endline)
                    linedex++;
                ParseSubRec(rec, startrec, linedex, startsubdex);
                linedex++;
            }
        }

        private GedParse _EventParseSingleton;

        protected KBRGedEvent CommonEventProcessing(GedRecord lines)
        {
            // TODO somehow push into GedEventParse

            var eRec = new KBRGedEvent(lines, _context.Tag);
            eRec.Detail = _context.Line.Substring(_context.Nextchar).Trim();
            if (_EventParseSingleton == null)
                _EventParseSingleton = new GedEventParse();
            _EventParseSingleton.Parse(eRec, _context);
            return eRec;
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

        private GedParse _SourParseSingleton;

        // Common Source Citation processing
        protected void SourCitProc(KBRGedRec _rec)
        {
            // TODO somehow push into GedSourCitParse

            // "1 SOUR @n@"
            // "1 SOUR text"
            // "1 SOUR text\n2 CONC text"

            string embed = null;
            string ident = null;
            int res = GedLineUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);
            if (res == -1 || string.IsNullOrWhiteSpace(ident))
            {
                embed = _context.Line.Substring(_context.Nextchar).Trim();
                // possibly error
                if (embed.Contains("@") || string.IsNullOrWhiteSpace(embed))
                {
                    ErrorRec("identifier error");
                    return;
                }

                // TODO possibly embedded text
                // TODO CONC/CONT lines with embedded text
            }

            GedSourCit sRec = new GedSourCit(_rec.Lines);
            sRec.Beg = _context.Begline;
            sRec.End = _context.Endline;
            sRec.XRef = ident;
            sRec.Embed = embed;
            _rec.Sources.Add(sRec);
            if (_SourParseSingleton == null)
                _SourParseSingleton = new GedSourCitParse();
            _SourParseSingleton.Parse(sRec, _context);
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
                UnknownTag(tag, startLineDex, maxLineDex);
            }
        }

        protected string Remainder()
        {
            return _context.Line.Substring(_context.Nextchar).Trim();
        }

    }
}
