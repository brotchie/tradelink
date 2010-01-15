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
        DebugControl _dc = new DebugControl(true);
        public RediMain()
        {
            InitializeComponent();
            _dc.Parent = this;
            _dc.Dock = DockStyle.Fill;
            tl.SendDebug += new TradeLink.API.DebugDelegate(tl_SendDebug);
            tl.Start();
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("report", new EventHandler(report));
            FormClosing += new FormClosingEventHandler(RediMain_FormClosing);
        }

        void report(object sender, EventArgs e)
        {
            CrashReport.Report(ServerRedi.PROGRAM, string.Empty, string.Empty, _msgs.ToString(),null, new AssemblaTicketWindow.LoginSucceedDel(success),false);
        }

        void success(string u, string p)
        {
            _msgs = new StringBuilder();
        }

        void RediMain_FormClosing(object sender, FormClosingEventArgs e)
        {
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
            _dc.GotDebug(msg);
        }
    }
}
