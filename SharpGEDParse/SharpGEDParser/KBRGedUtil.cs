using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Activation;

namespace SharpGEDParser
{
    static class KBRGedUtil
    {
        public static int FirstChar(string line)
        {
            int max = line.Length;
            int dex = 0;
            while (dex < max && line[dex] == ' ') // TODO allow tab?
                dex++;
            if (dex >= max) // empty line
                return -1;
            return dex;
        }
        public static int FirstChar(string line, int dex, int max)
        {
            while (dex < max && line[dex] == ' ') // TODO allow tab?
                dex++;
            if (dex >= max) // empty line
                return -1;
            return dex;
        }

        public static int AllCharsUntil(string line, int max, int dex, char target)
        {
            while (dex < max && line[dex] != target)
                dex++;
            if (dex >= max) // target not found
                return max; // TODO is this correct???
            return dex;
        }

        public static int CharsUntil(string line, int max, int dex, char target)
        {
            while (dex < max && line[dex] != target && line[dex] != ' ') // TODO allow tabs?
                dex++;
            if (dex >= max) // target not found
                return max; // TODO is this correct???
            return dex;
        }

        public static int Ident(string line, int max, int startDex, ref string ident)
        {
            startDex = FirstChar(line, startDex, max);
            if (line[startDex] == '@')
            {
                // get ident
                int endIdent = CharsUntil(line, max, startDex + 1, '@');
                // endIdent now points at the trailing '@' or ' '
                ident = line.Substring(startDex + 1, endIdent - startDex - 1);
                return endIdent;
            }
            return -1;
        }

        // startDex points at the first space after the level
        public static int IdentAndTag(string line, int startDex, ref string ident, ref string tag)
        {
            // "0 @I1@ INDI"
            int max = line.Length;
            if (startDex >= max || line[startDex] != ' ') // TODO allow tabs?
                return -1;

            // Get to either ident or tag
            startDex = FirstChar(line, startDex, max);
            if (line[startDex] == '@')
            {
                // get ident
                int endIdent = CharsUntil(line, max, startDex + 1, '@');
                // endIdent now points at the trailing '@' or ' '
                ident = line.Substring(startDex + 1, endIdent - startDex - 1);
                startDex = FirstChar(line, endIdent+1, max);

                int endTag = CharsUntil(line, max, startDex, ' ');
                tag = line.Substring(startDex, endTag - startDex);
                return endTag;
            }
            else
            {
                // startdex points at 'H' ("0 HEAD")
                int endTag = CharsUntil(line, max, startDex + 1, ' ');
                tag = line.Substring(startDex, endTag - startDex);
                return endTag;
            }
        }

        public static int TagAndRemain(string line, ref string ident, ref string tag, ref string remain)
        {
            int max = line.Length;

            // Move past level
            int dex = FirstChar(line, 0, max);
            dex = AllCharsUntil(line, max, dex, ' ');
            dex = IdentAndTag(line, dex, ref ident, ref tag);
            remain = line.Substring(dex); // TODO check for nothing remaining
            return dex;
        }

        public static string ParseFor(GedRecord glop, int dex, int max, string target)
        {
            for (; dex <= max; dex++)
            {
                string line = glop.GetLine(dex);
                string tag = "";
                string remain = "";
                string ident = "";
                int res = TagAndRemain(line, ref ident, ref tag, ref remain);

                if (tag == target)
                    return remain.Trim();
            }

            return null;
        }

        public static int LevelTagAndRemain(string line, ref char level, ref string ident, ref string tag, ref string remain)
        {
            int max = line.Length;

            // Move past level
            int dex = FirstChar(line, 0, max);
            level = line[dex];
            dex = AllCharsUntil(line, max, dex, ' ');
            dex = IdentAndTag(line, dex, ref ident, ref tag);
            remain = line.Substring(dex); // TODO check for nothing remaining
            return dex;
        }

        public static Tuple<int,int> ParseForMulti(GedRecord glop, int dex, int max, string target)
        {
            string tag = "";
            string remain = "";
            string ident = "";
            char level = '@';
            for (; dex <= max; dex++)
            {
                string line = glop.GetLine(dex);
                int res = LevelTagAndRemain(line, ref level, ref ident, ref tag, ref remain);

                if (tag == target)
                {
                    int end = ParseForEndOfMulti(glop, level, dex, max);
                    return new Tuple<int, int>(dex, end);
                }
            }

            return null;
        }

        public static int ParseForEndOfMulti(GedRecord glop, char level, int dex, int max)
        {
            int end = dex;
            // We found the first line of a target (e.g. 'NOTE')
            // Return the index of the end of the target
            for (int i = dex+1; i <= max; i++)
            {
                string line = glop.GetLine(i);
                int first = FirstChar(line);
                if (line[first] <= level)
                    break;
                end++;
            }
            return end;
        }

    }
}
