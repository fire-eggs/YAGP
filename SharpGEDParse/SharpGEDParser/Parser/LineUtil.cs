using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGEDParser.Parser
{
    // TODO one LineUtil instance per parsing thread
    public static class LineUtil
    {
        public class LineData
        {
            public char Level;
            public string Tag;
            public string Ident;
            public string Remain;
        }

        public static LineData _data = new LineData();         // TODO this forces single-threaded!

        private static int FirstChar(string line, int dex, int max)
        {
            while (dex < max && (line[dex] == ' ' || line[dex] == '\t'))
                dex++;
            if (dex >= max) // empty line
                return -1;
            return dex;
        }

        private static int CharsUntil(string line, int max, int dex, char target)
        {
            while (dex < max && line[dex] != target && line[dex] != ' ') // TODO allow tabs?
                dex++;
            if (dex >= max) // target not found
                return max; // TODO is this correct???
            return dex;
        }

        // startDex points at the first space after the level
        public static int IdentAndTag(string line, int startDex)
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
                _data.Ident = line.Substring(startDex + 1, endIdent - startDex - 1);
                startDex = FirstChar(line, endIdent + 1, max);

                int endTag = CharsUntil(line, max, startDex, ' ');
                _data.Tag = line.Substring(startDex, endTag - startDex);
                return endTag;
            }
            else
            {
                // startdex points at 'H' ("0 HEAD")
                int endTag = CharsUntil(line, max, startDex + 1, ' ');
                _data.Tag = line.Substring(startDex, endTag - startDex);
                return endTag;
            }
        }

        public static LineData LevelTagAndRemain(string line)
        {
            int max = line.Length;

            // Move past level
            int dex = FirstChar(line, 0, max);
            _data.Level = line[dex];
            dex = CharsUntil(line, max, dex, ' ');
            dex = IdentAndTag(line, dex);
            if (dex < max)
                _data.Remain = line.Substring(dex + 1);
            else
                _data.Remain = "";
            return _data;
        }
    }
}
