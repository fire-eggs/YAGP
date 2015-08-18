using System.Collections.Generic;

namespace SharpGEDParser
{
    public class KBRGedFam : KBRGedRec
    {
        public KBRGedFam(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "FAM";

            Childs = new List<string>();
            FamEvents = new List<KBRGedEvent>();
        }

        public List<string> Childs { get; set; } // identity strings for children
        public string Dad { get; set; } // identity string for Father
        public string Mom { get; set; } // identity string for Mother
        public List<KBRGedEvent> FamEvents { get; set; } // TODO COMMON
    }
}
