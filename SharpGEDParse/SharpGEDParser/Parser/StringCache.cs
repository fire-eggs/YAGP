using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpGEDParser.Parser
{
    public class StringCache
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
                for (int i=0; i < x.Length; i++)
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
                    var bytes = BitConverter.GetBytes(obj[i]);

                    // Rotate by 3 bits and XOR the new value.
                    for ( var j= 0; j < bytes.Length; j++)
                        hashCode = (hashCode << 3) | (hashCode >> (29)) ^ bytes[j];
                }
                return hashCode;
            }
        }

        // Concurrent dictionary used for parallelism
        private readonly ConcurrentDictionary<char[], string> _stringCache;

        public StringCache()
        {
            //int numProcs = Environment.ProcessorCount; 
            _stringCache = new ConcurrentDictionary<char[], string>(new TagComparer());
        }

        public string GetFromCache(char[] inval)
        {
            string outval;
            if (!_stringCache.TryGetValue(inval, out outval))
            {
                outval = new string(inval);
                _stringCache.TryAdd(inval, outval);
            }
            return outval;
        }

        public string GetFromCache(char[] value, int index, int len)
        {
            char [] tmp=new char[len];
            Array.Copy(value, index, tmp, 0, len);
            return GetFromCache(tmp);
        }
    }
}
