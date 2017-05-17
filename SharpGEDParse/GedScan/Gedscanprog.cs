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
                return;
            }

            _showDiags = args.FirstOrDefault(s => s == "-d") != null;
            var showErrors = args.FirstOrDefault(s => s == "-e") != null;
            var recurse = args.FirstOrDefault(s => s == "-r") != null;
            //var csv = args.FirstOrDefault(s => s == "-csv") != null;
            _dates = args.FirstOrDefault(s => s == "-date") != null;
            _ged = args.FirstOrDefault(s => s == "-b") != null;

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
        }

        private static void parseTree(string path, bool recurse, bool showErrors)
        {
            var files = Directory.GetFiles(path, "*.ged");
            if (files.Length > 0)
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
        private static void logit(string msg, bool first = false)
        {
            if (first)
                tick = Environment.TickCount;
            else
            {
                int delta = Environment.TickCount - tick;
                if (_showDiags)
                    Console.WriteLine(msg + "|" + delta);
            }
        }

        private static void parseFile(string path, bool showErrors)
        {
            long beforeMem = 0;
            long afterMem = 0;

            if (_showDiags)
            {
                beforeMem = GC.GetTotalMemory(true);
                logit("",true);
            }

            Console.WriteLine("   {0}", Path.GetFileName(path));
            Console.Out.Flush();
            using (Forest f = new Forest())
            {
                f.LoadGEDCOM(path);
                if (_showDiags)
                {
                    afterMem = GC.GetTotalMemory(false);
                    logit("__load complete (ms)");
                }
                if (_dates)
                    dumpDates(f);
                else if (_ged)
                    dump(f.AllRecords, f.Errors, f.Issues, showErrors);
                else
                    dump(f, showErrors);
            }
            //using (var fr = new FileRead())
            //{
            //    fr.ReadGed(path);
            //    dump(fr.Data, fr.Errors, showErrors);
            //    logit("\t---Ticks");
            //    afterMem = GC.GetTotalMemory(false);
            //}

            if (_showDiags)
            {
                long delta = (afterMem - beforeMem);
                double meg = delta/(1024*1024.0);
                Console.WriteLine("\t===Memory:{0:0.00}M", meg);
            }
            Console.Out.Flush();
        }

        //struct errBucket
        //{
        //    public int count;
        //    public object firstOne;
        //}

        private static void incr(Dictionary<string, int> dict, string key)
        {
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
            foreach (var tag in tagCounts.Keys)
            {
                if (!string.IsNullOrEmpty(tag))
                    Console.WriteLine("\t\t{0}:{1}", tag, tagCounts[tag]);
            }
            Console.WriteLine("\t\t----------");
            foreach (var tag in indiEventCounts.Keys)
            {
                Console.WriteLine("\t\t{0}:{1}", tag, indiEventCounts[tag]);
            }
            if (indiEventLoc > 0)
                Console.WriteLine("\t\tLocations:{0}", indiEventLoc);
            Console.WriteLine("\t\t----------");
            foreach (var tag in indiAttribCounts.Keys)
            {
                Console.WriteLine("\t\t{0}:{1}", tag, indiAttribCounts[tag]);
            }
            if (attribLoc > 0)
                Console.WriteLine("\t\tLocations:{0}", attribLoc);
            Console.WriteLine("\t\t----------");
            foreach (var tag in famEventCounts.Keys)
            {
                Console.WriteLine("\t\t{0}:{1}", tag, famEventCounts[tag]);
            }
            if (famEventLoc > 0)
                Console.WriteLine("\t\tLocations:{0}", famEventLoc);
        }

        private static void dump(IEnumerable<GEDCommon> kbrGedRecs, List<UnkRec> errors, IEnumerable<Issue> issues, bool showErrors)
        {
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

    }
}
