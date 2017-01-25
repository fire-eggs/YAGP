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

        public HashSet<Person> Childs { get; set; }

        public string DadId { get { return Husband == null ? "" : Husband.Indi.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Indi.Ident; } }

        public Union(FamRecord fam)
        {
            FamRec = fam;
            Childs = new HashSet<Person>();
        }

        public bool ReconcileFams(string indiId)
        {
            // TODO Is the provided indi the husb or wife?
            return true;
        }
    }
}
