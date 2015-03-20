using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public abstract class GedRecParse : GedParse
    {
        public class ParseContext
        {
            public string Line;
            public string Tag;
            public int begline;
            public int endline;
            public int nextchar;
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
                if (linedex >= Lines.Max)
                    break;
                while (Lines.GetLevel(linedex) > '1')
                    linedex++;
                ParseSubRec(rec, startrec, linedex - 1);
                if (linedex >= Lines.Max)
                    break;
            }
        }
    }
}
