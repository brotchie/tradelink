using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.AppKit
{
    public partial class TradeResultsWindow : Form
    {
        public bool AutoWatch { get { return tradeResults1.AutoWatch; } set { tradeResults1.AutoWatch = value; } }
        public void NewResultsFile(string filename) { tradeResults1.NewResultFile(filename); }
        public void NewResultsTrades(string name, List<Trade> trades) { tradeResults1.NewResultTrades(name, trades); }
        public event DebugDelegate SendDebug;
        public void Clear()
        {
            tradeResults1.Clear();
        }
        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }
        public TradeResultsWindow()
        {
            InitializeComponent();
            tradeResults1.SendDebug += new DebugDelegate(tradeResults1_SendDebug);
            FormClosing += new FormClosingEventHandler(TradeResultsWindow_FormClosing);
        }

        void TradeResultsWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Toggle();
        }

        void tradeResults1_SendDebug(string msg)
        {
            if (SendDebug != null)
                SendDebug(msg);
        }
    }
}
