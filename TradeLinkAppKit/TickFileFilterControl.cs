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
            InitializeComponent();
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
            TickFileFilter tff = new TickFileFilter(symfilter, datefilter);
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
                SecurityImpl s = Util.SecurityFromFileName(index[i,0]);
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
            stocklist.Enabled = !stocklist.Enabled;
            fupdate(sender);
        }

        private void usedates_CheckedChanged(object sender, EventArgs e)
        {
            yearlist.Enabled = !yearlist.Enabled;
            monthlist.Enabled = !monthlist.Enabled;
            daylist.Enabled = !daylist.Enabled;
            fupdate(sender);
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
        void fupdate(object sender) { if (FilterUpdate != null) FilterUpdate(sender, new EventArgs()); }
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





    }
}
