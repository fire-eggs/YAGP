using System.Collections.Generic;

namespace SharpGEDParser
{
    public class KBRGedFam : KBRGedRec
    {
        public KBRGedFam(GedRecord lines, string ident) : base(lines)
        {
            Ident = ident;
            Tag = "FAM";
        }

        private List<string> _childs;
        public List<string> Childs { get { return _childs ?? (_childs = new List<string>()); }}
        public string Dad { get; set; } // identity string for Father
        public string Mom { get; set; } // identity string for Mother

        private List<KBRGedEvent> _famEvents; // TODO common?
        public List<KBRGedEvent> FamEvents { get { return _famEvents ?? (_famEvents = new List<KBRGedEvent>()); }}

        public string Marriage
        {
            get
            {
                foreach (var kbrGedEvent in FamEvents)
                {
                    if (kbrGedEvent.Tag == "MARR")
                    {
                        return kbrGedEvent.Date + " " + kbrGedEvent.Place;
                    }
                }
                return "";
            }
        }
    }
}
