using GEDWrap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

// TODO currently Person specific - needs to be generic

// TODO taking several seconds to sort 300,000 people on different columns

// ReSharper disable RedundantCommaInArrayInitializer

namespace IndiTable
{
    public class Column
    {
        public string Header { get; set; }
        public string Name { get; set; }
        public string PropertyName { get; set; }

        public string AttribName { get; set; }

        public string MethodName { get; set; }

        public string ToolTip { get; set; }

        public IComparer<Person> Comparer { get; set; }

        // TODO formatter (e.g. name style)

        // TODO filter

        public string GetValue(Person p)
        {
            if (AttribName != null)
            {
                MethodInfo meth = typeof (Person).GetMethod(MethodName);
                var val = meth.Invoke(p, new object[] {AttribName});
                return val as string; // TODO apply formatting
            }
            PropertyInfo prop = typeof(Person).GetProperty(PropertyName);
            return prop.GetValue(p) as string; // TODO apply formatting
        }
    }

    public class Columns
    {
        public List<Column> AvailableColumns { get; private set; }

        public List<Column> ActiveColumns { get; private set; }

        private readonly Dictionary<string, Column> _lookup;

        public Columns()
        {
            ActiveColumns = new List<Column>();
            AvailableColumns = new List<Column>();
            _lookup = new Dictionary<string, Column>();
        }

        public void Add(Column col, string def)
        {
            AvailableColumns.Add(col);
            if (def == "Y")
                ActiveColumns.Add(col);
            _lookup.Add(col.Name, col);
        }

        public Column GetByName(string name)
        {
            return _lookup[name];
        }

        public void Generate(DataGridView dgv)
        {
            foreach (var column in ActiveColumns)
            {
                var dgvCol = new DataGridViewTextBoxColumn();
                dgvCol.HeaderText = column.Header;
                dgvCol.Name = column.Name;
                dgvCol.ToolTipText = column.ToolTip;
                dgv.Columns.Add(dgvCol);
            }
        }

        // TODO save and restore
    }

