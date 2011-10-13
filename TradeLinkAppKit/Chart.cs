using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    public partial class Chart : Form
    {
        public Chart() : this(null, false) { }
        public Chart(BarList b) : this(b, false) { }
        bool _allowclose = false;
        public bool AllowClosing { get { return _allowclose; } set { _allowclose = value; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="Chart"/> class.
        /// </summary>
        /// <param name="b">The barlist.</param>
        /// <param name="allowtype">if set to <c>true</c> [allowtype] will allow typing/changing of new symbols on the chart window.</param>
        public Chart(BarList b,bool allowtype)
        {
            InitializeComponent();
            chartControl1.DisplayInterval = false;
            MouseUp +=new MouseEventHandler(chartControl1.Chart_MouseUp);
            MouseWheel += new MouseEventHandler(Chart_MouseUp);
            if (allowtype) this.KeyUp += new KeyEventHandler(Chart_KeyUp);
            if (b != null)
            {
                chartControl1.NewBarList(b);
                Symbol = b.Symbol;
            }
            FormClosing += new FormClosingEventHandler(Chart_FormClosing);
        }

        void Chart_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_allowclose)
            {
                e.Cancel = true;
                Toggle();
            }
        }

        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }

        void Chart_MouseUp(object sender, MouseEventArgs e)
        {
            Text = chartControl1.Title;
        }

        string _sym = string.Empty;
        string _newstock = string.Empty;
        public string Symbol { get { return _sym; } set { _sym = value; Text = chartControl1.Title; } }

        public void NewBarList(BarList barlist)
        {
            chartControl1.NewBarList(barlist);
            Symbol = barlist.Symbol;
            Text = chartControl1.Title;
            Invalidate(true);
        }

        public void newTick(Tick k)
        {
            chartControl1.newTick(k);
            if (Visible)
                chartControl1.redraw();
        }

        public void newPoint(string symbol,decimal p, int time, int date, int size)
        {
            chartControl1.newPoint(symbol,p, time, date, size);
            if (Visible)
                chartControl1.redraw();

        }

        void Chart_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.KeyValue >= (int)Keys.A) && (e.KeyValue <= (int)Keys.Z)) _newstock += e.KeyCode.ToString();
                if ((_newstock.Length > 0) && (e.KeyValue == (int)Keys.Back)) _newstock = _newstock.Substring(0, _newstock.Length - 1);
                this.Text = chartControl1.Title + " " + _newstock;
            }
            catch (Exception) { }
            if (e.KeyValue == (int)Keys.Enter)
            {
                string stock = _newstock.ToString();
                _newstock = "";
                BarListImpl.DayFromGoogleAsync(stock, new BarListDelegate(gotchart));
            }


        }

        void gotchart(BarList chart)
        {
            NewBarList(chart);
        }

    }
}
