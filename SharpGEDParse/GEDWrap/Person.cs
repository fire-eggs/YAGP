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

        // NOTE: timing runs indicate using HashSet here (and in Union) is faster than List, 
        // for at least a 15% advantage
        internal HashSet<Union> _spouseIn;

        // Person may be spouse in more than one union; but said 
        // person cannot be the spouse more than once in a union
        public HashSet<Union> SpouseIn { get { return _spouseIn; } }

        internal HashSet<Union> _childIn;

        // Person may be child in more than one union; but said 
        // person cannot be a child more than once in a union
        public HashSet<Union> ChildIn { get { return _childIn; } }
            
        public string Name
        {
            get { return Indi == null || Indi.Names.Count < 1 ? "" : Indi.Names[0].Names + " " + Indi.Names[0].Surname; } // TODO need a better accessor? restore ToString?
        }

        public string Given
        {
            get { return Indi == null || Indi.Names.Count < 1 ? "" : Indi.Names[0].Names; }
        }

        public string Surname
        {
            get
            {
                return Indi == null || Indi.Names.Count < 1 ? "" : 
                (Indi.Names[0].Surname ?? ""); } 
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

        // TODO change to 'GetFact' to combine events/attribs?
        public IndiEvent GetEvent(string tag)
        {
            foreach (var kbrGedEvent in Indi.Events)
            {
                if (kbrGedEvent.Tag == tag)
                {
                    return kbrGedEvent;
                }
            }
            return GetAttrib(tag); // Allow attributes to be requested via a single interface
        }

        public IndiEvent GetAttrib(string tag)
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
            return "";
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

        public string Sex
        {
            get
            {
                switch (Indi.Sex)
                {
                    case 'M':
                        return "Male";
                    case 'F':
                        return "Female";
                    default:
                        return "Unknown";
                }
            }
        }

    }
}
