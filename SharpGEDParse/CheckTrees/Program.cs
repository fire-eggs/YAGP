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
    public class MultiMap<T,V>
    {
        // 1
        readonly Dictionary<T, List<V>> _dictionary = new Dictionary<T, List<V>>();

        // 2
        public void Add(T key, V value)
        {
            List<V> list;
            if (this._dictionary.TryGetValue(key, out list))
            {
                // 2A.
                list.Add(value);
            }
            else
            {
                // 2B.
                list = new List<V>();
                list.Add(value);
                this._dictionary[key] = list;
            }
        }

        // 3
        public IEnumerable<T> Keys
        {
            get
            {
                return this._dictionary.Keys;
            }
        }

        // 4
        public List<V> this[T key]
        {
            get
            {
                List<V> list;
                if (!this._dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    this._dictionary[key] = list;
                }
                return list;
            }
        }
    }

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
                AllInFamily(_treeBuild.FamFromIndi(iw.Indi.Ident));
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
            for (int i = counts.Count - 1; i >= 0; i--)
            {
                int countVal = counts[i];
                var trees = countToTreeNum[countVal];
                foreach (var tree in trees)
                {
                    Console.WriteLine("People in tree {0}:{1}" + (countVal == 1 ? " ['{2}']" : ""), tree, _treeCount[tree], aTreePerson[tree]);
                }
            }

            //for (int i = 1; i < treenum; i++)
            //{
            //    if (_treeCount[i] == 1)
            //    {
            //        foreach (var indiId in _treeBuild.IndiIds) // TODO we're re-scanning; build a hash of 1-person-trees earlier
            //        {
            //            var iw = _treeBuild.IndiFromId(indiId);
            //            if (iw.tree == i)
            //            {
            //                Console.WriteLine("People in tree {0}:{1} ['{2}']", i, _treeCount[i], indiId);
            //                break;
            //            }
            //        }
            //    }
            //    else
            //        Console.WriteLine("People in tree {0}:{1}", i, _treeCount[i]);
            //}
        }

        private static void parseFile(string path)
        {
            using (var fr = new FileRead())
            {
                fr.ReadGed(path);
                _treeBuild.BuildTree(fr.Data);
            }

            CalcTrees();
        }

        private static FamilyTreeBuild _treeBuild;
        private static int[] _treeCount;

        static void Main(string[] args)
        {
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
