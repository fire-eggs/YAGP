using SharpGEDParser.Parser;

// TODO the places in Remain, RemainLS and Tag where checks have to be made against '@' seem inefficient

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
        private readonly int _max;
        private readonly StringCache _tagCache;

        // A line is <level><ident><tag><remain> - so even 10 is overkill?
        private const int MAX_PARTS = 9;

        public GEDSplitter(StringCache tagCache) : this(MAX_PARTS+1)
        {
            _tagCache = tagCache; // TODO use a singleton rather than parameter?
        }

        public GEDSplitter(int bufferSize)
        {
            _max = bufferSize-1;
            _starts = new int[bufferSize];
            _lens = new int[bufferSize];

            if (_tagCache == null)
                _tagCache = new StringCache(); // unit testing
        }

        public int[] Starts { get { return _starts; } }
        public int[] Lengths {  get { return _lens; } }

        public int Split(char [] value, char separator)
        {
            int resultIndex = 0;
            int startIndex = 0;
            _count = 0;
            int max = value.Length;

            // Find the mid-parts
            for (int i = 0; i < max && resultIndex < _max; i++)
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

        private readonly char[] _identTrim = {'@'};
        public string Ident(char [] value)
        {
            // substring 1 starts with '@' == ident
            if (_count < 2 || value[_starts[1]] != '@' || _lens[1] < 3)
                return null;
            return new string(value, _starts[1], _lens[1]).Trim(_identTrim);
            //return new string(value, _starts[1]+1, _lens[1]-2); // trimming lead+trail '@'... assumes both exist
        }

        public string Tag(char [] value)
        {
            // substring 1 doesn't start with '@' == tag
            // else substring 2
            if (_count < 2 || _lens[1] < 1)
                return null;
            if (value[_starts[1]] != '@')
                return _tagCache.GetFromCache(value, _starts[1], _lens[1]);
                //return new string(value, _starts[1], _lens[1]);
            if (_count < 3)
                return null;
            if (_lens[2] > 0 && value[_starts[2]] == '@') // empty tag scenario
                return _tagCache.GetFromCache(value, _starts[3], _lens[3]);
                //return new string(value, _starts[3], _lens[3]);
            return _tagCache.GetFromCache(value, _starts[2], _lens[2]);
            //return new string(value, _starts[2], _lens[2]);
        }

        public char [] GetRest(char [] value, int dex)
        {
            if (_count <= dex)
                return null;

            int max = value.Length;
            while (value[max - 1] == '\0') // trim trailing nulls
                max--;

            int len = max - _starts[dex];
            var tmp = new char[len];
            for (int i = 0; i < len; i++)
                tmp[i] = value[i + _starts[dex]];
            return tmp;
            //return new string(value, _starts[dex], max-_starts[dex]);
        }

        public char [] Remain(char [] value)
        {
            if (_count < 3)
                return null;
            if (_lens[1] > 0 && value[_starts[1]] != '@')
                return GetRest(value, 2);
            if (_lens[2] > 0 && value[_starts[2]] == '@') // empty tag scenario
                return GetRest(value, 4);
            return GetRest(value, 3);
        }

        public void LevelTagAndRemain(char [] line, LineUtil.LineData ctx)
        {
            Split(line, ' ');
            ctx.Level = line[_starts[0]];
            ctx.Tag = Tag(line);
            ctx.Remain1 = Remain(line) ?? new char[0];
        }

        public string RemainLS(char[] value)
        {
            // NOTE records need to keep the (extra) leading spaces from the line remainder

            if (_count < 3)
                return null;
            if (_lens[1] > 0 && value[_starts[1]] != '@')
                return GetRestLS(value, 2);
            if (_lens[2] > 0 && value[_starts[2]] == '@') // empty tag scenario
                return GetRestLS(value, 4);
            return GetRestLS(value, 3);
        }

        public string GetRestLS(char[] value, int dex)
        {
            if (_count <= dex)
                return null;

            int max = value.Length;
            while (value[max - 1] == '\0') // trim trailing nulls
                max--;

            // Instead of starting at 'this' piece, where the leading spaces have
            // already been skipped, calculate the start from the previous piece.
            int start = _starts[dex - 1] + _lens[dex - 1] + 1;
            int len = max - start;
            var tmp = new char[len];
            for (int i = 0; i < len; i++)
                tmp[i] = value[i + start];
            return new string(tmp);
            //return new string(value, _starts[dex], max-_starts[dex]);
        }


    }
}
