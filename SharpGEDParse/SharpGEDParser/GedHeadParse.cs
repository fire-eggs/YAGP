﻿using System.Diagnostics;
using SharpGEDParser.Model;

namespace SharpGEDParser
{
    public class GedHeadParse : GedRecParse
    {
        protected override void BuildTagSet()
        {
            
        }

        public void Parse(KBRGedRec rec)
        {
//            Debug.Assert(false);
        }

        public void Parse(KBRGedRec rec, GedRecParse.ParseContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Parse(GEDCommon rec)
        {
            throw new System.NotImplementedException();
        }
    }
}
