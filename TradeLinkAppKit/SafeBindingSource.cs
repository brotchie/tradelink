using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace TradeLink.AppKit
{
    public class SafeBindingSource : BindingSource
    {
        bool _sortable = true;
        public SafeBindingSource() : this(true) { }
        public SafeBindingSource(bool sortallowed) : base() { _sortable = false; }
        public override bool SupportsSorting
        {
            get
            {
                return _sortable;
            }
        }
        public override int Add(object value)
        {
            lock (SyncRoot)
            {
                return base.Add(value);
            }
        }

        public override void Clear()
        {
            lock (SyncRoot)
            {
                base.Clear();
            }
        }

        public override int IndexOf(object value)
        {
            lock (SyncRoot)
            {
                return base.IndexOf(value);
            }
        }

        public override void Insert(int index, object value)
        {
            lock (SyncRoot)
            {
                base.Insert(index, value);
            }
        }

        public override void Remove(object value)
        {
            lock (SyncRoot)
            {
                base.Remove(value);
            }
        }

        public override void RemoveAt(int index)
        {
            lock (SyncRoot)
            {
                base.RemoveAt(index);
            }
        }

        public override int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return base.Count;
                }
            }
        }

        public override object this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    try
                    {
                        return base[index];
                    }
                    catch { return base[index]; }
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    try
                    {
                        base[index] = value;
                    }
                    catch { }
                }
            }
        }


        public override System.Collections.IEnumerator GetEnumerator()
        {
            lock (SyncRoot)
            {
                return base.GetEnumerator();
            }
        }

        delegate void booldel(DataGridView dg,BindingSource bs, bool v);
        public static void refreshgrid(DataGridView _dg,BindingSource _bs ) { refreshgrid(_dg,_bs,false); }
        public static void refreshgrid(DataGridView _dg, BindingSource _bs, bool endstate)
        {

            if (_dg.InvokeRequired)
            {
                try
                {
                    _dg.Invoke(new booldel(refreshgrid), new object[] { _dg,_bs,endstate });
                }
                catch (ObjectDisposedException) { }
            }
            else
            {
                // save screen position and selections
                List<int> sel = new List<int>();
                int first = -1;
                try
                {
                    lock (_dg)
                    {
                        first = _dg.FirstDisplayedScrollingRowIndex;
                        foreach (DataGridViewRow dr in _dg.SelectedRows)
                            sel.Add(dr.Index);
                    }



                    // update screen
                    _bs.RaiseListChangedEvents = true;
                    _bs.ResetBindings(false);
                    // diable updates again
                    _bs.RaiseListChangedEvents = endstate;

                }
                catch (Exception)
                {

                }

                // restore screen position and selections
                lock (_dg)
                {
                    try
                    {
                        if (first != -1)
                            _dg.FirstDisplayedScrollingRowIndex = first;
                        foreach (int r in sel)
                            _dg.Rows[r].Selected = true;
                    }
                    catch
                    {
                        // in case this row was deleted in the middle of an update
                    }
                }
            }
        }
    }
}
