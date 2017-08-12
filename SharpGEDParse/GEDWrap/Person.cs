using System;
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

        internal HashSet<Union> _spouseIn;

        // Person may be spouse in more than one union; but said 
        // person cannot be the spouse more than once in a union
        public HashSet<Union> SpouseIn { get { return _spouseIn; }}

        internal HashSet<Union> _childIn;

        // Person may be child in more than one union; but said 
        // person cannot be a child more than once in a union
        public HashSet<Union> ChildIn { get { return _childIn; }}
            
        public string Name
        {
            get { return Indi == null || Indi.Names.Count < 1 ? "" : Indi.Names[0].Names + " " + Indi.Names[0].Surname; } // TODO need a better accessor? restore ToString?
        }

        public string Given
        {
            get { return Indi == null ? "" : Indi.Names[0].Names; }
        }

        public string Surname
        {
            get { return Indi == null ? "" : 
                (Indi.Names[0].Surname ?? ""); } 
        }

        public string Text
        {
            get
            {
                if (Indi == null)
                    return "";
                string val1 = GetShowString("BIRT", "B: ");
                string val2 = GetShowString("DEAT", "D: ");
                string val3 = "";
                if (Marriage != null)
                {
                    val3 = string.Format("M: {0} {1}\r\n", Marriage.Date, Marriage.Place);
                }
                string val4 = GetShowString("CHR", "C: ");
                string val5 = GetShowString2("OCCU", "O: ");
                return val1 + val4 + val3 + val2 + val5;
            }
        }

        public Union MarriageUnion
        {
            get
            {
                if (SpouseIn.Count < 1)
                    return null;
                Union fam = SpouseIn.First(); // TODO punting: first one only
                return fam;
            }
        }

        public FamilyEvent Marriage
        {
            get
            {
                Union onion = MarriageUnion;
                return onion == null ? null : onion.Marriage;
            }
        }

        public string Id { get { return Indi.Ident; } }

        public IndiEvent GetEvent(string tag)
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

        private IndiEvent GetAttrib(string tag)
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

        // Return either the birth or christening event
        public IndiEvent Birth
        {
            get
            {
                var birth = GetEvent("BIRT");
                if (birth != null)
                    return birth;
                var christ = GetEvent("CHR");
                return christ;
            }
        }

        protected internal GEDDate _estimatedBirth;

        public GEDDate BirthDate
        {
            get
            {
                var b = Birth;
                if (b != null &&
                    b.GedDate != null &&
                    b.GedDate.Type != GEDDate.Types.Unknown)
                    return b.GedDate;
                return _estimatedBirth;
            }
        }

        // Return Death/Burial/Cremation event
        public IndiEvent Death
        {
            get
            {
                var evt = GetEvent("DEAT");
                if (evt != null)
                    return evt;
                evt = GetEvent("BURI");
                if (evt != null)
                    return evt;
                evt = GetEvent("CREM");
                return evt;
            }
        }

        public IndiEvent Occupation
        {
            get { return GetEvent("OCCU"); }
        }

        public string GetWhat(string eventName)
        {
            var gedEvent = GetEvent(eventName);
            if (gedEvent != null && gedEvent.Descriptor != null)
                return gedEvent.Descriptor;
            return null;
        }

        public string GetDate(string eventName) // TODO format
        {
            var gedEvent = GetEvent(eventName);
            if (gedEvent == null)
                return null;
            if (gedEvent.GedDate != null && 
                gedEvent.GedDate.Initialized)
                return gedEvent.GedDate.ToString();
            return gedEvent.Date;
        }

        public string GetPlace(string eventName)
        {
            var gedEvent = GetEvent(eventName);
            if (gedEvent != null && gedEvent.Place != null)
                return gedEvent.Place;
            return null;
        }

        public string GetParent(bool dad)
        {
            if (ChildIn != null && ChildIn.Count > 0)
            {
                // TODO adoption etc
                Union onion = ChildIn.ToArray()[0]; // TODO might not get the "right" one
                var val0 = dad ? onion.Husband : onion.Wife;
                return val0 == null ? null : val0.Name;
            }
            return null;
        }

    }
}
