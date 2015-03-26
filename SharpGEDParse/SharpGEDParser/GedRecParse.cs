
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
        internal static ParseContext _context;

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

        protected EventRec CommonEventProcessing(GedRecord lines)
        {
            int begline = _context.Begline;
            int endline = _context.Endline;

            var rec = new EventRec(_context.Tag);
            rec.Beg = begline;
            rec.End = endline;
            rec.Detail = _context.Line.Substring(_context.Nextchar).Trim();

            rec.Age = KBRGedUtil.ParseFor(lines, begline + 1, endline, "AGE");
            rec.Date = KBRGedUtil.ParseFor(lines, begline + 1, endline, "DATE");
            rec.Type = KBRGedUtil.ParseFor(lines, begline + 1, endline, "TYPE");
            rec.Cause = KBRGedUtil.ParseFor(lines, begline + 1, endline, "CAUS");
            rec.Place = KBRGedUtil.ParseFor(lines, begline + 1, endline, "PLAC");
            rec.Agency = KBRGedUtil.ParseFor(lines, begline + 1, endline, "AGNC");
            rec.Religion = KBRGedUtil.ParseFor(lines, begline + 1, endline, "RELI");
            rec.Restriction = KBRGedUtil.ParseFor(lines, begline + 1, endline, "RESN");

            // TODO CHAN - only one allowed!
            rec.Change = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "CHAN");

            // TODO more than one note permitted!
            rec.Note = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "NOTE");

            // TODO more than one source permitted!
            rec.Source = KBRGedUtil.ParseForMulti(lines, begline + 1, endline, "SOUR");

            // TODO OBJE tag

            return rec;
        }
    }
}
