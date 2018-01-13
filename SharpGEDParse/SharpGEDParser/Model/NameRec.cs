using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents an individual's name.
    /// </summary>
    public class NameRec : StructCommon, SourceCitHold, NoteHold
    {
        public string Names { get; set; }
        public string Surname { get; set; }
        public string Suffix { get; set; }

        // TODO do something better/complete with NAME sub-records
        private List<Tuple<string, string>> _parts;
        public List<Tuple<string, string>> Parts
        {
            get { return _parts ?? (_parts = new List<Tuple<string,string>>()); }             
        }

        private List<Note> _notes;
        private List<SourceCit> _cits;

        /// <summary>
        /// Any NOTEs associated with the record. 
        /// 
        /// An empty list if none.
        /// </summary>
        public List<Note> Notes { get { return _notes ?? (_notes = new List<Note>()); } }

        /// <summary>
        /// Any Source citations associated with the record. 
        /// </summary>
        /// An empty list if none.
        public List<SourceCit> Cits { get { return _cits ?? (_cits = new List<SourceCit>()); } }
    }
}
