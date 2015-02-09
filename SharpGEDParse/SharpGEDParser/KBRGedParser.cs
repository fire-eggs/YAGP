using System;

namespace SharpGEDParser
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
            ident.Parse(); // TODO execute in parallel
            ident.Validate(); // TODO execute in parallel
            return ident;
        }

        private KBRGedRec Make(GedRecord rec)
        {
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
            return GedRecFactory(rec, ident, tag);
        }

        private KBRGedRec GedRecFactory(GedRecord rec, string ident, string tag)
        {
            // TODO Very much brute force. If/until this is found to be optimizable
            switch (tag.ToUpper())
            {
                case "HEAD":
                    return new KBRGedHead(rec, ident);
                case "INDI":
                    return new KBRGedIndi(rec, ident);

                case "FAM":
                case "SUBM":
                case "REPO":
                case "SOUR":
                default:
                    return new KBRGedUnk(rec, ident, tag);
            }
        }
    }
}

// TODO separation of parsing and container logic
// TODO export
// TODO details in 'head' may impact further parsing? ANSEL, ANSI, etc?
// TODO Heredis has a 'PLAC'/'FORM' tag in header which defines the format of places
