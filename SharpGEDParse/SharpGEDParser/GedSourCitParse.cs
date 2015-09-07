using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    // Parse Source Citation (as opposed to Source Records)
    public class GedSourCitParse : GedRecParse
    {
        private static GedSourCit _rec;
        private delegate void SourTagProc();
        private readonly Dictionary<string, SourTagProc> _tagSet = new Dictionary<string, SourTagProc>();

        protected override void BuildTagSet()
        {
            _tagSet.Add("PAGE", pageProc);
            _tagSet.Add("EVEN", evenProc);
            _tagSet.Add("ROLE", roleProc);
            _tagSet.Add("DATA", dataProc);
            _tagSet.Add("OBJE", ignoreProc);
            _tagSet.Add("NOTE", noteProc);
            _tagSet.Add("QUAY", quayProc);

            _tagSet.Add("_RIN", rinProc); // Non-standard
        }

        private void UnknownTag(string tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            _rec.Unknowns.Add(rec);
        }

        protected override void ParseSubRec(KBRGedRec rec, int startLineDex, int maxLineDex)
        {
            string line = rec.Lines.GetLine(startLineDex);
            string ident = "";
            string tag = "";

            int nextChar = KBRGedUtil.IdentAndTag(line, 1, ref ident, ref tag); //HACK assuming no leading spaces
            if (_tagSet.ContainsKey(tag))
            {
                // TODO does this make parsing effectively single-threaded? need one context per thread?
                _context.Line = line;
                _context.Max = line.Length;
                _context.Tag = tag;
                _context.Begline = startLineDex;
                _context.Endline = maxLineDex;
                _context.Nextchar = nextChar;
                _rec = rec as GedSourCit;

                _tagSet[tag]();
            }
            else
            {
                UnknownTag(tag, startLineDex, maxLineDex);
            }
        }

        private string Remainder()
        {
            return _context.Line.Substring(_context.Nextchar).Trim();
        }

        private void noteProc()
        {
            throw new NotImplementedException();
        }

        private void quayProc()
        {
            _rec.Quay = Remainder();
        }

        private void ignoreProc()
        {
            // TODO do nothing?
        }

        private void pageProc()
        {
            _rec.Page = Remainder();
        }

        private void roleProc()
        {
            _rec.Role = Remainder();
        }

        private void evenProc()
        {
            _rec.Event = Remainder();
        }
        private void rinProc()
        {
            _rec.RIN = Remainder();
        }

        private void dataProc()
        {
            // brute-force processing for a DATA sub-record.
            // The DATA line itself should have no relevance.
            // Valid sub-tags are DATE and TEXT; TEXT may have CONC/CONT
        }
    }
}
