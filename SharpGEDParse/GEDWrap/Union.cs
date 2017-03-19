using SharpGEDParser.Model;
using System.Collections.Generic;

// Wrapper around the GEDCOM FAM record

namespace GEDWrap
{
    public class Union
    {
        public string Id { get { return FamRec.Ident; } }

        public FamRecord FamRec { get; set; }

        public Person Husband { get; set; }

        public Person Wife { get; set; }

        public HashSet<Person> Spouses { get; set; }

        public HashSet<Person> Childs { get; set; }

        public string DadId { get { return Husband == null ? "" : Husband.Indi.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Indi.Ident; } }

        public Union(FamRecord fam)
        {
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

        public GEDDate MarriageDate
        {
            get { return Marriage == null ? null : Marriage.GedDate; }
        }
    }
}
