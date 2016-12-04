using System;
using SharpGEDParser.Model;

// TODO the only diff between StructParseContext and ParseContext is the datatype of the Parent

namespace SharpGEDParser.Parser
{
    public class ParseContextCommon : LineUtil.LineData
    {
        public GedRecord Lines;
        public int Begline; // index of first line for this 'record'
        public int Endline; // index of last line FOUND for this 'record'

        public ParseContextCommon()
        {
        }

        public ParseContextCommon(ParseContextCommon ctx)
        {
            Lines = ctx.Lines;
            Begline = ctx.Begline;
            Endline = ctx.Endline;
            Level = ctx.Level;
            Remain = ctx.Remain;
        }
    }

    public class StructParseContext : ParseContextCommon
    {
        public StructCommon Parent;

        public StructParseContext()
        {
            throw new Exception(); // don't be calling me!
        }

        public StructParseContext(ParseContext2 ctx, StructCommon parent)
            : base(ctx)
        {
            Parent = parent;
        }

        public StructParseContext(StructParseContext ctx, int linedex, StructCommon parent)
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
