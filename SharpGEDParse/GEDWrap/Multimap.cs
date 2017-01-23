using System.Collections.Generic;

namespace GEDWrap
{
    public class MultiMap<T,V>
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