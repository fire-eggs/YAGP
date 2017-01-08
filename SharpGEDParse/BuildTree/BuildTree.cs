using System;
using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<string, IndiWrap> _indiHash;
        private MultiMap<string, FamilyUnit> _childsIn;

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

        public void BuildTree(IEnumerable<GEDCommon> gedRecs, bool showErrors, bool checkCHIL)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL but the CHIL are being ignored

            errorsCount = 0;

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            _indiHash = new Dictionary<string, IndiWrap>();    // indiIdent -> IndiRecord
            var famHash = new Dictionary<string, FamilyUnit>(); // familyIdent -> FamRecord
            string first = null;
            foreach (var gedCommon in gedRecs) // TODO really need 'INDI', 'FAM' accessors
            {
                if (gedCommon is IndiRecord)
                {
                    var ident = (gedCommon as IndiRecord).Ident;

                    if (_indiHash.ContainsKey(ident))
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Duplicate INDI ident {0}", ident);
                        errorsCount += 1;
                    }
                    else
                    {
                        IndiWrap iw = new IndiWrap();
                        iw.Indi = gedCommon as IndiRecord;
                        iw.Ahnen = 0;
                        //iw.ChildOf = null;
                        _indiHash.Add(ident, iw);

                        if (first == null)
                            first = ident;
                    }
                }
                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (gedCommon is FamRecord)
                {
                    var fam = gedCommon as FamRecord;
                    var ident = fam.Ident;
                    if (string.IsNullOrEmpty(ident))
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Missing FAM id at/near line {0}", fam.BegLine);
                        errorsCount += 1;
                        continue;
                    }
                    if (!famHash.ContainsKey(ident))
                        famHash.Add(ident, new FamilyUnit(fam));
                    else
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Duplicate family '{0}'", ident);
                        errorsCount += 1;
                    }
                }
            }

            // hash: child ids -> familyunit
            _childsIn  = new MultiMap<string, FamilyUnit>();

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
                        if (showErrors)
                            Console.WriteLine("Error: Empty link xref id for INDI {0}", indiId);
                        errorsCount += 1;
                        continue;
                    }
                    switch (indiLink.Tag)
                    {
                        case "FAMS":
                            if (famHash.TryGetValue(id, out fu))
                            {
                                indiWrap.SpouseIn.Add(fu);
                                if (fu.FamRec.Dad == indiId)
                                    fu.Husband = indiWrap.Indi;
                                else if (fu.FamRec.Mom == indiId)
                                    fu.Wife = indiWrap.Indi;
                                else
                                {
                                    if (showErrors)
                                        Console.WriteLine("Error: Could not identify spouse connection from FAM {0} to INDI {1}", indiLink.Xref, indiId);
                                    errorsCount += 1;
                                }
                            }
                            else
                            {
                                if (showErrors)
                                    Console.WriteLine("Error: INDI {0} has FAMS link {1} to non-existing family", indiId, id);
                                errorsCount += 1;
                            }
                            break;
                        case "FAMC":
                            if (famHash.TryGetValue(id, out fu))
                            {
                                _childsIn.Add(indiId, fu);
                                fu.Childs.Add(indiWrap.Indi);
                                indiWrap.ChildIn.Add(fu);
                            }
                            else
                            {
                                if (showErrors)
                                    Console.WriteLine("Error: INDI {0} has FAMC link {1} to non-existing family", indiId, id);
                                errorsCount += 1;
                            }
                            break;
                    }
                }
            }

            // Try to determine each spouse's family [the family they were born into]
            // Also check if HUSB/WIFE links are to valid people
            foreach (var familyUnit in famHash.Values)
            {
                if (familyUnit.Husband != null)
                { 
                    var dadFams = _childsIn[familyUnit.DadId];
                    if (dadFams != null && dadFams.Count > 0)
                    {
                        familyUnit.DadFam = dadFams[0];
                        if (dadFams.Count > 1)
                        {
                            Console.WriteLine("Warn: ambiguous dad connection for family {0}", familyUnit.FamRec.Ident);
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
                            Console.WriteLine("Warn: ambiguous mom connection for family {0}", familyUnit.FamRec.Ident);
                        }
                    }
                }

                var husbId = familyUnit.FamRec.Dad;
                if (husbId != null && !_indiHash.ContainsKey(husbId))
                {
                    if (showErrors)
                        Console.WriteLine("Error: family {0} has HUSB link {1} to non-existing INDI", familyUnit.FamRec.Ident, husbId);
                    errorsCount += 1;
                }
                var wifeId = familyUnit.FamRec.Mom;
                if (wifeId != null && !_indiHash.ContainsKey(wifeId))
                {
                    if (showErrors)
                        Console.WriteLine("Error: family {0} has WIFE link {1} to non-existing INDI", familyUnit.FamRec.Ident, wifeId);
                    errorsCount += 1;
                }
            }

            // verify if FAM.CHIL have matching INDI.FAMC
            if (checkCHIL)
            {
                _CHILErrorsCount = 0;
                foreach (var fam in famHash.Values)
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
                                if (showErrors)
                                    Console.WriteLine("Error: FAM {0} with CHIL link to {1} and no matching FAMC", fam.FamRec.Ident, id);
                                _CHILErrorsCount += 1;
                            }
                        }
                        else
                        {
                            if (showErrors)
                                Console.WriteLine("Error: FAM {0} has CHIL link {1} to non-existing INDI", fam.FamRec.Ident, id);
                        }
                    }
                }
            }

            famHash = null;
            families = null;
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

            // TODO how, if at all, is the tree check impacted?

            errorsCount = 0;

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            _indiHash = new Dictionary<string, IndiWrap>();    // indiIdent -> IndiRecord
            var famHash = new Dictionary<string, FamilyUnit>(); // familyIdent -> FamRecord
            string first = null;
            foreach (var gedCommon in gedRecs) // TODO really need 'INDI', 'FAM' accessors
            {
                if (gedCommon is IndiRecord)
                {
                    var ident = (gedCommon as IndiRecord).Ident;

                    if (_indiHash.ContainsKey(ident))
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Duplicate INDI ident {0}", ident);
                        errorsCount += 1;
                    }
                    else
                    {
                        IndiWrap iw = new IndiWrap();
                        iw.Indi = gedCommon as IndiRecord;
                        iw.Ahnen = 0;
                        //iw.ChildOf = null;
                        _indiHash.Add(ident, iw);

                        if (first == null)
                            first = ident;
                    }
                }
                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (gedCommon is FamRecord)
                {
                    var fam = gedCommon as FamRecord;
                    var ident = fam.Ident;
                    if (string.IsNullOrEmpty(ident))
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Missing FAM id at/near line {0}", fam.BegLine);
                        errorsCount += 1;
                        continue;
                    }
                    if (!famHash.ContainsKey(ident))
                        famHash.Add(ident, new FamilyUnit(fam));
                    else
                    {
                        if (showErrors)
                            Console.WriteLine("Error: Duplicate family {0}", ident);
                        errorsCount += 1;
                    }
                }
            }

            // hash: child ids -> familyunit
            _childsIn = new MultiMap<string, FamilyUnit>();

            // Iterate through the family records
            // For each HUSB/WIFE, connect to INDI
            // For each CHIL, connect to INDI
            foreach (var familyUnit in famHash.Values)
            {
                var famId = familyUnit.FamRec.Ident;
                var dadId = familyUnit.FamRec.Dad;
                if (dadId != null) // TODO mark as error?
                {
                    IndiWrap dadWrap;
                    if (_indiHash.TryGetValue(dadId, out dadWrap))
                    {
                        dadWrap.SpouseIn.Add(familyUnit); // TODO verify dadWrap has matching FAMS
                        familyUnit.Husband = dadWrap.Indi;
                    }
                    else
                    {
                        if (showErrors)
                            Console.WriteLine("Error: family {0} has HUSB link {1} to non-existing INDI", famId, dadId);
                        errorsCount += 1;
                    }
                }
                var momId = familyUnit.FamRec.Mom;
                if (momId != null) // TODO mark as error?
                {
                    IndiWrap momWrap;
                    if (_indiHash.TryGetValue(momId, out momWrap))
                    {
                        momWrap.SpouseIn.Add(familyUnit); // TODO verify momWrap has matching FAMS
                        familyUnit.Wife = momWrap.Indi;
                    }
                    else
                    {
                        if (showErrors)
                            Console.WriteLine("Error: family {0} has WIFE link {1} to non-existing INDI", famId, momId);
                        errorsCount += 1;
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
                        familyUnit.Childs.Add(childWrap.Indi);  // TODO shouldn't this be IndiWrap?
                        _childsIn.Add(childId, familyUnit);
                    }
                    else
                    {
                        if (showErrors)
                            Console.WriteLine("Error: family {0} has CHIL link {1} to non-existing INDI", famId, childId);
                        errorsCount += 1;
                    }
                }
                
            }

            // Connect family units
            foreach (var familyUnit in famHash.Values)
            {
                if (familyUnit.Husband != null)
                {
                    var dadFams = _childsIn[familyUnit.DadId];
                    if (dadFams != null && dadFams.Count > 0)
                    {
                        familyUnit.DadFam = dadFams[0];
                        if (dadFams.Count > 1)
                        {
                            Console.WriteLine("Warn: ambiguous dad connection for family {0}", familyUnit.FamRec.Ident);
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
                            Console.WriteLine("Warn: ambiguous mom connection for family {0}", familyUnit.FamRec.Ident);
                        }
                    }
                }
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

            famHash = null;
            families = null;
        }

    }

}
