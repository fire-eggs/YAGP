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

        public static int Ident(string line, int startDex, ref string ident)
        {
            int max = line.Length;
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
                return endTag; // TODO off-by-one?
            }
            else
            {
                // startdex points at 'H' ("0 HEAD")
                int endTag = CharsUntil(line, max, startDex + 1, ' ');
                tag = line.Substring(startDex, endTag - startDex);
                return endTag; // TODO off-by-one?
            }
        }
    }
}
