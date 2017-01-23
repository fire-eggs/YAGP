using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

// Wrapper around the GEDCOM INDI record

namespace GEDWrap
{
    public class Person
    {
        public IndiRecord Indi { get; set; }
        public int Ahnen { get; set; }
        public int Tree { get; set; }

        public Person()
        {
            Tree = -1;
            Ahnen = 0;
        }

        public Person(IndiRecord indi) : this()
        {
            Indi = indi;
            _spouseIn = new List<Union>();
            _childIn = new List<Union>();
        }

        private List<Union> _spouseIn;

        // Person may be spouse in more than one family unit
        public List<Union> SpouseIn { get { return _spouseIn; }}

        private List<Union> _childIn;

        // Person may be spouse in more than one family unit
        public List<Union> ChildIn { get { return _childIn; }}
            
        public string Name
        {
            get { return Indi == null ? "" : Indi.Names[0].Names + " " + Indi.Names[0].Surname; } // TODO need a better accessor? restore ToString?
        }

        public string Text
        {
            get
            {
                if (Indi == null)
                    return "";
                string val1 = GetShowString("BIRT", "B: ");
                string val2 = GetShowString("DEAT", "D: ");
                string val3 = string.IsNullOrWhiteSpace(Marriage) ? "" : "M: " + Marriage + "\r\n";
                string val4 = GetShowString("CHR", "C: ");
                string val5 = GetShowString2("OCCU", "O: ");
                return val1 + val4 + val3 + val2 + val5;
            }
        }

        public string Marriage
        {
            get
            {
                if (SpouseIn.Count < 1)
                    return "";
                var fam = SpouseIn[0].FamRec; // TODO 'first' one only
                return fam.Marriage;
            }
        }

        public string Id { get { return Indi.Ident; } }

        private FamilyEvent GetEvent(string tag)
        {
            foreach (var kbrGedEvent in Indi.Events)
            {
                if (kbrGedEvent.Tag == tag)
                {
                    return kbrGedEvent;
                }
            }
            return null;
        }

        private FamilyEvent GetAttrib(string tag)
        {
            foreach (var kbrGedEvent in Indi.Attribs)
            {
                if (kbrGedEvent.Tag == tag)
                {
                    return kbrGedEvent;
                }
            }
            return null;
        }

        private string GetShowString(string tag, string prefix)
        {
            var even = GetEvent(tag);
            if (even == null)
                return "";

            string val = even.Date + " " + even.Place;
            if (string.IsNullOrWhiteSpace(val))
                return "";

            return prefix + val + "\r\n";
        }

        private string GetShowString2(string tag, string prefix)
        {
            var even = GetAttrib(tag);
            if (even == null)
                return "";

            string val = even.Descriptor + " " + even.Place;
            if (string.IsNullOrWhiteSpace(val))
                return "";

            return prefix + val.Trim() + "\r\n";
        }

        public override string ToString()
        {
            return Indi.Ident + ":" + Name;
        }

    }
}
