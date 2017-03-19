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

        public string Given
        {
            get { return Indi == null ? "" : Indi.Names[0].Names; }
        }

        public string Surname
        {
            get { return Indi == null ? "" : Indi.Names[0].Surname; } 
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

        // Return either the birth or christening event
        public FamilyEvent Birth
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

        private void RangeCheck(Person p, ref long lastBorn, ref long firstDead)
        {
            if (p == null)
                return;
            if (p.Birth != null &&
            p.Birth.GedDate != null &&
            p.Birth.GedDate.Type != GEDDate.Types.Unknown)
            {
                GEDDate dadBorn = p.Birth.GedDate;
                if (dadBorn.JDN > lastBorn)
                    lastBorn = dadBorn.JDN;
            }
            if (p.Death != null &&
                p.Death.GedDate != null &&
                p.Death.GedDate.Type != GEDDate.Types.Unknown)
            {
                GEDDate dadBorn = p.Death.GedDate;
                if (dadBorn.JDN > firstDead)
                    firstDead = dadBorn.JDN;
            }
        }

        public GEDDate ExtrapolateBirth()
        {
            // 1. person not born before parent-birth+16
            // 2. person not born after parent-death+1
            // 3. person not born before own-marriage-date-16
            // 4. person not born before parent-marriage-date-1
            // 5. person not born before child-16
            // 6. person not born before spouse-birth-20

            long firstParentMarriage = long.MaxValue;
            long firstOwnMarriage = long.MaxValue;
            long lastParentBorn = long.MinValue;
            long firstParentDead = long.MaxValue;
            long firstSpouseBorn = long.MinValue;
            long firstChildBorn = long.MinValue;
            foreach (var union in _childIn)
            {
                // TODO this is not taking adoption into account!

                RangeCheck(union.Husband, ref lastParentBorn, ref firstParentDead);
                RangeCheck(union.Wife, ref lastParentBorn, ref firstParentDead);

                GEDDate md = union.MarriageDate;
                if (md != null && md.Type != GEDDate.Types.Unknown && md.JDN < firstParentMarriage)
                    firstParentMarriage = md.JDN;
            }
            foreach (var union in _spouseIn)
            {
                long junk = 0;
                GEDDate md = union.MarriageDate;
                if (md != null && md.Type != GEDDate.Types.Unknown && md.JDN < firstOwnMarriage)
                    firstOwnMarriage = md.JDN;
                if (union.Husband != this) // TODO 'otherspouse' accessor?
                    RangeCheck(union.Husband, ref firstSpouseBorn, ref junk);
                else
                    RangeCheck(union.Wife, ref firstSpouseBorn, ref junk);
                foreach (var child in union.Childs)
                {
                    RangeCheck(child, ref firstChildBorn, ref junk);
                }
            }

            long result = 0;
            if (lastParentBorn != long.MinValue)
            {
                // 1. person not born before parent-birth+16
                result = Math.Max(result, lastParentBorn + 16 * 365);
            }
            if (firstParentMarriage != long.MaxValue)
            {
                // 4. person not born before parent-marriage-date+1
                result = Math.Max(result, firstParentMarriage + 365);
            }
            if (firstParentDead != long.MaxValue)
            {
                // 2. person not born after parent-death+1
                result = Math.Min(result, firstParentDead + 365);
            }
            if (firstOwnMarriage != long.MaxValue)
            {
                // 3. person not born before own-marriage-date-16
                result = Math.Max(result, firstOwnMarriage - 16 * 365);
            }
            if (firstChildBorn != long.MinValue)
            {
                // 5. person not born before child-16
                if (result == 0) // TODO init to MAXVALUE?
                    result = firstChildBorn - 16*365;
                else
                    result = Math.Min(result, firstChildBorn - 16*365);
            }
            if (firstSpouseBorn != long.MinValue)
            {
                // 6. person not born before spouse-birth-20
                result = Math.Max(result, firstSpouseBorn - 20 * 365);
            }
            if (result == 0)
                return null;

            GEDDate output = new GEDDate(GEDDate.Types.Estimated);
            output.JDN = result;
            return output;
        }

        // Return Death/Burial/Cremation event
        public FamilyEvent Death
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
    }
}
