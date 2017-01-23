using System;
using System.Collections.Generic;
using System.Linq;
using SharpGEDParser;
using SharpGEDParser.Model;

// 20161225 01\00005.ged is an example of inconsistancy between the INDI.FAMC
// and the FAM.CHIL linkages. From my reading, the INDI.FAMC links are the 
// "master". The first version of this code was working from the FAM.CHIL links.

// 201701?? BuildTree2() is a variant where the FAM.CHIL links are the "master".

// 20170108 Handle "child in more than one family" by replacing child hash with a MultiMap. The FamilyUnit
// connections to "Dad's family" and "Mom's family" are now a problem because there could be more than one link.

namespace BuildTree
{
    public class FamilyTreeBuild
    {
        private int _CHILErrorsCount;
        private int errorsCount;

        private Dictionary<string, IndiWrap> _indiHash;  // INDI ident -> INDI record
        private Dictionary<string, FamilyUnit> _famHash; // FAM  ident -> FAM  record
        private MultiMap<string, FamilyUnit> _childsIn;  // INDI ident -> multi FAM
        private List<Issue> _issues;
        private string _firstPerson; // "First" person in tree - default initial person

        public IEnumerable<string> IndiIds
        {
            get { return _indiHash.Keys; }
        }

        private IndiWrap MakeFillerIndi(string ident, out IndiRecord hack)
        {
            // There is a reference to an individual who doesn't exist in
            // the GEDCOM. Create a placeholder.

            IndiWrap hack0 = new IndiWrap();

            // TODO need a library method to do this!!!
            hack = new IndiRecord(null, ident, null);
            var hack2 = new NameRec();
            hack2.Surname = "Missing";
            hack.Names.Add(hack2);
            hack0.Indi = hack;
            hack0.Ahnen = -1;
            return hack0;
        }

        private void MakeError(Issue.IssueCode code, params object[] evidence)
        {
            Issue err = new Issue(code, evidence);
            _issues.Add(err);
        }

        private void Pass1(IEnumerable<GEDCommon> gedRecs)
        {
            // Wrap INDI and FAM records, collect into hashes

            _indiHash = new Dictionary<string, IndiWrap>();
            _famHash = new Dictionary<string, FamilyUnit>();
            _childsIn = new MultiMap<string, FamilyUnit>();
            _issues = new List<Issue>();

            foreach (var gedCommon in gedRecs) // TODO really need 'INDI', 'FAM' accessors
            {
                if (gedCommon is IndiRecord)
                {
                    var ident = (gedCommon as IndiRecord).Ident;

                    if (_indiHash.ContainsKey(ident))
                    {
                        MakeError(Issue.IssueCode.DUPL_INDI, ident);
                    }
                    else
                    {
                        IndiWrap iw = new IndiWrap();
                        iw.Indi = gedCommon as IndiRecord;
                        iw.Ahnen = 0;
                        _indiHash.Add(ident, iw);

                        if (_firstPerson == null)
                            _firstPerson = ident;
                    }
                }

                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (gedCommon is FamRecord)
                {
                    var fam = gedCommon as FamRecord;
                    var ident = fam.Ident;
                    if (string.IsNullOrEmpty(ident))
                    {
                        MakeError(Issue.IssueCode.MISS_FAMID, fam.BegLine);
                        continue;
                    }
                    if (!_famHash.ContainsKey(ident))
                        _famHash.Add(ident, new FamilyUnit(fam));
                    else
                    {
                        MakeError(Issue.IssueCode.DUPL_FAM, ident);
                    }
                }
            }

        }

