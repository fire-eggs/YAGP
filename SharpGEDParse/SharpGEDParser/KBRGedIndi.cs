using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBRImport
{
    public class KBRGedIndi : KBRGedRec
    {
        public KBRGedIndi(GedRecord lines) : base(lines)
        {
            
        }

        public override string ToString()
        {
            return "KBRGedIndi" + Lines.ToString();
        }
    }
}
