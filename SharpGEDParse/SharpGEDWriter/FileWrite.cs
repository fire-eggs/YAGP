using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Model;

// TODO CHAN, REFN, RIN, other ids: common to all GEDCommon
// TODO cits, obje, sour, notes: almost all common to GEDCommon

// TODO what 5.5 specific data -> 5.5.1 ?

// TODO trailing spaces

namespace SharpGEDWriter
{
    public class FileWrite
    {
        public static void WriteGED(List<GEDCommon> records, string path)
        {
            // TODO submitter info

            using (var file = File.OpenWrite(path))
            using (var sw = new StreamWriter(file, Encoding.UTF8))
            {
                WriteHead(sw);
                WriteINDI.WriteINDIs(sw, records);
                WriteFAM(sw, records);
                WriteNOTE(sw, records);
                WriteSOUR(sw, records);
                WriteREPO(sw, records);
                WriteOBJE(sw, records);
                WriteOther(sw, records);
                WriteTrailer(sw);
            }
        }

        private static void WriteTrailer(StreamWriter file)
        {
            file.WriteLine("0 TRLR");
        }

        private static void WriteOther(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is Unknown)
                    WriteOneOther(file, gedCommon as Unknown);
            }
        }

        private static void WriteOneOther(StreamWriter file, Unknown unknown)
        {
            file.WriteLine("0 @{0}@ {1}", unknown.Ident, unknown.Tag);

            // TODO don't have access to full lines
        }

        private static void WriteOBJE(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is MediaRecord)
                    WriteOneObje(file, gedCommon as MediaRecord);
            }
        }

        private static void WriteOneObje(StreamWriter file, MediaRecord mediaRecord)
        {
            file.WriteLine("0 @{0}@ OBJE", mediaRecord.Ident);

            WriteCommon.writeIds(file, mediaRecord);
            if (!string.IsNullOrEmpty(mediaRecord.RIN))
                file.WriteLine("1 RIN {0}", mediaRecord.RIN);
            WriteCommon.writeSubNotes(file, mediaRecord);
            WriteCommon.writeSourCit(file, mediaRecord);
            WriteCommon.writeChan(file, mediaRecord);
        }

        private static void WriteREPO(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is Repository)
                    WriteOneRepo(file, gedCommon as Repository);
            }
        }

        private static void WriteOneRepo(StreamWriter file, Repository repository)
        {
            file.WriteLine("0 @{0}@ REPO", repository.Ident);

            // TODO a missing name is an error...
            WriteCommon.writeIfNotEmpty(file, "NAME", repository.Name, 1);

            WriteCommon.writeAddr(file, repository.Addr, 1);

            WriteCommon.writeIds(file, repository);
            if (!string.IsNullOrEmpty(repository.RIN))
                file.WriteLine("1 RIN {0}", repository.RIN);
            WriteCommon.writeSubNotes(file, repository);
            WriteCommon.writeChan(file, repository);
        }

        private static void WriteSOUR(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is SourceRecord)
                    WriteOneSour(file, gedCommon as SourceRecord);
            }
        }

        private static void WriteOneSour(StreamWriter file, SourceRecord sourceRecord)
        {
            file.WriteLine("0 @{0}@ SOUR", sourceRecord.Ident);

            foreach (var repoCit in sourceRecord.Cits)
            {
                WriteCommon.writeXrefIfNotEmpty(file, "REPO", repoCit.Xref, 1);
            }

            WriteCommon.writeIds(file, sourceRecord);
            WriteCommon.writeIfNotEmpty(file, "RIN", sourceRecord.RIN, 1);

            WriteCommon.writeSubNotes(file, sourceRecord);
            WriteCommon.writeObjeLink(file, sourceRecord);
            WriteCommon.writeChan(file, sourceRecord);
        }

        private static void WriteNOTE(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is NoteRecord)
                    WriteOneNote(file, gedCommon as NoteRecord);
            }
        }

        private static void WriteOneNote(StreamWriter file, NoteRecord noteRecord)
        {
            file.WriteLine("0 @{0}@ NOTE", noteRecord.Ident);

            WriteCommon.writeIds(file, noteRecord);
            if (!string.IsNullOrEmpty(noteRecord.RIN))
                file.WriteLine("1 RIN {0}", noteRecord.RIN);
            WriteCommon.writeSourCit(file, noteRecord);
            WriteCommon.writeChan(file, noteRecord);
        }

        private static void WriteFAM(StreamWriter file, List<GEDCommon> records)
        {
            foreach (var gedCommon in records)
            {
                if (gedCommon is FamRecord)
                    WriteOneFam(file, gedCommon as FamRecord);
            }
        }

        private static void WriteOneFam(StreamWriter file, FamRecord famRecord)
        {
            file.WriteLine("0 @{0}@ FAM", famRecord.Ident);

            if (!string.IsNullOrEmpty(famRecord.Restriction))
                file.WriteLine("1 RESN {0}", famRecord.Restriction);

            // TODO multiple HUSB/WIFE
            if (famRecord.Dads.Count > 0)
                file.WriteLine("1 HUSB @{0}@", famRecord.Dads[0]);
            if (famRecord.Moms.Count > 0)
                file.WriteLine("1 WIFE @{0}@", famRecord.Moms[0]);
            foreach (var child in famRecord.Childs)
            {
                file.WriteLine("1 CHIL @{0}@", child.Xref);
                // TODO parent relations
            }

            if (famRecord.ChildCount > 0)
                file.WriteLine("1 NCHI {0}", famRecord.ChildCount);

            WriteEvent.writeEvents(file, famRecord.FamEvents, 1);

            // TODO LDS Spouse Sealing

            // TODO why are INDI and FAM submitters treated different?
            foreach (var submitter in famRecord.FamSubm)
            {
                file.WriteLine("1 SUBM @{0}@", submitter);
            }

            // TODO replicated in INDI... commmon sub-class?
            WriteCommon.writeIds(file, famRecord);
            if (!string.IsNullOrEmpty(famRecord.RIN))
                file.WriteLine("1 RIN {0}", famRecord.RIN);
            WriteCommon.writeSubNotes(file, famRecord);
            WriteCommon.writeSourCit(file, famRecord);
            WriteCommon.writeObjeLink(file, famRecord);
            WriteCommon.writeChan(file, famRecord);
        }

        private static void WriteHead(StreamWriter file)
        {
            // TODO SUBM info
            // TODO file notes
            // TODO destination path?
            // TODO date/time

            file.WriteLine("0 HEAD");
            file.WriteLine("1 GEDC");
            file.WriteLine("2 VERS 5.5.1");
            file.WriteLine("2 FORM LINEAGE-LINKED");
            file.WriteLine("1 CHAR UTF-8");
            file.WriteLine("1 SOUR SharpGEDWriter"); // TODO not registered
            file.WriteLine("2 VERS V0.2-Alpha");
            file.WriteLine("1 SUBM @S0@");

            file.WriteLine("0 @S0@ SUBM");
        }
    }
}
