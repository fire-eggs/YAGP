using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

// Wrapper around the GEDCOM FAM record

namespace GEDWrap
{
    public class Union
    {
        public FamRecord FamRec { get; set; }

        public Person Husband { get; set; }

        public Person Wife { get; set; }

        public List<Person> Childs { get; set; }

        public string DadId { get { return Husband == null ? "" : Husband.Indi.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Indi.Ident; } }

        public Union(FamRecord fam)
        {
            FamRec = fam;
            Childs = new List<Person>();
        }
    }
}
