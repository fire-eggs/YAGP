using System.Linq;
using GEDWrap;
using System;
using System.IO;

// Read a GEDCOM file and find the number of disjoint trees / individuals

// ReSharper disable InconsistentNaming

namespace CheckTrees
{
    class Program
    {
        private static int[] _treeCount;
        private static bool _summaryOnly;
        private static bool _showErrors;
        private static Forest _gedtrees;

        private static void CalcTrees()
        {
            if (_showErrors)
            {
                foreach (var issue in _gedtrees.Issues)
                {
                    // NOTE: family relationship errors, not parsing problems
                    Console.WriteLine(issue.Message());
                }
            }

            int treenum = _gedtrees.NumberOfTrees;

            // Make a formatter based on the number of digits in tree count
            int treeCountLen = (int) Math.Floor(Math.Log10(treenum)) + 1;
            string treeFormat = new string('#', treeCountLen+1);
            int indiCountLen = (int)Math.Floor(Math.Log10(_gedtrees.AllPeople.Count())) + 1;

            // count the number of individuals in each disjoint tree
            _treeCount = new int[treenum+1];
            string[] aTreePerson = new string[treenum+1]; // grab one indi id for the tree

            int orphans = 0;
            foreach (var person in _gedtrees.AllPeople)
            {
                if (person.Tree == -1)
                    orphans++;
                else
                {
                    _treeCount[person.Tree] += 1;
                    aTreePerson[person.Tree] = person.Id;
                }
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
                    string treeval = tree.ToString().PadLeft(treeCountLen);
                    string countval = _treeCount[tree].ToString().PadLeft(indiCountLen);
                    Console.WriteLine("People in tree {0}:{1}" + (countVal != 1 ? ": sample ": "") + " ['{2}']", treeval, countval, aTreePerson[tree]);
                }
                j += 1;
                if (_summaryOnly && j == 3) // only show first three in summary mode
                    break;
            }

            Console.WriteLine("Total number of orphans:{0}", orphans);
            Console.WriteLine("Total number of trees:{0}", treenum);
            if (_gedtrees.ErrorsCount > 0)
                Console.WriteLine("Total number of errors: {0}", _gedtrees.ErrorsCount);
        }

#if false // TODO disabled for unit testing
        private static int tick;
        private static void logit(string msg, bool first = false)
        {
            int delta = 0;
            if (!first)
                delta = Environment.TickCount - tick;
            tick = Environment.TickCount;
            if (delta > 500) // only log if action took longer than .5 second
                Console.WriteLine(msg + "|" + delta + " milliseconds");
        }
#else
// ReSharper disable UnusedParameter.Local
        private static void logit(string msg, bool first = false)
        {
            
        }
// ReSharper restore UnusedParameter.Local
#endif
        private static void parseFile(string path)
        {
            logit("Start", true);
            using (_gedtrees = new Forest())
            {
                _gedtrees.LoadGEDCOM(path);
                logit("Log: Done load");
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
                return;
            }

            _summaryOnly = args.FirstOrDefault(s => s == "-s") != null;
            _showErrors = args.FirstOrDefault(s => s == "-e") != null;

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
