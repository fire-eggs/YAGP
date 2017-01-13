using System;
using System.Collections.Generic;
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
        private FamilyTreeBuild _tb;
        private List<List<IndiWrap>> _pedigrees;
        private IndiWrap _who;
        private PersonNode _root;

        public Pedigrees(IndiWrap person, FamilyTreeBuild tb)
        {
            _tb = tb;
            _pedigrees = new List<List<IndiWrap>>();
            _who = person;

            // degenerate case: person has no ancestors
            if (_who.ChildIn.Count < 1)
            {
                _trees = new List<IndiWrap[]>();
                _trees.Add(new IndiWrap[MAX_AHNEN]);
                _trees[0][1] = _who;
                return; 
            }

            CalcAllPedigrees();

            //_root = makeTree(_who, 0);

            //_treeStack = new Stack<FamilyUnit>();
            //doTree(_who, 0);
        }

        private Stack<FamilyUnit> _treeStack;
        private void doTree(IndiWrap who, int deep)
        {
            if (deep > 4 || who.ChildIn.Count == 0)
            {
                Console.WriteLine("Pedigree:");
                var fams = _treeStack.ToArray();
                foreach (var fam in fams)
                {
                    Console.WriteLine("  Family:{0} Dad:{1} Mom:{2}", fam.FamRec.Ident, fam.Husband.Name, fam.Wife.Name);
                }
                Console.WriteLine(_who.Name);
            }

            foreach (var familyUnit in who.ChildIn)
            {
                _treeStack.Push(familyUnit);
                if (familyUnit.Husband != null)
                    doTree(familyUnit.Husband, deep+1);
                if (familyUnit.Wife != null)
                    doTree(familyUnit.Wife, deep + 1);
                _treeStack.Pop();
            }
        }

        private PersonNode makeTree(IndiWrap who, int deep)
        {
            if (deep > 5)
                return null;

            // set up the root
            PersonNode root = new PersonNode();
            root.Who = who;
            root.Depth = deep;

            // for each family the person is a child in, set up a tree node
            foreach (var familyUnit in who.ChildIn)
            {
                var fn = new FamilyNode();
                fn.Who = familyUnit;
                root.Fams.Add(fn);

                // for mom and dad in each treenode, recurse
                fn.Dad = makeTree(familyUnit.Husband, deep + 1);
                fn.Mom = makeTree(familyUnit.Wife, deep + 1);
            }

            return root;
        }

        private static int MAX_AHNEN = 32;
        private IndiWrap[] _ancIndi;
        private List<IndiWrap[]> _trees;

        private void CalcAllPedigrees()
        {
            // Determine _all_ a person's pedigrees
            // Each person in the pedigree could be a child in more than one family
            // (e.g. adoption), resulting in a possible alternate pedigree

            _trees = new List<IndiWrap[]>();

            // 1. For the root person, calculate full pedigree for each family they are
            // a child in.
            foreach (var familyUnit in _who.ChildIn)
            {
                _ancIndi = new IndiWrap[MAX_AHNEN];
                _ancIndi[1] = _who;
                CalcAnce(familyUnit, 1);
                _trees.Add(_ancIndi);
            }

            // 2. For each ancestor, if they are a child in more than one family, must
            // establish an alternate pedigree and re-calculate at that point.
            
        }

        public int PedigreeCount { get { return _trees.Count; } }

        public IndiWrap[] GetPedigree(int num)
        {
            return _trees[num];
        }

        private int CalcAnce(FamilyUnit firstFam, int myNum)
        {
            // TODO move to BuildTree, taking a MAX parameter and returning a list<IndiWrap> in Ahnen order

            if (myNum >= MAX_AHNEN)
                return -1;

            int numRet = myNum;
            if (firstFam == null)
                return numRet;

            // From http://www.tamurajones.net/AhnenNumbering.xhtml : the Ahnen number 
            // of the father is double that of the current person. Mom's Ahnen number
            // is Dad's plus 1.

            int dadnum = myNum * 2;
            if (firstFam.Husband != null)
            {
                numRet = Math.Max(numRet, dadnum);
                if (dadnum < MAX_AHNEN)
                {
                    //IndiWrap hack = _tb.IndiFromId(firstFam.Husband.Indi.Ident); // _indiHash[firstFam.Husband.Ident];
                    //_ancIndi[dadnum] = hack;
                    _ancIndi[dadnum] = firstFam.Husband;
                }
                if (firstFam.DadFam != null) // TODO hard-coded to first: need to split on multiple
                    numRet = Math.Max(numRet, CalcAnce(firstFam.DadFam, dadnum));
            }
            if (firstFam.Wife != null)
            {
                numRet = Math.Max(numRet, dadnum + 1);
                if (dadnum + 1 < MAX_AHNEN)
                {
                    //IndiWrap hack = _tb.IndiFromId(firstFam.Wife.Indi.Ident); // _indiHash[firstFam.Wife.Ident];
                    //_ancIndi[dadnum + 1] = hack;
                    _ancIndi[dadnum + 1] = firstFam.Wife;
                }
                if (firstFam.MomFam != null) // TODO hard-coded to first: need to split on multiple
                    numRet = Math.Max(numRet, CalcAnce(firstFam.MomFam, dadnum + 1));
            }
            return numRet;
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
