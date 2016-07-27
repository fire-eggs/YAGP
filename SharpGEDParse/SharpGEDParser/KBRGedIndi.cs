using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SharpGEDParser
{
    public class Rec
    {
        public int Beg { get; set; }

        public int End { get; set; }

        // TODO enum?
        public string Tag { get; set; }
        public int LineCount { get { return End - Beg + 1; } }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format("{0}({1},{2})", Tag, Beg, End);
        }
    }

    //public class UnkRec : Rec
    //{
    //    public string Error { get; set; }

    //    public UnkRec(string tag)
    //    {
    //        Tag = tag;
    //    }
    //}

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

        public Tuple<int, int> Pedi { get; set; }

        public Tuple<int, int> Stat { get; set; }
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

        public Tuple<int, int> Note { get; set; }
        public Tuple<int, int> Rela { get; set; }
        public Tuple<int, int> Sour { get; set; }

// TODO multiple notes, CONC/CONT notes
// TODO multiple sources, sub-source-tags

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format("{0}[{3}]({1},{2})", Tag, Beg, End, XRef);
        }
    }

    public class LDSRec : Rec
    {
        public string Date { get; set; }
        public string Place { get; set; }
        public string Temple { get; set; }
        public Tuple<int, int> Status { get; set; }
        public Tuple<int, int> Note { get; set; }
        public Tuple<int, int> Source { get; set; }
        public string Famc { get; set; }  // For SLGC
        public LDSRec(string tag)
        {
            Tag = tag;
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            string note = Note != null ? ",Note:" + Note : "";
            string src = Source != null ? ",Sour:" + Source : "";
            return string.Format("__{0}:When:'{1}',Where:'{2}'{3}{4}",
                base.ToString(), Date, Place, note, src);
        }
    }

    public class NameRec : Rec
    {
        public string Names { get; set; }
        public string Surname { get; set; }
        public string Suffix { get; set; }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return string.Format(" {0} /{1}/ {2}", Names, Surname, Suffix).Trim();
        }
    }

    public class KBRGedIndi : KBRGedRec
    {
        // TODO track this via Data instead?
        // Restriction notice
        public string Restriction { get; set; }

        // Individual's sex
        public char Sex { get; set; }

        private List<KBRGedEvent> _events;
        public List<KBRGedEvent> Events { get { return _events ?? (_events = new List<KBRGedEvent>()); }}

        private List<KBRGedEvent> _attribs;
        public List<KBRGedEvent> Attribs { get { return _attribs ?? (_attribs = new List<KBRGedEvent>()); }}

        private List<LDSRec> _ldsEvents;
        public List<LDSRec> LDSEvents { get { return _ldsEvents ?? (_ldsEvents = new List<LDSRec>()); }}

        private List<NameRec> _names;
        public List<NameRec> Names { get { return _names ?? (_names = new List<NameRec>()); }}

        private List<XRefRec> _alia;
        public List<XRefRec> Alia { get { return _alia ?? (_alia = new List<XRefRec>()); }}
        private List<XRefRec> _anci;
        public List<XRefRec> Anci { get { return _anci ?? (_anci = new List<XRefRec>()); }}
        private List<XRefRec> _desi;
        public List<XRefRec> Desi { get { return _desi ?? (_desi = new List<XRefRec>()); }}
        private List<XRefRec> _subm;
        public List<XRefRec> Subm { get { return _subm ?? (_subm = new List<XRefRec>()); }}
        private List<XRefRec> _assoc;
        public List<XRefRec> Assoc { get { return _assoc ?? (_assoc = new List<XRefRec>()); }}

        private List<ChildLinkRec> _childLinks;
        public List<ChildLinkRec> ChildLinks { get { return _childLinks ?? (_childLinks = new List<ChildLinkRec>()); }}

        private List<FamLinkRec> _famLinks;
        public List<FamLinkRec> FamLinks { get { return _famLinks ?? (_famLinks = new List<FamLinkRec>()); }}

        public bool Living { get; set; }

        public KBRGedIndi(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "INDI"; // TODO use enum?

            // TODO consider tracking ALIA ANCI DESI SUBM in a single list

            Living = false;
            Sex = 'U'; // unknown until proven otherwise
        }

        //public override string ToString()
        //{
        //    return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        //}

        public bool HasData(string tag)
        {
            return Data.Any(dataRec => dataRec.Tag == tag);
        }
    }
}
