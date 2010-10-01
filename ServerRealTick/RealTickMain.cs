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

        
        DebugWindow _dw = new DebugWindow();
        public const string PROGRAM = "RealTickServer";
        Log _log = new Log(PROGRAM);

        private ServerRealTick tl;
        public RealTickMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();

            tl = new ServerRealTick();
            tl.SendDebug += tl_SendDebug;
            FormClosing += RediMain_FormClosing;
            InitializeComponent();
            tl.SendDebug += new TradeLink.API.DebugDelegate(tl_SendDebug);

            Text = "RealTick Connector";
            start();
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

        void start()
        {
            if (tl.Start())
                BackColor = Color.Green;
            else
                BackColor = Color.Red;
            Invalidate(true);
        }

        private void _start_Click(object sender, EventArgs e)
        {
            start();
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
