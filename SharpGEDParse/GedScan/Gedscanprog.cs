// A q&d program to scan GEDCOM files and report statistics

using System.Linq;
using GEDWrap;
using SharpGEDParser;
using System;
using System.Collections.Generic;
using System.IO;
using SharpGEDParser.Model;

// TODO errors in sub-records
// TODO lost track of 'custom' records/sub-records?
// TODO Need to pull unknown text from original file?
// TODO don't repeat the same error more than once

// ReSharper disable InconsistentNaming

namespace GedScan
{
    class Gedscanprog
    {
        private static bool _showDiags;
        private static bool _dates;
        private static bool _ged; // show basic GED record stats
        private static bool _csv; // output to CSV
        private static bool _dumpAllErrors;
        private static int _bsize;
        private static bool _noforest; // Use sharpgedparser only, not the Forest

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: gedparse [-e] [-r] [-d] [-date] <.ged file OR folder>");
                Console.WriteLine("Specify a .GED file or path");
                Console.WriteLine("-e : show error/custom details");
                Console.WriteLine("-r : recurse through folders");
                Console.WriteLine("-d : show performance diagnostics");
                Console.WriteLine("-date : date parse testing");
                Console.WriteLine("-b : basic GED record stats");
                Console.WriteLine("-edump: dump all errors");
                Console.WriteLine("-csv: write to csv [distinct from -b or -date]");
                Console.WriteLine("-f : NO forest");
                return;
            }

