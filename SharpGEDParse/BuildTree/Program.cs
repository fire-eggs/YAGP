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

        private static void BuildTree(List<KBRGedRec> gedRecs)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            var indiHash = new Dictionary<string, KBRGedIndi>();
            var famHash = new Dictionary<string, KBRGedFam>();
            string first = null;
            foreach (var kbrGedRec in gedRecs)
            {
                if (kbrGedRec is KBRGedIndi)
                {
                    indiHash.Add(kbrGedRec.Ident, kbrGedRec as KBRGedIndi);
                    if (first == null)
                        first = kbrGedRec.Ident;
                }
                if (kbrGedRec is KBRGedFam)
                    famHash.Add(kbrGedRec.Ident, kbrGedRec as KBRGedFam);
            }

            // hash: child ids -> familyunit
            var childHash = new Dictionary<string, FamilyUnit>();

            // Accumulate family units
            // TODO indi with no fam
            // TODO indi with fams/famc only
            foreach (var kbrGedFam in famHash.Values)
            {
                var famU = new FamilyUnit(kbrGedFam);
                if (kbrGedFam.Dad != null)
                    famU.Husband = indiHash[kbrGedFam.Dad];
                if (kbrGedFam.Mom != null)
                    famU.Wife = indiHash[kbrGedFam.Mom];
                foreach (var child in kbrGedFam.Childs)
                {
                    famU.Childs.Add(indiHash[child]);

                    // TODO punting on adoption where a child could be part of more than one family see allged.ged
                    if (!childHash.ContainsKey(child))
                        childHash.Add(child, famU);
                }
                families.Add(famU);
            }

            // Connect family units
            foreach (var familyUnit in families)
            {
                if (childHash.ContainsKey(familyUnit.DadId))
                    familyUnit.DadFam = childHash[familyUnit.DadId];
                if (childHash.ContainsKey(familyUnit.MomId))
                    familyUnit.MomFam = childHash[familyUnit.MomId];
            }

            // For each person, dump their ancestry
            foreach (var indiId in indiHash.Keys)
            {
                KBRGedIndi firstP = indiHash[indiId];
                Console.WriteLine("First person:" + firstP.Names[0]);
                if (childHash.ContainsKey(indiId))
                {
                    FamilyUnit firstFam = childHash[indiId];
                    DumpAnce(firstFam, childHash, firstP, 1);
                }
                else
                {
                    Console.WriteLine(" No ancestry");
                }
                Console.WriteLine("==========================================================");
            }
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
