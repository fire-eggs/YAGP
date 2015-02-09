using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace SharpGEDParser
{
    public class Rec
    {
        public int Beg { get; set; }

        public int End { get; set; }

        public string Tag { get; set; } // TODO enum?
    }

    public class UnkRec : Rec
    {
        public UnkRec(string tag)
        {
            Tag = tag;
        }
    }

    public class SourceRec : Rec
    {
        public string XRef { get; set; }

        public SourceRec(string xref)
        {
            Tag = "SOUR";
            XRef = xref;
        }
    }

    // NOTE: also used for birth (FAMC extra); ADOP (FAMC, ADOP extra)
    public class EventRec : Rec
    {
        public EventRec(string tag)
        {
            Tag = tag;
        }
    }

    public class NameRec : Rec
    {
        public string Names { get; set; }
        public string Surname { get; set; }
    }

    public class UserRef
    {
        
    }

    public class KBRGedIndi : KBRGedRec
    {
        // Restriction notice
        public string Restriction { get; set; }

        // Individual's sex
        public char Sex { get; set; }

        // Permanent record information
        public string PermanentRecord { get; set; }

        // Ancestral File Number
        public string AncestralFileNumber { get; set; }

        // Source records
        public List<SourceRec> Sources { get; set; }

        // Unknown records
        public List<UnkRec> Unknowns { get; set; }

        public List<EventRec> Events { get; set; }

        public List<NameRec> Names { get; set; }

        public List<UserRef> UserRefs { get; set; }

        public KBRGedIndi(GedRecord lines, string ident) : base(lines)
        {
            BuildTagSet();
            Ident = ident;
            Tag = "INDI"; // TODO use enum?

            Sources = new List<SourceRec>();
            Unknowns = new List<UnkRec>();
            Events = new List<EventRec>();
            Names = new List<NameRec>();
            UserRefs = new List<UserRef>();
        }

        public override void Parse()
        {
            // At this point we know it is an INDI record and its ident.
            // TODO any trailing data after 'INDI' keyword?

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
            Console.WriteLine("%%%%{2}({3}):{0}-{1}", startLineDex, maxLineDex, _tag, ident);

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
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }

        private delegate void IndiTagProc(int begLine, int endLine, int nextChar);

        private Dictionary<string, IndiTagProc> tagSet = new Dictionary<string, IndiTagProc>();

        // TODO *MUST* move to single instance parser, otherwise one instance per INDI record!!!
        private void BuildTagSet()
        {
            tagSet.Add("NAME", NameProc);
            tagSet.Add("RESN", ResnProc);
            tagSet.Add("SEX", SexProc);
            tagSet.Add("BAPM", EventProc); // simple event
            tagSet.Add("BIRT", BirtProc); // birth,adoption
            tagSet.Add("ADOP", BirtProc); // birth,adoption
            tagSet.Add("CAST", AttribProc);
            tagSet.Add("BAPL", LdsOrdProc);
            tagSet.Add("FAMC", ChildLink);
            tagSet.Add("FAMS", SpouseLink);
            tagSet.Add("SUBM", SubmProc);
            tagSet.Add("ASSO", AssocProc);
            tagSet.Add("ALIA", AliasProc);
            tagSet.Add("ANCI", AnciProc);
            tagSet.Add("DESI", DesiProc);
            tagSet.Add("RFN", PermRecProc);
            tagSet.Add("AFN", AfnProc);
            tagSet.Add("SOUR", SourceProc);
        }

        private void AfnProc(int begline, int endline, int nextchar)
        {
            AncestralFileNumber = _line.Substring(nextchar);
        }

        private void BirtProc(int begline, int endline, int nextchar)
        {
            var rec = new EventRec(_tag);
            rec.Beg = begline;
            rec.End = endline;
            Events.Add(rec);

            // TODO parse event details
            // TODO parse birt, adop specific
        }

        private void PermRecProc(int begline, int endline, int nextchar)
        {
            PermanentRecord = _line.Substring(nextchar);
        }

        private void DesiProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void AnciProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void AliasProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void AssocProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void SubmProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void SpouseLink(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void ChildLink(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void LdsOrdProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void AttribProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void EventProc(int begline, int endline, int nextchar)
        {
            var rec = new EventRec(_tag);
            rec.Beg = begline;
            rec.End = endline;
            Events.Add(rec);

            // TODO parse event details
        }

        private void SexProc(int begline, int endline, int nextchar)
        {
            string line = Lines.GetLine(begline);
            Sex = line[nextchar];
        }

        private void ResnProc(int begline, int endline, int nextchar)
        {
            Restriction = _line.Substring(nextchar);
        }

        private void NameProc(int begline, int endline, int nextchar)
        {
            int max = _line.Length;
            int startName = KBRGedUtil.FirstChar(_line, nextchar, max);

            int startSur = KBRGedUtil.AllCharsUntil(_line, max, startName, '/');
            int endSur = KBRGedUtil.AllCharsUntil(_line, max, startSur+1, '/');

            var rec = new NameRec();
            rec.Beg = begline;
            rec.End = endline;
            rec.Names = _line.Substring(startName, startSur-startName).Trim();
            rec.Surname = _line.Substring(startSur+1, endSur-startSur-1);
            Names.Add(rec);

            // TODO parse more details
        }

        private void SourceProc(int begline, int endline, int nextchar)
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
        }
    }
}

/*
 * RESN
 * NAME
 * SEX
 * BIRT/CHR/DEAT/BURI/CREM/ADOP/....
 * CAST/DSCR/EDUC/IDNO/NATI/...
 * -
 * FAMC
 * FAMS 
 * SUBM
 * ASSO
 * ALIA
 * ANCI
 * DESI
 * RFN
 * AFN
 * REFN
 * RIN
 * CHAN
 * -
 * -
 * -
*/