using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser
{
    public class KBRGedFam : KBRGedRec
    {
        public KBRGedFam(GedRecord lines, string ident) : base(lines)
        {
            BuildTagSet();
            Ident = ident;
            Tag = "FAM";

            Sources = new List<SourceRec>();
            Childs = new List<string>();

            Unknowns = new List<UnkRec>();
            FamEvents = new List<EventRec>();
            Data = new List<DataRec>();
        }

        public List<UnkRec> Unknowns { get; set; } // TODO COMMON

        public List<string> Childs { get; set; }

        public List<SourceRec> Sources { get; set; } // TODO COMMON

        public List<DataRec> Data { get; set; } // TODO COMMON

        public string Dad { get; set; } // identity string for Father
        public string Mom { get; set; } // identity string for Mother

        public Tuple<int, int> Note { get; set; } // TODO COMMON
        public Tuple<int, int> Change { get; set; } // TODO COMMON

        public List<EventRec> FamEvents { get; set; } // TODO COMMON

        public override void Parse()
        {
            // At this point we know it is an FAM record and its ident.
            // TODO any trailing data after 'FAM' keyword?

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
                ParseSubRec(startrec, linedex - 1);
                if (linedex >= Lines.Max)
                    break;
            }

            // TODO all this can be refactored in common for other record types (FAM,INDI, etc), except the "parseSub" delegate invocation
        }

        private string _tag; // HACK
        private string _line; // HACK

        public void ParseSubRec(int startLineDex, int maxLineDex)
        {
            _line = Lines.GetLine(startLineDex);
            string ident = "";
            _tag = "";
            int nextChar = KBRGedUtil.IdentAndTag(_line, 1, ref ident, ref _tag); //HACK assuming no leading spaces
            //            Console.WriteLine("____{2}({3}):{0}-{1}", startLineDex, maxLineDex, _tag, ident);

            if (tagSet.ContainsKey(_tag))
            {
                tagSet[_tag](startLineDex, maxLineDex, nextChar);
            }
            else
            {
                UnknownTag(startLineDex, maxLineDex);
            }
        }

        private void UnknownTag(int startLineDex, int maxLineDex)
        {
            var rec = new UnkRec(_tag);
            rec.Beg = startLineDex;
            rec.End = maxLineDex;
            Unknowns.Add(rec);

//            Console.WriteLine("***" + rec);
        }

        private delegate void FamTagProc(int begLine, int endLine, int nextChar);

        private Dictionary<string, FamTagProc> tagSet = new Dictionary<string, FamTagProc>();

        private void BuildTagSet()
        {
            tagSet.Add("HUSB", dadProc);
            tagSet.Add("WIFE", momProc);
            tagSet.Add("CHIL", kidProc);

            tagSet.Add("NOTE", NoteProc);
            tagSet.Add("SOUR", SourProc);
            tagSet.Add("CHAN", ChanProc);

            tagSet.Add("_UID", DataProc);
            tagSet.Add("MARR", FamEventProc);
            tagSet.Add("DIV", FamEventProc);
            tagSet.Add("EVEN", FamEventProc);
            tagSet.Add("ENGA", FamEventProc);
            tagSet.Add("MARB", FamEventProc);
            tagSet.Add("MARC", FamEventProc);
        }

        private void SourProc(int begline, int endline, int nextchar)
        {
            // "1 SOUR @n@"
            // TODO "1 SOUR descr"

            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);

            var rec = new SourceRec(ident);
            rec.Beg = begline;
            rec.End = endline;
            Sources.Add(rec);

            // TODO parse more stuff
//            Console.WriteLine(rec);
        }

        private void NoteProc(int begline, int endline, int nextchar)
        {
            Note = new Tuple<int, int>(begline, endline);
        }

        private void kidProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            Childs.Add(ident);
        }

        private void momProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            Mom = ident;
        }

        private void dadProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            Dad = ident;
        }
        private void ChanProc(int begline, int endline, int nextchar)
        {
            Change = new Tuple<int, int>(begline, endline);
        }
        private void DataProc(int begline, int endline, int nextchar)
        {
            string data = _line.Substring(nextchar);
            var rec = new DataRec(_tag, data);
            rec.Beg = begline;
            rec.End = endline;
            Data.Add(rec);
        }

        private void FamEventProc(int begline, int endline, int nextchar)
        {
            // A family event: same as an event but has additional husband, wife tags
            var rec = new EventRec(_tag);
            rec.Beg = begline;
            rec.End = endline;
            FamEvents.Add(rec);

            // TODO parse event details
            rec.Date = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "DATE");
            rec.Place = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "PLAC");
            rec.Age = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "AGE");

            rec.Change = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "CHAN");
            rec.Note = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "NOTE");
            rec.Source = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "SOUR");

//            Console.WriteLine(rec);

            Debug.Assert(KBRGedUtil.ParseFor(Lines, begline, endline, "HUSB") == null);
            Debug.Assert(KBRGedUtil.ParseFor(Lines, begline, endline, "WIFE") == null);
        }

    }
}
