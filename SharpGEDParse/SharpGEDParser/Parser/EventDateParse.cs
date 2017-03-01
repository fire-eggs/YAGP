using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpGEDParser.Model;

// TODO validation should include checks for non-standard months, keywords

namespace SharpGEDParser.Parser
{
    static class EventDateParse
    {
        private static object[] gregMonData =
        {
            "JAN", 1,
            "FEB", 2,
            "MAR", 3,
            "APR", 4,
            "MAY", 5,
            "JUN", 6,
            "JUL", 7,
            "AUG", 8,
            "SEP", 9,
            "OCT", 10,
            "NOV", 11,
            "DEC", 12,
        };

        private static Dictionary<string, Cal> calLookup = new Dictionary<string, Cal>
        {
            {"#DGREGORIAN", Cal.Greg},
            {"#DJULIAN", Cal.Jul},
            {"#DHEBREW", Cal.Hebr},
            {"#DFRENCH R", Cal.Fre},
            {"#DROMAN", Cal.Rom},
            {"#DUNKNOWN", Cal.Fut},
        };

        private static Dictionary<string, int> gregMonLookup = new Dictionary<string, int>()
        {
            {"JAN", 1},
            {"FEB", 2},
            {"MAR", 3},
            {"APR", 4},
            {"MAY", 5},
            {"JUN", 6},
            {"JUL", 7},
            {"AUG", 8},
            {"SEP", 9},
            {"OCT",10},
            {"NOV",11},
            {"DEC",12},
        };
        // Variations on the gregorian months - accepted but should be flagged
        // as non-standard
        private static Dictionary<string, int> gregMonNonStdLookup = new Dictionary<string, int>()
        {
            {"JAN.", 1},
            {"FEB.", 2},
            {"MAR.", 3},
            {"APR.", 4},
            {"MAY.", 5},
            {"JUN.", 6},
            {"JUL.", 7},
            {"AUG.", 8},
            {"SEP.", 9},
            {"OCT.",10},
            {"NOV.",11},
            {"DEC.",12},
            {"SEPT.", 9},
            {"SEPT", 9},
            {"JANUARY", 1},
            {"FEBRUARY", 2},
            {"MARCH", 3},
            {"APRIL", 4},
            {"JUNE", 6},
            {"JULY", 7},
            {"AUGUST", 8},
            {"SEPTEMBER", 9},
            {"OCTOBER",10},
            {"NOVEMBER",11},
            {"DECEMBER",12},
        };
 
        static EventDateParse()
        {
        }

        private enum Cal
        {
            Greg, Jul, Hebr, Fre, Rom, Fut
        }

        private static Cal CheckCalendar(string calstr)
        {
            Cal calen;
            if (calLookup.TryGetValue(calstr, out calen))
                return calen;
            return Cal.Fut;
        }

        private class Context
        {
            private List<Token> toks;
            private int tokDex;
            private int tokLen;
            public string datestr;
            public GEDDate gd;

            public Context( string _str, List<Token> _toks )
            {
                datestr = _str;
                toks = _toks;
                tokDex = 0;
                tokLen = toks.Count;
            }

            public Token LookAhead(int far=0)
            {
                if (tokDex+far >= tokLen)
                    return new Token {type = TokType.EOF};
                return toks[tokDex+far];
            }

            public Token Consume()
            {
                tokDex++;
                return LookAhead();
            }

            public string getString()
            {
                return toks[tokDex].getString(datestr);
            }

            public int getInt()
            {
                return toks[tokDex].getInt(datestr);
            }
        }

        public static GEDDate DateParser(string datestr)
        {
            Context ctx = new Context(datestr, new DateTokens().Tokenize(datestr));

            Token tok = ctx.LookAhead();
            Cal calen = Cal.Greg;
            if (tok.type == TokType.CALEN)
            {
                calen = CheckCalendar(ctx.getString());
                ctx.Consume();
            }
            if (calen != Cal.Greg) // TODO other calendar support
                return new GEDDate(GEDDate.Types.Unknown);

            // TODO punting past keywords

            ctx.gd = new GEDDate();
            if (!parseDate(ref ctx, calen))
                return ctx.gd;

            // check for an era

            return ctx.gd;
        }

        private static bool parseDate(ref Context ctx, Cal calen)
        {
            // parse a date.
            // TODO dd/mm/yyyy
            // TODO dd-mm-yyyy
            // day mon year : num word num
            // mon year : word num
            // year : num 
            int day = -1;
            int mon = -1;
            int year = -1;

            Token tok = ctx.LookAhead();
            if (tok.type == TokType.WORD)
            {
                // mon year?
                if (!getMonth(ctx.getString(), calen, ref mon))
                    return false; // Not a known month
                tok = ctx.Consume();
                if (tok.type != TokType.NUM)
                    return false; // no year following
                year = ctx.getInt();
                ctx.Consume();
                ctx.gd.Month = mon;
                ctx.gd.Year = year;
                ctx.gd.Type = GEDDate.Types.Range;
                return true;
            }

            if (tok.type != TokType.NUM)
            {
                return false; // Dunno what we got
            }

            // day mon year OR year
            if (ctx.LookAhead(1).type == TokType.WORD)
            {
                // day mon year
                day = ctx.getInt();
                ctx.Consume();
                if (!getMonth(ctx.getString(), calen, ref mon))
                    return false; // Not a known month
                tok = ctx.Consume();
                if (tok.type != TokType.NUM)
                    return false; // Not a year, invalid
                year = ctx.getInt();
                ctx.Consume();
                ctx.gd.Type = GEDDate.Types.Exact;
            }
            else
            {
                year = ctx.getInt();
                ctx.Consume();
                ctx.gd.Type = GEDDate.Types.Range;
            }
            ctx.gd.Day = day; // TODO may be invalid
            ctx.gd.Month = mon;
            ctx.gd.Year = year; // TODO may be invalid
            return true;
        }

        private static bool getMonth(string str, Cal calen, ref int mon)
        {
            // TODO swap month lookup based on calendar
            // TODO may find month match in list for different calendar - change calendar?
            // TODO non-standard match needs flagging mechanism

            string str2 = str.Substring(0, 3);
            int monNum;
            if (gregMonLookup.TryGetValue(str2, out monNum))
            {
                mon = monNum;
                return true;
            }
            if (gregMonNonStdLookup.TryGetValue(str, out monNum))
            {
                mon = monNum;
                return true;
            }

            return false;
       }
    }
}
