using System;
using System.Collections.Generic;
using SharpGEDParser.Model;

// 01\00001.ged has one person I0009 who is in more than one family

namespace BuildTree
{
    public class FamilyTreeBuild
    {
        private Dictionary<string, IndiWrap> _indiHash;
        private Dictionary<string, FamilyUnit> _childHash;

        public IEnumerable<string> IndiIds
        {
            get { return _indiHash.Keys; }
        }

        private IndiWrap MakeFillerIndi(string ident, out IndiRecord hack)
        {
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

        public void BuildTree(IEnumerable<GEDCommon> gedRecs)
        {
            // an indi has a FAMS or FAMC
            // a FAM has HUSB WIFE CHIL

            List<FamilyUnit> families = new List<FamilyUnit>();

            // Build a hash of Indi ids
            // Build a hash of family ids
            _indiHash = new Dictionary<string, IndiWrap>();
            var famHash = new Dictionary<string, FamRecord>();
            string first = null;
            foreach (var gedCommon in gedRecs) // TODO really need 'INDI', 'FAM' accessors
            {
                if (gedCommon is IndiRecord)
                {
                    var ident = (gedCommon as IndiRecord).Ident;

                    IndiWrap iw = new IndiWrap();
                    iw.Indi = gedCommon as IndiRecord;
                    iw.Ahnen = 0;
                    //iw.ChildOf = null;
                    _indiHash.Add(ident, iw);

                    if (first == null)
                        first = ident;
                }
                // TODO GEDCOM_Amssoms.ged has a duplicate family "X0". Needs to be caught by validate, flag as error, and not reach here.
                if (gedCommon is FamRecord)
                {
                    var ident = (gedCommon as FamRecord).Ident;
                    if (!famHash.ContainsKey(ident))
                        famHash.Add(ident, gedCommon as FamRecord);
                    else
                    {
                        Console.WriteLine("duplicate family");
                    }
                }
            }

            // hash: child ids -> familyunit
            _childHash = new Dictionary<string, FamilyUnit>();

            // Accumulate family units
            // TODO indi with no fam
            // TODO indi with fams/famc only : see GEDCOM_Amssoms
            foreach (var famRecord in famHash.Values)
            {
                var famU = new FamilyUnit(famRecord);
                if (famRecord.Dad != null)
                {
                    // TODO GEDCOM_Amssoms has a family with reference to non-existant individual. Needs to be caught by validate and 'fixed' there.
                    if (_indiHash.ContainsKey(famRecord.Dad))
                    {
                        var iw = _indiHash[famRecord.Dad];
                        famU.Husband = iw.Indi;
                        iw.SpouseIn.Add(famU);
                    }
                    else
                    {
                        IndiRecord hack;
                        var hack0 = MakeFillerIndi(famRecord.Dad, out hack);
                        famU.Husband = hack;
                        _indiHash.Add(famRecord.Dad, hack0);
                        hack0.SpouseIn.Add(famU);

                        //IndiWrap hack0 = new IndiWrap();

                        //// TODO need a library method to do this!!!
                        //IndiRecord hack = new IndiRecord(null, famRecord.Dad, null);
                        //var hack2 = new NameRec();
                        //hack2.Surname = "Missing";
                        //hack.Names.Add(hack2);
                        //famU.Husband = hack;
                        //hack0.Indi = hack;
                        //hack0.Ahnen = -1;
                        //hack0.SpouseIn.Add(famU);
                        //_indiHash.Add(famRecord.Dad, hack0);
                    }
                }
                if (famRecord.Mom != null)
                {
                    if (_indiHash.ContainsKey(famRecord.Mom))
                    {
                        var iw = _indiHash[famRecord.Mom];
                        famU.Wife = iw.Indi; // TODO handle mom as non-existant individual.
                        iw.SpouseIn.Add(famU);
                    }
                    else
                    {
                        // 11tp.ged, 16334-rlb.ged, 5nhj.ged : missing mom
                        IndiRecord hack;
                        var hack0 = MakeFillerIndi(famRecord.Mom, out hack);
                        famU.Wife = hack;
                        _indiHash.Add(famRecord.Mom, hack0);
                        hack0.SpouseIn.Add(famU);
                    }
                }
                foreach (var child in famRecord.Childs)
                {
                    famU.Childs.Add(_indiHash[child].Indi);

                    // TODO punting on adoption where a child could be part of more than one family see allged.ged
                    if (!_childHash.ContainsKey(child))
                        _childHash.Add(child, famU);
                    else
                    {
                        Console.WriteLine("indi {0} a child of more than one family",child);
                    }
                }
                families.Add(famU);
            }

            // Connect family units
            foreach (var familyUnit in families)
            {
                if (_childHash.ContainsKey(familyUnit.DadId))
                    familyUnit.DadFam = _childHash[familyUnit.DadId];
                if (_childHash.ContainsKey(familyUnit.MomId))
                    familyUnit.MomFam = _childHash[familyUnit.MomId];
            }

            famHash = null;
            families = null;
        }

        public IndiWrap IndiFromId(string indiId)
        {
            return _indiHash[indiId];
        }

        public FamilyUnit FamFromIndi(string ident)
        {
            FamilyUnit fu;
            return _childHash.TryGetValue(ident, out fu) ? fu : null;
        }
    }

}
