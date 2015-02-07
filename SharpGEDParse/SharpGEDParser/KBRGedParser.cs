using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KBRImport
{
    public class KBRGedParser
    {
        public KBRGedParser(string gedPath)
        {
            // TODO dunno yet if the path to the GED is useful
        }

        public KBRGedRec Parse(GedRecord rec)
        {
            // Given a glop of lines which represent a 'record', parse it into GED data (HEAD/INDI/FAM/etc)
            KBRGedRec ident = Make(rec);
            ident.Validate();
            return ident;
        }

        private KBRGedRec Make(GedRecord rec)
        {
            // TODO should this be some sort of Factory?

            // 1. The first line in the rec should start with '0'
            string head = rec.FirstLine();
            int firstDex = KBRGedUtil.FirstChar(head);
            if (head[firstDex] != '0')
                throw new Exception("record head not zero");

            // 2. search for and find the tag
            string ident = "";
            string tag = "";
            KBRGedUtil.IdentAndTag(head, firstDex + 1, ref ident, ref tag);

            // 3. create a KBRGedRec derived class

            var tmpOut = new KBRGedRec(rec);
            tmpOut.Ident = ident;
            tmpOut.Tag = tag;
            return tmpOut;
        }
    }
}
