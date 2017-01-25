using System.Collections.Generic;

// Multi-map collections: one key to multiple values

namespace GEDWrap
{
    // Set variation: one key to a set of unique values
    public class MultiHash<T, V>
    {
        readonly Dictionary<T, HashSet<V>> _dictionary = new Dictionary<T, HashSet<V>>();

        public void Add(T key, V value)
        {
            HashSet<V> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                list.Add(value);
            }
            else
            {
                list = new HashSet<V>();
                list.Add(value);
                _dictionary[key] = list;
            }
        }

        public IEnumerable<T> Keys { get { return _dictionary.Keys; } }

        public HashSet<V> this[T key]
        {
            get
            {
                HashSet<V> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new HashSet<V>();
                    _dictionary[key] = list;
                }
                return list;
            }
        }
    }

    // List variation: one key to a list of values
    public class MultiMap<T, V>
    {
        readonly Dictionary<T, List<V>> _dictionary = new Dictionary<T, List<V>>();

        public void Add(T key, V value)
        {
            List<V> list;
            if (_dictionary.TryGetValue(key, out list))
            {
                // 2A.
                list.Add(value);
            }
            else
            {
                // 2B.
                list = new List<V>();
                list.Add(value);
                _dictionary[key] = list;
            }
        }

        public IEnumerable<T> Keys { get { return _dictionary.Keys; } }

        public List<V> this[T key]
        {
            get
            {
                List<V> list;
                if (!_dictionary.TryGetValue(key, out list))
                {
                    list = new List<V>();
                    _dictionary[key] = list;
                }
                return list;
            }
        }
    }
}