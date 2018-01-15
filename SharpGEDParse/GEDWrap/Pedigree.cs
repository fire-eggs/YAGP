using System.Collections.Generic;
using System.Linq;

// TODO currently uses a hard-coded max, take parameter?

namespace GEDWrap
{
    /// <summary>
    /// Determine a person's ancestry (pedigree). 
    /// </summary>
    /// Calculates an array (indexed by Ahnen number)
    /// of individuals for each possible pedigree. I.e. takes adoption/alternate families
    /// into account as separate pedigrees.
    public class Pedigrees
    {
        private readonly Person _who;
        private const int MAX_AHNEN = 32;
        private Person[] _ancIndi;
        private List<Person[]> _trees;
        private Stack<Retry> _retry;

        public Pedigrees(Person person, bool firstOnly)
        {
            _who = person;

            // degenerate case: person has no ancestors
            if (_who.ChildIn.Count < 1)
            {
                _trees = new List<Person[]>();
                _trees.Add(new Person[MAX_AHNEN]);
                _trees[0][1] = _who;
                return; 
            }

            CalcAllPedigrees(firstOnly);
        }

        private void CalcAllPedigrees(bool firstOnly)
        {
            // Determine _all_ a person's pedigrees
            // Each person in the pedigree could be a child in more than one family
            // (e.g. adoption), resulting in a possible alternate pedigree

            _trees = new List<Person[]>();
            _retry = new Stack<Retry>();

            // 1. For the root person, calculate full pedigree for each family they are
            // a child in.
            foreach (var familyUnit in _who.ChildIn)
            {
                _ancIndi = new Person[MAX_AHNEN];
                _ancIndi[1] = _who;
                CalcAnce(familyUnit, 1);
                _trees.Add(_ancIndi);

                if (firstOnly)
                    return;
            }

            // 2. For each ancestor, if they are a child in more than one family, must
            // establish an alternate pedigree and re-calculate at that point.
            // In CalcAnce above, branch points were pushed on the retry stack.
            while (_retry.Count != 0)
            {
                var data = _retry.Pop();
                _ancIndi = (Person[])data.ancIndi.Clone(); // don't modify the original!
                wipeTree(data.personNum); // about to re-calc all descendants, clear the way
                CalcAnce(data.famToDo, data.personNum);
                _trees.Add(_ancIndi);
            }
        }

        // Container for re-calculating an alternate pedigree
        private struct Retry
        {
            public Person[] ancIndi; // the existing pedigree
            public int personNum;      // the index in the pedigree of the person to re-calc
            public Union famToDo; // the _alternate_ family unit to recalc with
        }

        public int PedigreeCount { get { return _trees.Count; } }

        public Person[] GetPedigree(int num)
        {
            return _trees[num];
        }

        public int GetPedigreeMax(int num)
        {
            // Largest index of people in pedigree (i.e. maximum Ahnen value)
            var ped = GetPedigree(num);
            int count = 0;
            for (int i = 0; i < ped.Length; i++)
                if (ped[i] != null)
                    count = i;
            return count;
        }

        private void CalcAnce(Union fam, int myNum)
        {
            if (myNum >= MAX_AHNEN || fam == null)
                return;

            // From http://www.tamurajones.net/AhnenNumbering.xhtml : the Ahnen number 
            // of the father is double that of the current person. Mom's Ahnen number
            // is Dad's plus 1.

            int dadnum = myNum * 2;
            int momnum = dadnum + 1;
            if (fam.Husband != null)
            {
                if (dadnum < MAX_AHNEN)
                {
                    _ancIndi[dadnum] = fam.Husband;
                }
                int famCount = fam.Husband.ChildIn.Count;
                if (famCount > 0)
                {
                    Union firstFam = fam.Husband.ChildIn.First();
                    if (famCount > 1)
                    {
                        //                    Debugger.Break();
                        foreach (var union in fam.Husband.ChildIn)
                        {
                            if (union == firstFam) // NOTE: skipping first fam, the family we're about to do
                                continue;
                            Retry branch = new Retry();
                            branch.ancIndi = _ancIndi;
                            branch.personNum = dadnum;
                            branch.famToDo = union;
                            _retry.Push(branch);
                        }
                    }
                    if (firstFam != null)
                        CalcAnce(firstFam, dadnum);
                }
            }
            if (fam.Wife != null)
            {
                if (momnum < MAX_AHNEN)
                {
                    _ancIndi[momnum] = fam.Wife;
                }
                int famCount = fam.Wife.ChildIn.Count;
                if (famCount > 0)
                {
                    Union firstFam = fam.Wife.ChildIn.First();
                    if (famCount > 1)
                    {
                        foreach (var union in fam.Wife.ChildIn)
                        {
                            if (union == firstFam) // NOTE: skipping first fam, the family we're about to do
                                continue;
                            Retry branch = new Retry();
                            branch.ancIndi = _ancIndi;
                            branch.personNum = momnum;
                            branch.famToDo = union;
                            _retry.Push(branch);
                        }
                    }

                    if (firstFam != null)
                        CalcAnce(firstFam, momnum);
                }
            }
        }

        // When re-calculating a pedigree at a branch, clear all
        // existing descendants.
        private void wipeTree(int mynum)
        {
            if (mynum >= MAX_AHNEN)
                return;

            int dadn = mynum*2;
            if (dadn >= MAX_AHNEN)
                return;

            _ancIndi[dadn] = null;
            wipeTree(dadn);

            int momn = dadn + 1;
            _ancIndi[momn] = null;
            wipeTree(momn);
        }
    }
}
