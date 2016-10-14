using System.Collections.Generic;
using SharpGEDParser.Model;

namespace BuildTree
{
    public class FamilyUnit
    {
        public IndiRecord Husband;
        public IndiRecord Wife;
        public List<IndiRecord> Childs;
        public FamRecord FamRec;

        public FamilyUnit DadFam;
        public FamilyUnit MomFam;

        public FamilyUnit(FamRecord fam)
        {
            FamRec = fam;
            Husband = null;
            Wife = null;
            Childs = new List<IndiRecord>();

            DadFam = null;
            MomFam = null;
        }

        public string DadId { get { return Husband == null ? "" : Husband.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Ident; } }
    }
}
