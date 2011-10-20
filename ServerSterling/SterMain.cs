using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using SterlingLib;
using TradeLink.API;
using TradeLink.AppKit;

namespace SterServer
{
    public partial class SterMain : AppTracker
    {
        // basic structures needed for operation
        ServerSterling tl;
        public const string PROGRAM = "SterServer ";
        DebugControl _dc = new DebugControl(true);
        Log _log = new Log(PROGRAM);
        public SterMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            _dc.Parent = this;
            _dc.Dock = DockStyle.Fill;
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("account", new EventHandler(setaccount));
            ContextMenu.MenuItems.Add("report", new EventHandler(report));
            try
            {
                TradeLink.API.TLServer tls;
                if (Properties.Settings.Default.TLClientAddress == string.Empty)
                {
                    tls = new TradeLink.Common.TLServer_WM();

                }
                else
                    tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);

                tl = new ServerSterling(tls, Properties.Settings.Default.Sleep,Properties.Settings.Default.OrderSleep,debug);
                tl.UseServerStops = Properties.Settings.Default.UseServerStops;
                tl.MinLotSize = Properties.Settings.Default.MinLotSize;
                tl.PostSymSubscribeWait = Properties.Settings.Default.PostSymbolSubscribeWait;
                tl.AutoSubscribeOrderSymbol = Properties.Settings.Default.AutosubscribeOrderSym;
                tl.OversellSplit = Properties.Settings.Default.OversellSplit;
                tl.RegSHOShorts = Properties.Settings.Default.RegSHOShorts;
                tl.LimitPositionUpdates = Properties.Settings.Default.PositionUpdateLimit;
                tl.isPaperTradeUsingBidAsk = Properties.Settings.Default.PaperTradeUseBidAsk;
                tl.isPaperTradeEnabled = Properties.Settings.Default.PaperTrade;
                tl.UseXmlMode = Properties.Settings.Default.UseXMLQuotes;
                tl.AutoCapAccounts = Properties.Settings.Default.AutoCapitilizeAccounts;
                tl.CoverEnabled = Properties.Settings.Default.CoverEnabled;
                tl.Accounts = Properties.Settings.Default.defaultaccount.Split(',');
                tl.CancelWait = Properties.Settings.Default.CancelWait;
                tl.VerboseDebugging = Properties.Settings.Default.VerboseDebugging;
                tl.FixOrderDecimalPlace = Properties.Settings.Default.FixOrderDecimalPlaces;
                tl.IgnoreOutOfOrderTicks = Properties.Settings.Default.IgnoreOutOfOrderTicks;
                tl.AutosetUnsetId = Properties.Settings.Default.AutoSeUnsetId;
                tl.SendCancelOnReject = Properties.Settings.Default.SendCancelOnRejects;
                tl.SendCancelOnError = Properties.Settings.Default.SendCancelOnError;
                tl.UseSubscribedSymbolForNotify = Properties.Settings.Default.UseSubscribedSymbolForNotify;
                tl.Start();
            }
            catch (Exception ex)
            {
                const string URL = @"http://code.google.com/p/tradelink/wiki/SterConfig";
                debug("problem connecting to sterling...");
                debug("please check guide at: " + URL);
                System.Diagnostics.Process.Start(URL);
                debug(ex.Message+ex.StackTrace);
            }
            FormClosing += new FormClosingEventHandler(SterMain_FormClosing);
        }

        void setaccount(object sender, EventArgs e)
        {
            string acct = Microsoft.VisualBasic.Interaction.InputBox("Provide default account name: ", "Default Sterling Account", Properties.Settings.Default.defaultaccount, 0, 0);
            Properties.Settings.Default.defaultaccount = acct;
            tl.Accounts = acct.Split(',');
        }

        void report(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _msgs.ToString(), null, new AssemblaTicketWindow.LoginSucceedDel(success), false);
        }

        StringBuilder _msgs = new StringBuilder();
        void success(string u, string p)
        {
            _msgs = new StringBuilder();
        }

        void debug(string msg)
        {
            _log.GotDebug(msg);
            _msgs.AppendLine(Util.ToTLTime() + " " + msg);
            _dc.GotDebug(msg);
        }



        void SterMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                
                Properties.Settings.Default.Save();
                tl.Stop();
                _log.Stop();
            }
            catch (Exception)
            {
                // incase stering was already closed 
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            setaccount(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            report(sender, e);
        }

        private void verbosetoggle_Click(object sender, EventArgs e)
        {
            tl.VerboseDebugging = !tl.VerboseDebugging;
            Properties.Settings.Default.VerboseDebugging = tl.VerboseDebugging;
            debug("verbose mode: "+ (tl.VerboseDebugging ? "ON" : "OFF"));
        }






    }
}
