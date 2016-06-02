using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Error { get; set; }
        public UnkRec(string tag)
        {
            Tag = tag;
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
    // NOTE: also used for family events (HUSB, WIFE extras)
    public class EventRec : Rec
    {
        public string Detail { get; set; } // e.g. caste_name
        public string Date { get; set; }
        public string Place { get; set; }
        public string Age { get; set; }
        public string Type { get; set; } // detail, classification
        public string Agency { get; set; }
        public string Cause { get; set; }
        public string Religion { get; set; }
        public string Restriction { get; set; }

        public Tuple<int, int> Change { get; set; } // TODO enforce single change
        public Tuple<int, int> Note { get; set; } // TODO multiple notes
        public Tuple<int, int> Source { get; set; } // TODO multiple source

        // TODO OBJE tag, multiple

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

        public List<KBRGedEvent> Events { get; set; }

        public List<KBRGedEvent> Attribs { get; set; }

        public List<LDSRec> LDSEvents { get; set; }

        public List<NameRec> Names { get; set; }

        public List<XRefRec> Alia { get; set; }
        public List<XRefRec> Anci { get; set; }
        public List<XRefRec> Desi { get; set; }
        public List<XRefRec> Subm { get; set; }
        public List<XRefRec> Assoc { get; set; }

        public List<ChildLinkRec> ChildLinks { get; set; }

        public List<FamLinkRec> FamLinks { get; set; }

        public bool Living { get; set; }

        public KBRGedIndi(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "INDI"; // TODO use enum?

            Events = new List<KBRGedEvent>();
            Attribs = new List<KBRGedEvent>();
            Names = new List<NameRec>();
            LDSEvents = new List<LDSRec>();

            // TODO consider tracking in a single list
            Alia = new List<XRefRec>();
            Anci = new List<XRefRec>();
            Desi = new List<XRefRec>();
            Subm = new List<XRefRec>();

            ChildLinks = new List<ChildLinkRec>();
            FamLinks = new List<FamLinkRec>();

            Assoc = new List<XRefRec>();

            Living = false;
            Sex = 'U'; // unknown until proven otherwise

            // TODO can any properties, especially List<>, not be initialized until use? Efficiently?
        }

        public override string ToString()
        {
            return string.Format("{0}({1}):{2}", Tag, Ident, Lines);
        }

        public bool HasData(string tag)
        {
            return Data.Any(dataRec => dataRec.Tag == tag);
        }

        public string Birth
        {
            get
            {
                foreach (var kbrGedEvent in Events)
                {
                    if (kbrGedEvent.Tag == "BIRT")
                    {
                        string val = kbrGedEvent.Date + " " + kbrGedEvent.Place;
                        return val;
                    }
                }
                return "";
            }
        }

        public string Death
        {
            get
            {
                foreach (var kbrGedEvent in Events)
                {
                    if (kbrGedEvent.Tag == "DEAT")
                    {
                        string val = kbrGedEvent.Date + " " + kbrGedEvent.Place;
                        return val;
                    }
                }
                return "";
            }
        }

        public string Christening
        {
            get
            {
                foreach (var kbrGedEvent in Events)
                {
                    if (kbrGedEvent.Tag == "CHR")
                    {
                        string val = kbrGedEvent.Date + " " + kbrGedEvent.Place;
                        return val;
                    }
                }
                return "";
            }
        }

        public string Occupation
        {
            get
            {
                foreach (var attrib in Attribs)
                {
                    if (attrib.Tag == "OCCU")
                    {
                        string val = attrib.Detail + " " + attrib.Place;
                        return val;
                    }
                }
                return "";
            }
        }
    }
}
