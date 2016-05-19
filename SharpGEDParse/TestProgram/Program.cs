﻿using System;
using System.Collections.Generic;
using System.IO;
using SharpGEDParser;

namespace TestProgram
{
    class Program
    {
        static void Main(string[] args)
        {
//            string fpath = args[0];
            //            new FileRead().ReadGed(fpath);

            string apath = @"E:\TestGeds";
            var files = Directory.GetFiles(apath, "*.ged");
            foreach (var afile in files)
            {
                Console.WriteLine(afile);
                var fr = new FileRead();
                fr.ReadGed(afile);
                dump(fr.Data);
            }
        }

        private static void dump(List<KBRGedRec> kbrGedRecs)
        {
            int errs = 0;
            int inds = 0;
            int fams = 0;
            int unks = 0;
            int oths = 0;
            foreach (var gedRec in kbrGedRecs)
            {
                errs += gedRec.Errors.Count; // TODO errors in sub-records
                if (gedRec is KBRGedIndi)
                    inds++;
                else if (gedRec is KBRGedFam)
                    fams++;
                else if (gedRec is KBRGedUnk)
                    unks++;
                else
                    oths++;
            }

            Console.WriteLine("\tINDI: {0}\n\tFAM: {1}\n\tUnknown: {2}\n\tOther: {3}\n****Errors: {4}", inds, fams, unks, oths, errs);
        }
    }
}
