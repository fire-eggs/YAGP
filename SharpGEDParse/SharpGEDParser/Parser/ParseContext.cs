using System;
using System.Diagnostics.CodeAnalysis;
using SharpGEDParser.Model;

// TODO the only diff between StructParseContext and ParseContext is the datatype of the Parent

namespace SharpGEDParser.Parser
{
    public class ParseContextCommon : LineUtil.LineData
    {
        public GedRecord Lines;
        public int Begline; // index of first line for this 'record'
        public int Endline; // index of last line FOUND for this 'record'
        public GEDSplitter gs;

        public StringCache tagCache;

        public ParseContextCommon()
        {
        }

        public ParseContextCommon(ParseContextCommon ctx)
        {
            Lines = ctx.Lines;
            Begline = ctx.Begline;
            Endline = ctx.Endline;
            Level = ctx.Level;
            Remain1 = ctx.Remain1;
            gs = ctx.gs;

            tagCache = ctx.tagCache;
        }
    }

    public class StructParseContext : ParseContextCommon
    {
        public StructCommon Parent;

        [ExcludeFromCodeCoverage]
        public StructParseContext()
        {
            throw new Exception(); // don't be calling me!
        }

        public StructParseContext(ParseContext2 ctx, StructCommon parent)
            : base(ctx)
        {
            Parent = parent;
        }

        public StructParseContext(ParseContextCommon ctx, StructCommon parent, int linedex = 0)
            : base(ctx)
        {
            Parent = parent;
            Begline = linedex;
            Endline = linedex;
        }

        public StructParseContext(ParseContextCommon ctx, int linedex, char level, StructCommon parent)
            : base(ctx)
        {
            Parent = parent;
            Begline = linedex;
            Endline = linedex;
            Level = level;
        }
    }

    public class ParseContext2 : ParseContextCommon
    {
        public GEDCommon Parent;
    }
}
