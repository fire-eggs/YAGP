
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

        internal static int FirstChar(char [] line, int dex, int max)
        {
            while (dex < max && (line[dex] == ' ' || line[dex] == '\t'))
                dex++;
            if (dex >= max) // empty line
                return -1;
            return dex;
        }

        internal static int FirstChar(string line, int dex, int max)
        {
            while (dex < max && (line[dex] == ' ' || line[dex] == '\t'))
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
                return max+1;
            return dex;
        }

        public static int ReverseSearch(string line, int max, int limit, char target)
        {
            int dex = max - 1;
            while (dex > limit && line[dex] != target)
                dex--;
            if (dex <= limit) // target not found
                return max;
            return dex;
        }

        //private static int CharsUntil(string line, int max, int dex, char target)
        //{
        //    while (dex < max && line[dex] != target && line[dex] != ' ') // TODO allow tabs?
        //        dex++;
        //    if (dex >= max) // target not found
        //        return max; // TODO is this correct???
        //    return dex;
        //}

        // startDex points at the first space after the level
        //public static int IdentAndTag(LineData data, string line, int startDex)
        //{
        //    // "0 @I1@ INDI"
        //    int max = line.Length;
        //    if (startDex >= max || line[startDex] != ' ') // TODO allow tabs?
        //        return -1;

        //    // Get to either ident or tag
        //    startDex = FirstChar(line, startDex, max);
        //    if (startDex < 0) // TODO raganfam.ged has garbage lines consisting only of a number, no tag; invalid line breaks, etc
        //        return startDex; 

        //    if (line[startDex] == '@')
        //    {
        //        // get ident
        //        int endIdent = AllCharsUntil(line, max, startDex + 1, '@');
        //        // endIdent now points at the trailing '@' or ' '
        //        data.Ident = line.Substring(startDex + 1, endIdent - startDex - 1);
        //        startDex = FirstChar(line, endIdent + 1, max);

        //        int endTag = CharsUntil(line, max, startDex, ' ');
        //        data.Tag = line.Substring(startDex, endTag - startDex);
        //        return endTag;
        //    }
        //    else
        //    {
        //        // startdex points at 'H' ("0 HEAD")
        //        int endTag = CharsUntil(line, max, startDex + 1, ' ');
        //        data.Tag = line.Substring(startDex, endTag - startDex);
        //        return endTag;
        //    }
        //}

        //public static LineData LevelTagAndRemain(LineData data, string line)
        //{
        //    int max = line.Length;

        //    // Move past level
        //    int dex = FirstChar(line, 0, max);
        //    data.Level = line[dex];
        //    data.Tag = ""; // in case of error
        //    data.Remain = ""; // in case of error
        //    dex = AllCharsUntil(line, max, dex, ' ');
        //    dex = IdentAndTag(data, line, dex);
        //    if (dex < max)
        //        data.Remain = line.Substring(dex + 1);
        //    else
        //        data.Remain = "";
        //    return data;
        //}
    }
}
