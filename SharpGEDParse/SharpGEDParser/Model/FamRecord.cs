using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SharpGEDParser.Model
{
    public class FamRecord : GEDCommon, NoteHold, SourceCitHold, MediaHold
    {
        public static string Tag = "FAM";

        private List<Note> _notes;
        private List<SourceCit> _cits;
        private List<MediaLink> _media;
        private List<LDSEvent> _ldsEvents; // TODO common?

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
        public List<MediaLink> Media { get { return _media ?? (_media = new List<MediaLink>()); } }
        public List<LDSEvent> LDSEvents { get { return _ldsEvents ?? (_ldsEvents = new List<LDSEvent>()); } }

        // Identity strings for children
        private List<string> _childs;
        public List<string> Childs { get { return _childs ?? (_childs = new List<string>()); } }
        public string Dad { get; set; } // identity string for Father
        public string Mom { get; set; } // identity string for Mother

        private List<FamilyEvent> _famEvents; // TODO common?
        public List<FamilyEvent> FamEvents { get { return _famEvents ?? (_famEvents = new List<FamilyEvent>()); } }

        // Identity strings for submitters
        private List<string> _famSubm; // TODO common?
        public List<string> FamSubm { get { return _famSubm ?? (_famSubm = new List<string>()); } }

        public FamRecord(GedRecord lines, string ident, string remain) : base(lines, ident)
        {
            _childCount = -1;
            GedRecParse.NonStandardRemain(remain, this);
        }

        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return Tag;
        }

        [ExcludeFromCodeCoverage]
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

        private int _childCount;
        private string _restriction;

        public int ChildCount
        {
            get { return _childCount; }
            set { _childCount = value; }
        }

        public string Restriction
        {
            get { return _restriction; }
            set { _restriction = value; }
        }
    }
}
