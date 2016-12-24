using System;
using SharpGEDParser.Model;
using SharpGEDParser.Parser;
#if PARALLEL
using System.Collections.Generic;
using System.Threading.Tasks;
#endif

// ReSharper disable InconsistentNaming

namespace SharpGEDParser
{
    public class GedParser
    {
        public GedParser(string gedPath)
        {
            // TODO dunno yet if the path to the GED is useful

            _IndiParseSingleton = new IndiParse();
            // TODO GEDCommon based Head record _HeadParseSingleton = new GedHeadParse();

            _FamParseSingleton = new FamParse();
            _SourParseSingleton = new SourceRecParse();
            _RepoParseSingleton = new RepoParse();
            _NoteParseSingleton = new NoteParse();
            _MediaParseSingleton = new MediaParse();
        }

#if PARALLEL
        public List<Task> _allTasks = new List<Task>();
#endif

        public void Wrap()
        {
#if PARALLEL
            Task.WaitAll(_allTasks.ToArray());
#endif
        }

        public GEDCommon Parse(GedRecord rec)
        {
            // Given a glop of lines which represent a 'record', parse it into GED data (INDI/FAM/NOTE/OBJE/REPO/SOUR/etc)
            Tuple<object, GedParse> parseSet = Make(rec);

            if (parseSet.Item2 == null) 
                return parseSet.Item1 as GEDCommon;
            GEDCommon recC2 = parseSet.Item1 as GEDCommon;
#if PARALLEL
            _allTasks.Add(Task.Run(() => parseSet.Item2.Parse(recC2, rec)));
#else
            parseSet.Item2.Parse(recC2, rec);
#endif
            return parseSet.Item1 as GEDCommon;
        }

        private Tuple<object, GedParse> Make(GedRecord rec)
        {
            // 1. The first line in the rec should start with '0'
            string head = rec.FirstLine();
            int firstDex = LineUtil.FirstChar(head);
            if (head[firstDex] != '0')
                throw new Exception("record head not zero"); // TODO should this be an error record instead?

            // 2. search for and find the tag
            LineUtil.LineData ld = new LineUtil.LineData(); // TODO static?
            LineUtil.LevelTagAndRemain(ld, head);

            // 3. create a KBRGedRec derived class
            return GedRecFactory(rec, ld.Ident, ld.Tag, ld.Remain);
        }

        private Tuple<object, GedParse> GedRecFactory(GedRecord rec, string ident, string tag, string remain)
        {
            // Parse 'top level' records. Parsing of some record types (e.g. NOTE, SOUR, etc) are likely to be in 'common' with sub-record parsing

            // TODO Very much brute force. If/until this is found to be optimizable
            switch (tag.ToUpper())
            {
                case "INDI":
                    return new Tuple<object, GedParse>(new IndiRecord(rec, ident, remain), _IndiParseSingleton);

                case "FAM":
                    return new Tuple<object, GedParse>(new FamRecord(rec, ident, remain), _FamParseSingleton);

                case "SOUR":
                    return new Tuple<object, GedParse>(new SourceRecord(rec, ident, remain), _SourParseSingleton);

                case "REPO":
                    return new Tuple<object, GedParse>(new Repository(rec, ident, remain), _RepoParseSingleton);

                case "NOTE":
                    return new Tuple<object, GedParse>(new NoteRecord(rec, ident, remain), _NoteParseSingleton);

                case "OBJE":
                    return new Tuple<object, GedParse>(new MediaRecord(rec, ident, remain), _MediaParseSingleton);

                case "SUBM": // TODO temp ignore
                case "HEAD": // TODO temp ignore
                case "SUBN": // TODO temp ignore
                default:  // TODO leading underscore signals a custom record
                {
                    var foo = new Unknown(rec, ident);
                    return new Tuple<object, GedParse>(foo, null);
                }
            }
        }

        private readonly GedParse _IndiParseSingleton;
        private readonly GedParse _FamParseSingleton;
        // TODO gedcommon based HEAD parsing private GedParse _HeadParseSingleton;
        private readonly GedParse _SourParseSingleton;
        private readonly GedParse _RepoParseSingleton;
        private readonly GedParse _NoteParseSingleton;
        private readonly GedParse _MediaParseSingleton;
    }

    public interface GedParse
    {
        void Parse(GEDCommon rec, GedRecord Lines);
    }
}

// TODO separation of parsing and container logic
// TODO export
// TODO details in 'head' may impact further parsing? ANSEL, ANSI, etc?
// TODO Heredis has a 'PLAC'/'FORM' tag in header which defines the format of places
