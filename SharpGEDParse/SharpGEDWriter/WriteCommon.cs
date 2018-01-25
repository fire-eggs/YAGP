using SharpGEDParser.Model;
using System.Collections.Generic;
using System.IO;

// ReSharper disable InconsistentNaming

namespace SharpGEDWriter
{
    class WriteCommon
    {
        internal static void writeSubNotes(StreamWriter file, NoteHold rec, int level = 1)
        {
            // TODO embedded notes -> note xref
            if (rec == null || rec.Notes.Count == 0)
                return;
            foreach (var note in rec.Notes)
            {
                if (!string.IsNullOrWhiteSpace(note.Xref))
                    file.WriteLine("{0} NOTE @{1}@", level, note.Xref);
                else
                {
                    writeExtended(file, level, "NOTE", note.Text);
                }
            }
        }

        private const int MAXLEN = 245; // Must be no more than 247: "n CONC <text>"

        private static void writeWithConc(StreamWriter file, int level, string tag, string text, bool incrLevel)
        {
            // Start by including the tag/ident for the "full" string. This means the initial line will
            // be shorter than subsequent CONC lines, but not at the risk of being too long because of
            // a long ident string.
            var fullStr = string.Format("{0} {1} {2}", level, tag, text);
            if (fullStr.Length < MAXLEN)
            {
                file.WriteLine(fullStr);
                return;
            }

            int dex;
            for (dex = 0; dex+MAXLEN < fullStr.Length; )
            {
                int beg = dex;
                int len = MAXLEN;
                while (fullStr[beg+len-1] == ' ' ||
                       fullStr[beg+len] == ' ') // DO NOT end the line on a space! Or start the next
                    len -= 1;

                if (dex == 0)
                    file.WriteLine(fullStr.Substring(beg, len)); // Initial level/ident/tag already in place
                else
                    file.WriteLine("{0} CONC {1}", level+1, fullStr.Substring(beg, len));
                dex = beg+len;
            }
            if (dex < fullStr.Length) // Write any leftovers
            {
                file.WriteLine("{0} CONC {1}", level + 1, fullStr.Substring(dex));
            }
        }

        private static char[] nlSplit = {'\n'};

        public static void writeExtended(StreamWriter file, int level, string tag, string text)
        {
            // write a tag which may have extended text (requiring the use of CONC/CONT tags)
            // GEDCOM standard specifies that line length is limited to 255 chars
            if (string.IsNullOrEmpty(text))
                text = "";

            // Don't do extra work for short/unsplit lines
            if (text.Length < 247 && !text.Contains("\n"))
            {
                file.WriteLine(string.Format("{0} {1} {2}", level, tag, text).Trim());
                return;
            }

            // original CONT tags were marked as embedded newlines; separate out and add required tags
            var lines = text.Split(nlSplit);
            writeWithConc(file, level, tag, lines[0], false);
            for (int i = 1; i < lines.Length; i++)
            {
                writeWithConc(file, level + 1, "CONT", lines[i], true);
            }
        }

        internal static void writeObjeLink(StreamWriter file, MediaHold rec, int level=1)
        {
            // TODO embedded OBJE -> OBJE xref
            if (rec == null || rec.Media.Count == 0)
                return;
            foreach (var mediaLink in rec.Media)
            {
                if (string.IsNullOrWhiteSpace(mediaLink.Xref))
                {
                    // variant
                    file.WriteLine("{0} OBJE", level);
                    writeIfNotEmpty(file, "TITL", mediaLink.Title, level+1);
                    foreach (var fileref in mediaLink.Files)
                    {
                        file.WriteLine("{0} FILE {1}", level + 1, fileref.FileRefn);
                        writeIfNotEmpty(file, "FORM", fileref.Form, level+2);
                        writeIfNotEmpty(file, "MEDI", fileref.Type, level + 3);
                    }
                }
                else
                    file.WriteLine("{0} OBJE @{1}@", level, mediaLink.Xref);
                // TODO other lines
            }
        }

        internal static void writeChan(StreamWriter file, GEDCommon rec)
        {
            if (rec.CHAN.Date == null)
                return;
            file.WriteLine("1 CHAN");
            file.WriteLine("2 DATE {0}", rec.CHAN.Date.Value.ToString("d MMM yyyy").ToUpper());
            // TODO change time?
            writeSubNotes(file, rec.CHAN, 2);

            // TODO otherlines
        }

        // TODO conversion from descriptive to reference source citations

