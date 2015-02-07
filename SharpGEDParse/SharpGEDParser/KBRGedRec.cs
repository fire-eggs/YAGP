using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBRImport
{
    public class KBRGedRec
    {
        protected GedRecord Lines { get; set; }

        public KBRGedRec(GedRecord lines)
        {
            if (lines.LineCount < 1)
                throw new Exception("Empty GedRecord!");
            Lines = lines;
        }

        public override string ToString()
        {
            return string.Format("KBRGedRec:{0}:{1}:{2}", Tag, Ident, Lines);
        }

        public void Validate()
        {
            // TODO check lines and add errors to an error set
        }

        public string Ident { get; set; }
        public string Tag { get; set; }
    }
}
