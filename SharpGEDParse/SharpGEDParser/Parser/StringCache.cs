using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

// This is an idea from GEDCOM.Net. By using a cache of strings, multiple
// distinct instances of a given string should be replacable by a reference
// to a single string instance. All GEDCOM files will have a large number 
// of duplicated strings: tags, surnames and placenames are typical. Studies
// with DotMemory also show that sex values, names, and sometimes notes are
// frequently duplicated.
//
// It seems my choice of char[] for input line components is a problem, as 
// the built-in Compare functions don't handle them. Thus the cache can
// prove to be much slower than allocating strings. In addition, studies
// with DotMemory suggest the overhead of the cache (due to excess dictionary
// capacity) might be larger than the savings of non-duplicated strings.
//
// As of 20171220, the Dictionary-based cache for tags seems to be worthwhile.
// Said cache is too memory inefficient for other duplicate strings; the
// SortedList-based cache is far too slow.

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

    // DotMemory claims the Dictionary based string cache is not memory efficient,
    // with lots of empty 'buckets'. This is an attempt to use a SortedList, but
    // when used for surnames, places, and sex, causes an extreme slowdown in 
    // performance. My theory is it's the thread locking which is the problem,
    // aggrevated by an inefficient char[] IComparer.
    //
    // Studies with DotMemory suggest the memory savings of the cache are not
    // significant. 
    public class StringCache2
    {
        private ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        private readonly SortedList<char[],string> _stringCache;

        private class MyCompare : IComparer<char []>
        {
            public int Compare(char[] x, char[] y)
            {
                try
                {
                    return StructuralComparisons.StructuralComparer.Compare(x, y);
                }
                catch (Exception)
                {
                    return x.Length.CompareTo(y.Length);
                }
            }
        }

        public StringCache2()
        {
            _stringCache = new SortedList<char[],string>(new MyCompare());
        }

        public string GetFromCache(char[] inval)
        {
            cacheLock.EnterUpgradeableReadLock();
            try
            {
                string result = null;
                if (_stringCache.TryGetValue(inval, out result))
                    return result;
                else
                {
                    cacheLock.EnterWriteLock();
                    try
                    {
                        var outval = new string(inval);
                        _stringCache.Add(inval, outval);
                        return outval;
                    }
                    finally
                    {
                        cacheLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }
        }

        public string GetFromCache(char[] value, int index, int len)
        {
            char[] tmp = new char[len];
            Array.Copy(value, index, tmp, 0, len);
            return GetFromCache(tmp);
        }
        
    }
}
