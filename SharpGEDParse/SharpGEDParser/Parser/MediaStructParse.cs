using System.Collections.Generic;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // multimedia link parsing. [Not to be confused with Multimedia Record]
    // Multimedia links are referenced from:
    // Records: SUBMITTER, SOURCE, INDI, FAM
    // Structures: EVENT_DETAIL, SOURCE_CITATION
    //
    // There are these variants of Multimedia links:
    // "1 OBJE @<xref>@                                       (either version; hardly ever used [see OBJE record])
    // "1 OBJE\n2 FILE <refn>\n3 FORM <form>\n4 MEDI <type>"  (5.5.1 syntax)
    // "1 OBJE\n2 FORM <form>\n2 FILE <refn>"                 (5.5 syntax from 5.5 standard; note that FILE may appear first!)

    // TODO some sort of post-parse checking: a) missing Xref w/ no FILE; b) use of FILE with xref; c) 5.5 variant w/ 5.5.1 file and visa-versa
    
    public class MediaStructParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"FILE", fileProc},
            {"FORM", formProc},
            {"TITL", titlProc},
            {"MEDI", mediProc},
            {"NOTE", noteProc}
        };

        private static MediaFile getFile(MediaLink dad)
        {
            if (dad.Files.Count == 0)
                dad.Files.Add(new MediaFile());
            return dad.Files[dad.Files.Count - 1];
        }

        private static void fileProc(StructParseContext ctx, int linedex, char level)
        {
            MediaLink mlink = (ctx.Parent as MediaLink);
            MediaFile file = getFile(mlink);

            // TODO handle the 5.5.1 FILE sub-sub-structure as a separate parse?
            // TODO missing file reference
            // Attempt to deal with the 5.5/5.5.1 difference: if there is no reference but is a form, apply to the *current*.
            if (string.IsNullOrEmpty(file.FileRefn))
            {
                file.FileRefn = ctx.Remain;
            }
            else
            {
                file = new MediaFile();
                mlink.Files.Add(file);
                file.FileRefn = ctx.Remain;
            }
        }

        private static void formProc(StructParseContext ctx, int linedex, char level)
        {
            // FORM may or may not follow an owning FILE tag
            MediaLink mlink = (ctx.Parent as MediaLink);
            MediaFile file = getFile(mlink);
            file.Form = ctx.Remain;
        }

        private static void mediProc(StructParseContext ctx, int linedex, char level)
        {
            // HACK assuming here the MEDI tag follows a FORM tag, using the last FILE object
            MediaLink mlink = (ctx.Parent as MediaLink);
            MediaFile file = mlink.Files[mlink.Files.Count - 1];
            file.Type = ctx.Remain;
        }

        private static void titlProc(StructParseContext ctx, int linedex, char level)
        {
            MediaLink mlink = (ctx.Parent as MediaLink);
            mlink.Title = ctx.Remain;
        }

        public static MediaLink MediaParser(StructParseContext ctx, int linedex, char level)
        {
            MediaLink mlink = new MediaLink();
            StructParseContext ctx2 = new StructParseContext(ctx, linedex, mlink);
            ctx2.Level = level;
// TODO parse xref using standard xref parser
// TODO preserve non-xref remain as note
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                mlink.Xref = ctx.Remain.Trim(new char[] { '@' });
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return mlink;
        }

        public static MediaLink MediaParser(ParseContext2 ctx)
        {
            MediaLink mlink = new MediaLink();
            StructParseContext ctx2 = new StructParseContext(ctx, mlink);
// TODO parse xref using standard xref parser
// TODO preserve non-xref remain as note
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                mlink.Xref = ctx.Remain.Trim(new char[] { '@' });
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return mlink;
        }
    }
}
