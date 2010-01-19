using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;

namespace ServerRedi
{
    public partial class RediMain : Form
    {
        ServerRedi tl = new ServerRedi();
        DebugWindow _dw = new DebugWindow();
        public RediMain()
        {
            InitializeComponent();
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
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (tl.Start(_user.Text, _pass.Text))
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
            CrashReport.Report(ServerRedi.PROGRAM, string.Empty, string.Empty, _msgs.ToString(), null, new AssemblaTicketWindow.LoginSucceedDel(success), false);
        }
    }
}