        private void Pass3()
        {
            // Try to determine each spouse's family [the family they were born into]
            // TODO currently of dubious value because dad/mom may be adopted and currently keeping only the 'first' family connection

            // Also check if HUSB/WIFE links are to valid people
            // Also check if CHIL links are valid (exist and matched)

            foreach (var familyUnit in _famHash.Values)
            {
                var famIdent = familyUnit.FamRec.Ident;

                if (familyUnit.Husband != null)
                {
                    var dadFams = _childsIn[familyUnit.DadId];
                    if (dadFams != null && dadFams.Count > 0)
                    {
                        familyUnit.DadFam = dadFams[0];
                        if (dadFams.Count > 1)
                        {
                            MakeError(Issue.IssueCode.AMB_CONN, "dad", famIdent);
                        }
                    }
                }

                if (familyUnit.Wife != null)
                {
                    var momFams = _childsIn[familyUnit.MomId];
                    if (momFams != null && momFams.Count > 0)
                    {
                        familyUnit.MomFam = momFams[0];
                        if (momFams.Count > 1)
                        {
                            MakeError(Issue.IssueCode.AMB_CONN, "mom", famIdent);
                        }
                    }
                }

                // TODO what happens in parse if more than one HUSB/WIFE specified?

                var husbId = familyUnit.FamRec.Dad;
                if (husbId != null && !_indiHash.ContainsKey(husbId))
                {
                    MakeError(Issue.IssueCode.SPOUSE_CONN2, famIdent, husbId, "HUSB");
                }
                var wifeId = familyUnit.FamRec.Mom;
                if (wifeId != null && !_indiHash.ContainsKey(wifeId))
                {
                    MakeError(Issue.IssueCode.SPOUSE_CONN2, famIdent, wifeId, "WIFE");
                }

                foreach (var childId in familyUnit.FamRec.Childs)
                {
                    if (childId == null) 
                        continue;

                    IndiWrap indi;
                    if (!_indiHash.TryGetValue(childId, out indi))
                    {
                        MakeError(Issue.IssueCode.CHIL_MISS, famIdent, childId);
                    }
                    else
                    {
                        // TODO need a simple FAMC link accessor
                        bool found = false;
                        foreach (var link in indi.Indi.Links)
                        {
                            if (link.Tag == "FAMC" && link.Xref == famIdent)
                                found = true;
                        }
                        if (!found)
                            MakeError(Issue.IssueCode.CHIL_NOTMATCH, famIdent, childId);
                    }
                }
            }
            
        }
        public void BuildTree(IEnumerable<GEDCommon> gedRecs, bool showErrors, bool checkCHIL)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL but the CHIL are being ignored

            Pass1(gedRecs);

            // Iterate through the indi records.
            // For each FAMS, identify the husb/wife relation
            // For each FAMC, add to childhash
            foreach (var indiWrap in _indiHash.Values)
            {
                var indiId = indiWrap.Indi.Ident;
                foreach (var indiLink in indiWrap.Indi.Links) // TODO wow this is awkward
                {
                    FamilyUnit fu;
                    var id = indiLink.Xref;
                    if (string.IsNullOrEmpty(id))
                    {
                        MakeError(Issue.IssueCode.MISS_XREFID, indiId, indiLink.Tag);
                        continue;
                    }
                    switch (indiLink.Tag)
                    {
                        case "FAMS":
                            if (_famHash.TryGetValue(id, out fu))
                            {
                                indiWrap.SpouseIn.Add(fu);
                                if (fu.FamRec.Dad == indiId)
                                    fu.Husband = indiWrap;
                                else if (fu.FamRec.Mom == indiId)
                                    fu.Wife = indiWrap;
                                else
                                {
                                    MakeError(Issue.IssueCode.SPOUSE_CONN, indiLink.Xref, indiId);
                                }
                            }
                            else
                            {
                                MakeError(Issue.IssueCode.FAMS_MISSING, indiId, id);
                            }
                            break;
                        case "FAMC":
                            if (_famHash.TryGetValue(id, out fu))
                            {
                                _childsIn.Add(indiId, fu);
                                fu.Childs.Add(indiWrap);
                                indiWrap.ChildIn.Add(fu);
                            }
                            else
                            {
                                MakeError(Issue.IssueCode.FAMC_MISSING, indiId, id);
                            }
                            break;
                    }
                }
            }

            Pass3();

            errorsCount = 0;
            foreach (var issue in _issues)
            {
                var msg = issue.Message();
                if (msg.StartsWith("Error:")) // TODO unit testing
                    errorsCount++;
                if (showErrors)
                    Console.WriteLine(msg);
            }

            // verify if FAM.CHIL have matching INDI.FAMC
            if (checkCHIL)
            {
                _CHILErrorsCount = 0;
                foreach (var fam in _famHash.Values)
                {
                    foreach (var child in fam.FamRec.Childs)
                    {
                        var id = child;
                        if (_indiHash.ContainsKey(id))
                        {
                            var indi = _indiHash[id];
                            bool found = false;
                            foreach (var link in indi.Indi.Links)
                            {
                                if (link.Tag == "FAMC")
                                {
                                    if (link.Xref == fam.FamRec.Ident)
                                        found = true;
                                }
                            }
                            if (!found)
                            {
                                // TODO duplicated in Pass3
                                //if (showErrors)
                                //    Console.WriteLine("Error: FAM {0} with CHIL link to {1} and no matching FAMC", fam.FamRec.Ident, id);
                                //_CHILErrorsCount += 1;
                            }
                        }
                        else
                        {
                            // TODO duplicated in Pass3
                            //if (showErrors)
                            //    Console.WriteLine("Error: FAM {0} has CHIL link {1} to non-existing INDI", fam.FamRec.Ident, id);
                        }
                    }
                }
            }
        }

