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
    public class KBRGedParser
    {
        public KBRGedParser(string gedPath)
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

        public object Parse(GedRecord rec)
        {
            // Given a glop of lines which represent a 'record', parse it into GED data (INDI/FAM/NOTE/OBJE/REPO/SOUR/etc)
            Tuple<object, GedParse> parseSet = Make(rec);

            if (parseSet.Item2 == null) 
                return parseSet.Item1;
            GEDCommon recC2 = parseSet.Item1 as GEDCommon;
#if PARALLEL
            _allTasks.Add(Task.Run(() => parseSet.Item2.Parse(recC2, rec)));
#else
            parseSet.Item2.Parse(recC2, rec);
#endif
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
            string remain = "";
            char level = ' ';
            GedLineUtil.LevelTagAndRemain(head, ref level, ref ident, ref tag, ref remain);
            //GedLineUtil.IdentAndTag(head, firstDex + 1, ref ident, ref tag);

            // 3. create a KBRGedRec derived class
            return GedRecFactory(rec, ident, tag, remain);
        }

        private Tuple<object, GedParse> GedRecFactory(GedRecord rec, string ident, string tag, string remain)
        {
            // Parse 'top level' records. Parsing of some record types (e.g. NOTE, SOUR, etc) are likely to be in 'common' with sub-record parsing

            // TODO Very much brute force. If/until this is found to be optimizable
            switch (tag.ToUpper())
            {
                case "INDI":
                {
                    var foo = new IndiRecord(rec, ident);
                    NonStandardRemain(remain, foo);
                    return new Tuple<object, GedParse>(foo, _IndiParseSingleton);
                }
                    //data = new KBRGedIndi(rec, ident);
                    //return new Tuple<object, GedParse>(data, _IndiParseSingleton);
                case "FAM":
                {
                    var foo = new FamRecord(rec, ident);
                    NonStandardRemain(remain, foo);
                    return new Tuple<object, GedParse>(foo, _FamParseSingleton);
                }
                case "SOUR":
                {
                    var foo = new SourceRecord(rec, ident);
                    NonStandardRemain(remain, foo);
                    return new Tuple<object, GedParse>(foo, _SourParseSingleton);
                }
                case "REPO":
                {
                    var foo = new Repository(rec, ident);
                    NonStandardRemain(remain, foo);
                    return new Tuple<object, GedParse>(foo, _RepoParseSingleton);
                }
                case "NOTE":
                {
                    var foo = new NoteRecord(rec, ident, remain);
                    return new Tuple<object, GedParse>(foo, _NoteParseSingleton);
                }
                case "OBJE":
                {
                    var foo = new MediaRecord(rec, ident);
                    NonStandardRemain(remain, foo);
                    return new Tuple<object, GedParse>(foo, _MediaParseSingleton);
                }
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

        private void NonStandardRemain(string remain, GEDCommon rec)
        {
            // Extra text on the record line (e.g. "0 @R1@ REPO blah blah blah") is not standard for
            // most record types. Preserve it as a note if possible.
            if (!string.IsNullOrWhiteSpace(remain))
            {
                UnkRec err = new UnkRec();
                err.Beg = err.End = rec.BegLine;
                err.Error = string.Format("Non-standard extra text: '{0}'", remain);
                rec.Errors.Add(err);

                if (rec is NoteHold)
                {
                    Note not = new Note();
                    not.Text = remain;
                    (rec as NoteHold).Notes.Add(not);
                }
            }
            
        }
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
