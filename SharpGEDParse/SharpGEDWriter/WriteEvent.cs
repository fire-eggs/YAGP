using SharpGEDParser.Model;
using System.Collections.Generic;
using System.IO;

namespace SharpGEDWriter
{
    class WriteEvent
    {
        internal static void writeEvents(StreamWriter file, List<FamilyEvent> events, int level)
        {
            foreach (var familyEvent in events)
            {
                writeEventCommon(file, familyEvent, level);
                if (familyEvent.HusbDetail != null)
                {
                    file.WriteLine(string.Format("{0} HUSB {1}", level + 1, familyEvent.HusbDetail.Detail).Trim()); // TODO extra is non-standard : how to deal?
                    file.WriteLine("{0} AGE {1}", level+2, familyEvent.HusbDetail.Age);
                }
                if (familyEvent.WifeDetail != null)
                {
                    file.WriteLine(string.Format("{0} WIFE {1}", level+1, familyEvent.WifeDetail.Detail).Trim()); // TODO extra is non-standard : how to deal?
                    file.WriteLine("{0} AGE {1}", level+2, familyEvent.WifeDetail.Age);
                }
            }
        }

        internal static void writeEvents(StreamWriter file, List<IndiEvent> events, int level)
        {
            foreach (var indiEvent in events)
            {
                writeEventCommon(file, indiEvent, level);
                if (!string.IsNullOrWhiteSpace(indiEvent.Famc))
                {
                    // INDI.BIRT, INDI.CHR, INDI.ADOP
                    WriteCommon.writeXrefIfNotEmpty(file, "FAMC", indiEvent.Famc, level+1);
                    WriteCommon.writeIfNotEmpty(file, "ADOP", indiEvent.FamcAdop,level+2);
                }
            }
        }

        internal static void writeEventCommon(StreamWriter file, EventCommon data, int level)
        {
            WriteCommon.writeExtended(file, level, data.Tag.ToString(), data.Descriptor); // TODO use GedTag?

            WriteCommon.writeIfNotEmpty(file, "TYPE", data.Type, level + 1);
            WriteCommon.writeIfNotEmpty(file, "DATE", data.Date, level + 1);

            WriteCommon.writeIfNotEmpty(file, "PLAC", data.Place, level + 1);
            // TODO place structure

            WriteCommon.writeIfNotEmpty(file, "AGNC", data.Agency, level + 1);
            WriteCommon.writeIfNotEmpty(file, "RELI", data.Religion, level + 1);
            WriteCommon.writeIfNotEmpty(file, "CAUS", data.Cause, level + 1);
            WriteCommon.writeIfNotEmpty(file, "RESN", data.Restriction, level + 1);

            WriteCommon.writeSubNotes(file, data, level + 1);
            WriteCommon.writeSourCit(file, data, level + 1);
            WriteCommon.writeObjeLink(file, data, level + 1);
        }

    }
}
