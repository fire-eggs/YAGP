using System.IO;
using SharpGEDParser;
using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.Linq;

// Container wrapper around an imported GEDCOM
// Use to access records
// Connects and verifies links between INDI and FAM

// ReSharper disable InconsistentNaming

namespace GEDWrap
{
    public sealed class Forest : IDisposable
    {
        private FileRead _gedReader;

        public bool IsEmpty { get { return _indiHash == null || _indiHash.Count == 0; } }

        public HeadRecord Header
        {
            get
            {
                return _gedReader.Data.OfType<HeadRecord>().FirstOrDefault();
            }
        }

        public List<IndiRecord> Indi
        {
            get
            {
                return _gedReader.Data.OfType<IndiRecord>().Select(gedCommon => gedCommon).ToList();
            }
        }
        public List<FamRecord> Fams
        {
            get
            {
                return _gedReader.Data.OfType<FamRecord>().Select(gedCommon => gedCommon).ToList();
            }
        }

        public IndiRecord FindIndiByIdent(string ident)
        {
            return _gedReader.Data.OfType<IndiRecord>().FirstOrDefault(indi => indi.Ident == ident);
        }

        public List<UnkRec> Errors
        {
            get { return _gedReader.AllErrors ?? _gedReader.Errors; }
        }

        public List<UnkRec> Unknowns
        {
            get { return _gedReader.AllUnknowns; }
        }

        /// <summary>
        /// Only parse a GEDCOM file. Useful for syntax validation. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bufferSize"></param>
        public void ParseGEDCOM(string path, int bufferSize=0)
        {
            _gedReader = new FileRead(bufferSize);
            _gedReader.ReadGed(path);
        }

        /// <summary>
        /// Parse a GEDCOM file, establish family relations 
        /// </summary>
        /// <param name="path"></param>
        public void LoadGEDCOM(string path)
        {
            _issues = new List<Issue>();

            ParseGEDCOM(path);
            if (_gedReader == null || _gedReader.Data == null) // nothing to do!
                return;

            BuildTree();
            CalcTrees();
        }

        public void LoadGEDCOM(string path, int bufferSize)
        {
            _issues = new List<Issue>();

            ParseGEDCOM(path, bufferSize);
            if (_gedReader == null || _gedReader.Data == null) // nothing to do!
                return;

            BuildTree();
            CalcTrees();
        }

        public void LoadFromStream(StreamReader stream)
        {
            _issues = new List<Issue>();

            _gedReader = new FileRead();
            _gedReader.ReadGed(null, stream);
            if (_gedReader.Data.Count < 1) // nothing to do!
                return;
            BuildTree();
            CalcTrees();
        }

        public Person PersonById(string ident)
        {
            return _indiHash[ident];
        }

        public IEnumerable<string> AllIndiIds
        {
            get { return _indiHash.Keys; }
        }

        public int ErrorsCount
        {
            get
            {
                //return _issues.Count(issue => issue.Message().StartsWith("Error:"));
                // TODO for testing 
                return _issues.Count;
            }
        }

        public IEnumerable<Person> AllPeople
        {
            get { return _indiHash.Values; }
        }

        public IEnumerable<Union> AllUnions
        {
            get { return _famHash.Values; }
        }

        public List<GEDCommon> AllRecords
        {
            get { return _gedReader.Data; }
        }

        public int NumberOfTrees
        {
            get
            {
                int treeCount = 0;
                foreach (var person in _indiHash.Values)
                {
                    if (person.Tree > treeCount)
                        treeCount = person.Tree;
                }
                return treeCount;
            }
        }

        public IEnumerable<Issue> Issues { get { return _issues; } }
        public int NumberOfLines { get { return _gedReader.NumberLines; } }

        #region BuildTree (Establish family relationships)

        private Dictionary<string, Person> _indiHash;  // INDI ident -> INDI record
        private Dictionary<string, Union> _famHash; // FAM  ident -> FAM  record
        private List<Issue> _issues;
        private string _firstPerson; // "First" person in tree - default initial person

        private void BuildTree()
        {
            WrapAndHashRecords();   

            // Making connections using _only_ the INDI, or _only_ the FAM,
            // has not proven to be effective.
            ConnectFamLinks();  // Make connections using FAM.HUSB/WIFE/CHIL
            ConnectIndiLinks(); // Make connections using INDI.FAMC/FAMS

            VerifyFAMLinks();
            VerifyINDILinks();
        }

        private void MakeError(Issue.IssueCode code, params object[] evidence)
        {
            Issue err = new Issue(code, evidence);
            _issues.Add(err);
        }

