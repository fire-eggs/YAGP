using System;
using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Represents an individual's name.
    /// </summary>
    public class NameRec : StructCommon
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
    }
}
