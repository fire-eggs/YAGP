using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class GedSourParse : GedRecParse
    {
        private static GedSource _rec;

        private delegate void SourTagProc(int begLine, int endLine, int nextChar);

        private readonly Dictionary<string, SourTagProc> _tagSet = new Dictionary<string, SourTagProc>();

        protected override void BuildTagSet()
        {
            _tagSet.Add("AUTH", authProc);
            _tagSet.Add("TITL", titlProc);
            _tagSet.Add("PAGE", pageProc);
            _tagSet.Add("DATA", ignoreProc);
            _tagSet.Add("QUAY", quayProc);
            _tagSet.Add("DATE", dateProc);
            _tagSet.Add("TEXT", textProc);
            _tagSet.Add("NOTE", noteProc);
            _tagSet.Add("CHAN", chngProc);
            _tagSet.Add("_RIN", rinProc);
        }

        private void rinProc(int begline, int endline, int nextchar)
        {
            _rec.RIN = _context.Line.Substring(nextchar).Trim();
        }

        private void chngProc(int begline, int endline, int nextchar)
        {
            // GEDCOM spec says only one change allowed; says to take the FIRST one
            if (_rec.Change == null)
                _rec.Change = new Tuple<int, int>(begline, endline);
            else
            {
                _rec.Errors.Add(ErrorRec("More than one change record"));
            }
        }

        private void noteProc(int begline, int endline, int nextchar)
        {
            throw new NotImplementedException();
        }

        private void textProc(int begline, int endline, int nextchar)
        {
            _rec.Text = _context.Line.Substring(nextchar);
            // TODO CONC/CONT
        }

        private void dateProc(int begline, int endline, int nextchar)
        {
            _rec.Date = _context.Line.Substring(nextchar).Trim();
        }

        private void quayProc(int begline, int endline, int nextchar)
        {
            _rec.Quay = _context.Line.Substring(nextchar).Trim();
        }

        private void ignoreProc(int begline, int endline, int nextchar)
        {
            // TODO do nothing?
        }

        private void pageProc(int begline, int endline, int nextchar)
        {
            _rec.Page = _context.Line.Substring(nextchar).Trim();
        }

        private void titlProc(int begline, int endline, int nextchar)
        {
            throw new NotImplementedException();
        }

        private void authProc(int begline, int endline, int nextchar)
        {
            throw new NotImplementedException();
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
                _rec = rec as GedSource;

                _tagSet[tag](startLineDex, maxLineDex, nextChar);
            }
            else
            {
                UnknownTag(rec, tag, startLineDex, maxLineDex);
            }
        }

        private void UnknownTag(KBRGedRec mRec, string _tag, int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(_tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            (mRec as GedSource).Unknowns.Add(rec); // TODO general property?
        }

    }
}
