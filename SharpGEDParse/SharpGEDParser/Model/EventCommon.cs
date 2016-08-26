using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser.Model
{
    public class EventCommon : StructCommon, NoteHold
    {
        public string Tag { get; set; }

        public string Descriptor { get; set; }

        public string Type { get; set; } // detail, classification
        public string Agency { get; set; }
        public string Cause { get; set; }
        public string Religion { get; set; }
        public string Restriction { get; set; }

        public Address Address { get; set; }

        public string Place { get; set; } // TODO temporary - need full PLACE_STRUCTURE support

        public string Date { get; set; }

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
