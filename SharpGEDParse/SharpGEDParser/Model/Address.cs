using System.Collections.Generic;

namespace SharpGEDParser.Model
{
    /// <summary>
    /// Contact information data.
    /// </summary>
    ///
    /// The ADDR and associated CONT tag(s) should be used to contain
    /// the full "mailing label" address.
    /// All other address fields are 'optional' and recorded for backward
    /// compatibility.
    ///
    /// The standard limits PHON, EMAIL, FAX and WWW entries to three, but
    /// that limit is not enforced.
    /// \todo example
    /// \todo better description
    public class Address : StructCommon
    {
        /// <summary>
        /// The full "mailing label" address as entered.
        /// </summary>
        public string Adr { get; set; }
        /// <summary>
        /// Any ADR1 data as entered.
        /// </summary>
        public string Adr1 { get; set; }
        /// <summary>
        /// Any ADR2 data as entered.
        /// </summary>
        public string Adr2 { get; set; }
        /// <summary>
        /// Any ADR3 data as entered.
        /// </summary>
        public string Adr3 { get; set; }
        /// <summary>
        /// The CITY data as entered.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// The STAE (state) data as entered.
        /// </summary>
        public string Stae { get; set; }
        /// <summary>
        /// The POST (postal code) data as entered.
        /// </summary>
        public string Post { get; set; }
        /// <summary>
        /// The CTRY (coutry) data as entered.
        /// </summary>
        public string Ctry { get; set; }

        private List<string> _phon;
        private List<string> _www;
        private List<string> _email;
        private List<string> _fax;
        /// <summary>
        /// Any PHON data as entered.
        /// </summary>
        /// Will be an empty list if none.
        public List<string> Phon { get { return _phon ?? (_phon = new List<string>()); } }
        /// <summary>
        /// Any email addresses as entered.
        /// </summary>
        /// Will be an empty list if none.
        public List<string> Email { get { return _email ?? (_email = new List<string>()); } }
        /// <summary>
        /// Any web (URL) addresses as entered.
        /// </summary>
        /// Will be an empty list if none.
        public List<string> WWW { get { return _www ?? (_www = new List<string>()); } }
        /// <summary>
        /// Any FAX data as entered.
        /// </summary>
        /// Will be an empty list if none.
        public List<string> Fax { get { return _fax ?? (_fax = new List<string>()); } }
    }
}