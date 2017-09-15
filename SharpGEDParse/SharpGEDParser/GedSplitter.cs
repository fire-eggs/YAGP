using SharpGEDParser.Parser;

namespace SharpGEDParser
{
    // This is a modified version of StringSplitter (see
    // http://dejan-pelzel.com/post/109194509740/c-high-performance-net-string-split )
    // The mods here are to track indices/lengths rather than
    // actual substrings, and to fix extra separators.
    //
    // I found the original StringSplitter to be about 2x faster than string.Split().
    // This version is about 2x faster than StringSplitter.
    //
    // Useful only because much of the time, GEDCOM parsing wants the actual substrings
    // only for the first (level), second (tag or id), and sometimes third (tag)
    // portions of the line. E.g. when parsing DATE or CONC tags, the remainder
    // of the text after the tag is taken as a whole. Even though the code calculates
    // indices/lengths for the "unneeded" substrings, this is still faster than
    // actually pulling out the substrings.
    // 
    // The original StringSplitter did not behave as desired with extra separating
    // characters, e.g. "0   @I1@   INDI" resulted in "0", " ", " ", "@I1", etc.
    //
    public class GEDSplitter
    {
        private readonly int[] _starts;
        private readonly int[] _lens;
        private int _count;
        private readonly string[] _buf;

        // private const int MAX_PARTS = 49; // TODO verify this works as desired
        // A line is <level><ident><tag><remain> - so really only need 10?
        private const int MAX_PARTS = 9;

        public GEDSplitter() : this(MAX_PARTS+1) 
        {
        }

        public GEDSplitter(int bufferSize)
        {
            _starts = new int[bufferSize];
            _lens = new int[bufferSize];
            _buf = new string[5];
        }

        public int[] Starts { get { return _starts; } }
        public int[] Lengths {  get { return _lens; } }

        public string Get(string value, int dex)
        {
            return value.Substring(_starts[dex], _lens[dex]);
        }

        public string GetRest(string value, int dex)
        {
            if (_count <= dex)
                return null;
            return value.Substring(_starts[dex]); //, value.Length - _starts[dex]);
        }

        public string[] ZeroOneMany(string value)
        {
            switch (_count)
            {
                case 1:
                    _buf[0] = value.Substring(_starts[0], _lens[0]);
                    _buf[1] = _buf[2] = "";
                    break;
                case 2:
                    _buf[0] = value.Substring(_starts[0], _lens[0]);
                    _buf[1] = value.Substring(_starts[1], _lens[1]);
                    _buf[2] = "";
                    break;
                case 3:
                default:
                    _buf[0] = value.Substring(_starts[0], _lens[0]);
                    _buf[1] = value.Substring(_starts[1], _lens[1]);
                    _buf[2] = value.Substring(_starts[2], value.Length - _starts[2]);
                    break;
            }
            return _buf;
        }

        public int Split(string value, char separator)
        {
            return Split(value.ToCharArray(), separator);
        }

        public int Split(char [] value, char separator)
        {
            int resultIndex = 0;
            int startIndex = 0;
            _count = 0;
            int max = value.Length;

            // Find the mid-parts
            for (int i = 0; i < max && resultIndex < MAX_PARTS; i++)
            {
                if (value[i] == separator)
                {
                    if (i > 0 && value[i - 1] != separator)
                    {
                        _starts[resultIndex] = startIndex;
                        _lens[resultIndex] = i - startIndex;
                        resultIndex++;
                    }
                    startIndex = i + 1;
                }
            }

            // Find the last part
            _starts[resultIndex] = startIndex;
            _lens[resultIndex] = max - startIndex;
            resultIndex++;

            _count = resultIndex;
            return resultIndex;
        }

        public char Level(char [] value)
        {
            return value[_starts[0]];
        }

        public string Ident(char [] value)
        {
            // substring 1 starts with '@' == ident
            if (_count < 2 || value[_starts[1]] != '@')
                return null;
            return new string(value, _starts[1]+1, _lens[1]-2); // trimming lead+trail '@'... assumes both exist
        }

        //public string Tag(string value)
        //{
        //    // substring 1 doesn't start with '@' == tag
        //    // else substring 2
        //    if (_count < 2 || _lens[1] < 1)
        //        return null;
        //    if (value[_starts[1]] != '@')
        //        return value.Substring(_starts[1], _lens[1]);
        //    if (_count < 3)
        //        return null;
        //    return value.Substring(_starts[2], _lens[2]);
        //}

        public string Tag(char [] value)
        {
            // substring 1 doesn't start with '@' == tag
            // else substring 2
            if (_count < 2 || _lens[1] < 1)
                return null;
            if (value[_starts[1]] != '@')
                return new string(value, _starts[1], _lens[1]);
            if (_count < 3)
                return null;
            return new string(value, _starts[2], _lens[2]);
        }

        //public string Remain(string value)
        //{
        //    if (_count < 2)
        //        return null;
        //    if (_lens[1] > 0 && value[_starts[1]] != '@')
        //        return GetRest(value, 2);
        //    return GetRest(value, 3);
        //}

        public string GetRest(char [] value, int dex)
        {
            if (_count <= dex)
                return null;

            int max = value.Length;
            while (value[max - 1] == '\0')
                max--;

            return new string(value, _starts[dex], max-_starts[dex]);
        }

        public string Remain(char [] value)
        {
            if (_count < 2)
                return null;
            if (_lens[1] > 0 && value[_starts[1]] != '@')
                return GetRest(value, 2);
            return GetRest(value, 3);
        }

        //public void LevelTagAndRemain(string line, ParseContext2 ctx)
        //{
        //    Split(line, ' ');
        //    ctx.Level = line[_starts[0]];
        //    ctx.Tag = Tag(line);
        //    ctx.Remain = Remain(line) ?? "";
        //}

        //public void LevelTagAndRemain(string line, LineUtil.LineData ctx)
        //{
        //    Split(line, ' ');
        //    ctx.Level = line[_starts[0]];
        //    ctx.Tag = Tag(line);
        //    ctx.Remain = Remain(line) ?? "";
        //}

        public void LevelTagAndRemain(char [] line, LineUtil.LineData ctx)
        {
            Split(line, ' ');
            ctx.Level = line[_starts[0]];
            ctx.Tag = Tag(line);
            ctx.Remain = Remain(line) ?? "";
        }

        public void LevelTagAndRemain(char [] line, ParseContext2 ctx)
        {
            Split(line, ' ');
            ctx.Level = line[_starts[0]];
            ctx.Tag = Tag(line);
            ctx.Remain = Remain(line) ?? "";
        }

    }
}
