using SharpGEDParser.Model;
using System.Collections.Generic;

// Wrapper around the GEDCOM FAM record

namespace GEDWrap
{
    public class Union
    {
        public int TreeNum { get; set; } // Tracking treenum here during tree calc seems to have a small benefit

        public string Id { get { return FamRec.Ident; } }

        public FamRecord FamRec { get; set; }

        public Person Husband { get; set; }

        public Person Wife { get; set; }

        // NOTE: timing runs indicate using HashSet here (and in Person) is faster than List, 
        // for at least a 15% advantage

        public HashSet<Person> Spouses { get; set; }

        public HashSet<Person> Childs { get; set; }

        public string DadId { get { return Husband == null ? "" : Husband.Indi.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Indi.Ident; } }

        public Union(FamRecord fam)
        {
            TreeNum = -1;
            FamRec = fam;
            Childs = new HashSet<Person>();
            Spouses = new HashSet<Person>();
        }

        public bool ReconcileFams(Person indi)
        {
            Spouses.Add(indi);
            // Is the provided indi the husb or wife?
            return (MomId == indi.Id || DadId == indi.Id);
        }

        public FamilyEvent GetEvent(string tag)
        {
            foreach (var kbrGedEvent in FamRec.FamEvents)
            {
                if (kbrGedEvent.Tag == tag)
                {
                    return kbrGedEvent;
                }
            }
            return null;
        }

        public FamilyEvent Marriage
        {
            get { return GetEvent("MARR"); }
        }

        public FamilyEvent Divorce
        {
            get { return GetEvent("DIV"); }
        }

        public GEDDate MarriageDate
        {
            get { return Marriage == null ? null : Marriage.GedDate; }
        }

        public string MarriagePlace
        {
            get { return Marriage == null ? null : Marriage.Place; }
        }

        public Person Spouse(Person who)
        {
            if (who == Husband)
                return Wife;
            return Husband;

            // TODO more than one spouse?
        }

        public string GetDate(string eventName) // TODO format
        {
            var gedEvent = GetEvent(eventName);
            if (gedEvent != null && gedEvent.GedDate != null)
                return gedEvent.GedDate.ToString();
            return null;
        }

        public string GetPlace(string eventName)
        {
            var gedEvent = GetEvent(eventName);
            if (gedEvent != null)
                return gedEvent.Place;
            return null;
        }
    }
}
