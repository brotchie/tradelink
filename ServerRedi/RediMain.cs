using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;
using TradeLink.Common;

namespace ServerRedi
{
    public partial class RediMain : AppTracker
    {
        ServerRedi tl;
        DebugWindow _dw = new DebugWindow();
        public const string PROGRAM = "RediServer";
        Log _log = new Log(PROGRAM);
        public RediMain()
        {
            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tls = new TradeLink.Common.TLServer_WM();
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);

            tl = new ServerRedi(tls);
            if (Properties.Settings.Default.AccountsAvailable != string.Empty)
            {
                tl.Accounts = Properties.Settings.Default.AccountsAvailable.Split(',');
                debug("Advertising static accounts: " + tl.Accounts);
            }
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl.TickDebugVerbose = Properties.Settings.Default.TickDebuggingVerbose;

            tl.isPaperTradeEnabled = Properties.Settings.Default.PaperTrade;
            tl.isPaperTradeUsingBidAsk = Properties.Settings.Default.PaperTradeBidAsk;
            tl.VerboseDebugging = Properties.Settings.Default.VerboseDebugging;
            tl.SendDebug += new TradeLink.API.DebugDelegate(tl_SendDebug);

            FormClosing += new FormClosingEventHandler(RediMain_FormClosing);
        }

        void success(string u, string p)
        {
            _msgs = new StringBuilder();
        }

        void RediMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            tl.Stop();
            _log.Stop();
        }

        void tl_SendDebug(string msg)
        {

            debug(msg);
        }

        StringBuilder _msgs = new StringBuilder();

        public void debug(string msg)
        {
            _msgs.AppendLine(msg);
            _dw.GotDebug(msg);
            _log.GotDebug(msg);
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (tl.Start(_user.Text, _pass.Text, txtAcronym.Text + txtAccount.Text))
                BackColor = Color.Green;
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
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _msgs.ToString(), null, new AssemblaTicketWindow.LoginSucceedDel(success), false);
        }
    }
}
