using System.Collections.Generic;
using System.Linq;
using GEDWrap;
using System;
using System.IO;

// Read a GEDCOM file and find the number of disjoint trees / individuals

// TODO ? bails on ADOP (indi is part of more than one family)
// TODO ? should people in multiple families be special / near-disjoint trees?

// TODO tree count is out of whack if people is zero: see 4b09cb694b032.ged
namespace CheckTrees
{
    class Program
    {
        private static int[] _treeCount;
        private static bool _summaryOnly;
        private static bool _showErrors;
        private static bool _checkCHIL;
        private static bool _driverIsCHIL;
        private static Forest _gedtrees;

        private static void CalcTrees()
        {
            if (_showErrors)
            {
                foreach (var issue in _gedtrees.Issues)
                {
                    Console.WriteLine(issue.Message());
                }
            }

            int treenum = _gedtrees.NumberOfTrees;

            // count the number of individuals in each disjoint tree
            _treeCount = new int[treenum+1];
            string[] aTreePerson = new string[treenum+1]; // grab one indi id for the tree

            foreach (var person in _gedtrees.AllPeople)
            {
                _treeCount[person.Tree] += 1;
                aTreePerson[person.Tree] = person.Id;
            }

            // we have a list of tree counts. turn this into a map of counts->treenums
            MultiMap<int,int> countToTreeNum = new MultiMap<int,int>();
            for (int i = 1; i <= treenum; i++)
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

            Console.WriteLine("Total number of trees:{0}", treenum);
            if (_gedtrees.ErrorsCount > 0)
                Console.WriteLine("Total number of errors: {0}", _gedtrees.ErrorsCount);
            if (_checkCHIL && _gedtrees.ChilErrorsCount > 0)
                if (_driverIsCHIL)
                    Console.WriteLine("FAM.CHIL reference errors: {0}", _gedtrees.ChilErrorsCount);
                else
                    Console.WriteLine("INDI.FAMC reference errors: {0}", _gedtrees.ChilErrorsCount);
        }

        private static int tick;
        private static void logit(string msg, bool first = false)
        {
            return; // TODO unit testing
            int delta = 0;
            if (!first)
                delta = Environment.TickCount - tick;
            tick = Environment.TickCount;
            if (delta > 500) // only log if action took longer than .5 second
                Console.WriteLine(msg + "|" + delta + " milliseconds");
        }

        private static void parseFile(string path)
        {
            logit("Start", true);
            using (_gedtrees = new Forest())
            {
                _gedtrees.LoadGEDCOM(path);
                //logit("Log: Done read");
                //if (!_driverIsCHIL)
                //    _treeBuild.BuildTree(fr.Data, _showErrors, _checkCHIL); // INDI.FAMC is driver
                //else
                //    _treeBuild.BuildTree2(fr.Data, _showErrors, _checkCHIL); // FAM.CHIL is driver
                //logit("Log: Done build");
            }

            CalcTrees();
            logit("Log: Done calc");
        }

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