        internal static void writeSourCit(StreamWriter file, SourceCitHold rec, int level=1)
        {
            if (rec == null || rec.Cits.Count == 0)
                return;
            foreach (var cit in rec.Cits)
            {
                if (string.IsNullOrWhiteSpace(cit.Xref))
                {
                    // not using source records variant
                    if (string.IsNullOrWhiteSpace(cit.Desc))
                        file.WriteLine("{0} SOUR", level);
                    else
                        writeExtended(file, level, "SOUR", cit.Desc);
                }
                else
                {
                    // pointer to source record variant
                    file.WriteLine("{0} SOUR @{1}@", level, cit.Xref);
                    // TODO anytext?
                }

                writeIfNotEmpty(file, "PAGE", cit.Page, level+1);
                writeIfNotEmpty(file, "EVEN", cit.Event, level+1);
                writeIfNotEmpty(file, "ROLE", cit.Role, level+2); // TODO role specified but not event

                if (cit.Data)
                {
                    file.WriteLine("{0} DATA", level+1);
                    writeIfNotEmpty(file, "DATE", cit.Date, level+2);
                    foreach (var aTxt in cit.Text)
                    {
                        writeExtIfNotEmpty(file, "TEXT", aTxt, level + 2);
                    }
                }
                else
                {
                    foreach (var aTxt in cit.Text)
                    {
                        writeExtIfNotEmpty(file, "TEXT", aTxt, level + 1);
                    }
                }

                writeIfNotEmpty(file, "QUAY", cit.Quay, level+1);

                writeSubNotes(file, cit, level+1);
                writeObjeLink(file, cit, level+1);
            }
        }

        internal static void writeIds(StreamWriter file, GEDCommon rec, int level = 1)
        {
            if (rec.REFNs.Count < 1 && rec.UID == null && rec.AFN == null && rec.RFN == null)
                return;
            foreach (var refN in rec.REFNs)
            {
                file.WriteLine("{0} REFN {1}", level, refN.Value);
                // TODO don't have original 'TYPE' lines
            }

            if (rec.UID != null)
            {
                var val = System.Text.Encoding.ASCII.GetString(rec.UID);
                file.WriteLine("{0} _UID {1}", level, val);
            }
            if (rec.AFN != null)
                file.WriteLine("{0} AFN {1}", level, rec.AFN.Value);
            if (rec.RFN != null)
                file.WriteLine("{0} RFN {1}", level, rec.RFN.Value);
        }

        public static void writeExtIfNotEmpty(StreamWriter file, string tag, string value, int level)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            writeExtended(file, level, tag, value);
        }

        public static void writeIfNotEmpty(StreamWriter file, string tag, string value, int level)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            file.WriteLine("{0} {1} {2}", level, tag, value);
        }
        public static void writeXrefIfNotEmpty(StreamWriter file, string tag, string value, int level)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            file.WriteLine("{0} {1} @{2}@", level, tag, value);
        }

        public static void writeMultiple(StreamWriter file, string tag, List<string> vals, int level)
        {
            foreach (var val in vals)
            {
                writeIfNotEmpty(file, tag, val, level);
            }
        }

        public static void writeAddr(StreamWriter file, Address addr, int level)
        {
            if (addr == null)
                return;

            // TODO continuation
            file.WriteLine("{0} ADDR {1}", level, addr.Adr);
            writeIfNotEmpty(file, "ADR1", addr.Adr1, level + 1);
            writeIfNotEmpty(file, "ADR2", addr.Adr2, level + 1);
            writeIfNotEmpty(file, "ADR3", addr.Adr3, level + 1);
            writeIfNotEmpty(file, "CITY", addr.City, level + 1);
            writeIfNotEmpty(file, "STAE", addr.Stae, level + 1);
            writeIfNotEmpty(file, "POST", addr.Post, level + 1);
            writeIfNotEmpty(file, "CTRY", addr.Ctry, level + 1);

            writeMultiple(file, "PHON", addr.Phon, level);
            writeMultiple(file, "EMAIL", addr.Email, level);
            writeMultiple(file, "FAX", addr.Fax, level);
            writeMultiple(file, "WWW", addr.WWW, level);
        }

        internal static void writeRecordTrailer(StreamWriter file, GEDCommon rec, int level)
        {
            writeIds(file, rec);
            writeIfNotEmpty(file, "RIN", rec.RIN, level);
            writeSubNotes(file, rec as NoteHold);
            writeSourCit(file, rec as SourceCitHold);
            writeObjeLink(file, rec as MediaHold);
            writeChan(file, rec);
        }
    }
}
