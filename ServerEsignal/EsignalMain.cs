using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;
using IESignal;


namespace ServerEsignal
{
    public partial class EsignalMain : AppTracker
    {

        EsignalServer tl;
        public const string PROGRAM = "ServerEsignal";
        Log _log = new Log(PROGRAM);
        DebugWindow _dw = new DebugWindow();
        
        public EsignalMain()
        {
            TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tls = new TLServer_WM();
            else
                tls = new TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);

            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl = new EsignalServer(tls);
            // set defaults
            tl.isPaperTradeEnabled = Properties.Settings.Default.PaperTrade;
            tl.isPaperTradeUsingBidAsk = Properties.Settings.Default.PaperTradeUseBidAsk;
            tl.AllowSendInvalidBars = Properties.Settings.Default.AllowSendOfInvalidBars;
            tl.DefaultBarsBack = Properties.Settings.Default.DefaultBarsBack;
            tl.VerboseDebugging = Properties.Settings.Default.VerboseDebugging;
            tl.ReleaseBarHistoryAfteRequest = Properties.Settings.Default.ReleaseBarHistoryAfterSending;
            tl.ReleaseDeadSymbols = Properties.Settings.Default.ReleaseDeadSymbols;
            tl.WaitBetweenEvents = Properties.Settings.Default.WaitBetweenEvents;
            tl.BarRequestsGetAllData = Properties.Settings.Default.BarRequestsuseAllData;
            // send debug messages to log file
            tl.GotDebug += new DebugFullDelegate(debug);
            debug("Started " + PROGRAM + Util.TLVersion());
            // attempt to connect to esignal
            _ok_Click(null, null);
            // handle connector exits
            FormClosing += new FormClosingEventHandler(EsignalMain_FormClosing);
        }


        void debug(Debug deb)
        {
            debug(deb.Msg);
        }

        void debug(string msg)
        {

            _dw.GotDebug(msg);
            _log.GotDebug(msg);


        }

        void EsignalMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            tl.Stop();
            _log.Stop();
        }


        // attempt to connect to esignal
        private void _ok_Click(object sender, EventArgs e)
        {
            if (_acctapp.Text == string.Empty) return;
            tl.Start(_acctapp.Text,null,null,0);
            if (tl.isValid) { BackColor = Color.Green; }
            else
                BackColor = Color.Red;
            Invalidate(true); 
        }

        private void _msg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(PROGRAM, _dw.Content, null, null, false, string.Empty);
        }
    }
}
