using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

namespace SharpGEDParser
{
    public class Rec
    {
        public int Beg { get; set; }

        public int End { get; set; }

        public string Tag { get; set; } // TODO enum?

        public override string ToString()
        {
            return string.Format("{0}({1},{2})", Tag, Beg, End);
        }
    }

    public class UnkRec : Rec
    {
        public UnkRec(string tag)
        {
            Tag = tag;
        }
    }

    public class SourceRec : XRefRec
    {
        public SourceRec(string xref) : base ("SOUR", xref)
        {
        }
    }

    // Currently used for _UID
    public class DataRec : Rec
    {
        public string Data { get; set; }

        public DataRec(string tag, string data)
        {
            Tag = tag;
            Data = data;
        }
    }

    public class ChildLinkRec : XRefRec
    {
        public ChildLinkRec(string ident) : base("FAMC", ident)
        {
        }
    }

    public class FamLinkRec : XRefRec
    {
        public FamLinkRec(string ident) : base("FAMS", ident)
        {
        }
    }

    public class XRefRec : Rec
    {
        public string XRef { get; set; }
        public XRefRec(string tag, string xref)
        {
            Tag = tag;
            XRef = xref;
        }
        public override string ToString()
        {
            return string.Format("{0}[{3}]({1},{2})", Tag, Beg, End, XRef);
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
        public override string ToString()
        {
            return string.Format(" {0} /{1}/", Names, Surname);
        }
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

        public List<XRefRec> Alia { get; set; }
        public List<XRefRec> Anci { get; set; }
        public List<XRefRec> Desi { get; set; }
        public List<XRefRec> Subm { get; set; }

        public List<ChildLinkRec> ChildLinks { get; set; }

        public List<FamLinkRec> FamLinks { get; set; }

        public List<DataRec> Data { get; set; }

        public bool Living { get; set; }

        public KBRGedIndi(GedRecord lines, string ident) : base(lines)
        {
            BuildTagSet();
            Ident = ident;
            Tag = "INDI"; // TODO use enum?

            Sources = new List<SourceRec>();
            Unknowns = new List<UnkRec>();
            Events = new List<EventRec>();
            Names = new List<NameRec>();
            Data = new List<DataRec>();

            Alia = new List<XRefRec>();
            Anci = new List<XRefRec>();
            Desi = new List<XRefRec>();
            Subm = new List<XRefRec>();

            ChildLinks = new List<ChildLinkRec>();
            FamLinks = new List<FamLinkRec>();

            Living = false;
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

            Console.WriteLine("***" + rec);
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
            tagSet.Add("DEAT", EventProc); // simple event
            tagSet.Add("BURI", EventProc); // simple event
            tagSet.Add("CHR", EventProc); // simple event
            tagSet.Add("EVEN", EventProc); // simple event

            tagSet.Add("BIRT", BirtProc); // birth,adoption
            tagSet.Add("ADOP", BirtProc); // birth,adoption

            tagSet.Add("CAST", AttribProc);
            tagSet.Add("TITL", AttribProc);

            tagSet.Add("BAPL", LdsOrdProc);
            tagSet.Add("FAMC", ChildLink);
            tagSet.Add("FAMS", SpouseLink);
            tagSet.Add("SUBM", SubmProc);
            tagSet.Add("ASSO", AssocProc);
            tagSet.Add("ALIA", AliasProc);
            tagSet.Add("ANCI", AnciProc);
            tagSet.Add("DESI", DesiProc);
            tagSet.Add("RFN", PermRecProc);
            tagSet.Add("AFN", AfnProc); // TODO switch to DataProc?
            tagSet.Add("SOUR", SourceProc);
            tagSet.Add("_UID", DataProc);
            tagSet.Add("NOTE", DataProc); // TODO temporary
            tagSet.Add("CHAN", DataProc); // TODO temporary
            tagSet.Add("OBJE", DataProc); // TODO temporary

            tagSet.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            tagSet.Add("LVNG", LivingProc); // "Generations" custom
        }

        private void AfnProc(int begline, int endline, int nextchar)
        {
            AncestralFileNumber = _line.Substring(nextchar);
        }

        private void LivingProc(int begline, int endline, int nextchar)
        {
            // Some programs explicitly indicate 'living'. 
            // TODO handle as a generic data tag?
            Living = true;
        }

        private void BirtProc(int begline, int endline, int nextchar)
        {
            var rec = new EventRec(_tag);
            rec.Beg = begline;
            rec.End = endline;
            Events.Add(rec);

            // TODO parse event details
            // TODO parse birt, adop specific

            Console.WriteLine(rec);
        }

        private void PermRecProc(int begline, int endline, int nextchar)
        {
            PermanentRecord = _line.Substring(nextchar);
        }

        private void DesiProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            var rec = new XRefRec(_tag, ident);
            rec.Beg = begline;
            rec.End = endline;
            Desi.Add(rec);
        }

        private void AnciProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            var rec = new XRefRec(_tag, ident);
            rec.Beg = begline;
            rec.End = endline;
            Anci.Add(rec);
        }

        private void AliasProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            var rec = new XRefRec(_tag, ident);
            rec.Beg = begline;
            rec.End = endline;
            Alia.Add(rec);
        }

        private void AssocProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void SubmProc(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            var rec = new XRefRec(_tag, ident);
            rec.Beg = begline;
            rec.End = endline;
            Subm.Add(rec);
            Console.WriteLine(rec);
        }

        private void SpouseLink(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            // TODO parse NOTE

            var rec = new FamLinkRec(ident);
            rec.Beg = begline;
            rec.End = endline;
            FamLinks.Add(rec);
            Console.WriteLine(rec);
        }

        private void ChildLink(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            // TODO parse PEDI, STAT, NOTE

            var rec = new ChildLinkRec(ident);
            rec.Beg = begline;
            rec.End = endline;
            ChildLinks.Add(rec);
            Console.WriteLine(rec);
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
            Console.WriteLine(rec);
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
            Console.WriteLine(rec);
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
            Console.WriteLine(rec);
        }

        private void DataProc(int begline, int endline, int nextchar)
        {
            string data = _line.Substring(nextchar);
            var rec = new DataRec(_tag, data);
            rec.Beg = begline;
            rec.End = endline;
            Data.Add(rec);
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