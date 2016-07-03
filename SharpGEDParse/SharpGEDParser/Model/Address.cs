using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Address
    {
        public string Adr { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string Adr3 { get; set; }
        public string City { get; set; }
        public string Stae { get; set; }
        public string Post { get; set; }
        public string Ctry { get; set; }
        public string Phon { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string WWW { get; set; }

        // All other lines (typically custom/unknown)
        private List<LineSet> _other;
        public List<LineSet> OtherLines { get { return _other ?? (_other = new List<LineSet>()); } }
    }
}
