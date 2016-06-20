using System;

namespace SharpGEDParser
{
    public class KBRGedParser
    {
        public KBRGedParser(string gedPath)
        {
            // TODO dunno yet if the path to the GED is useful

            _IndiParseSingleton = new GedIndiParse();
            _FamParseSingleton  = new GedFamParse();
            _HeadParseSingleton = new GedHeadParse();
            _SourParseSingleton = new GedSourParse();
        }

        public KBRGedRec Parse(GedRecord rec)
        {
            // Given a glop of lines which represent a 'record', parse it into GED data (HEAD/INDI/FAM/etc)
            Tuple<KBRGedRec, GedParse> parseSet = Make(rec);

            // TODO execute in parallel
            if (parseSet.Item2 != null)
            {
                parseSet.Item2.Parse(parseSet.Item1);
                //parseSet.Item2.Validate(parseSet.Item1);
            }
            Console.WriteLine(parseSet.Item1);
            return parseSet.Item1;
        }

        private Tuple<KBRGedRec, GedParse> Make(GedRecord rec)
        {
            // 1. The first line in the rec should start with '0'
            string head = rec.FirstLine();
            int firstDex = GedLineUtil.FirstChar(head);
            if (head[firstDex] != '0')
                throw new Exception("record head not zero"); // TODO should this be an error record instead?

            // 2. search for and find the tag
            string ident = "";
            string tag = "";
            GedLineUtil.IdentAndTag(head, firstDex + 1, ref ident, ref tag);

            // 3. create a KBRGedRec derived class
            return GedRecFactory(rec, ident, tag);
        }

        private Tuple<KBRGedRec, GedParse> GedRecFactory(GedRecord rec, string ident, string tag)
        {
            // Parse 'top level' records. Parsing of some record types (e.g. NOTE, SOUR, etc) are likely to be in 'common' with sub-record parsing

            KBRGedRec data;

            // TODO Very much brute force. If/until this is found to be optimizable
            switch (tag.ToUpper())
            {
                case "HEAD":
                    data = new KBRGedHead(rec, ident);
                    return new Tuple<KBRGedRec, GedParse>(data, _HeadParseSingleton);
                case "INDI":
                    data = new KBRGedIndi(rec, ident);
                    return new Tuple<KBRGedRec, GedParse>(data, _IndiParseSingleton);
                case "FAM":
                    data = new KBRGedFam(rec, ident);
                    return new Tuple<KBRGedRec, GedParse>(data, _FamParseSingleton);

                case "SOUR":
                    GedSource data2 = new GedSource(rec);
                    data2.XRef = ident;
                    return new Tuple<KBRGedRec, GedParse>(data2, _SourParseSingleton);

                case "SUBM":
                case "REPO":
                case "OBJE":
                case "NOTE":
                case "SUBN":
                default:
                    data = new KBRGedUnk(rec, ident, tag);
                    return new Tuple<KBRGedRec, GedParse>(data, null);
            }
        }

        private GedParse _IndiParseSingleton;
        private GedParse _FamParseSingleton;
        private GedParse _HeadParseSingleton;
        private GedParse _SourParseSingleton;
        private static GedRecParse _EventParseSingleton;
        private static GedRecParse _SourceCitParseSingleton;

        public static GedRecParse EventParser
        {
            get { return _EventParseSingleton ?? (_EventParseSingleton = new GedEventParse()); }
        }

        public static GedRecParse SourceCitParseSingleton
        {
            get { return _SourceCitParseSingleton ?? (_SourceCitParseSingleton = new GedSourCitParse()); }
        }
    }

    public interface GedParse
    {
        void Parse(KBRGedRec rec);

        void Parse(KBRGedRec rec, GedRecParse.ParseContext context);
    }
}

// TODO separation of parsing and container logic
// TODO export
// TODO details in 'head' may impact further parsing? ANSEL, ANSI, etc?
// TODO Heredis has a 'PLAC'/'FORM' tag in header which defines the format of places
