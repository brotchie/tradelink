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
    public partial class RecordMain : AppTracker
    {
        DebugWindow _dw = new DebugWindow();
        TickArchiver _ta = new TickArchiver();

        TLClient tl;
        Basket mb = new BasketImpl();
        public const string PROGRAM = "Record";
        AsyncResponse _ar = new AsyncResponse();
        TLTracker _tlt;
        
        public RecordMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            string ipaddr = Properties.Settings.Default.ServerIpAddresses;
            string[] servers = ipaddr.Split(',');
            if (ipaddr==string.Empty)
                tl = new TLClient_WM();
            else
                tl = new TLClient_IP(servers, Properties.Settings.Default.ServerPort, debug);
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
            if (tl.ProvidersAvailable.Length > 0)
            {
                if (tl.BrokerName != Providers.TradeLink)
                {
                    if (Environment.ProcessorCount == 1)
                        tl.gotTick += new TickDelegate(tl_gotTick);
                    else
                    {
                        tl.gotTick += new TickDelegate(tl_gotTick2);
                        _ar.GotTick += new TickDelegate(tl_gotTick);
                    }
                }
            }
            TradeLink.AppKit.Versions.UpgradeAlert(tl);
            FormClosing += new FormClosingEventHandler(RecordMain_FormClosing);
            // process command line
            processcommands();

        }

        void tl_gotTick2(Tick t)
        {
            _ar.newTick(t);
        }

        void processcommands()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2) return;
            try
            {
                debug("attempting to auto-record basket: " + args[1]);
                Basket b = BasketImpl.FromFile(args[1]);
                subscribe(b);
                return;
            }
            catch (Exception ex)
            {
                debug("Error auto-subscribing to: " + args[1]);
                debug(ex.Message + ex.StackTrace);
            }
        }

        void _tlt_GotDebug(string msg)
        {
            
        }

        void _tlt_GotConnectFail()
        {
            if (_tlt.tw.RecentTime != 0)
            {
                debug("Broker disconnected.");
                status("Stopped");
            }
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
            _ar.Stop();
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
            string syms = TextPrompt.Prompt("Enter symbols seperated by commas", "Symbols", mb.ToString());
            Basket b = BasketImpl.FromString(syms);
            subscribe(b);

        }

        void subscribe(Basket b)
        {
            mb = b;
            try
            {
                tl.Subscribe(mb);
            }
            catch (TLServerNotFound) 
            {
                debug("Unable to record symbols because no tradelink connector was running.");
                return;
            }
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
            tl.TLSend(MessageTypes.DOMREQUEST, tl.Name+ "+" + depth);
            debug("Set depth to: " + depth);

        }

    }
}