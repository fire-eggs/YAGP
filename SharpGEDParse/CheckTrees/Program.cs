using System.Collections.Generic;
using System.Linq;
using BuildTree;
using SharpGEDParser;
using System;
using System.IO;

// Read a GEDCOM file and find the number of disjoint trees / individuals

// TODO ? bails on ADOP (indi is part of more than one family)
// TODO ? should people in multiple families be special / near-disjoint trees?

namespace CheckTrees
{
    class Program
    {
        private static void AllInFamily(FamilyUnit fu)
        {
            // Add all individuals referenced from this family to the processing stack
            if (fu == null) 
                return;
            if (fu.Husband != null)
            {
                var id = fu.Husband.Ident;
                var iw2 = _treeBuild.IndiFromId(id);
                if (iw2.tree == -1)
                    treeStack.Push(iw2);
            }
            if (fu.Wife != null)
            {
                var id = fu.Wife.Ident;
                var iw2 = _treeBuild.IndiFromId(id);
                if (iw2.tree == -1)
                    treeStack.Push(iw2);
            }

            foreach (var indiRecord in fu.Childs)
            {
                var id = indiRecord.Ident;
                var iw2 = _treeBuild.IndiFromId(id);
                if (iw2.tree == -1)
                    treeStack.Push(iw2);
            }
        }

        // stack-based list of individuals to mark - eliminate recursion due to deep
        // trees resulting in stack overflow (at depth 15,700+).
        private static Stack<IndiWrap> treeStack = new Stack<IndiWrap>();

        private static void MakeTree(int treenum)
        {
            while (treeStack.Count > 0)
            {
                var iw = treeStack.Pop();
                iw.tree = treenum;

                // everybody where this person is a spouse
                foreach (var familyUnit in iw.SpouseIn)
                {
                    AllInFamily(familyUnit);
                }
                // everybody where this person is a child
                foreach (var familyUnit in iw.ChildIn)
                {
                    AllInFamily(familyUnit);
                }
//                AllInFamily(_treeBuild.FamFromIndi(iw.Indi.Ident)); // TODO replace with iw.ChildIn when BuildTree() supports
            }
        }

        private static void CalcTrees()
        {
            // identify disjoint trees across the individuals
            int treenum = 1;
            foreach (var indiId in _treeBuild.IndiIds)
            {
                var iw = _treeBuild.IndiFromId(indiId);
                if (iw.tree == -1)
                {
                    treeStack.Push(iw);
                    MakeTree(treenum);
                    treenum += 1;
                }
            }

            // count the number of individuals in each disjoint tree
            _treeCount = new int[treenum];
            string[] aTreePerson = new string[treenum]; // grab one indi id for the tree
            foreach (var indiId in _treeBuild.IndiIds)
            {
                var iw = _treeBuild.IndiFromId(indiId);
                _treeCount[iw.tree] += 1;
                aTreePerson[iw.tree] = indiId;
            }

            // we have a list of tree counts. turn this into a map of counts->treenums
            MultiMap<int,int> countToTreeNum = new MultiMap<int,int>();
            for (int i = 1; i < treenum; i++)
            {
                countToTreeNum.Add(_treeCount[i], i);
            }
            // Now get a list of tree-counts in sorted order
            var counts = countToTreeNum.Keys.ToList();
            counts.Sort();

            // iterate through the list of tree-counts in REVERSE order. largest to smallest.
            // for each tree count, show how many people are in the trees with that count.
            // This is useful for comparing against the GEDCOM errors report from Genealogica Grafica.
            int j = 1;
            for (int i = counts.Count - 1; i >= 0; i--)
            {
                int countVal = counts[i];
                var trees = countToTreeNum[countVal];
                foreach (var tree in trees)
                {
                    Console.WriteLine("People in tree {0}:{1}" + (countVal == 1 ? " ['{2}']" : ""), tree, _treeCount[tree], aTreePerson[tree]);
                }
                j += 1;
                if (_summaryOnly && j == 3) // only show first three in summary mode
                    break;
            }

            Console.WriteLine("Total number of trees:{0}", treenum-1);
            if (_treeBuild.ErrorsCount > 0)
                Console.WriteLine("Total number of errors: {0}", _treeBuild.ErrorsCount);
            if (_checkCHIL && _treeBuild.ChilErrorsCount > 0)
                if (_driverIsCHIL)
                    Console.WriteLine("FAM.CHIL reference errors: {0}", _treeBuild.ChilErrorsCount);
                else
                    Console.WriteLine("INDI.FAMC reference errors: {0}", _treeBuild.ChilErrorsCount);
        }

        private static void parseFile(string path)
        {
            using (var fr = new FileRead())
            {
                fr.ReadGed(path);
                if (!_driverIsCHIL)
                    _treeBuild.BuildTree(fr.Data, _showErrors, _checkCHIL); // INDI.FAMC is driver
                else
                    _treeBuild.BuildTree2(fr.Data, _showErrors, _checkCHIL); // FAM.CHIL is driver
            }

            CalcTrees();
        }

        private static FamilyTreeBuild _treeBuild;
        private static int[] _treeCount;
        private static bool _summaryOnly;
        private static bool _showErrors;
        private static bool _checkCHIL;
        private static bool _driverIsCHIL;

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: checktrees [-s] [-e] [-c] [-i] <.ged file>");
                Console.WriteLine("Specify a .GED file");
                Console.WriteLine("-s : summary - show few details");
                Console.WriteLine("-e : show error details");
                Console.WriteLine("-c : verify FAM.CHIL vs INDI.FAMC");
                Console.WriteLine("-i : if specified, INDI.FAMC is master, otherwise FAM.CHIL is master");
                return;
            }

            _summaryOnly = args.FirstOrDefault(s => s == "-s") != null;
            _showErrors = args.FirstOrDefault(s => s == "-e") != null;
            _checkCHIL = args.FirstOrDefault(s => s == "-c") != null;
            _driverIsCHIL = args.FirstOrDefault(s => s == "-i") == null; // NOTE: reverse logic!

            int lastarg = args.Length - 1;
            if (File.Exists(args[lastarg]))
            {
                _treeBuild = new FamilyTreeBuild();
                Console.WriteLine(args[lastarg]);
                parseFile(args[lastarg]);
            }
            else
            {
                Console.WriteLine("Specified file doesn't exist!");
            }
        }
    }
}
