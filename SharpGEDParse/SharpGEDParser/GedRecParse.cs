
namespace SharpGEDParser
{
    public abstract class GedRecParse : GedParse
    {
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

        protected abstract void ParseSubRec(KBRGedRec rec, int start, int max);

        // TODO does this make parsing effectively single-threaded? need one context per thread?
        internal ParseContext _context;

        // Common parsing logic for all record types
        public void Parse(KBRGedRec rec)
        {
            // At this point we know the record 'type' and its ident.
            // TODO any trailing data after the keyword?

            var Lines = rec.Lines;

            int linedex = 1;
            while (Lines.GetLevel(linedex) != '1')
                linedex++;
            if (linedex > Lines.Max)
                return;

            while (true)
            {
                int startrec = linedex;
                linedex++;
                if (linedex > Lines.Max)
                    break;
                while (Lines.GetLevel(linedex) > '1')
                    linedex++;
                ParseSubRec(rec, startrec, linedex - 1);
                if (linedex >= Lines.Max)
                    break;
            }
        }

        public void Parse(KBRGedRec rec, ParseContext context)
        {
            var Lines = rec.Lines;

            int linedex = context.Begline+1;
            if (linedex > context.Endline)
                return;

            char startLevel = Lines.GetLevel(linedex);

            while (true)
            {
                if (linedex > context.Endline)
                    break;
                int startrec = linedex;
                while (Lines.GetLevel(linedex+1) > startLevel && linedex+1 <= context.Endline)
                    linedex++;
                ParseSubRec(rec, startrec, linedex);
                linedex++;
            }
        }

        private GedParse _EventParseSingleton;

        protected KBRGedEvent CommonEventProcessing(GedRecord lines)
        {
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
            return rec;
        }

        private GedParse _SourParseSingleton;

        // Common Source Citation processing
        protected void SourCitProc(KBRGedRec _rec)
        {
            // TODO somehow push into GedSourCitParse???

            // "1 SOUR @n@"
            // "1 SOUR text"
            // "1 SOUR text\n2 CONC text"

            string embed = null;
            string ident = null;
            int res = KBRGedUtil.Ident(_context.Line, _context.Max, _context.Nextchar, ref ident);
            if (res == -1 || string.IsNullOrWhiteSpace(ident))
            {
                embed = _context.Line.Substring(_context.Nextchar).Trim();
                // possibly error
                if (embed.Contains("@") || string.IsNullOrWhiteSpace(embed))
                {
                    _rec.Errors.Add(ErrorRec("identifier error"));
                    return;
                }

                // TODO possibly embedded text
                // TODO CONC/CONT lines
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
    }
}
