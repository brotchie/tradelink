using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ServerNxCore
{
    public partial class ServerNxCoreMain : Form
    {
        ServerNxCore tl;
        public const string PROGRAM = "ServerNxCore";
        TradeLink.AppKit.Log _log = new TradeLink.AppKit.Log(PROGRAM);
        public ServerNxCoreMain()
        {
            InitializeComponent();
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("report", new EventHandler(report)));
            tl = new ServerNxCore(ServerNxCore.LIVEFEED, debug);
            FormClosing += new FormClosingEventHandler(ServerNxCoreMain_FormClosing);
            tl.Start();
        }

        void ServerNxCoreMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tl != null)
            {
                tl.Stop();
            }
            _log.Stop();
        }

        void report(object s, EventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(PROGRAM, string.Empty, string.Empty, sb.ToString(), null, new TradeLink.AppKit.AssemblaTicketWindow.LoginSucceedDel(success), false);
        }

        void success(string u, string p)
        {
            sb = new StringBuilder();
        }
        
        StringBuilder sb = new StringBuilder();
        public string DebugContent { get { return sb.ToString(); } }
        void debug(string msg)
        {
            sb.AppendLine(msg);
            debugControl1.GotDebug(msg);
            _log.GotDebug(msg);
        }
    }
}
