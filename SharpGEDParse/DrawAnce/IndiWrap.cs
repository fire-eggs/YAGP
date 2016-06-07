using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildTree;
using SharpGEDParser;

namespace DrawAnce
{
    public class IndiWrap
    {
        public KBRGedIndi Indi;
        public int Ahnen;
        public FamilyUnit ChildOf;

        public string Name
        {
            get { return Indi == null ? "" : Indi.Names[0].ToString(); }
        }

        public string Text
        {
            get
            {
                if (Indi == null)
                    return "";
                string val = string.IsNullOrEmpty(Indi.Birth) ? "" : "B: " + Indi.Birth + "\r\n";
                string val4 = string.IsNullOrEmpty(Indi.Christening) ? "" : "C: " + Indi.Christening + "\r\n";
                string val3 = string.IsNullOrWhiteSpace(Marriage) ? "" : "M: " + Marriage + "\r\n";
                string val2 = string.IsNullOrEmpty(Indi.Death) ? "" : "D: " + Indi.Death + "\r\n";
                string val5 = string.IsNullOrEmpty(Indi.Occupation) ? "" : "O: " + Indi.Occupation + "\r\n";
                return val + val4 + val3 + val2 + val5;
            }
        }

        public FamilyUnit SpouseIn { get; set; }

        // TODO divorce?

        public string Marriage
        {
            get
            {
                if (SpouseIn == null)
                    return "";
                var fam = SpouseIn.FamRec;
                return fam.Marriage;
            }
        }
    }
}
