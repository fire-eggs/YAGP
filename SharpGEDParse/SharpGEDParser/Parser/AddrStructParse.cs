using SharpGEDParser.Model;

namespace SharpGEDParser.Parser
{
    // TODO what common/custom tags from programs?

    public class AddrStructParse
    {
        public static Address AddrParse(GedRecParse.ParseContext2 ctx)
        {
            Address addr = new Address();

            addr.Adr += ctx.Remain;

            int i = ctx.Begline + 1;

            char level = ' ';
            string ident = null;
            string tag = null;
            string remain = null;
            for (; i < ctx.Lines.Max; i++)
            {
                GedLineUtil.LevelTagAndRemain(ctx.Lines.GetLine(i), ref level, ref ident, ref tag, ref remain);
                if (level <= ctx.Level)
                    break; // end of sub-record
                switch (tag)
                {
                    case "CONT":
                        addr.Adr += "\n" + remain;
                        break;
                    case "ADR1":
                        addr.Adr1 = remain;
                        break;
                    case "ADR2":
                        addr.Adr2 = remain;
                        break;
                    case "ADR3":
                        addr.Adr3 = remain;
                        break;
                    case "CITY":
                        addr.City = remain;
                        break;
                    case "STAE":
                        addr.Stae = remain;
                        break;
                    case "POST":
                        addr.Post = remain;
                        break;
                    case "CTRY":
                        addr.Ctry = remain;
                        break;
                    case "PHON":
                        addr.Phon = remain;
                        break;
                    case "EMAIL":
                        addr.Email = remain;
                        break;
                    case "FAX":
                        addr.Fax = remain;
                        break;
                    case "WWW":
                        addr.WWW = remain;
                        break;

                    default:
                        LineSet extra = new LineSet();
                        ctx.Begline = i;
                        GedRecParse.LookAhead(ctx);
                        extra.Beg = ctx.Begline;
                        extra.End = ctx.Endline;
                        addr.OtherLines.Add(extra);
                        i = ctx.Endline;
                        break;
                }
            }
            ctx.Endline = i - 1;
            return addr;
        }
    }
}
