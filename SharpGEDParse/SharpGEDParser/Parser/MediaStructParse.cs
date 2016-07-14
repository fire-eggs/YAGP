using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // There are two variants of Multimedia links
    // "1 OBJE\n2 FILE <refn>\n3 FORM <form>\n4 MEDI <type>"
    // "1 OBJE\n2 FILE\n2 FORM <form>\n3 MEDI <type>"

    public class MediaStructParse : StructParser
    {
        private static readonly Dictionary<string, TagProc> tagDict = new Dictionary<string, TagProc>()
        {
            {"FILE", fileProc},
            {"FORM", formProc},
            {"TITL", titlProc},
            {"MEDI", mediProc}
        };

        private static void fileProc(StructParseContext ctx, int linedex, char level)
        {
            MediaLink mlink = (ctx.Parent as MediaLink);
            MediaFile file = new MediaFile();
            file.FileRefn = ctx.Remain;
            mlink.Files.Add(file);
        }

        private static void formProc(StructParseContext ctx, int linedex, char level)
        {
            // HACK assuming here the FORM tag follows a FILE tag, using the last FILE object
            MediaLink mlink = (ctx.Parent as MediaLink);
            MediaFile file = mlink.Files[mlink.Files.Count - 1];
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
            if (!string.IsNullOrEmpty(ctx.Remain) && ctx.Remain[0] == '@')
            {
                mlink.Xref = ctx.Remain.Trim(new char[] { '@' });
            }
            else
            {
                // TODO need an error mechanism here: non-xref for OBJE link
            }

            StructParse(ctx2, tagDict);
            ctx.Endline = ctx2.Endline;
            return mlink;
        }
    }
}
