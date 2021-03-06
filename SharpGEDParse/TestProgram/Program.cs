﻿using System;
using System.Collections.Generic;
using System.IO;
using SharpGEDParser;
using SharpGEDParser.Model;
using SharpGEDWriter;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
            string fpath = args[0];
            var fr = new FileRead();
            fr.ReadGed(fpath);

            string opath = Path.ChangeExtension(fpath, "ged_out");
            FileWrite.WriteGED(fr.Data, opath);
            

            //string apath = @"E:\TestGeds";
            //string apath = @"Z:\HOST_E\projects\GED\GED files\Ged too big\2524482.ged";
            //Console.WriteLine(apath);
            //dump(fr.Data);

            //var files = Directory.GetFiles(apath, "*.ged");
            //foreach (var afile in files)
            //{
            //    Console.WriteLine(afile);
            //    var fr = new FileRead();
            //    fr.ReadGed(afile);
            //    dump(fr.Data);
            //}
        }

        private static void dump(IEnumerable<GEDCommon> kbrGedRecs)
        {
            int errs = 0;
            int inds = 0;
            int fams = 0;
            int unks = 0;
            int oths = 0;
            foreach (var gedRec in kbrGedRecs)
            {
                errs += gedRec.Errors.Count;
                if (gedRec is IndiRecord)
                    inds++;
                else if (gedRec is FamRecord)
                    fams++;
                else if (gedRec is Unknown)
                    unks++;
                else
                    oths++;
            }

            Console.WriteLine("\tINDI: {0}\n\tFAM: {1}\n\tUnknown: {2}\n\tOther: {3}\n****Errors: {4}", inds, fams, unks, oths, errs);
        }
    }
}
