using SharpGEDParser.Model;
using System.Collections.Generic;
using System.Linq;

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
            _spouseIn = new HashSet<Union>();
            _childIn = new HashSet<Union>();
        }

        private HashSet<Union> _spouseIn;

        // Person may be spouse in more than one union; but said 
        // person cannot be the spouse more than once in a union
        public HashSet<Union> SpouseIn { get { return _spouseIn; }}

        private HashSet<Union> _childIn;

        // Person may be child in more than one union; but said 
        // person cannot be a child more than once in a union
        public HashSet<Union> ChildIn { get { return _childIn; }}
            
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
                Union fam = SpouseIn.First(); // TODO punting: first one only
                return fam.FamRec.Marriage;
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
