using SharpGEDParser;
using System;
using System.Collections.Generic;
using System.IO;

namespace BuildTree
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Specify a GED file.");
                return;
            }
            string path = args[0];
            if (!File.Exists(path))
            {
                Console.WriteLine("File doesn't exist");
                return;
            }

            // TODO exercise invalid .GED file

            var fr = new FileRead();
            fr.ReadGed(args[0]);
            BuildTree(fr.Data);
        }


        // recursively determine the ancestors of an individual from the FamilyUnits.
        // each person's Ahnen number is calculated.
        private static void DumpAnce(FamilyUnit firstFam, Dictionary<string, FamilyUnit> childHash, KBRGedIndi firstP, int myNum)
        {
            // From http://www.tamurajones.net/AhnenNumbering.xhtml : the Ahnen number 
            // of the father is double that of the current person. Mom's Ahnen number
            // is Dad's plus 1.

            int dadnum = myNum * 2;
            // Determine how many generations down the current person is.
            // Used at the moment for spacing, not really important.
            int depth = 0;
            while (dadnum > Math.Pow(2,depth)-1)
                depth++;

            string spacer = new string('.', depth-1);
            if (firstFam.Husband != null)
            {
                Console.WriteLine("{2}{0}: Dad: {1}", dadnum, firstFam.Husband.Names[0], spacer);
                if (firstFam.DadFam != null)
                    DumpAnce(firstFam.DadFam, childHash, firstFam.Husband, dadnum);
            }
            if (firstFam.Wife != null)
            {
                Console.WriteLine("{2}{0}: Mom: {1}", dadnum+1, firstFam.Wife.Names[0], spacer);
                if (firstFam.MomFam != null)
                    DumpAnce(firstFam.MomFam, childHash, firstFam.Wife, dadnum+1);
            }
        }
    }
}
