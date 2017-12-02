using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpGEDParser.Model;

// TODO what 5.5 specific data -> 5.5.1 ?

// TODO trailing spaces

// TODO embedded note conversion: do custom/unknown tags go to the NOTE record?

namespace SharpGEDWriter
{
    public class FileWrite
    {
        public static void WriteGED(List<GEDCommon> records, string path)
        {
            using (var file = File.OpenWrite(path))
            {
                WriteRecs(file, records);
            }            
        }

        public static void WriteRecs(Stream outStream, List<GEDCommon> records, bool noHead=false, bool unix=true)
        {
            // TODO CR-LF vs LF
            // TODO submitter info
            StreamWriter sw;
            if (noHead) // Unit testing
                sw = new StreamWriter(outStream); // BOM confuses unit tests
            else
                sw = new StreamWriter(outStream, Encoding.UTF8);

            if (unix)
                sw.NewLine = "\n";

            {
                if (!noHead)
                    WriteHead(sw);
                WriteINDI.WriteINDIs(sw, records);
                WriteFAM(sw, records);
                WriteNOTE(sw, records);
                WriteSOUR(sw, records);
                WriteREPO(sw, records);
                WriteOBJE(sw, records);
                WriteOther(sw, records);
                if (!noHead)
                    WriteTrailer(sw);
            }
            // TODO dispose issue?
            sw.Flush();
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

            // TODO were 5.5 OBJE records converted to 5.5.1 ? kinda - don't have a REFN (error?)
            foreach (var mediaFile in mediaRecord.Files)
            {
                file.WriteLine("1 FILE {0}", mediaFile.FileRefn);
                WriteCommon.writeIfNotEmpty(file, "TITL", mediaFile.Title, 2);
                file.WriteLine("2 FORM {0}", mediaFile.Form);
                WriteCommon.writeIfNotEmpty(file, "TYPE", mediaFile.Type, 3);
            }
            WriteCommon.writeRecordTrailer(file, mediaRecord, 1);

            // TODO other lines /unknowns : don't have access to original text
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

            WriteCommon.writeRecordTrailer(file, repository, 1);
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

            if (sourceRecord.Data != null && sourceRecord.Data.Events.Count > 0)
            {
                file.WriteLine("1 DATA");
                WriteCommon.writeIfNotEmpty(file, "AGNC", sourceRecord.Data.Agency, 2);
                foreach (var sourEvent in sourceRecord.Data.Events)
                {
                    file.WriteLine("2 EVEN {0}", sourEvent.Text);
                    WriteCommon.writeIfNotEmpty(file, "DATE", sourEvent.Date, 3);
                    WriteCommon.writeIfNotEmpty(file, "PLAC", sourEvent.Place, 3);
                }
            }

            WriteCommon.writeIfNotEmpty(file, "AUTH", sourceRecord.Author, 1);
            WriteCommon.writeIfNotEmpty(file, "TITL", sourceRecord.Title, 1);
            WriteCommon.writeIfNotEmpty(file, "ABBR", sourceRecord.Abbreviation, 1);
            WriteCommon.writeIfNotEmpty(file, "PUBL", sourceRecord.Publication, 1);
            WriteCommon.writeIfNotEmpty(file, "TEXT", sourceRecord.Text, 1);

            // TODO PUBL conc/cont
            // TODO TEXT conc/cont
            // TODO TITL conc/cont
            // TODO AUTH conc/cont

            // TODO have 5.5 repository citations been converted to 5.5.1? E.g. SOUR.REPO.MEDI?
            foreach (var repoCit in sourceRecord.Cits)
            {
                file.WriteLine("1 REPO {0}", repoCit.Xref);
                foreach (var callNum in repoCit.CallNums)
                {
                    file.WriteLine("2 CALN {0}", callNum.Number);
                    WriteCommon.writeIfNotEmpty(file, "MEDI", callNum.Media, 3);
                }
                WriteCommon.writeSubNotes(file, repoCit, 2);
            }

            WriteCommon.writeRecordTrailer(file, sourceRecord, 1);
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

            WriteCommon.writeRecordTrailer(file, noteRecord, 1);
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

            WriteCommon.writeIfNotEmpty(file, "RESN", famRecord.Restriction, 1);

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

            WriteCommon.writeRecordTrailer(file, famRecord, 1);
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
