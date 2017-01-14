using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BuildTree;

namespace DrawAnce
{
    public class Pedigrees
    {
//        private FamilyTreeBuild _tb;
//        private List<List<IndiWrap>> _pedigrees;
        private IndiWrap _who;

        public Pedigrees(IndiWrap person, FamilyTreeBuild tb, bool firstOnly)
        {
//            _tb = tb;
//            _pedigrees = new List<List<IndiWrap>>();
            _who = person;

            // degenerate case: person has no ancestors
            if (_who.ChildIn.Count < 1)
            {
                _trees = new List<IndiWrap[]>();
                _trees.Add(new IndiWrap[MAX_AHNEN]);
                _trees[0][1] = _who;
                return; 
            }

            CalcAllPedigrees(firstOnly);
        }

        private static int MAX_AHNEN = 32;
        private IndiWrap[] _ancIndi;
        private List<IndiWrap[]> _trees;
        private Stack<Retry> _retry;

        private void CalcAllPedigrees(bool firstOnly)
        {
            // Determine _all_ a person's pedigrees
            // Each person in the pedigree could be a child in more than one family
            // (e.g. adoption), resulting in a possible alternate pedigree

            _trees = new List<IndiWrap[]>();
            _retry = new Stack<Retry>();

            // 1. For the root person, calculate full pedigree for each family they are
            // a child in.
            foreach (var familyUnit in _who.ChildIn)
            {
                _ancIndi = new IndiWrap[MAX_AHNEN];
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
                _ancIndi = (IndiWrap[])data.ancIndi.Clone(); // TODO need to wipe the branch
                wipeTree(data.personNum);
                CalcAnce(data.famToDo, data.personNum);
                _trees.Add(_ancIndi);
            }
        }

        private class Retry
        {
            public IndiWrap[] ancIndi;
            public int personNum;
            public FamilyUnit famToDo;
        }

        public int PedigreeCount { get { return _trees.Count; } }

        public IndiWrap[] GetPedigree(int num)
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

        private void CalcAnce(FamilyUnit fam, int myNum)
        {
            // TODO move to BuildTree, taking a MAX parameter and returning a list<IndiWrap> in Ahnen order

            if (myNum >= MAX_AHNEN || fam == null)
                return;

            // From http://www.tamurajones.net/AhnenNumbering.xhtml : the Ahnen number 
            // of the father is double that of the current person. Mom's Ahnen number
            // is Dad's plus 1.

            int dadnum = myNum * 2;
            if (fam.Husband != null)
            {
                if (dadnum < MAX_AHNEN)
                {
                    _ancIndi[dadnum] = fam.Husband;
                }
                if (fam.Husband.ChildIn.Count > 1)
                {
//                    Debugger.Break();
                    foreach (var familyUnit in fam.Husband.ChildIn)
                    {
                        if (familyUnit == fam.DadFam)
                            continue; // This is the family we're about to do
                        Retry branch = new Retry();
                        branch.ancIndi = _ancIndi;
                        branch.personNum = dadnum;
                        branch.famToDo = familyUnit;
                        _retry.Push(branch);
                    }
                }
                if (fam.DadFam != null) // TODO hard-coded to first: need to split on multiple
                    CalcAnce(fam.DadFam, dadnum);
            }
            if (fam.Wife != null)
            {
                if (dadnum + 1 < MAX_AHNEN)
                {
                    _ancIndi[dadnum + 1] = fam.Wife;
                }
                //if (fam.Wife.ChildIn.Count > 1)
                //    Debugger.Break();
                if (fam.MomFam != null) // TODO hard-coded to first: need to split on multiple
                    CalcAnce(fam.MomFam, dadnum + 1);
            }
        }

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

    class PersonNode
    {
        public IndiWrap Who { get; set; }
        public List<FamilyNode> Fams { get; set; }
        public int Depth { get; set; }

        public PersonNode()
        {
            Fams = new List<FamilyNode>();
        }
    }

    class FamilyNode
    {
        public FamilyUnit Who { get; set; }
        public PersonNode Mom { get; set; }
        public PersonNode Dad { get; set; }
    }
}
