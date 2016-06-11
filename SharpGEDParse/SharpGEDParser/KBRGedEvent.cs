
namespace SharpGEDParser
{
    public class KBRGedEvent : KBRGedRec
    {
        public KBRGedEvent(GedRecord lines, string tag) : base(lines)
        {
            Tag = tag;
        }

        public string Detail { get; set; } // e.g. caste_name
        public string Date { get; set; }
        public string Place { get; set; }
        public string Age { get; set; }
        public string Type { get; set; } // detail, classification
        public string Agency { get; set; }
        public string Cause { get; set; }
        public string Religion { get; set; }
        public string Restriction { get; set; }

        // Family Event support
        public string HusbDetail { get; set; }
        public string HusbAge { get; set; }
        public string WifeDetail { get; set; }
        public string WifeAge { get; set; }

        // BIRT/CHR/ADOP individual event support
        public string Famc { get; set; }
        public string FamcAdop { get; set; }

        public string Address { get; set; } // TODO a more detailed structure?
    }
}