        private void WrapAndHashRecords()
        {
            // Wrap INDI and FAM records, collect into hashes

            _indiHash = new Dictionary<string, Person>(Indi.Count);
            _famHash = new Dictionary<string, Union>(Fams.Count);
            if (Indi.Count > 0)
                _firstPerson = Indi[0].Ident;

            // NOTE I've tried Parallel.For here, timings suggest it's slightly slower
            // (probably due to the required locking around the dictionaries).

            // 20170520 From Peach fuzzing: missing ident would crash
            // TODO fix missing INDI/FAM ids in parsing

            foreach (var indiRecord in Indi)
            {
                var ident = indiRecord.Ident;
                Person iw = new Person(indiRecord);
                try
                {
                    _indiHash.Add(ident, iw);
                }
                catch (ArgumentNullException)
                {
                    // Not necessary, caught by parser MakeError(Issue.IssueCode.MISS_INDIID, indiRecord.BegLine);
                }
                catch (ArgumentException)
                {
                    MakeError(Issue.IssueCode.DUPL_INDI, ident);
                }
            }

            foreach (var fam in Fams)
            {
                var ident = fam.Ident;
                Union iw = new Union(fam);
                try
                {
                    _famHash.Add(ident, iw);
                }
                catch (ArgumentNullException)
                {
                    MakeError(Issue.IssueCode.MISS_FAMID, fam.BegLine);
                }
                catch (ArgumentException)
                {
                    MakeError(Issue.IssueCode.DUPL_FAM, ident); // TODO add fam.BegLine
                }
            }
        }

        private void ConnectFamLinks()
        {
            // This is a good 'first guess' at family relations: also need to use INDI.FAMS/INDI.FAMC

            // Iterate through the family records
            // For each HUSB/WIFE, connect to INDI
            // For each CHIL, connect to INDI
            foreach (var familyUnit in _famHash.Values)
            {
                foreach (var dadId in familyUnit.FamRec.Dads)
                {
                    Person dadWrap;
                    if (_indiHash.TryGetValue(dadId, out dadWrap))
                    {
                        dadWrap.SpouseIn.Add(familyUnit);
                        familyUnit.Spouses.Add(dadWrap);
                        familyUnit.Husband = dadWrap; // TODO taking the last one... save all?
                    }
                }
                foreach (var momId in familyUnit.FamRec.Moms)
                {
                    Person momWrap;
                    if (_indiHash.TryGetValue(momId, out momWrap))
                    {
                        momWrap.SpouseIn.Add(familyUnit);
                        familyUnit.Spouses.Add(momWrap);
                        familyUnit.Wife = momWrap; // TODO taking the last one... save all?
                    }
                }
                foreach (var childData in familyUnit.FamRec.Childs)
                {
                    // does childId exist in _indiHash: if yes, add to familyUnit.Childs
                    // if no, error
                    Person childWrap;
                    if (_indiHash.TryGetValue(childData.Xref, out childWrap))
                    {
                        childWrap.ChildIn.Add(familyUnit);
                        familyUnit.Childs.Add(childWrap);
                    }
                }
            }
        }

        private void ConnectIndiLinks()
        {
            // Iterate through INDI records.
            // For each FAMC, connect to FAM.
            // For each FAMS, connect to FAM.
            foreach (var person in _indiHash.Values)
            {
                var indiId = person.Id;
                foreach (var link in person.Indi.Links)
                {
                    var famId = link.Xref;

                    // Some GEDCOM end up with gibberish xref. TODO were these marked during parse?
                    if (string.IsNullOrWhiteSpace(famId))
                    {
                        MakeError(Issue.IssueCode.MISS_XREFID, indiId, link.Tag);
                        continue;
                    }
                    Union famU;
                    if (!_famHash.TryGetValue(famId, out famU))
                        continue; // Ignore missing family here, catch later
                    switch (link.Tag)
                    {
                        case "FAMS":
                            person.SpouseIn.Add(famU);
                            if (!famU.ReconcileFams(person))
                                MakeError(Issue.IssueCode.SPOUSE_CONN, famId, indiId);
                            break;
                        case "FAMC":
                            famU.Childs.Add(person);
                            person.ChildIn.Add(famU);
                            break;
                    }
                }
            }
        }

