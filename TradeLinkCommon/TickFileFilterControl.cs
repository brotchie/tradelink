using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.API;

namespace TradeLink.Common
{
    public partial class TickFileFilterControl : UserControl
    {
        public event DebugFullDelegate SendDebug;
        public TickFileFilterControl()
        {
            InitializeComponent();
        }
        public TickFileFilterControl(string path)
            : this()
        {
            SetSymbols(path);
        }
        public TickFileFilterControl(string[,] index)
        {
            SetSymbols(index);
        }

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
            //return it
            return tff;

        }

        string[,] _index = new string[0, 0];

        public void SetSymbols(string path)
        {
            // build list of available stocks and dates available
            stocklist.Items.Clear();
            yearlist.Items.Clear();
            daylist.Items.Clear();
            monthlist.Items.Clear();

            try
            {
                const string ext = "*.epf";
                _index = Util.TickFileIndex(path, ext);
            }
            catch (Exception ex) { status("exception loading stocks: " + ex.ToString()); return; }
            SetSymbols(_index);
        }
        public void SetSymbols(string[,] index)
        {
            _index = index;
            int[] years = new int[200];
            int[] days = new int[31];
            int[] months = new int[12];
            int yc = 0;
            int dc = 0;
            int mc = 0;
            int count = _index.GetLength(0);

            for (int i = 0; i < count; i++)
            {
                SecurityImpl s = Util.SecurityFromFileName(_index[i,0]);
                if (!s.isValid) continue;
                DateTime d = Util.ToDateTime(s.Date, 0);
                if (!stocklist.Items.Contains(s.Symbol))
                    stocklist.Items.Add(s.Symbol);
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

        bool contains(int number, int[] array) { for (int i = 0; i < array.Length; i++) if (array[i] == number) return true; return false; }


        private void usestocks_CheckedChanged(object sender, EventArgs e)
        {
            stocklist.Enabled = !stocklist.Enabled;
        }

        private void usedates_CheckedChanged(object sender, EventArgs e)
        {
            yearlist.Enabled = !yearlist.Enabled;
            monthlist.Enabled = !monthlist.Enabled;
            daylist.Enabled = !daylist.Enabled;
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


    }
}
