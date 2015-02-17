using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public string Date { get; set; }
        public string Place { get; set; }

        public string Age { get; set; }

        public Tuple<int, int> Change { get; set; }

        public Tuple<int, int> Note { get; set; }

        public Tuple<int, int> Source { get; set; }

        public EventRec(string tag)
        {
            Tag = tag;
        }

        public override string ToString()
        {
            string note = Note != null ? ",Note:" + Note : "";
            string chan = Change != null ? ",Chan:" + Change : "";
            string src = Source != null ? ",Sour:" + Source : "";
            return string.Format("__{0}:When:'{1}',Where:'{2}'{3}{4}{5}", 
                base.ToString(), Date, Place, note, chan, src);
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

        // Source records
        public List<SourceRec> Sources { get; set; }

        // Unknown records
        public List<UnkRec> Unknowns { get; set; }

        public List<EventRec> Events { get; set; }

        public List<EventRec> FamEvents { get; set; }

        public List<NameRec> Names { get; set; }

        public List<XRefRec> Alia { get; set; }
        public List<XRefRec> Anci { get; set; }
        public List<XRefRec> Desi { get; set; }
        public List<XRefRec> Subm { get; set; }

        public List<ChildLinkRec> ChildLinks { get; set; }

        public List<FamLinkRec> FamLinks { get; set; }

        public List<DataRec> Data { get; set; }

        public bool Living { get; set; }

        public Tuple<int, int> Note { get; set; }
        public Tuple<int, int> Change { get; set; }

        public KBRGedIndi(GedRecord lines, string ident) : base(lines)
        {
            BuildTagSet();
            Ident = ident;
            Tag = "INDI"; // TODO use enum?

            Sources = new List<SourceRec>();
            Unknowns = new List<UnkRec>();
            Events = new List<EventRec>();
            FamEvents = new List<EventRec>();
            Names = new List<NameRec>();
            Data = new List<DataRec>();

            Alia = new List<XRefRec>();
            Anci = new List<XRefRec>();
            Desi = new List<XRefRec>();
            Subm = new List<XRefRec>();

            ChildLinks = new List<ChildLinkRec>();
            FamLinks = new List<FamLinkRec>();

            Living = false;

            // TODO can any properties, especially List<>, not be initialized until use? Efficiently?
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

//            Console.WriteLine("***" + rec);
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
            tagSet.Add("CREM", EventProc); // simple event
            tagSet.Add("BURI", EventProc); // simple event
            tagSet.Add("CHR", EventProc); // simple event
            tagSet.Add("NATU", EventProc);
            tagSet.Add("IMMI", EventProc);
            tagSet.Add("WILL", EventProc);
            tagSet.Add("EMIG", EventProc);

            tagSet.Add("EVEN", EventProc); // TODO simple or family event?

            tagSet.Add("BIRT", BirtProc); // birth,adoption
            tagSet.Add("ADOP", BirtProc); // birth,adoption

            tagSet.Add("CAST", AttribProc);
            tagSet.Add("TITL", AttribProc);
            tagSet.Add("RESI", AttribProc);
            tagSet.Add("OCCU", AttribProc);
            tagSet.Add("FACT", AttribProc);
            tagSet.Add("DSCR", AttribProc);

            tagSet.Add("ANUL", FamEventProc);
            tagSet.Add("MARL", FamEventProc);
            tagSet.Add("DIV", FamEventProc);

            tagSet.Add("BAPL", LdsOrdProc);

            tagSet.Add("FAMC", ChildLink);
            tagSet.Add("FAMS", SpouseLink);

            tagSet.Add("SUBM", SubmProc);
            tagSet.Add("ASSO", AssocProc);
            tagSet.Add("ALIA", AliasProc);
            tagSet.Add("ANCI", AnciProc);
            tagSet.Add("DESI", DesiProc);
            tagSet.Add("RFN", PermRecProc);
            tagSet.Add("AFN", DataProc);
            tagSet.Add("REFN", DataProc);
            tagSet.Add("SOUR", SourceProc);
            tagSet.Add("_UID", DataProc);
            tagSet.Add("NOTE", NoteProc);
            tagSet.Add("CHAN", ChanProc);
            tagSet.Add("OBJE", DataProc); // TODO temporary

            tagSet.Add("LVG", LivingProc); // "Family Tree Maker for Windows" custom
            tagSet.Add("LVNG", LivingProc); // "Generations" custom
        }

        private void LivingProc(int begline, int endline, int nextchar)
        {
            // Some programs explicitly indicate 'living'. 
            // TODO handle as a generic data tag?
            Living = true;
        }

        private void BirtProc(int begline, int endline, int nextchar)
        {
            // TODO save this rec 'special' so easy to find later?

            var rec = CommonEventProcessing(begline, endline);
            Events.Add(rec);

            // TODO parse birt, adop specific
            Debug.Assert(KBRGedUtil.ParseFor(Lines, begline, endline, "FAMC") == null);
            // ADOP is a sub-tag on FAMC
//            Debug.Assert(_tag == "BIRT" && KBRGedUtil.ParseFor(Lines, begline, endline, "ADOP") == null);
        }

        private void PermRecProc(int begline, int endline, int nextchar)
        {
            PermanentRecord = _line.Substring(nextchar);
        }

        private XRefRec CommonXRefProcessing(int begline, int endline, int nextchar)
        {
            string ident = "";
            int res = KBRGedUtil.Ident(_line, nextchar, ref ident);
            var rec = new XRefRec(_tag, ident);
            rec.Beg = begline;
            rec.End = endline;
            return rec;
        }

        private void DesiProc(int begline, int endline, int nextchar)
        {
            var rec = CommonXRefProcessing(begline, endline, nextchar);
            Desi.Add(rec);
        }

        private void AnciProc(int begline, int endline, int nextchar)
        {
            var rec = CommonXRefProcessing(begline, endline, nextchar);
            Anci.Add(rec);
        }

        private void AliasProc(int begline, int endline, int nextchar)
        {
            var rec = CommonXRefProcessing(begline, endline, nextchar);
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
            // TODO submitter details?
//            Console.WriteLine(rec);
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
//            Console.WriteLine(rec);
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
//            Console.WriteLine(rec);
        }

        private void LdsOrdProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void AttribProc(int begline, int endline, int nextchar)
        {
            // TODO
        }

        private void FamEventProc(int begline, int endline, int nextchar)
        {
            // A family event: same as an event but has additional husband, wife tags
            var rec = CommonEventProcessing(begline, endline);
            FamEvents.Add(rec);

            // TODO family event specific processing
            Debug.Assert(KBRGedUtil.ParseFor(Lines, begline, endline, "HUSB") == null);
            Debug.Assert(KBRGedUtil.ParseFor(Lines, begline, endline, "WIFE") == null);
        }

        private EventRec CommonEventProcessing(int begline, int endline)
        {
            var rec = new EventRec(_tag);
            rec.Beg = begline;
            rec.End = endline;

            rec.Date = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "DATE");
            rec.Place = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "PLAC");
            rec.Age = KBRGedUtil.ParseFor(Lines, begline + 1, endline, "AGE");

            rec.Change = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "CHAN");
            rec.Note = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "NOTE");
            rec.Source = KBRGedUtil.ParseForMulti(Lines, begline + 1, endline, "SOUR");
            return rec;
        }

        private void EventProc(int begline, int endline, int nextchar)
        {
            var rec = CommonEventProcessing(begline, endline);
            Events.Add(rec);
//            Console.WriteLine(rec);
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
            if (startSur < max) // e.g. "1 NAME LIVING"
                rec.Surname = _line.Substring(startSur+1, endSur-startSur-1);
            Names.Add(rec);

            // TODO parse more details
//            Console.WriteLine(rec);
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
//            Console.WriteLine(rec);
        }

        private void DataProc(int begline, int endline, int nextchar)
        {
            string data = _line.Substring(nextchar);
            var rec = new DataRec(_tag, data);
            rec.Beg = begline;
            rec.End = endline;
            Data.Add(rec);
        }

        private void NoteProc(int begline, int endline, int nextchar)
        {
            Note = new Tuple<int, int>(begline, endline);
        }

        private void ChanProc(int begline, int endline, int nextchar)
        {
            Change = new Tuple<int, int>(begline, endline);
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