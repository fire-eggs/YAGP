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

namespace GedScan
{
    class Gedscanprog
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: gedparse [-e] [-r] [-d] <.ged file OR folder>");
                Console.WriteLine("Specify a .GED file or path");
                Console.WriteLine("-e : show error/custom details");
                Console.WriteLine("-r : recurse through folders");
                Console.WriteLine("-d : show performance diagnostics");
                return;
            }

            _showDiags = args.FirstOrDefault(s => s == "-d") != null;
            var showErrors = args.FirstOrDefault(s => s == "-e") != null;
            var recurse = args.FirstOrDefault(s => s == "-r") != null;
            var csv = args.FirstOrDefault(s => s == "-csv") != null;

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

        private static bool _showDiags;

        private static int tick;
        private static void logit(string msg, bool first = false)
        {
            int delta = 0;
            if (first)
                tick = Environment.TickCount;
            else
            {
                delta = Environment.TickCount - tick;
                if (_showDiags)
                    Console.WriteLine(msg + "|" + delta);
            }
        }

        private static void parseFile(string path, bool showErrors)
        {
            long beforeMem = GC.GetTotalMemory(true);
            long afterMem;
            Console.WriteLine("   {0}", Path.GetFileName(path));
            logit("", true);
            using (Forest f = new Forest())
            {
                f.LoadGEDCOM(path);
                afterMem = GC.GetTotalMemory(false);
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

        private static void dump(IEnumerable<GEDCommon> kbrGedRecs, List<UnkRec> errors, bool showErrors)
        {
            int errs = errors.Count;
            int inds = 0;
            int fams = 0;
            int unks = 0;
            int oths = 0;
            int src = 0;
            int repo = 0;
            int note = 0;
            int media = 0;

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
                        Console.WriteLine("\t\tError:{0}", errRec.Error);
                    }
                }
                unks += gedRec2.Unknowns.Count;
                if (gedRec2.Unknowns.Count > 0 && showErrors)
                {
                    foreach (var errRec in gedRec2.Unknowns)
                    {
                        Console.WriteLine("\t\tUnknown:{0} at line {2} in {1}", errRec.Tag, gedRec2, errRec.Beg);
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
                    note++;
                else if (gedRec2 is MediaRecord)
                    media++;
                else if (gedRec2 is Unknown)
                {
                    if (showErrors)
                    {
                        //if (gedRec2.Unknowns.  .Lines == null)
                        //    Console.WriteLine("Empty record!");
                        //else
                            Console.WriteLine("\t\tUnknown:\"{0}\"[{1}:{2}]", "<need to pull from original file>", gedRec2.BegLine, gedRec2.EndLine);
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

            if (showErrors)
            {
                //foreach (var err in errCounts.Keys)
                //{
                //    var bucket = errCounts[err];
                //    Console.WriteLine("\t\tError:{0} Count:{1} First:{2}", err, bucket.count, (bucket.firstOne as UnkRec).Beg);
                //}

                foreach (var unkRec in errors)
                {
                    Console.WriteLine("\t\tError:{0}", unkRec.Error);
                }
            }
            Console.WriteLine("\tINDI: {0}\n\tFAM: {1}\n\tSource: {5}\n\tRepository:{6}\n\tNote: {7}\n\tUnknown: {2}\n\tMedia: {9}\n\tOther: {3}\n\t*Errors: {4}", inds, fams, unks, oths, errs, src, repo, note, 0, media);
        }

    }
}
