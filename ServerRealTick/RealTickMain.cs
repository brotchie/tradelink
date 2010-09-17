using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TalTrade.Toolkit;
using TradeLink.AppKit;
using TradeLink.Common;

namespace RealTickConnector
{
    public partial class RealTickMain : AppTracker
    {

        ToolkitApp _app;
        DebugWindow _dw = new DebugWindow();
        public const string PROGRAM = "RealTickServer";
        Log _log = new Log(PROGRAM);

        private ServerRealTick tl;
        public RealTickMain(ToolkitApp app)
        {
            _app = app;
            tl = new ServerRealTick(app);
            InitializeComponent();
            tl.SendDebug += new TradeLink.API.DebugDelegate(tl_SendDebug);
            ConfigSection cfg = new ConfigSection("ToolkitExamples");
            // The default test symbols for this example are ZVZZT and ZWZZT.  You can add an
            // additional symbol in your TAL.INI file, e.g.:
            //    [ToolkitExamples]
            //    TestSymbol=TEST
            // but be extremely cautious ... if you use a real symbol, you may in fact execute shares
            // of your stock!
            string symbol = cfg.GetValue("IBM", "");

            // The default test routes for this example are DEMO, DEMOEUR, and TALX.  You can add an
            // additional route in your TAL.INI file, e.g.:
            //    [ToolkitExamples]
            //    TestRoute=MYTESTROUTE
            // but be extremely cautious ... if you use a live route, you may in fact execute shares
            // of your stock!

            Text = "RealTick v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public RealTickMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl.SendDebug += tl_SendDebug;
            FormClosing += RediMain_FormClosing;
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

        private void _start_Click(object sender, EventArgs e)
        {
            if (tl.Start())
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
