using System;
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
        private readonly int _max;

        // A line is <level><ident><tag><remain> - so even 10 is overkill?
        private const int MAX_PARTS = 9;

        public GEDSplitter() : this(MAX_PARTS+1) 
        {
        }

        public GEDSplitter(int bufferSize)
        {
            _max = bufferSize-1;
            _starts = new int[bufferSize];
            _lens = new int[bufferSize];
        }

        public int[] Starts { get { return _starts; } }
        public int[] Lengths {  get { return _lens; } }

        //public string Get(string value, int dex)
        //{
        //    return value.Substring(_starts[dex], _lens[dex]);
        //}

        //public string GetRest(string value, int dex)
        //{
        //    if (_count <= dex)
        //        return null;
        //    return value.Substring(_starts[dex]);
        //}

        //public int Split(string value, char separator)
        //{
        //    return Split(value.ToCharArray(), separator);
        //}

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

        public char [] Tag(char [] value)
        {
            // substring 1 doesn't start with '@' == tag
            // else substring 2
            if (_count < 2 || _lens[1] < 1)
                return null;
            if (value[_starts[1]] != '@')
                return make(value, _starts[1], _lens[1]);
            if (_count < 3)
                return null;
            if (_lens[2] > 0 && value[_starts[2]] == '@') // empty tag scenario
                return make(value, _starts[3], _lens[3]);
            return make(value, _starts[2], _lens[2]);
        }

        private char[] make(char[] value, int beg, int len)
        {
            var tmp = new char[len];
            for (int i = 0; i < len; i++)
                tmp[i] = value[i + beg];
            return tmp;
        }

        public char [] GetRest(char [] value, int dex)
        {
            if (_count <= dex)
                return null;

            int max = value.Length;
            while (value[max - 1] == '\0')
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

        public void LevelIdentTagRemain(char[] line, out char level, out char[] tag, out string ident, out char[] remain)
        {
            int maxL = line.Length;
            SplitForIdent(line, maxL);
            level = line[_starts[0]];
            int endIdent = _starts[1] + _lens[1] - 1;
            if (endIdent < 0 || endIdent >= maxL || line[endIdent] != '@')
            {
                // didn't see a properly terminated ident; assume it is the tag
                ident = "";
                tag = make(line, _starts[1], _lens[1]);
            }
            else
            {
                ident = new string(line, _starts[1], _lens[1]-1);
                tag = make(line, _starts[2], _lens[2]);
            }
            //try
            //{
            //    if (_lens[2] == 0)
            //        tag = make(line, _starts[1], _lens[1]);
            //    else
            //        tag = make(line, _starts[2], _lens[2]);
            //}
            //catch (Exception)
            //{
            //    tag = make(line, _starts[1], _lens[1]); ; // 0 TRLR under obscure conditions
            //}
            remain = make(line, _starts[3], _lens[3]);
            //remain = make(line, _starts[3], line.Length - _starts[3]);
        }

        public int SplitForIdent(char[] value, int maxL)
        {
            int resultIndex = 0;
            int startIndex = 0;
            _count = 0;

            bool sawAt = false;
            // Find the mid-parts
            for (int i = 0; i < maxL && resultIndex < 3; i++)
            {
                char val = value[i];
                if (sawAt)
                {
                    if (val == '@')
                        sawAt = false;
                    continue;
                }

                if (val == '@')
                {
                    sawAt = true;
                    startIndex = i + 1;
                }
                else if (val == ' ')
                {
                    if (i > 0)
                    {
                        char val2 = value[i - 1];
                        if (val2 != ' ')
                        {
                            _starts[resultIndex] = startIndex;
                            _lens[resultIndex] = i - startIndex; // - (val2 == '@' ? 1 : 0);
                            resultIndex++;
                        }
                    }
                    startIndex = i + 1;
                }
            }

            // Find the last part
            _starts[resultIndex] = startIndex;
            _lens[resultIndex] = maxL - startIndex; // - (value[max-1] == '@' ? 1 : 0);
            resultIndex++;

            _count = resultIndex;
            return resultIndex;
        }

    }
}
