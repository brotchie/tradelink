using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    /// <summary>
    /// a GUI for building tick file filters easily.
    /// useful for selecting a backtesting run.
    /// </summary>
    public partial class TickFileFilterControl : UserControl
    {
        /// <summary>
        /// sends debug messages from control
        /// </summary>
        public event DebugFullDelegate SendDebug;
        /// <summary>
        /// sent whenever user changes filter
        /// </summary>
        public event EventHandler FilterUpdate;
        /// <summary>
        /// creates a tick file filter control
        /// </summary>
        public TickFileFilterControl()
        {
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("View tickdata", new EventHandler(viewdata));
            ContextMenu.MenuItems.Add("Export filter", new EventHandler(export));
            _dw.Text = "Selected Tickdata:";
            _dw.TimeStamps = false;
            _dw.GotDebug("No tickdata selected.");
            InitializeComponent();
        }

        void export(object o, EventArgs e)
        {
            TickFileFilter tff = GetFilter();
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Filter (*.txt)|*.txt|All files (*.*)|*.*";
            sfd.AddExtension = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (TickFileFilter.ToFile(tff, sfd.FileName))
                    _dw.GotDebug("saved filter as: " + sfd.FileName);
                else
                    _dw.GotDebug("unable to save filter as: " + sfd.FileName);
            }
        }

        void viewdata(object o, EventArgs e)
        {
            _dw.Toggle();
        }

        /// <summary>
        /// creates a tick file filter control from a tickfolder path
        /// </summary>
        /// <param name="path"></param>
        public TickFileFilterControl(string path)
            : this()
        {
            SetSymbols(path);
        }

        /// <summary>
        /// creates tickfilefilter control from an index
        /// </summary>
        /// <param name="index"></param>
        public TickFileFilterControl(string[,] index)
        {
            SetSymbols(index);
        }
        /// <summary>
        /// gets currently selected filter
        /// </summary>
        /// <returns></returns>
        public TickFileFilter GetFilter()
        {
            TickFileFilter tff = new TickFileFilter();
            // prepare date filter
            List<TickFileFilter.TLDateFilter> datefilter = new List<TickFileFilter.TLDateFilter>();
            if (usedates.Checked)
            {
                for (int j = 0; j < yearlist.SelectedIndices.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(yearlist.Items[yearlist.SelectedIndices[j]]) * 10000, DateMatchType.Year));
                for (int j = 0; j < monthlist.SelectedItems.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(monthlist.Items[monthlist.SelectedIndices[j]]) * 100, DateMatchType.Month));
                for (int j = 0; j < daylist.SelectedItems.Count; j++)
                    datefilter.Add(new TickFileFilter.TLDateFilter(Convert.ToInt32(daylist.Items[daylist.SelectedIndices[j]]), DateMatchType.Day));
            }
            // prepare symbol filter
            List<string> symfilter = new List<string>();
            if (usestocks.Checked)
                for (int j = 0; j < stocklist.SelectedItems.Count; j++)
                    symfilter.Add(stocklist.Items[stocklist.SelectedIndices[j]].ToString());

            // build consolidated filter
            if (usestocks.Checked && usedates.Checked)
                tff = new TickFileFilter(symfilter, datefilter);
            else if (usestocks.Checked)
                tff = new TickFileFilter(symfilter);
            else if (usedates.Checked)
                tff = new TickFileFilter(datefilter);
            // set search options
            tff.isDateMatchUnion = _dateor.Checked;
            tff.isSymbolDateMatchUnion = !_symdateand.Checked;
            //return filter
            return tff;

        }

        string[,] _index = new string[0, 0];
        /// <summary>
        /// sets available symbols found in a system path
        /// </summary>
        /// <param name="path"></param>
        public void SetSymbols(string path)
        {
            // build list of available stocks and dates available
            stocklist.Items.Clear();
            yearlist.Items.Clear();
            daylist.Items.Clear();
            monthlist.Items.Clear();

            try
            {
                _index = Util.TickFileIndex(path, TikConst.WILDCARD_EXT);
            }
            catch (Exception ex) { status("exception loading stocks: " + ex.ToString()); return; }
            SetSymbols(_index);
        }
        
        /// <summary>
        /// sets available symbols from an index
        /// </summary>
        /// <param name="index"></param>
        public void SetSymbols(string[,] index)
        {
            List<string> tmpstk = new List<string>();
            int[] years = new int[200];
            int[] days = new int[31];
            int[] months = new int[12];
            int yc = 0;
            int dc = 0;
            int mc = 0;
            int count = index.GetLength(0);

            for (int i = 0; i < count; i++)
            {
                SecurityImpl s = SecurityImpl.SecurityFromFileName(index[i,0]);
                if (!s.isValid) continue;
                DateTime d = Util.ToDateTime(s.Date, 0);
                if (!tmpstk.Contains(s.Symbol))
                    tmpstk.Add(s.Symbol);
                if (!contains(d.Year, years))
                    years[yc++] = d.Year;
                if (!contains(d.Month, months))
                    months[mc++] = d.Month;
                if (!contains(d.Day, days))
                    days[dc++] = d.Day;
            }
            Array.Sort(years);
            Array.Sort(days);
            Array.Sort(months);
            tmpstk.Sort();
            updateGUI(tmpstk.ToArray(), days, years, months, index);

        }

        delegate void guidata(string[] stk, int[] d, int[] y, int[] m, string[,] idx);
        void updateGUI(string[] stocks, int[] days, int[] years, int[] months, string[,] index)
        {
            if (InvokeRequired)
                Invoke(new guidata(updateGUI), new object[] { stocks, days, years, months, index });
            else
            {
                // clear gui elements
                stocklist.Items.Clear();
                yearlist.Items.Clear();
                monthlist.Items.Clear();
                daylist.Items.Clear();
                // save index so if filter changes can recompute
                _index = index;
                // add items to gui
                foreach (string sym in stocks)
                    stocklist.Items.Add(sym);
                for (int i = 0; i < years.Length; i++)
                    if (years[i] == 0) continue;
                    else yearlist.Items.Add(years[i]);
                for (int i = 0; i < months.Length; i++)
                    if (months[i] == 0) continue;
                    else monthlist.Items.Add(months[i]);
                for (int i = 0; i < days.Length; i++)
                    if (days[i] == 0) continue;
                    else daylist.Items.Add(days[i]);
                Invalidate(true);
            }
        }

        bool contains(int number, int[] array) { for (int i = 0; i < array.Length; i++) if (array[i] == number) return true; return false; }


        private void usestocks_CheckedChanged(object sender, EventArgs e)
        {
            if (stocklist.Enabled)
            {
                stocklist.ClearSelected();
                stocklist.Invalidate();
            }
            stocklist.Enabled = !stocklist.Enabled;
            fupdate(sender,false);
        }

        private void usedates_CheckedChanged(object sender, EventArgs e)
        {

            if (yearlist.Enabled)
            {
                yearlist.ClearSelected();
                daylist.ClearSelected();
                monthlist.ClearSelected();
                yearlist.Invalidate();
                daylist.Invalidate();
                monthlist.Invalidate();
            }
            yearlist.Enabled = !yearlist.Enabled;
            monthlist.Enabled = !monthlist.Enabled;
            daylist.Enabled = !daylist.Enabled;
            fupdate(sender,false);
        }

        private void stocklist_SelectedIndexChanged(object sender, EventArgs e)
        {
            fupdate(sender);
        }

        private void daylist_SelectedIndexChanged(object sender, EventArgs e)
        {
            fupdate(sender);
        }

        private void monthlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            fupdate(sender);
        }

        private void yearlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            fupdate(sender);
        }

        void fupdate(object sender) { fupdate(sender, true); }
        void fupdate(object sender, bool setunions) 
        {
            // set default search options
            if (setunions)
            {
                // if they have both dates and symbols selected, default
                // to intersection of both sets
                if (usedates.Checked && usestocks.Checked)
                {
                    _symdateand.Checked = true;
                }
                else
                {
                    _symdateand.Checked = false;
                }

                // if there are more than one year, more than one month,
                // or more than one day selected... default to a union of sets
                if ((yearlist.SelectedIndices.Count > 1)
                    || (monthlist.SelectedIndices.Count > 1)
                    || (daylist.SelectedIndices.Count > 1))
                {
                    _dateor.Checked = true;
                }
                // otherwise if dates are enabled, default to intersection only
                else if (yearlist.Enabled)
                {
                    _dateand.Checked = true;
                }
            }
            // if we're watching files
            if (_dw.Visible)
            {
                // clear window
                _dw.Clear();
                // get current filter
                TickFileFilter tff = GetFilter();
                // get matching files
                string [] files = tff.Allows(TikUtil.GetFiles());
                // display in window
                foreach (string file in files)
                    _dw.GotDebug(System.IO.Path.GetFileNameWithoutExtension(file));

            }



            // notify listeners
            if (FilterUpdate != null) 
                FilterUpdate(sender, new EventArgs()); 
        }
        void status(string msg)
        {
            if (SendDebug != null)
                SendDebug(DebugImpl.Create(msg, DebugLevel.Status));
        }

        void debug(string msg)
        {
            if (SendDebug != null)
                SendDebug(DebugImpl.Create(msg, DebugLevel.Debug));

        }

        DebugWindow _dw = new DebugWindow();

        private void _symdateand_CheckedChanged(object sender, EventArgs e)
        {
            _symdateor.Checked = !_symdateand.Checked;
            fupdate(sender,false);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // sym date or
            _symdateand.Checked = !_symdateor.Checked;
            fupdate(sender,false);
        }

        private void _dateand_CheckedChanged(object sender, EventArgs e)
        {
            _dateor.Checked = !_dateand.Checked;
            fupdate(sender,false);
        }

        private void _dateor_CheckedChanged(object sender, EventArgs e)
        {
            _dateand.Checked = !_dateor.Checked;
            fupdate(sender,false);
        }






    }
}
