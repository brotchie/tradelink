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

namespace ServerBlackwood
{
    public partial class ServerBlackwoodMain : Form
    {
        public const string PROGRAM = "ServerBlackwood";
        public DebugWindow _dw = new DebugWindow();

        ServerBlackwood _con = new ServerBlackwood();

        public ServerBlackwoodMain()
        {
            InitializeComponent();
            _con.SendDebug+=new DebugFullDelegate(_dw.GotDebug);
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (_con.Start(_un.Text, _pw.Text, _ipaddress.Text, 0))
            {
                BackColor = Color.Green;
                _dw.GotDebug("login successful");
                Invalidate();
            }
            else
            {
                BackColor = Color.Red;
                _dw.GotDebug("login failed.");
            }
        }

        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            CrashReport.BugReport(PROGRAM, _dw.Content);
        }
    }
}
