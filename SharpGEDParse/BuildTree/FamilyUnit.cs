using SharpGEDParser;
using System.Collections.Generic;
using SharpGEDParser.Model;

namespace BuildTree
{
    public class FamilyUnit
    {
        public KBRGedIndi Husband;
        public KBRGedIndi Wife;
        public List<KBRGedIndi> Childs;
        public FamRecord FamRec;

        public FamilyUnit DadFam;
        public FamilyUnit MomFam;

        public FamilyUnit(FamRecord _fam)
        {
            FamRec = _fam;
            Husband = null;
            Wife = null;
            Childs = new List<KBRGedIndi>();

            DadFam = null;
            MomFam = null;
        }

        public string DadId { get { return Husband == null ? "" : Husband.Ident; } }

        public string MomId { get { return Wife == null ? "" : Wife.Ident; } }
    }
}
