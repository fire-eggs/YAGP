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
        private enum Cal
        {
            Greg, Jul, Hebr, Fre, Rom, Fut
        }

        private enum KeyW
        {
            Bef, Aft, Est, Calc, Int, Range, None
        }

        private enum Era
        {
            BC, AD, None
        }

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

        // The GEDCOM standard keywords - initial appearance
        private static Dictionary<string, KeyW> initialKeyLookup = new Dictionary<string, KeyW>()
        {
            {"BEF", KeyW.Bef},
            {"AFT", KeyW.Aft},
            {"ABT", KeyW.Est},
            {"EST", KeyW.Est},
            {"CAL", KeyW.Calc},
            {"FROM", KeyW.Range},
            {"TO", KeyW.Range},
            {"INT", KeyW.Int}
        };
        // TODO 'NOT' variation: "NOT BEF"==after, "NOT AFT"==before, etc.
        // Non-standard variants on GEDCOM initial keywords
        private static Dictionary<string, KeyW> initialNonStdKeyLookup = new Dictionary<string, KeyW>()
        {
            {"BEF.", KeyW.Bef},
            {"AFT.", KeyW.Aft},
            {"ABT.", KeyW.Est},
            {"EST.", KeyW.Est},
            {"CAL.", KeyW.Calc},
            {"INT.", KeyW.Int},
            {"BEFORE", KeyW.Bef},
            {"AFTER", KeyW.Aft},
            {"ABOUT", KeyW.Est}, // BROSKEEP?
            {"CIRCA", KeyW.Est},
            {"C.", KeyW.Est}, // BROSKEEP ?
            {"AB.", KeyW.Est}, // personal
            {"MAYBE", KeyW.Calc}, // personal
        };

        private static Dictionary<string, Era> eraLookup = new Dictionary<string, Era>
        {
            {"B.C.", Era.BC}
        };

        private static Dictionary<string, Era> nonStdEraLookup = new Dictionary<string, Era>
        {
            {"BC", Era.BC},
            {"BCE", Era.BC},
            {"B.C.E.", Era.BC},
            {"AD", Era.AD},
            {"A.D.", Era.AD},
            {"CE", Era.AD},
            {"C.E.", Era.AD},
        };

        static EventDateParse()
        {
        }

        private static Cal CheckCalendar(Context ctx)
        {
            Token tok = ctx.LookAhead();
            Cal calen = Cal.Greg;
            if (tok.type == TokType.CALEN)
            {
                if (!calLookup.TryGetValue(ctx.getString(), out calen))
                    calen = Cal.Fut;
                ctx.Consume();
            }
            return calen;
        }

        private static KeyW CheckInitialKeyword(Context ctx)
        {
            // TODO non-standard match needs flagging mechanism

            Token tok = ctx.LookAhead();
            if (tok.type != TokType.WORD)
                return KeyW.None;
            KeyW keyw;
            if (initialKeyLookup.TryGetValue(ctx.getString(), out keyw))
            {
                ctx.Consume();
                return keyw;
            }
            if (initialNonStdKeyLookup.TryGetValue(ctx.getString(), out keyw))
            {
                ctx.Consume();
                return keyw;
            }
            return KeyW.None;
        }

        private static Era CheckEra(Context ctx)
        {
            // TODO non-standard match needs flagging mechanism

            Token tok = ctx.LookAhead();
            if (tok.type != TokType.WORD)
                return Era.None;
            Era era;
            if (eraLookup.TryGetValue(ctx.getString(), out era))
            {
                ctx.Consume();
                return era;
            }
            if (nonStdEraLookup.TryGetValue(ctx.getString(), out era))
            {
                ctx.Consume();
                return era;
            }
            return Era.None;
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

            Cal calen = CheckCalendar(ctx);
            if (calen != Cal.Greg) // TODO other calendar support
                return new GEDDate(GEDDate.Types.Unknown);

            // TODO punting past keywords
            KeyW initKeyword = CheckInitialKeyword(ctx);

            ctx.gd = new GEDDate();
            if (!parseDate(ref ctx, calen))
                return ctx.gd;

            // check for an era
            Era era = CheckEra(ctx);
            ctx.gd.IsBC = (era == Era.BC);

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
