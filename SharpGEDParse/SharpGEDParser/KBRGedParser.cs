using System;
using SharpGEDParser.Model;
using SharpGEDParser.Parser;

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
            _RepoParseSingleton = new GedRepoParse();
        }

        public object Parse(GedRecord rec)
        {
            // Given a glop of lines which represent a 'record', parse it into GED data (HEAD/INDI/FAM/etc)
            Tuple<object, GedParse> parseSet = Make(rec);

            // TODO execute in parallel
            if (parseSet.Item2 != null)
            {
                if (parseSet.Item1 is KBRGedRec)
                    parseSet.Item2.Parse(parseSet.Item1 as KBRGedRec);
                else
                    parseSet.Item2.Parse(parseSet.Item1 as GEDCommon, rec);
            }
            return parseSet.Item1;
        }

        private Tuple<object, GedParse> Make(GedRecord rec)
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

        private Tuple<object, GedParse> GedRecFactory(GedRecord rec, string ident, string tag)
        {
            // Parse 'top level' records. Parsing of some record types (e.g. NOTE, SOUR, etc) are likely to be in 'common' with sub-record parsing

            KBRGedRec data;

            // TODO Very much brute force. If/until this is found to be optimizable
            switch (tag.ToUpper())
            {
                case "HEAD":
                    data = new KBRGedHead(rec, ident);
                    return new Tuple<object, GedParse>(data, _HeadParseSingleton);
                case "INDI":
                    data = new KBRGedIndi(rec, ident);
                    return new Tuple<object, GedParse>(data, _IndiParseSingleton);
                case "FAM":
                    data = new KBRGedFam(rec, ident);
                    return new Tuple<object, GedParse>(data, _FamParseSingleton);
                case "SOUR":
                    GedSource data2 = new GedSource(rec);
                    data2.XRef = ident;
                    return new Tuple<object, GedParse>(data2, _SourParseSingleton);
                case "SUBM":
                    data = new GedSubm(rec, ident);
                    return new Tuple<object, GedParse>(data, _HeadParseSingleton); // TODO temporary 'ignore' parsing
                case "REPO":
                {
                    var foo = new GedRepository(rec, ident);
                    return new Tuple<object, GedParse>(foo, _RepoParseSingleton);
                }
                case "NOTE":
                    data = new GedNote(rec, ident);
                    return new Tuple<object, GedParse>(data, _HeadParseSingleton); // TODO temporary 'ignore' parsing
                case "OBJE":
                case "SUBN":
                default:  // TODO leading underscore signals a custom record
                    data = new KBRGedUnk(rec, ident, tag);
                    return new Tuple<object, GedParse>(data, null);
            }
        }

        private GedParse _IndiParseSingleton;
        private GedParse _FamParseSingleton;
        private GedParse _HeadParseSingleton;
        private GedParse _SourParseSingleton;
        private GedParse _RepoParseSingleton;
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

        void Parse(GEDCommon rec, GedRecord Lines);
    }
}

// TODO separation of parsing and container logic
// TODO export
// TODO details in 'head' may impact further parsing? ANSEL, ANSI, etc?
// TODO Heredis has a 'PLAC'/'FORM' tag in header which defines the format of places
