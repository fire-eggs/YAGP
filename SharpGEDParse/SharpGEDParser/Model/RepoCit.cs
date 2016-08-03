using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    // Repository Citation from SOURCE record
    public class RepoCit : StructCommon, NoteHold
    {
        public class CallNum
        {
            public string Number { get; set; }
            public string Media { get; set; }
        }

        public string Xref { get; set; }

        private List<CallNum> _callNums;
        public List<CallNum> CallNums { get { return _callNums ?? (_callNums = new List<CallNum>()); }}

        public string Media { get; set; }

        private List<Note> _notes;

        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }
    }
}
