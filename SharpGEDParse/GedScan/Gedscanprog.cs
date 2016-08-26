// A q&d program to scan GEDCOM files and report statistics

using System.Diagnostics;
using System.Linq;
using SharpGEDParser;
using System;
using System.Collections.Generic;
using System.IO;
using SharpGEDParser.Model;

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
            using (var fr = new FileRead())
            {
                fr.ReadGed(path);
                dump(fr.Data, fr.Errors, showErrors);
                logit("\t---Ticks");
                afterMem = GC.GetTotalMemory(false);
            }

            if (_showDiags)
                Console.WriteLine("\t===Memory:{0}", (afterMem-beforeMem));
        }

        private static void dump(List<object> kbrGedRecs, List<UnkRec> errors, bool showErrors)
        {
            // TODO showErrors and errors list

            int errs = errors.Count;
            int inds = 0;
            int fams = 0;
            int unks = 0;
            int oths = 0;
            int src = 0;
            int repo = 0;
            int note = 0;
            int custom = 0;
            int media = 0;

            KBRGedRec gedRec;
            GEDCommon gedRec2 = null;
            int index = 0;
            foreach (var gedRec0 in kbrGedRecs)
            {
                //if (index == 52790) // wemightbekin testing
                //    Debugger.Break();

                gedRec = gedRec0 as KBRGedRec;
                if (gedRec == null)
                {
                    gedRec2 = gedRec0 as GEDCommon;
                    if (gedRec2 == null)
                        continue;
                }
                else
                {
                    gedRec2 = null;
                }

                if (gedRec != null && !(gedRec is KBRGedHead || gedRec is GedSubm)) // TODO skip head and subm for now
                {
                    errs += gedRec.Errors.Count; // TODO errors in sub-records

                    if (gedRec.Errors.Count > 0 && showErrors)
                    {
                        foreach (var errRec in gedRec.Errors)
                        {
                            Console.WriteLine("\t\tError:{0}", errRec.Error);
                        }
                    }
                    unks += gedRec.Unknowns.Count;
                    if (gedRec.Unknowns.Count > 0 && showErrors)
                    {
                        foreach (var errRec in gedRec.Unknowns)
                        {
                            Console.WriteLine("\t\tUnknown:{0} in {1}", errRec.Tag, gedRec);
                        }
                    }
                }

                if (gedRec2 != null)
                {
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
                }

                // TODO this is awkward
                if (gedRec is KBRGedIndi)
                    inds++;
                else if (gedRec2 is FamRecord) //KBRGedFam)
                    fams++;
                else if (gedRec2 is SourceRecord)
                    src++;
                else if (gedRec2 is Repository) // TODO ????
                    repo++;
                else if (gedRec2 is NoteRecord) // TODO ????
                    note++;
                else if (gedRec2 is MediaRecord)
                    media++;
                else if (gedRec is KBRGedUnk)
                {
                    if (showErrors)
                    {
                        if (gedRec.Lines == null)
                            Console.WriteLine("Empty record!");
                        else
                            Console.WriteLine("\t\tUnknown:\"{0}\"[{1}:{2}]", gedRec.Lines.FirstLine(), gedRec.Lines.Beg, gedRec.Lines.End);
                    }
                    unks++;
                }
                else if (gedRec is KBRGedHead ||
                         gedRec is GedSubm)
                {
                    // ignore
                }
                else
                {
                    if (showErrors)
                        Console.WriteLine("\t\tOther:{0}", gedRec);
                    oths++;
                }

                index++;
            }

            if (showErrors)
            {
                foreach (var unkRec in errors)
                {
                    Console.WriteLine("\t\tError:{0}", unkRec.Error);
                }
            }
            Console.WriteLine("\tINDI: {0}\n\tFAM: {1}\n\tSource: {5}\n\tRepository:{6}\n\tNote: {7}\n\tCustom: {8}\n\tUnknown: {2}\n\tMedia: {9}\n\tOther: {3}\n\t*Errors: {4}", inds, fams, unks, oths, errs, src, repo, note, custom, media);
        }

    }
}
