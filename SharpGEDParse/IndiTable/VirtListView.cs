using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using GEDWrap;

namespace IndiTable
{
    public class MyListView : ListView
    {
        public MyListView()
        {
            DoubleBuffered = true;
            //SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }

    public partial class VirtListView : Form
    {
        public VirtListView()
        {
            InitializeComponent();
        }

	    MyListView _lv;

        private Person[] _data;

        readonly string [] _columnT =  
        {
			"Id",
			"Full Name",
			"Sex",
			"Birth Date",
			"Birth Place",
			"Death Date",
			"Death Place",
		};

        public VirtListView(Forest trees)
	    {
            _data = trees.AllPeople.ToArray();
            InitializeUIComponents();
	    }

        public new void Dispose()
        {
            _data = null;
            base.Dispose();
        }

        private int ItemsCount
        {
            get { return _data.Length; }
        }

	    void InitializeUIComponents ()
	    {
		    _lv = new MyListView ();
		    _lv.Location = new Point (10, 10);
		    _lv.Size = new Size (500, 500);
		    _lv.FullRowSelect = true;
		    _lv.RetrieveVirtualItem += ListViewRetrieveItem;
		    _lv.VirtualListSize = ItemsCount;
		    _lv.VirtualMode = true;
            _lv.View = View.Details;
	        _lv.GridLines = true;
	        _lv.AllowColumnReorder = true;

	        foreach (var s in _columnT)
	        {
	            _lv.Columns.Add(s);
	        }
            //lv.Columns.AddRange(_columnT);
            //lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

            _lv.ColumnClick += lv_ColumnClick;
		    Controls.AddRange (new Control [] { _lv });

		    Size = new Size (630, 580);
		    Text = "VirtualMode tester";
	    }

	    void ListViewRetrieveItem (object o, RetrieveVirtualItemEventArgs args)
	    {
#if false
            if (args.ItemIndex == ItemsCount - 1 && !IsHandleCreated)
	            ;//warning_label.Text = "Warning: The very last item was requested, which should not happen in load time (not visible yet)";
#endif

	        Person p = _data[args.ItemIndex];

            List<string> items = new List<string>();
            items.Add(p.Id);
            items.Add(p.Name);
            items.Add(p.Sex);
            items.Add(p.GetDate("BIRT"));
            items.Add(p.GetPlace("BIRT"));
            items.Add(p.GetDate("DEAT"));
            items.Add(p.GetPlace("DEAT"));
            ListViewItem lvi = new ListViewItem(items.ToArray()) { BackColor = args.ItemIndex % 2 == 0 ? Color.AntiqueWhite : Color.White };

		    args.Item = lvi;
	    }

        Dictionary<int, SortOrder> mySortOrderMap = new Dictionary<int, SortOrder>
        {
            {0, SortOrder.None },
            {1, SortOrder.None },
            {2, SortOrder.None },
            {4, SortOrder.None },
            {6, SortOrder.None },
        };

        // define comparer for each column
        Dictionary<int, Comparison<Person>> myComparers = new Dictionary<int, Comparison<Person>>
        {
            {0, (a,b) => a.Id.CompareTo(b.Id)},
            {1, (a,b) => a.Name.CompareTo(b.Name)},
            {2, (a,b) => a.Sex.CompareTo(b.Sex)},
            {4, (a,b) => a.GetPlace("BIRT").CompareTo(b.GetPlace("BIRT")) },
            {6, (a,b) => a.GetPlace("DEAT").CompareTo(b.GetPlace("DEAT")) },
        };

        // State transitions from one sort order to another
        Dictionary<SortOrder, SortOrder> myToggle = new Dictionary<SortOrder, SortOrder>
        {
            {SortOrder.None, SortOrder.Ascending },
            {SortOrder.Ascending, SortOrder.Descending},
            {SortOrder.Descending, SortOrder.Ascending}
        };

        void SortBy(SortOrder order, Comparison<Person> comparer)
        {
            Array.Sort(_data, (a, b) =>
            {
                int lret = comparer(a, b); // Do the actual comparison
                if (order == SortOrder.Descending) // reverse when necessary
                {
                    lret *= -1;
                }
                return lret;
            });
        }


        void lv_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            var newSortOrder = myToggle[mySortOrderMap[e.Column]];
            mySortOrderMap[e.Column] = newSortOrder;     // Store sort order for current column
            FastSort(e.Column, newSortOrder);
            //SortBy(newSortOrder, myComparers[e.Column]); // Do the actual sorting
            _lv.Refresh();
        }

        delegate string Fetcher<in T>(T x);

        readonly Dictionary<int, Fetcher<Person>> _fetchers = new Dictionary<int, Fetcher<Person>>
        {
            {0, a => a.Id},
            {1, a => a.Name},
            {2, a => a.Sex},
            {4, a => a.GetPlace("BIRT")},
            {6, a => a.GetPlace("DEAT")},
        };

        void FastSort(int column, SortOrder newSortOrder)
        {
            var fetch = _fetchers[column];
            int count = _data.Length;
#if false
            KeyedList<string, Person> kl = new KeyedList<string,Person>(count);
            foreach (var person in _data)
            {
                kl.Add(fetch(person), person);
            }
            kl.SortAscd = newSortOrder == SortOrder.Ascending;
            kl.Sort();
            _data = new Person[count];
            int i = 0;
            foreach (var keyValuePair in kl)
            {
                _data[i] = keyValuePair.Value;
                i++;
            }
#endif
            KeyValuePair<string, Person>[] kl2 = new KeyValuePair<string, Person>[count];
            int j = 0;
            foreach (var person in _data)
            {
                kl2[j] = new KeyValuePair<string, Person>(fetch(person), person);
                j++;
            }
            KeyedList<string,Person>.Sort(kl2);
            _data = new Person[count];
            if (newSortOrder == SortOrder.Descending)
            {
                for (int k = 0; k < count; k++)
                    _data[k] = kl2[count-k-1].Value;
            }
            else
            {
                for (int k = 0; k < count; k++)
                    _data[k] = kl2[k].Value;
            }
        }
    }
}
