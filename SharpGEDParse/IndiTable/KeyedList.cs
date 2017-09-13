using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IndiTable
{
  /// <summary>
  /// Holds a List(T) of KeyValuePair objects.
  /// Overrides Sort() to Sort with a Compare function that sorts on either TKey of Int32, Int64, or String type
  /// Implements IXmlSerializable for customized serialization
  /// </summary>
  /// <typeparam name="TKey"></typeparam>
  /// <typeparam name="TValue"></typeparam>
  [XmlRoot("KeyedList")]
  public class KeyedList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>, IXmlSerializable
  {
    /// <summary>
    /// Default Constructor with initial capacity of 100
    /// </summary>
    public KeyedList() : base() { }

    /// <summary>
    /// Constructor to receive initial size as parameter
    /// </summary>
    /// <param name="size"></param>
    public KeyedList(int size) : base(size) { }

    /// <summary>
    /// Set to false if Descending sort order is desired when using Sort() method.
    /// </summary>
    public bool SortAscd = true;

    /// <summary>
    /// Set to false to enforce unique collection of keys
    /// </summary>
    public bool AllowDuplicateKeys
    {
      get { return _AllowDuplicateKeys; }

      set
      {
        if (value || this.Count == 0)
        {
          _AllowDuplicateKeys = value;
          return;
        }
        if (!value && !_AllowDuplicateKeys) { return; } //nothing changes

        //need to check to see if there are already duplicate keys or property can't be changed to false
        int len = this.Count;
        List<TKey> Lk = new List<TKey>(len);
        for (int i = 0; i < len; ++i)
        {
          Lk.Add(this[i].Key);
        }
        bool HasDuplicates = false;
        Lk.Sort();
        TKey lastKey = this[0].Key;
        for (int i = 1; i < len; ++i)
        {
          if (lastKey.Equals(this[i].Key))
          {
            HasDuplicates = true;
            break;
          }
          lastKey = this[i].Key;
        }
        if (HasDuplicates)
        {
          throw new Exception("Can't set property of AllowDuplicateKeys to false because KeyedList already contains duplicates.");
        }
      }
    }

    /// <summary>
    /// Get the IsSorted property to see if KeyedList has been sorted since last add
    /// </summary>
    public bool IsSorted
    {
      get { return _IsSorted; }
    }

    private bool _IsSorted = false;
    private bool _AllowDuplicateKeys = true;

    private int iKeyIndex(TKey key)
    {
      if (!_IsSorted)
      { //scan list to find if key is in list
        int len = this.Count;
        for (int i = 0; i < len; ++i)
        {
          if (key.Equals(this[i].Key)) { return i; }
        }
        return -1; //a match was not found
      }
      return this.BinarySearch(key, 0, this.Count - 1);
    }

    /// <summary>
    /// Add a KeyValuePair object to the end of the list
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
      if (_AllowDuplicateKeys)
      {
        this.Add(new KeyValuePair<TKey, TValue>(key, value));
        _IsSorted = false;
      }
      else
      {
        if (iKeyIndex(key) > -1)
        {
          throw new Exception("Duplicate key is not allowed.");
        }
        else
        {
          this.Add(new KeyValuePair<TKey, TValue>(key, value));
          _IsSorted = false;
        }
      }
    }

    /// <summary>
    /// Try to add key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns>true if key was added, false if could not add key</returns>
    public bool TryAdd(TKey key, TValue value)
    {
      try
      {
        if (_AllowDuplicateKeys)
        {
          this.Add(new KeyValuePair<TKey, TValue>(key, value));
          _IsSorted = false;
          return true;
        }
        else
        {
          if (iKeyIndex(key) > -1)
          {
            return false;
          }
          else
          {
            this.Add(new KeyValuePair<TKey, TValue>(key, value));
            _IsSorted = false;
            return true;
          }
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Adds new key with value at a position that is one past the matched key if one exists.
    /// List must be sorted and will be sorted if not already sorted.
    /// Throws exception if adding a key violates "AllowDuplicateKeys == false" propery
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void AddSorted(TKey key, TValue value)
    {
      if (!_IsSorted) //sort list if not already sorted
      {
        this.Sort();
      }
      int iPos = iKeyIndex(key);
      if (!_AllowDuplicateKeys)
      {
        if (iPos > -1)
        {
          throw new Exception("Duplicate key is not allowed.");
        }
        else
        {
          this.Add(new KeyValuePair<TKey, TValue>(key, value));
          this.Sort();
        }
      }
      else if (iPos == -1 || (iPos + 1) == this.Count)
      {
        this.Add(new KeyValuePair<TKey, TValue>(key, value));
        this.Sort();
      }
      else
      {
        if (SortAscd) //insert 1 position past found key in list
          ++iPos;
        //else if descending insert before the current item
        this.Insert(iPos, new KeyValuePair<TKey, TValue>(key, value));
      }
    }

    /// <summary>
    /// Use for default sorting on int or string keys
    /// </summary>
    public new void Sort()
    {
      if (typeof(TKey) == typeof(String))
      {
        KeyValuePair<TKey, TValue>[] kvpArray = this.ToArray();
        this.Clear();
        Sort(kvpArray); //sort String key type
        this.AddRange(kvpArray);
        if (!this.SortAscd)
        {
          this.Reverse();
        }
      }
      else
      {
        this.Sort(CompareKey);
      }
      //this.Sort(CompareKey);
      _IsSorted = true;
    }

    /// <summary>
    /// Recursive method to find position of TKey object within list of KeyValuePairs
    /// </summary>
    /// <param name="key"></param>
    /// <param name="iMin"></param>
    /// <param name="iMax"></param>
    /// <returns>index if found, else -1</returns>
    public int BinarySearch(TKey key, int iMin, int iMax)
    {
      if (iMax < iMin) { return -1; } //no items found

      int iMid = (iMin + iMax) / 2;
      int iCompareResult = CompareKey(this[iMid].Key, key);
      if (iCompareResult > 0)
      { //key is in lower subset
        return BinarySearch(key, iMin, iMid - 1);
      }
      else if (iCompareResult < 0)
      { //key is in upper subset
        return BinarySearch(key, iMid + 1, iMax);
      }
      return iMid; //match found at iMid index
    }

    /// <summary>
    /// Gets an array of all possible TValue objects that are paired with matching key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue[] GetValuesForKey(TKey key)
    {
      if (!_AllowDuplicateKeys)
      {
        int i = iKeyIndex(key);
        if (i > -1)
        {
          return new TValue[1] { this[i].Value };
        }
        return new TValue[0]; //return empty array
      }
      else
      {
        List<TValue> Lv = new List<TValue>();
        int len = this.Count;
        for (int i = 0; i < len; ++i)
        {
          if (key.Equals(this[i].Key))
          {
            Lv.Add(this[i].Value);
          }
        }
        return Lv.ToArray();
      }
    }

    /// <summary>
    /// Key based indexer that implements a getter and a setter
    /// for accessing or modifying values based on key.
    /// set modifies value(s) when match or adds new kvp when no match is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>TValue result</returns>
    public TValue this[TKey key]
    {
      get
      {
        if (!_AllowDuplicateKeys)
        {
          TValue[] tva = GetValuesForKey(key);
          if (tva.Length == 1) { return tva[0]; }
          return default(TValue); //return a legal "uninitialized" value for TValue type
        }
        throw new Exception("Cannot use get indexer for lists that allow Duplicate Keys");
      }
      set
      {
        if (!_AllowDuplicateKeys)
        {
          int i = iKeyIndex(key);
          if (i > -1)
          {
            this[i] = new KeyValuePair<TKey, TValue>(key, value);
          }
          else
          { //add new key-value pair to list
            this.Add(key, value);
            _IsSorted = false;
          }
        }
        else
        { //change all values matching key
          int len = this.Count;
          int i = 0;
          for (; i < len; ++i)
          {
            if (key.Equals(this[i].Key))
            {
              this[i] = new KeyValuePair<TKey, TValue>(key, value);
            }
          }
          if (i == len)
          { //add new key-value pair to list
            this.Add(key, value);
            _IsSorted = false;
          }
        }
      }
    }

    #region Compare methods

    /// <summary>
    /// used internally by BinarySearch to compare keys
    /// </summary>
    /// <param name="key1"></param>
    /// <param name="key2"></param>
    /// <returns></returns>
    private int CompareKey(TKey key1, TKey key2)
    {
      object o1 = key1;
      object o2 = key2;
      int iCompareResult = 0;
      if (key1 is String)
      {
        iCompareResult = String.Compare(o1 as string, o2 as string, false);
      }
      else if (key1 is Int32)
      {
        if ((Int32)o1 == (Int32)o2)
          iCompareResult = 0;
        else if ((Int32)o1 > (Int32)o2)
          iCompareResult = 1;
        else
          iCompareResult = -1;
      }
      else if (key1 is Int64)
      {
        if ((Int64)o1 == (Int64)o2)
          iCompareResult = 0;
        else if ((Int64)o1 > (Int64)o2)
          iCompareResult = 1;
        else
          iCompareResult = -1;
      }
      else
      {
        throw new Exception("Binary search compare key type is not supported");
      }
      if (!this.SortAscd) { iCompareResult = -iCompareResult; }
      return iCompareResult;
    }

    /// <summary>
    /// Compare two KeyValuePair objects by TKey.
    /// Supports Int32, Int64 or String types of TKey
    /// </summary>
    /// <param name="kv1"></param>
    /// <param name="kv2"></param>
    /// <returns></returns>
    public int CompareKey(KeyValuePair<TKey, TValue> kv1, KeyValuePair<TKey, TValue> kv2)
    {
      object o1 = kv1.Key;
      object o2 = kv2.Key;
      int iCompareResult = 0;
      if (kv1.Key is String)
      {
        iCompareResult = String.Compare(o1 as string, o2 as string, false);
      }
      else if (kv1.Key is Int32)
      {
        if ((Int32)o1 == (Int32)o2)
          iCompareResult = 0;
        else if ((Int32)o1 > (Int32)o2)
          iCompareResult = 1;
        else
          iCompareResult = -1;
      }
      else if (kv1.Key is Int64)
      {
        if ((Int64)o1 == (Int64)o2)
          iCompareResult = 0;
        else if ((Int64)o1 > (Int64)o2)
          iCompareResult = 1;
        else
          iCompareResult = -1;
      }
      else
      {
        throw new Exception("Sort key type is not supported");
      }
      if (!this.SortAscd) { iCompareResult = -iCompareResult; }
      return iCompareResult;
    }

    #endregion

    #region IXmlSerializable Members

    /// <summary>
    /// Implementation required by IXmlSerizable
    /// </summary>
    /// <returns></returns>
    public System.Xml.Schema.XmlSchema GetSchema()
    {
      return null;
    }

    /// <summary>
    /// Use to deserialize from xml
    /// </summary>
    /// <param name="reader"></param>
    public void ReadXml(System.Xml.XmlReader reader)
    {
      XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
      XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
      bool wasEmpty = reader.IsEmptyElement;

      reader.Read();
      if (wasEmpty)
        return;

      while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
      {
        reader.ReadStartElement("item");
        reader.ReadStartElement("key");
        TKey key = (TKey)keySerializer.Deserialize(reader);
        reader.ReadEndElement();
        reader.ReadStartElement("value");
        TValue value = (TValue)valueSerializer.Deserialize(reader);
        reader.ReadEndElement();
        this.Add(key, value);
        reader.ReadEndElement();
        reader.MoveToContent();
      }
      reader.ReadEndElement();
    }

    /// <summary>
    /// Use to serialize to xml
    /// </summary>
    /// <param name="writer"></param>
    public void WriteXml(System.Xml.XmlWriter writer)
    {
      XmlSerializerNamespaces xmlnsOverride = new XmlSerializerNamespaces();
      xmlnsOverride.Add("", ""); //save time by eliminating  default class namespace
      XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
      XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
      foreach (KeyValuePair<TKey, TValue> kvp in this)
      {
        writer.WriteStartElement("item");
        writer.WriteStartElement("key");
        keySerializer.Serialize(writer, kvp.Key);
        writer.WriteEndElement();
        writer.WriteStartElement("value");
        valueSerializer.Serialize(writer, kvp.Value, xmlnsOverride);
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    #endregion

    #region StringSort methods used to sort KeyedList where key is a String type

    public static void Sort(KeyValuePair<TKey, TValue>[] sList)
    {
      InPlaceSort(sList, 0, 0, sList.Length);
    }

    public static void InPlaceSort(KeyValuePair<TKey, TValue>[] input, int depth, int st, int ed)
    {
      int len = ed - st;
      if (len > 1)
      {
        if (len < 10)
        {
          //this in general depends on the length of the strings to be sorted
          //if the strings are long make the bound less (e.g. 10)
          insertionSort(input, st, len, depth);
        }
        else
        {
          int st1 = st;
          int ed1 = ed - 1;
          int eqStart;
          int eqEnd;

          int pl = st;
          //int pm = st + (len / 2);
          int pm = st + (len >> 1);
          int pn = ed1;
          if (len > 30)
          { // On big arrays, pseudomedian of 9
            //int d = (len / 8);
            int d = (len >> 3);
            pl = med3func(input, pl, pl + d, pl + 2 * d, depth);
            pm = med3func(input, pm - d, pm, pm + d, depth);
            pn = med3func(input, pn - 2 * d, pn - d, pn, depth);
          }
          pm = med3func(input, pl, pm, pn, depth);
          string pivot = input[pm].Key.ToString();

          if (depth < pivot.Length)
          {
            char pivotChar = pivot[depth];
            while (st1 <= ed1 && 0 == (cmpByPivotChar(depth, input[st1].Key.ToString(), pivotChar)))
            {
              st1 = st1 + 1;
            }

            eqStart = st1 - 1;

            while (st1 <= ed1 && 0 == (cmpByPivotChar(depth, input[ed1].Key.ToString(), pivotChar)))
            {
              ed1 = ed1 - 1;
            }

            eqEnd = ed1 + 1;

            int cmpFlag = 0;
            bool flag = (st1 <= ed1);

            while (flag)
            {
              while (flag)
              {
                cmpFlag = cmpByPivotChar(depth, input[st1].Key.ToString(), pivotChar);
                if (cmpFlag < 0)
                {
                  st1 = st1 + 1;
                  flag = (st1 <= ed1);
                }
                else if (cmpFlag == 0)
                {
                  eqStart = eqStart + 1;
                  swap(input, st1, eqStart);
                  st1 = st1 + 1;
                }
                else
                  flag = false;
              }


              flag = (st1 <= ed1);
              while (flag)
              {
                cmpFlag = cmpByPivotChar(depth, input[ed1].Key.ToString(), pivotChar);
                if (cmpFlag > 0)
                {
                  ed1 = ed1 - 1;
                  flag = (st1 <= ed1);
                }
                else if (cmpFlag == 0)
                {
                  eqEnd = eqEnd - 1;
                  swap(input, ed1, eqEnd);
                  ed1 = ed1 - 1;
                }
                else
                  flag = false;
              }
              flag = (st1 <= ed1);
              if (flag)
              {
                swap(input, st1, ed1);
                st1 = st1 + 1;
                ed1 = ed1 - 1;
              }
            }
          }
          else
          {
            while (st1 <= ed1 && 0 == (cmpByPivotLen(depth, input[st1].Key.ToString())))
            {
              st1 = st1 + 1;
            }
            eqStart = st1 - 1;

            while (st1 <= ed1 && 0 == (cmpByPivotLen(depth, input[ed1].Key.ToString())))
            {
              ed1 = ed1 - 1;
            }
            eqEnd = ed1 + 1;

            int cmpFlag = 0;
            bool flag = (st1 <= ed1);

            while (flag)
            {
              while (flag)
              {
                cmpFlag = cmpByPivotLen(depth, input[st1].Key.ToString());
                if (cmpFlag < 0)
                {
                  st1 = st1 + 1;
                  flag = (st1 <= ed1);
                }
                else if (cmpFlag == 0)
                {
                  eqStart = eqStart + 1;
                  swap(input, st1, eqStart);
                  st1 = st1 + 1;
                }
                else
                  flag = false;
              }
              flag = (st1 <= ed1);
              while (flag)
              {
                cmpFlag = cmpByPivotLen(depth, input[ed1].Key.ToString());
                if (cmpFlag > 0)
                {
                  ed1 = ed1 - 1;
                  flag = (st1 <= ed1);
                }
                else if (cmpFlag == 0)
                {
                  eqEnd = eqEnd - 1;
                  swap(input, ed1, eqEnd);
                  ed1 = ed1 - 1;
                }
                else
                  flag = false;
              }
              flag = (st1 <= ed1);
              if (flag)
              {
                swap(input, st1, ed1);
                st1 = st1 + 1;
                ed1 = ed1 - 1;
              }
            }
          }
          int i = st;
          int j = st1 - 1;
          while (i <= eqStart)
          {
            swap(input, i, j);
            i = i + 1;
            j = j - 1;
          }

          i = ed - 1;
          j = st1;
          while (i >= eqEnd)
          {
            swap(input, i, j);
            i = i - 1;
            j = j + 1;
          }

          eqEnd = st1 + (ed - eqEnd);

          if (ed - eqEnd > 1)
          {
            InPlaceSort(input, depth, eqEnd, ed);
          }
          eqStart = st1 - (eqStart + 1 - st);
          //st..eqStart..eqEnd..ed
          if (eqEnd - eqStart > 1)
          {
            if (input[eqStart].Key.ToString().Length > depth)
            {
              InPlaceSort(input, (depth + 1), eqStart, eqEnd);
            }
          }
          if (eqStart - st > 1)
          {
            InPlaceSort(input, depth, st, eqStart);
          }
        }
      }
    }

    static void swap(KeyValuePair<TKey, TValue>[] arr, int i, int j)
    {
      KeyValuePair<TKey, TValue> tmp = arr[i];
      arr[i] = arr[j];
      arr[j] = tmp;
    }

    static int med3func(KeyValuePair<TKey, TValue>[] x, int a, int b, int c, int depth)
    {
      char va, vb, vc;
      if ((va = at(x[a].Key.ToString(), depth)) == (vb = at(x[b].Key.ToString(), depth)))
        return a;

      if ((vc = at(x[c].Key.ToString(), depth)) == va || vc == vb)
        return c;
      return va < vb ?
            (vb < vc ? b : (va < vc ? c : a))
          : (vb > vc ? b : (va < vc ? a : c));
    }

    static char at(string s, int pos)
    {
      if (pos >= s.Length)
        return (char)0;
      return s[pos];
    }

    static int cmpByPivotChar(int depth, string a, char ch2)
    {
      int lenA = a.Length;
      if (lenA == depth)
        return -1;
      else
      {
        char ch1 = a[depth];
        if (ch1 < ch2)
          return (-1);
        else if (ch1 == ch2)
          return 0;
        else
          return 1;
      }
    }

    static int cmpByPivotLen(int depth, string a)
    {
      int lenA = a.Length;
      if (lenA == depth)
        return 0;
      else
        return 1;
    }

    static void insertionSort(KeyValuePair<TKey, TValue>[] x, int a, int len, int depth)
    {
      int pi = a + 1;
      int n = len - 1;
      //propages maximum to last position
      while (n > 0)
      {
        int pj = pi;
        while (pj > a)
        {
          int d = depth;
          int pj1 = pj - 1;
          string s = x[pj1].Key.ToString();
          string t = x[pj].Key.ToString();
          int s_len = s.Length;
          int t_len = t.Length;
          int min_len = s_len;
          if (t_len < s_len) min_len = t_len;
          while (d < min_len && s[d] == t[d])
          {
            d = d + 1;
          }
          if (d == s_len || (d < t_len && s[d] <= t[d]))
          {
            break;
          }
          else
          {
            KeyValuePair<TKey, TValue> tmp = x[pj];
            x[pj] = x[pj1];
            x[pj1] = tmp;
            pj = pj1;
          }
        }
        n = n - 1;
        pi = pi + 1;
      }
    }

    #endregion
  }

}
