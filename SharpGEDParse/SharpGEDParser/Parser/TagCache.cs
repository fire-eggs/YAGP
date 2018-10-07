using SharpGEDParser.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SharpGEDParser.Parser
{
    public class TagCache
    {
        // A comparator is necessary because just using vanilla char[] will result in
        // a different hash for each *instance*. This comparator works on the contents.
        private class TagComparer : IEqualityComparer<char[]>
        {
            public bool Equals(char[] x, char[] y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null | y == null) // One but not both?
                    return false;
                if (x.Length != y.Length)
                    return false;
                for (int i = 0; i < x.Length; i++)
                    if (x[i] != y[i])
                        return false;
                return true;
            }

            public int GetHashCode(char[] obj)
            {
                if (obj == null || obj.Length == 0)
                    return 0;
                var hashCode = 0;
                for (var i = 0; i < obj.Length; i++)
                {
                    // avoid a major allocation by skipping BitConverter.GetBytes
                    char val = obj[i];
                    byte byte1 = (byte)(val & 0xFF);
                    byte byte2 = (byte)((val >> 8) & 0xFF);
                    hashCode = (hashCode << 3) | (hashCode >> (29)) ^ byte1;
                    hashCode = (hashCode << 3) | (hashCode >> (29)) ^ byte2;
                }
                return hashCode;
            }
        }

        // Concurrent dictionary used for parallelism
        private readonly ConcurrentDictionary<char[], Tag.GedTag> _stringCache;

        private const int BUFF_LEN = 15;

        public TagCache()
        {
            //int numProcs = Environment.ProcessorCount; 
            _stringCache = new ConcurrentDictionary<char[], Tag.GedTag>(new TagComparer());
            InitCache();
            //_buffer = new char[BUFF_LEN];
        }

        public Tag.GedTag GetFromCache(char[] inval)
        {
            Tag.GedTag outval;
            if (!_stringCache.TryGetValue(inval, out outval))
            {
                return Tag.GedTag.INVALID;
            }
            return outval;
        }

        public void Add(Tag.GedTag val, string name)
        {
            _stringCache.TryAdd(name.ToCharArray(), val);
        }

        //private char[] _buffer; // re-use single buffer

        public Tag.GedTag GetFromCache(char[] value, int index, int len)
        {
            if (len == 0)
                return Tag.GedTag.MISSING;
#if ARRAYPOOL
            var temp = _bufferPool.Allocate(len);
#else
            var temp = new char[len];
#endif

            Array.Copy(value, index, temp, 0, len);
            var tag = GetFromCache(temp);

#if ARRAYPOOL
            _bufferPool.Free(temp);
#endif
            return tag;
            //Array.Clear(_buffer, 0, BUFF_LEN);
            //Array.Copy(value, index, _buffer, 0, len);
            //return GetFromCache(_buffer);
        }

        private void InitCache()
        {
            var vals = Enum.GetValues(typeof(Tag.GedTag));
            foreach (Tag.GedTag foo in vals)
                Add(foo, foo.ToString());
        }

#if ARRAYPOOL
        private ConcurrentArrayPool<char> _bufferPool = new ConcurrentArrayPool<char>();
#endif

    }
}
