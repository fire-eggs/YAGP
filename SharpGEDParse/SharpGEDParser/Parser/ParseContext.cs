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
            Init(ctx);
        }

        internal void Init(ParseContextCommon ctx)
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
        public GEDCommon Record;

        [ExcludeFromCodeCoverage]
        public StructParseContext()
        {
//            throw new Exception(); // don't be calling me!
        }

        public void Init(ParseContext2 ctx, StructCommon parent)
        {
            base.Init(ctx);
            Parent = parent;
            Record = ctx.Parent;
        }

        public StructParseContext(ParseContext2 ctx, StructCommon parent)
            : base(ctx)
        {
            Parent = parent;
            Record = ctx.Parent;
        }

        public StructParseContext(ParseContextCommon ctx, StructCommon parent, int linedex = 0)
            : base(ctx)
        {
            // TODO pass Level, Record into here?
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

    public static class PContextFactory
    {
        private static ObjectPool<StructParseContext> _pool = new ObjectPool<StructParseContext>(() => new StructParseContext());

        public static StructParseContext Alloc(ParseContext2 ctx, StructCommon parent)
        {
            var spc = _pool.GetObject();
            spc.Init(ctx, parent);
            return spc;
        }
        public static void Free(StructParseContext spc)
        {
            spc.Parent = null;
            spc.Record = null;
            spc.Lines = null;
            spc.gs = null;
            spc.tagCache = null;
            _pool.PutObject(spc);
        }
    }
}
