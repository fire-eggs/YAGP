using System.Collections.Generic;
using SharpGEDParser.Model;

namespace BuildTree
{
    public class FamilyUnit
    {
        public IndiWrap Husband;
        public IndiWrap Wife;
        public List<IndiWrap> Childs;
        public FamRecord FamRec;

        public FamilyUnit DadFam;
        public FamilyUnit MomFam;

        public FamilyUnit(FamRecord fam)
        {
            FamRec = fam;
            Husband = null;
            Wife = null;
            Childs = new List<IndiWrap>();

            DadFam = null;
            MomFam = null;
        }

        public string DadId { get { return Husband == null ? "" : Husband.Indi.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Indi.Ident; } }
    }
}