        private int verifyFAMLink(string ident, string famIdent, string linkType,
                                  string relType, Issue.IssueCode err1, Issue.IssueCode err2)
        {
            // Verify a single FAM record link
            // returns: 0 no problem; -1 missing; 1 unmatched

            if (string.IsNullOrEmpty(ident))
                return 0;

            Person indi;
            if (!_indiHash.TryGetValue(ident, out indi))
            {
                MakeError(err1, famIdent, ident, relType);
                return -1;
            }

            // TODO need a simple link accessor
            bool found = false;
            foreach (var link in indi.Indi.Links)
            {
                if (link.Tag == linkType && link.Xref == famIdent)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                MakeError(err2, famIdent, ident, relType);

            return !found ? 1 : 0;
        }

        private void VerifyFAMLinks()
        {
            // Verify links from FAM records.
            // 1. CHIL link to missing INDI
            // 2. HUSB link to missing INDI
            // 3. WIFE link to missing INDI
            // 4. CHIL link w/ no matching FAMC
            // 5. HUSB link w/ no matching FAMS
            // 6. WIFE link w/ no matching FAMS

            foreach (var familyUnit in _famHash.Values)
            {
                var famIdent = familyUnit.FamRec.Ident;

                // Identify spouse ambiguity: more than one HUSB; mismatch in spouses and HUSB/WIFE counts
                int identifiedSpouseCount = 0;
                bool warned = false; // only one message
                if (familyUnit.Husband != null)
                {
                    identifiedSpouseCount++;
                    if (familyUnit.FamRec.Dads.Count > 1)
                    {
                        MakeError(Issue.IssueCode.AMB_CONN, "HUSB", famIdent);
                        warned = true;
                    }
                }

                if (familyUnit.Wife != null)
                {
                    identifiedSpouseCount++;
                    if (familyUnit.FamRec.Moms.Count > 1)
                    {
                        MakeError(Issue.IssueCode.AMB_CONN, "WIFE", famIdent);
                        warned = true;
                    }
                }

                if (identifiedSpouseCount < familyUnit.Spouses.Count && !warned) // don't repeat
                {
                    MakeError(Issue.IssueCode.AMB_CONN, "unknown", famIdent);
                }

                foreach (var husbId in familyUnit.FamRec.Dads)
                {
                    verifyFAMLink(husbId, famIdent, "FAMS", "HUSB",
                        Issue.IssueCode.SPOUSE_CONN_MISS, Issue.IssueCode.SPOUSE_CONN_UNM);
                }
                foreach (var wifeId in familyUnit.FamRec.Moms)
                {
                    verifyFAMLink(wifeId, famIdent, "FAMS", "WIFE",
                        Issue.IssueCode.SPOUSE_CONN_MISS, Issue.IssueCode.SPOUSE_CONN_UNM);
                }
                foreach (var childData in familyUnit.FamRec.Childs)
                {
                    var childId = childData.Xref;
                    verifyFAMLink(childId, famIdent, "FAMC", "CHIL",
                        Issue.IssueCode.CHIL_MISS, Issue.IssueCode.CHIL_NOTMATCH);
                }
            }
        }

        private void VerifyINDILinks()
        {
            foreach (var person in _indiHash.Values)
            {
                var indiId = person.Id;
                foreach (var link in person.Indi.Links)
                {
                    // TODO verify parsing error
                    if (string.IsNullOrEmpty(link.Xref))
                        continue; // tabors.ged, twparker.ged
                    switch (link.Tag)
                    {
                        case "FAMC":
                            if (!_famHash.ContainsKey(link.Xref))
                            {
                                MakeError(Issue.IssueCode.FAMC_MISSING, indiId, link.Xref);
                            }
                            else
                            {
                                // verify that the FAM record has a matching CHIL link
                                bool found = false;
                                foreach (var famU in person.ChildIn)
                                {
                                    foreach (var childData in famU.FamRec.Childs)
                                    {
                                        if (childData.Xref == indiId)
                                            found = true;
                                    }
                                }
                                if (!found)
                                {
                                    MakeError(Issue.IssueCode.FAMC_UNM, indiId, link.Xref);
                                }
                            }
                            break;
                        case "FAMS":
                            if (!_famHash.ContainsKey(link.Xref))
                            {
                                MakeError(Issue.IssueCode.FAMS_MISSING, indiId, link.Xref);
                            }
                            else
                            {
                                // verify that the FAM record has a matching HUSB/WIFE link
                                bool found = false;
                                foreach (var famU in person.SpouseIn)
                                {
                                    foreach (var dad in famU.FamRec.Dads)
                                    {
                                        if (dad == indiId)
                                            found = true;
                                    }
                                    foreach (var mom in famU.FamRec.Moms)
                                    {
                                        if (mom == indiId)
                                            found = true;
                                    }
                                }
                                if (!found)
                                {
                                    MakeError(Issue.IssueCode.FAMS_UNM, indiId, link.Xref);
                                }
                            }
                            break;
                    }
                }
            }
        }
        #endregion

        #region Mark Trees

        // stack-based list of individuals to mark - eliminate recursion due to deep
        // trees resulting in stack overflow (at depth 15,700+).
        private Stack<Person> _treeStack = new Stack<Person>();

        private void AllInFamily(Union fu)
        {
            // Add all individuals referenced from this family to the processing stack
            if (fu == null)
                return;
            if (fu.Husband != null)
            {
                var iw2 = fu.Husband;
                if (iw2.Tree == -1)
                    _treeStack.Push(iw2);
            }
            if (fu.Wife != null)
            {
                var iw2 = fu.Wife;
                if (iw2.Tree == -1)
                    _treeStack.Push(iw2);
            }

            foreach (var iw2 in fu.Childs)
            {
                if (iw2.Tree == -1)
                    _treeStack.Push(iw2);
            }
        }

        private void MakeTree(int treenum)
        {
            while (_treeStack.Count > 0)
            {
                var iw = _treeStack.Pop();
                iw.Tree = treenum;

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
            }
        }

        private void CalcTrees()
        {
            // determine which tree a person belongs
            int treenum = 1;
            foreach (var person in _indiHash.Values)
            {
                if (person.Tree != -1)
                    continue;
                _treeStack.Push(person);
                MakeTree(treenum);
                treenum += 1;
            }

            _treeStack = null;
        }
        #endregion

        public void Dispose()
        {
            _gedReader.Dispose();
        }
    }
}