    public class GedDateComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            throw new NotImplementedException();
        }
    }

    public class PropStringComparer : IComparer<Person>
    {
        // A string comparer fetching value from property via reflection

        private readonly PropertyInfo _prop;
        private Func<Person, string> _propDelegate;

        public PropStringComparer(string propName)
        {
            _prop = typeof (Person).GetProperty(propName);
            _propDelegate = (Func<Person, string>) Delegate.CreateDelegate
                (typeof (Func<Person, string>), _prop.GetGetMethod());
        }
        public int Compare(Person x, Person y)
        {
            string s1, s2;
            s1 = _propDelegate(x);
            s2 = _propDelegate(y);

            // TODO culture
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class PropStringComparer<T> : IComparer<T>
    {
        // A string comparer fetching value from property via reflection

        private readonly PropertyInfo _prop;
        private Func<T, string> _propDelegate;

        public PropStringComparer(string propName)
        {
            _prop = typeof(T).GetProperty(propName);
            _propDelegate = (Func<T, string>)Delegate.CreateDelegate
                (typeof(Func<T, string>), _prop.GetGetMethod());
        }
        public int Compare(T x, T y)
        {
            string s1, s2;
            s1 = _propDelegate(x);
            s2 = _propDelegate(y);

            // TODO culture
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }

    public class StringComparer : IComparer<Person>
    {
        private readonly PropertyInfo _prop;
        private readonly MethodInfo _meth;
        private readonly object[] _methArgs;

        private Func<Person, string> _propDelegate;

        private delegate string MethDelegate(Person p, string val);

        private MethDelegate _methDelegate;

        public StringComparer(string propName)
        {
            _prop = typeof (Person).GetProperty(propName);
            _propDelegate = (Func<Person, string>) Delegate.CreateDelegate
                (typeof (Func<Person, string>), _prop.GetGetMethod());
        }

        public StringComparer(string attrib, string meth)
        {
            _meth = typeof(Person).GetMethod(meth);
            _methDelegate = (MethDelegate) Delegate.CreateDelegate(typeof (MethDelegate), _meth);
            _methArgs = new object[] {attrib};
            _attribName = attrib;
        }

        private string _attribName;

        public int Compare(Person x, Person y)
        {
            string s1, s2;
            if (_prop == null)
            {
                s1 = _methDelegate(x, _attribName);
                s2 = _methDelegate(y, _attribName);
            }
            else
            {
                s1 = _propDelegate(x);
                s2 = _propDelegate(y);
            }

            // TODO culture
            return String.Compare(s1, s2, StringComparison.OrdinalIgnoreCase);
        }
    }

    // http://www.davekoelle.com/files/AlphanumComparator.cs
    public class CompareNumberOrder : IComparer<Person>
    {
        private readonly PropertyInfo _prop;
        public CompareNumberOrder(string propName)
        {
            _prop = typeof (Person).GetProperty(propName);
        }

        private enum ChunkType { Alphanumeric, Numeric };
        private bool InChunk(char ch, char otherCh)
        {
            ChunkType type = ChunkType.Alphanumeric;

            if (char.IsDigit(otherCh))
            {
                type = ChunkType.Numeric;
            }

            if ((type == ChunkType.Alphanumeric && char.IsDigit(ch))
                || (type == ChunkType.Numeric && !char.IsDigit(ch)))
            {
                return false;
            }

            return true;
        }

        public int Compare(Person x, Person y)
        {
            string s1 = _prop.GetValue(x) as string;
            string s2 = _prop.GetValue(y) as string;
            if (s1 == null || s2 == null)
            {
                return 0;
            }

            int thisMarker = 0;
            int thatMarker = 0;

            while ((thisMarker < s1.Length) || (thatMarker < s2.Length))
            {
                if (thisMarker >= s1.Length)
                {
                    return -1;
                }
                if (thatMarker >= s2.Length)
                {
                    return 1;
                }
                char thisCh = s1[thisMarker];
                char thatCh = s2[thatMarker];

                StringBuilder thisChunk = new StringBuilder();
                StringBuilder thatChunk = new StringBuilder();

                while ((thisMarker < s1.Length) && (thisChunk.Length == 0 || InChunk(thisCh, thisChunk[0])))
                {
                    thisChunk.Append(thisCh);
                    thisMarker++;

                    if (thisMarker < s1.Length)
                    {
                        thisCh = s1[thisMarker];
                    }
                }

                while ((thatMarker < s2.Length) && (thatChunk.Length == 0 || InChunk(thatCh, thatChunk[0])))
                {
                    thatChunk.Append(thatCh);
                    thatMarker++;

                    if (thatMarker < s2.Length)
                    {
                        thatCh = s2[thatMarker];
                    }
                }

                int result = 0;
                // If both chunks contain numeric characters, sort them numerically
                if (char.IsDigit(thisChunk[0]) && char.IsDigit(thatChunk[0]))
                {
                    int thisNumericChunk = Convert.ToInt32(thisChunk.ToString());
                    int thatNumericChunk = Convert.ToInt32(thatChunk.ToString());

                    if (thisNumericChunk < thatNumericChunk)
                    {
                        result = -1;
                    }

                    if (thisNumericChunk > thatNumericChunk)
                    {
                        result = 1;
                    }
                }
                else
                {
                    // TODO culture
                    result = thisChunk.ToString().CompareTo(thatChunk.ToString());
                }

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }

    public static class ColumnFactory
    {
        private static Columns _indiCol;

        // TODO consider a JSON resource??
        private static readonly string[][] indiFull =
        {
            // Header, Name, PropertyName, Default(Y/N),Attrib,Method
            new [] {"Id","Id","Id","Y",null,null,"numeric"},
            new [] {"Full Name", "Name","Name","Y",null,null,"String"},
            new [] {"Sex", "Sex", "Sex", "Y",null,null,"string"},
            new [] {"Birth Date", "bdate", null, "Y", "BIRT", "GetDate", "Date"},
            new [] {"Birth Place", "bplace", null, "Y", "BIRT", "GetPlace", "String"},
            new [] {"Death Date", "ddate", null, "Y", "DEAT", "GetDate", "Date"},
            new [] {"Death Place", "dplace", null, "Y", "DEAT", "GetPlace", "String"},
        };

        private static void initIndi()
        {
            _indiCol = new Columns();
            foreach (var s in indiFull)
            {
                Column iCol = new Column();
                iCol.Header = s[0];
                iCol.Name = s[1];
                iCol.PropertyName = s[2];
                iCol.AttribName = s[4];
                iCol.MethodName = s[5];
                switch (s[6].ToLower())
                {
                    case "string":
                        if (s[2] == null)
                            iCol.Comparer = new StringComparer(s[4],s[5]);
                        else
                            iCol.Comparer = new PropStringComparer(s[2]);
                        break;
                    case "numeric":
                        iCol.Comparer = new CompareNumberOrder(iCol.PropertyName);
                        break;
                    //case "date":
                    //    iCol.Comparer = new GedDateComparer();
                    //    break;
                }

                _indiCol.Add(iCol, s[3]);
            }
        }

        public static Columns IndiColumns()
        {
            if (_indiCol == null)
                initIndi();
            return _indiCol;
        }
    }
}