        public int ChilErrorsCount
        {
            get { return _CHILErrorsCount; }
        }

        public int ErrorsCount
        {
            get { return errorsCount; }
        }

        public IndiWrap IndiFromId(string indiId)
        {
            return _indiHash[indiId];
        }

        public List<FamilyUnit> FamFromIndi(string ident)
        {
            return _childsIn[ident];
        }

        public void BuildTree2(IEnumerable<GEDCommon> gedRecs, bool showErrors, bool checkCHIL)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL
            // This variant of BuildTree believes the CHIL links are correct

            Pass1(gedRecs);

            errorsCount = 0;

            // TODO how, if at all, is the tree check impacted?

            // Iterate through the family records
            // For each HUSB/WIFE, connect to INDI
            // For each CHIL, connect to INDI
            foreach (var familyUnit in _famHash.Values)
            {
                var famId = familyUnit.FamRec.Ident;
                var dadId = familyUnit.FamRec.Dad;
                if (dadId != null) // TODO mark as error?
                {
                    IndiWrap dadWrap;
                    if (_indiHash.TryGetValue(dadId, out dadWrap))
                    {
                        dadWrap.SpouseIn.Add(familyUnit); // TODO verify dadWrap has matching FAMS
                        familyUnit.Husband = dadWrap;
                    }
                    else
                    {
                        // TODO duplicated in Pass3
                        //if (showErrors)
                        //    Console.WriteLine("Error: family {0} has HUSB link {1} to non-existing INDI", famId, dadId);
                        //errorsCount += 1;
                    }
                }
                var momId = familyUnit.FamRec.Mom;
                if (momId != null) // TODO mark as error?
                {
                    IndiWrap momWrap;
                    if (_indiHash.TryGetValue(momId, out momWrap))
                    {
                        momWrap.SpouseIn.Add(familyUnit); // TODO verify momWrap has matching FAMS
                        familyUnit.Wife = momWrap;
                    }
                    else
                    {
                        // TODO duplicated in Pass3
                        //if (showErrors)
                        //    Console.WriteLine("Error: family {0} has WIFE link {1} to non-existing INDI", famId, momId);
                        //errorsCount += 1;
                    }
                }
                foreach (var childId in familyUnit.FamRec.Childs)
                {
                    // does childId exist in _indiHash: if yes, add to familyUnit.Childs
                    // if no, error
                    IndiWrap childWrap;
                    if (_indiHash.TryGetValue(childId, out childWrap))
                    {
                        childWrap.ChildIn.Add(familyUnit);
                        familyUnit.Childs.Add(childWrap);  // TODO shouldn't this be IndiWrap?
                        _childsIn.Add(childId, familyUnit);
                    }
                    else
                    {
                        // TODO now duplicate in Pass3
                        //if (showErrors)
                        //    Console.WriteLine("Error: family {0} has CHIL link {1} to non-existing INDI", famId, childId);
                        //errorsCount += 1;
                    }
                }
                
            }

            Pass3();

            foreach (var issue in _issues)
            {
                var msg = issue.Message();
                if (msg.StartsWith("Error:")) // TODO unit testing
                    errorsCount ++;
                if (showErrors)
                    Console.WriteLine(msg);
            }

            // verify if INDI.FAMC have matching FAM.CHIL
            if (checkCHIL)
            {
                _CHILErrorsCount = 0;
                foreach (var indi in _indiHash.Values)
                {
                    foreach (var link in indi.Indi.Links)
                    {
                        if (link.Tag == "FAMC")
                        {
                            bool found = indi.ChildIn.Any(familyUnit => link.Xref == familyUnit.FamRec.Ident);
                            if (!found)
                            {
                                if (showErrors)
                                    Console.WriteLine("Error: INDI {0} with FAMC link to {1} and no matching CHIL", indi.Indi.Ident, link.Xref);
                                _CHILErrorsCount += 1;
                            }
                        }
                        if (link.Tag == "FAMS") // TODO inaccurate to check INDI.FAMS inside "child check"
                        {
                            bool found = indi.SpouseIn.Any(familyUnit => link.Xref == familyUnit.FamRec.Ident);
                            if (!found)
                            {
                                if (showErrors)
                                    Console.WriteLine("Error: INDI {0} has FAMS link {1} to non-existing family", indi.Indi.Ident, link.Xref);
                                errorsCount += 1;  // NOTE: not CHIL error!
                            }
                        }
                    }
                }

                // TODO are FAMC links to non-existing FAM being skipped?
            }
        }

    }

}
