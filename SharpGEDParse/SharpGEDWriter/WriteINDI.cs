using SharpGEDParser.Model;
using System;
using System.Collections.Generic;
using System.IO;

// TODO can record ident be generalized?

namespace SharpGEDWriter
{
    class WriteINDI
    {
        internal static void WriteINDIs(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is IndiRecord)
                    WriteOneIndi(file, gedCommon as IndiRecord);
            }
        }

        private static void WriteOneIndi(StreamWriter file, IndiRecord indiRecord)
        {
            file.WriteLine("0 @{0}@ INDI", indiRecord.Ident);

            WriteEvent.writeEvents(file, indiRecord.Events, 1);
            WriteEvent.writeEvents(file, indiRecord.Attribs, 1);

            // TODO LDS events

            // TODO why are INDI and FAM submitters treated different?
            foreach (var submitter in indiRecord.Submitters)
            {
                file.WriteLine("1 SUBM @{0}@", submitter.Xref);
            }

            // TODO this is replicated in FAM... common sub-type?
            WriteCommon.writeIds(file, indiRecord);
            if (!string.IsNullOrEmpty(indiRecord.RIN))
                file.WriteLine("1 RIN {0}", indiRecord.RIN);
            WriteCommon.writeSubNotes(file, indiRecord);
            WriteCommon.writeSourCit(file, indiRecord);
            WriteCommon.writeObjeLink(file, indiRecord);
            WriteCommon.writeChan(file, indiRecord);

        }
    }
}
