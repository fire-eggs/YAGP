using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    public class Address : StructCommon
    {
        public string Adr { get; set; }
        public string Adr1 { get; set; }
        public string Adr2 { get; set; }
        public string Adr3 { get; set; }
        public string City { get; set; }
        public string Stae { get; set; }
        public string Post { get; set; }
        public string Ctry { get; set; }

        private List<string> _phon;
        private List<string> _www;
        private List<string> _email;
        private List<string> _fax;
        public List<string> Phon { get { return _phon ?? (_phon = new List<string>()); } }
        public List<string> Email { get { return _email ?? (_email = new List<string>()); } }
        public List<string> WWW { get { return _www ?? (_www = new List<string>()); } }
        public List<string> Fax { get { return _fax ?? (_fax = new List<string>()); } }
    }
}
