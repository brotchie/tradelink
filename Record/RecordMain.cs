using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace Record
{
    public partial class RecordMain : Form
    {
        DebugWindow _dw = new DebugWindow();
        TickArchiver _ta = new TickArchiver();

        TLClient_WM tl = new TLClient_WM();
        Basket mb = new BasketImpl();
        public const string PROGRAM = "Record";
        AsyncResponse _ar = new AsyncResponse();
        TLTracker _tlt;
        
        public RecordMain()
        {
            InitializeComponent();
            int pollms = (int)(((double)Properties.Settings.Default.brokertimeoutsec * 1000) / 2);
            _tlt = new TLTracker(pollms, Properties.Settings.Default.brokertimeoutsec, tl, Providers.Unknown, true);
            _tlt.GotConnect += new VoidDelegate(_tlt_GotConnect);
            _tlt.GotConnectFail += new VoidDelegate(_tlt_GotConnectFail);
            _tlt.GotDebug += new DebugDelegate(_tlt_GotDebug);
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("SymbolList", new EventHandler(symlist));
            ContextMenu.MenuItems.Add("Level2", new EventHandler(level2));
            ContextMenu.MenuItems.Add("Timeout", new EventHandler(timeout));
            ContextMenu.MenuItems.Add("ReportBug", new EventHandler(report));
            ContextMenu.MenuItems.Add("Messages", new EventHandler(togdebug));
            if (tl.RequestFeatureList.Contains(MessageTypes.LIVEDATA) )
            {
                if (Environment.ProcessorCount == 1)
                    tl.gotTick += new TickDelegate(tl_gotTick);
                else
                {
                    tl.gotTick+=new TickDelegate(_ar.newTick);
                    _ar.GotTick+=new TickDelegate(tl_gotTick);
                }
            }
            TradeLink.AppKit.Versions.UpgradeAlert(tl);
            FormClosing += new FormClosingEventHandler(RecordMain_FormClosing);
        }

        void _tlt_GotDebug(string msg)
        {
            
        }

        void _tlt_GotConnectFail()
        {
            debug("Broker disconnected.");
            status("stopped");
        }

        void status(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { msg });
            else
            {
                Text = PROGRAM + " (" + msg + ")";
                Invalidate(true);
            }
        }

        void _tlt_GotConnect()
        {
            debug(tl.BrokerName + " " + tl.ServerVersion + " connected.");
            status(tl.BrokerName.ToString());
            try
            {
                // resubscribe
                if (mb.Count > 0)
                    tl.Subscribe(mb);
            }
            catch { }
            if (tl.BrokerName == Providers.TradeLink)
            {
                _ta.Stop();
                _tlt.Stop();
            }
        }

        void RecordMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            _ta.Stop();
            _tlt.Stop();
        }

        void togdebug(object o, EventArgs e)
        {
            _dw.Toggle();
        }

        void report(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        void symlist(object sender, EventArgs e)
        {
            string syms = Microsoft.VisualBasic.Interaction.InputBox("Enter symbols seperated by commas", "Symbols", mb.ToString(), 0, 0);
            Basket b = BasketImpl.FromString(syms);
            mb = b;
            try
            {
                tl.Subscribe(mb);
            }
            catch (TLServerNotFound) { }
            refreshlist();
            debug("Recording: " + mb.ToString());

        }

        void debug(string msg)
        {
            _dw.GotDebug(msg);
        }

        void timeout(object sender, EventArgs e)
        {
            string times = Microsoft.VisualBasic.Interaction.InputBox("Enter Broker Reconnect Timeout", "Broker Timeout", _tlt.AlertThreshold.ToString(), 0, 0);
            uint timeout = 0;
            if (!uint.TryParse(times, out timeout)) return;
            _tlt.AlertThreshold = (int)timeout;
            debug("Timeout: " + timeout);
        }



        void tl_gotTick(Tick t)
        {
            _ta.newTick(t);
            _tlt.newTick(t);
        }


        void refreshlist()
        {
            stockslist.Items.Clear();
            for (int i = 0; i < mb.Count; i++)
                stockslist.Items.Add(mb[i].Symbol);
        }

        bool _l2 = false;
        private void level2(object sender, EventArgs e)
        {
            _l2 = !_l2;
            ContextMenu.MenuItems[1].Checked = _l2;
            Invalidate(true);
            int depth = _l2 ? Book.MAXBOOK : 1;
            tl.RequestDOM(depth);
            debug("Set depth to: " + depth);

        }

    }
}