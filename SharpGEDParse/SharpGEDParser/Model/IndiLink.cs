using System.Collections.Generic;

// Container for data associated to the INDI.FAMC, INDI.FAMS tags
// The 'extra' data is extremely unlikely 'in the wild'

namespace SharpGEDParser.Model
{
    public class IndiLink : StructCommon, NoteHold
    {
        public const byte FAMC_TYPE = 0;
        public const byte FAMS_TYPE = 1;

        public byte Type { get; set; }

        public string Xref { get; set; }

        public string Extra { get; set; } // Any 'extra' text // TODO switch to empty-if-not-used container

        public string Pedi { get; set; } // TODO switch to empty-if-not-used container

        public string Stat { get; set; } // TODO switch to empty-if-not-used container

        private List<Note> _notes;
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