            _showDiags = args.FirstOrDefault(s => s == "-d") != null;
            var showErrors = args.FirstOrDefault(s => s == "-e") != null;
            var recurse = args.FirstOrDefault(s => s == "-r") != null;
            _csv = args.FirstOrDefault(s => s == "-csv") != null;
            _dates = args.FirstOrDefault(s => s == "-date") != null;
            _ged = args.FirstOrDefault(s => s == "-b") != null;
            _dumpAllErrors = args.FirstOrDefault(s => s == "-edump") != null;
            _noforest = args.FirstOrDefault(s => s == "-f") != null;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-z")
                {
                    _bsize = int.Parse(args[i + 1]);
                    break;
                }
            }

            if (_bsize != 0)
                logit("", true);

            if (_csv)
                Console.WriteLine("filename,file KB,Millisec,Mem MB,# Lines,GED Version,Product1,Product2,Product Version,GED Date,Charset,INDI,FAM,SOUR,REPO,NOTE,OBJE,UNK,ERR,Note Len,Sub-Notes,Sub-Note Len");

            int lastarg = args.Length-1;
            if (File.Exists(args[lastarg]))
            {
                parseFile(args[lastarg], showErrors);
            }
            else if (Directory.Exists(args[lastarg]))
            {
                parseTree(args[lastarg], recurse, showErrors);
            }
            else
            {
                Console.WriteLine("Specified file/path doesn't exist!");
            }
            if (_bsize != 0)
                logit("__All load complete (ms)");
        }

        private static void parseTree(string path, bool recurse, bool showErrors)
        {
            var files = Directory.GetFiles(path, "*.ged");
            if (files.Length > 0 && !_csv)
                Console.WriteLine("---{0}", path); // Only echo path if it has GED files

            foreach (var aFile in files)
            {
                parseFile(aFile, showErrors);
            }

            if (recurse)
            {
                var folders = Directory.GetDirectories(path);
                foreach (var aFolder in folders)
                {
                    parseTree(aFolder, true, showErrors);
                }
            }
        }

        private static int tick;
        private static int logit(string msg, bool first = false)
        {
            if (first)
                tick = Environment.TickCount;
            else
            {
                int delta = Environment.TickCount - tick;
                if (_showDiags || _bsize != 0)
                    Console.WriteLine(msg + "|" + delta);
                return delta;
            }
            return 0;
        }

        private static void parseFile(string path, bool showErrors)
        {
            long beforeMem = GC.GetTotalMemory(true);
            if (_bsize == 0)
                logit("",true);

            if (!_csv)
                Console.WriteLine("   {0}", Path.GetFileName(path));

            Console.Out.Flush();

            if (_noforest)
            {
                using (FileRead reader = new FileRead(_bsize))
                {
                    reader.ReadGed(path);
                    long afterMem = GC.GetTotalMemory(false);
                    int ms = 0;
                    if (_bsize == 0)
                        ms = logit("__load complete (ms)");
                    long delta = (afterMem - beforeMem);
                    double meg = delta / (1024 * 1024.0);
                    if (_showDiags)
                    {
                        Console.WriteLine("\t===Memory:{0:0.00}M", meg);
                        long maxMem = Process.GetCurrentProcess().PeakWorkingSet64;
                        double maxM = maxMem / (1024 * 1024.0);
                        Console.WriteLine("\t===MaxWSet:{0:0.00}M", maxM);
                    }
                    dump(reader.Data, reader.AllErrors, null, false);
                }
            }
            else
            {
                using (Forest f = new Forest())
                {
                    f.LoadGEDCOM(path, _bsize);

                    var fil = File.OpenRead(path);
                    long flen = fil.Length;
                    fil.Close();
                    double fkb = flen/(1024.0);
                
                    long afterMem = GC.GetTotalMemory(false);
                    int ms = 0;
                    if (_bsize == 0)
                        ms = logit("__load complete (ms)");
                    long delta = (afterMem - beforeMem);
                    double meg = delta / (1024 * 1024.0);
                    if (_showDiags)
                    {
                        Console.WriteLine("\t===Memory:{0:0.00}M", meg);
                        long maxMem = Process.GetCurrentProcess().PeakWorkingSet64;
                        double maxM = maxMem/(1024*1024.0);
                        Console.WriteLine("\t===MaxWSet:{0:0.00}M", maxM);
                    }

                    if (_dates)
                        dumpDates(f);
                    else if (_ged)
                        dump(f.AllRecords, f.Errors, f.Issues, showErrors);
                    else if (_csv)
                        dumpCSV(Path.GetFileNameWithoutExtension(path), f, ms, meg, fkb);
                    else
                        dump(f, showErrors);
                    if (_dumpAllErrors)
                        dumpAllErrors(null, f);
                }
            }

            Console.Out.Flush();
        }

        private static void incr(Dictionary<string, int> dict, string key)
        {
            if (string.IsNullOrEmpty(key))
                key = "<empty>";
            if (dict.ContainsKey(key))
            {
                int val = dict[key];
                val++;
                dict[key] = val;
            }
            else
            {
                dict.Add(key, 1);
            }
        }

        private enum DateState { Success=0, WftEst, Fail, FailAndDump, Empty, Exact }
        private static DateState dumpDate(EventCommon evt)
        {
            if (evt.GedDate != null && evt.GedDate.Type != GEDDate.Types.Unknown)
                return evt.GedDate.Type == GEDDate.Types.Exact ? DateState.Exact : DateState.Success;

            // skip empty or successful dates
            if (evt.GedDate == null || evt.GedDate.Type != GEDDate.Types.Unknown || string.IsNullOrWhiteSpace(evt.Date))
                return DateState.Empty;

            // Ignore common entries
            string[] skip = {"unknown", "private", "infant", "deceased", "dead", "not married", "child", "died", 
                             "young", "stillborn", "never married","seenotes","nao informado"};
            string test = (evt.Date.Trim().ToLower());
            if (test.StartsWith("wft est"))
                return DateState.WftEst;

            if (skip.Any(s => s == test))
                return DateState.Fail;
            //Console.WriteLine("\t{0}", evt.Date);
            return DateState.FailAndDump;
        }

        private static void IncrCount(DateState state, int[] counts)
        {
            int dex = (int) state;
            counts[dex]++;
        }

        private static void dumpDates(Forest f)
        {
            int [] counts = new int[6];

            //bool hasWFTEst = false;
            foreach (var person in f.AllPeople)
            {
                IndiRecord ged = person.Indi;
                foreach (var familyEvent in ged.Events)
                {
                    var state = dumpDate(familyEvent);
                    IncrCount(state, counts);
                }
                foreach (var familyEvent in ged.Attribs)
                {
                    var state = dumpDate(familyEvent);
                    IncrCount(state, counts);
                }
            }
            foreach (var union in f.AllUnions)
            {
                FamRecord ged = union.FamRec;
                foreach (var familyEvent in ged.FamEvents)
                {
                    var state = dumpDate(familyEvent);
                    IncrCount(state, counts);
                }
            }

            Console.WriteLine("\t        Success:{0}", counts[0]);
            Console.WriteLine("\tSuccess (exact):{0}", counts[5]);
            Console.WriteLine("\t        WFT Est:{0}", counts[1]);
            Console.WriteLine("\t           Fail:{0}", counts[2]);
            Console.WriteLine("\t      Fail/Dump:{0}", counts[3]);
            //if (hasWFTEst)
            //    Console.WriteLine("\tWFT Est not shown");
        }

        private static void dump(Forest f, bool showErrors)
        {
            if (f.Errors.Count > 0)
            {
                foreach (var unkRec in f.Errors)
                {
                    if (unkRec.Error == UnkRec.ErrorCode.EmptyFile)
                    {
                        Console.WriteLine("Empty file");
                        return;
                    }
                }
            }

            if (f.AllRecords.Count == 0)
            {
                Console.WriteLine("*****Failed to parse");
                return;
            }

            Dictionary<string, int> tagCounts = new Dictionary<string, int>();
            foreach (var record in f.AllRecords)
            {
                incr(tagCounts, record.Tag);
            }

            Dictionary<string, int> indiEventCounts = new Dictionary<string, int>();
            int indiEventLoc = 0;
            Dictionary<string, int> indiAttribCounts = new Dictionary<string, int>();
            int attribLoc = 0;
            Dictionary<string, int> famEventCounts = new Dictionary<string, int>();
            int famEventLoc = 0;
            foreach (var person in f.AllPeople)
            {
                IndiRecord ged = person.Indi;
                foreach (var familyEvent in ged.Events)
                {
                    string tag = familyEvent.Tag;
                    incr(indiEventCounts, tag);
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                        indiEventLoc++;
                }
                foreach (var familyEvent in ged.Attribs)
                {
                    string tag = familyEvent.Tag;
                    incr(indiAttribCounts, tag);
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                        attribLoc++;
                }
            }

            foreach (var union in f.AllUnions)
            {
                FamRecord ged = union.FamRec;
                foreach (var familyEvent in ged.FamEvents)
                {
                    string tag = familyEvent.Tag;
                    incr(famEventCounts, tag);
                    if (!string.IsNullOrEmpty(familyEvent.Place))
                        famEventLoc++;
                }
            }

            HeadRecord head = f.Header;
            if (head == null)
                Console.WriteLine("No head");
            else
                Console.WriteLine("  {0}-{1}:{2} ({3})", head.GedVersion, head.Product, head.ProductVersion, head.GedDate.ToString("yyyyMMdd"));
            Console.Write("\t");
            foreach (var tag in tagCounts.Keys)
            {
                if (!string.IsNullOrEmpty(tag))
                    Console.Write("{0}:{1};", tag, tagCounts[tag]);
            }
            Console.WriteLine();
            Console.WriteLine("\t\t----------");
            Console.Write("\t");
            foreach (var tag in indiEventCounts.Keys)
            {
                Console.Write("{0}:{1};", tag, indiEventCounts[tag]);
            }
            if (indiEventLoc > 0)
                Console.Write("Locations:{0}", indiEventLoc);
            Console.WriteLine();
            Console.WriteLine("\t\t----------");
            //foreach (var tag in indiAttribCounts.Keys)
            //{
            //    Console.WriteLine("\t\t{0}:{1}", tag, indiAttribCounts[tag]);
            //}
            //if (attribLoc > 0)
            //    Console.WriteLine("\t\tLocations:{0}", attribLoc);
            //Console.WriteLine("\t\t----------");
            Console.WriteLine("\t\t----------");
            Console.Write("\t");
            foreach (var tag in famEventCounts.Keys)
            {
                Console.Write("{0}:{1};", tag, famEventCounts[tag]);
            }
            if (famEventLoc > 0)
                Console.Write("Locations:{0}", famEventLoc);
            Console.WriteLine();
            if (f.NumberOfTrees > 1)
                Console.WriteLine("Number of trees:{0}", f.NumberOfTrees);

        }

        private static void dump(IEnumerable<GEDCommon> kbrGedRecs, List<UnkRec> errors, IEnumerable<Issue> issues, bool showErrors)
        {
            if (kbrGedRecs == null)
                return;

            int errs = errors == null ?0 : errors.Count;
            int inds = 0;
            int fams = 0;
            int unks = 0;
            int oths = 0;
            int src = 0;
            int repo = 0;
            int note = 0;
            int media = 0;

            int nLen = 0; // total length of NOTE record text

            int subN = 0; // NOTE sub-records
            int subNLen = 0; // total length of sub-record note text

            var errRollup = new Dictionary<int, UnkRec>();
            var unkRollup = new Dictionary<string, UnkRec>();
            var unkRecRollup = new Dictionary<string, GEDCommon>();

            // int index = 0; // testing
            foreach (var gedRec2 in kbrGedRecs)
            {
                //if (index == 52790) // wemightbekin testing
                //    Debugger.Break();

                errs += gedRec2.Errors.Count; // TODO errors in sub-records
                if (gedRec2.Errors.Count > 0 && showErrors)
                {
                    foreach (var errRec in gedRec2.Errors)
                    {
                        if (!errRollup.ContainsKey((int)errRec.Error))
                            errRollup.Add((int)errRec.Error, errRec);
                        //Console.WriteLine("\t\tError:{0}", errRec.Error);
                    }
                }
                unks += gedRec2.Unknowns.Count;
                if (gedRec2.Unknowns.Count > 0 && showErrors)
                {
                    foreach (var errRec in gedRec2.Unknowns)
                    {
                        if (!unkRollup.ContainsKey(errRec.Tag ?? ""))
                            unkRollup.Add(errRec.Tag ?? "", errRec);
                        //Console.WriteLine("\t\tUnknown:{0} at line {2} in {1}", errRec.Tag, gedRec2, errRec.Beg);
                    }
                }

                if (gedRec2 is NoteHold)
                {
                    var gr = gedRec2 as NoteHold;
                    subN += gr.Notes.Count;
                    foreach (var anote in gr.Notes)
                    {
                        subNLen += anote.Text.Length;
                    }
                }
                // TODO this is awkward
                if (gedRec2 is IndiRecord)
                    inds++;
                else if (gedRec2 is FamRecord) //KBRGedFam)
                    fams++;
                else if (gedRec2 is SourceRecord)
                    src++;
                else if (gedRec2 is Repository)
                    repo++;
                else if (gedRec2 is NoteRecord)
                {
                    note++;
                    var gr = gedRec2 as NoteRecord;
                    nLen += gr.Text.Length;
                }
                else if (gedRec2 is MediaRecord)
                    media++;
                else if (gedRec2 is Unknown)
                {
                    if (showErrors)
                    {
                        string tag = gedRec2.Tag;
                        switch (tag)
                        {
                            case "HEAD": // TODO known NYI
                            case "SUBM":
                            case "SUBN":
                            case "TRLR":
                                break;
                            default:
                                if (!unkRecRollup.ContainsKey(gedRec2.Tag))
                                    unkRecRollup.Add(gedRec2.Tag, gedRec2);
                                //Console.WriteLine("\t\tUnknown:\"{0}\"[{1}:{2}]", gedRec2.Tag, gedRec2.BegLine, gedRec2.EndLine);
                                break;
                        }
                    }
                    unks++;
                }
                else
                {
                    if (showErrors)
                        Console.WriteLine("\t\tOther:{0}", gedRec2);
                    oths++;
                }

                // index++; // testing
            }

            var issRollup = new Dictionary<Issue.IssueCode, string>();
            if (showErrors)
            {
                //foreach (var err in errCounts.Keys)
                //{
                //    var bucket = errCounts[err];
                //    Console.WriteLine("\t\tError:{0} Count:{1} First:{2}", err, bucket.count, (bucket.firstOne as UnkRec).Beg);
                //}

                foreach (var errRec in errors)
                {
                    if (!errRollup.ContainsKey((int)errRec.Error))
                        errRollup.Add((int)errRec.Error, errRec);
                    //Console.WriteLine("\t\tError:{0}", unkRec.Error);
                }
                if (issues != null)
                    foreach (var issue in issues)
                    {
                        if (!issRollup.ContainsKey(issue.IssueId))
                            issRollup.Add(issue.IssueId, issue.Message());
                    }
            }
            Console.WriteLine("\tINDI: {0}\n\tFAM: {1}\n\tSource: {5}\n\tRepository:{6}\n\tNote: {7}[{10}]\n\tUnknown: {2}\n\tMedia: {9}\n\tOther: {3}\n\t*Errors: {4}; Sub-Notes:{11}[{12}]", 
                inds, fams, unks, oths, errs, src, repo, note, 0, media, nLen, subN, subNLen);
            Console.WriteLine("\tAvg note len:{0}\tAvg sub-note len:{1}\tSub-note len ratio:{2}",
                nLen / (note==0?1:note), subNLen / (subN==0?1:subN), subNLen / ((fams+inds) == 0 ? 1 : (fams+inds)));
            if (showErrors)
            {
                foreach (var err in issRollup)
                {
                    Console.WriteLine("\t\tIss:{0}", err.Value);
                }
                foreach (var err in errRollup)
                {
                    Console.WriteLine("\t\tErr:{0} [line {1}]", err.Key, err.Value.Beg);
                }
                foreach (var err in unkRollup)
                {
                    Console.WriteLine("\t\tUnk:{0} [line {1}]", err.Key, err.Value.Beg);
                }
                foreach (var rec in unkRecRollup)
                {
                    Console.WriteLine("\t\tUnk record:\"{0}\"[{1}:{2}]", rec.Key, rec.Value.BegLine, rec.Value.EndLine);
                }
            }
            Console.WriteLine(new string('-',50));
        }

        private static void dumpAllErrors(string filename, Forest f)
        {
            if (f.AllRecords == null) // empty, failure to parse, ?
                return;
            int counter = 0;

            // f.Errors is a rollup of f.AllRecords.Errors; ditto for unks
            // TODO errors in sub-records?

            foreach (var err in f.Errors)
            {
                var text = err.Error >= UnkRec.ErrorCode.Exception ? err.Error.ToString() : err.Tag;
                Console.WriteLine("\t\tErr:{0} [line {1}]", text, err.Beg);
                counter++;
            }
            foreach (var err in f.Issues)
            {
                Console.WriteLine("\t\tIss:{0} [Id {1}]", err.Message(), err.IssueId);
                counter++;
            }
            foreach (var err in f.Unknowns)
            {
                var text = err.Error >= UnkRec.ErrorCode.Exception ? err.Error.ToString() : err.Tag;
                Console.WriteLine("\t\tUnk:{0} [lines {1}:{2}]", text, err.Beg, err.End);
                counter++;
            }
            bool customRecord = false;
            foreach (var gedRec2 in f.AllRecords)
            {
                //foreach (var err in gedRec2.Errors)
                //{
                //    var text = err.Error >= UnkRec.ErrorCode.Exception ? err.Error.ToString() : err.Tag;
                //    Console.WriteLine("\t\tErr:{0} [line {1}]", text, err.Beg);
                //    counter++;
                //}
                //foreach (var err in gedRec2.Unknowns)
                //{
                //    var text = err.Error >= UnkRec.ErrorCode.Exception ? err.Error.ToString() : err.Tag;
                //    Console.WriteLine("\t\tUnk:{0} [line {1}]", text, err.Beg);
                //    counter++;
                //}
                if (gedRec2 is Unknown)
                {
                    if (gedRec2.Tag.StartsWith("_"))
                        customRecord = true;
                    else
                    {
                        Console.WriteLine("\t\tUnk record:\"{0}\"[{1}:{2}]", gedRec2.Tag, gedRec2.BegLine,
                            gedRec2.EndLine);
                        counter++;
                    }
                }
            }
            if (customRecord)
                Console.WriteLine("\t\tOne or more custom records observed.");
            Console.WriteLine("total:{0}", counter);
        }

        private static void dumpCSV(string filename, Forest f, int ms, double meg, double fmeg)
        {
            if (f.AllRecords == null) // empty, failure to parse, ?
                return;

            int inds = 0;
            int fams = 0;
            int src = 0;
            int repo = 0;
            int note = 0;
            int media = 0;
            int errs = f.Errors == null ? 0 : f.Errors.Count; // TODO ErrorsCount was 0 but Errors was not empty?
            int unks = f.Unknowns == null ? 0 : f.Unknowns.Count;

            int nLen = 0; // total length of NOTE record text
            int subN = 0; // NOTE sub-records
            int subNLen = 0; // total length of sub-record note text

            foreach (var gedRec2 in f.AllRecords)
            {
                //if (index == 52790) // wemightbekin testing
                //    Debugger.Break();

                // Forest has already wrapped up errors/unknowns.
                //errs += gedRec2.Errors.Count; 
                //unks += gedRec2.Unknowns.Count;

                if (gedRec2 is NoteHold)
                {
                    var gr = gedRec2 as NoteHold;
                    subN += gr.Notes.Count;
                    foreach (var anote in gr.Notes)
                    {
                        subNLen += anote.Text.Length;
                    }
                }
                // TODO this is awkward
                if (gedRec2 is IndiRecord)
                    inds++;
                else if (gedRec2 is FamRecord)
                    fams++;
                else if (gedRec2 is SourceRecord)
                    src++;
                else if (gedRec2 is Repository)
                    repo++;
                else if (gedRec2 is NoteRecord)
                {
                    note++;
                    var gr = gedRec2 as NoteRecord;
                    nLen += gr.Text.Length;
                }
                else if (gedRec2 is MediaRecord)
                    media++;
                else if (gedRec2 is Unknown)
                {
                    unks++;
                }

                // index++; // testing
            }

            var headr = f.Header;
            string sauce;
            string gedv;
            string prod;
            string prodv;
            string gedD;
            string chrS;
            if (headr == null)
            {
                sauce = gedv = prod = prodv = gedD = chrS = "NO HEAD";
            }
            else
            {
                sauce = headr.Source.Replace('\"',' ').Trim();
                gedv =  headr.GedVersion;
                prod =  headr.Product.Replace('\"',' ').Trim();
                prodv = headr.ProductVersion.Replace('\"', ' ').Trim();
                gedD =  headr.GedDate.ToString("yyyyMMdd");
                chrS =  headr.CharSet;
            }

            // filename,"file KB","Millisec","Mem MB","# Lines","GED Version","Product","Product","Product Version", "GED Date","Charset",
            // INDI,FAM,SOUR,REPO,NOTE,OBJE,UNK,ERR,Note Len,Sub-Notes,Sub-Note Len
            Console.WriteLine("\"{0}.ged\",{1:0.#},{2},{3:0.#},{21},\"{4}\",\"{20}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
                filename,fmeg,ms,meg,gedv, // 0-4
                prod, prodv,gedD, chrS,inds, // 5-9
                fams,src,repo,note,media, // 10-14
                unks,errs,nLen,subN,subNLen, // 15-19
                sauce, f.NumberOfLines);
        }

    }
}
