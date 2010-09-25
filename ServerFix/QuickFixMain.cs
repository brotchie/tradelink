using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;

namespace ServerFix
{
    public partial class QuickFixMain : Form
    {
        public const string PROGRAM = "ServerFIX";
        DebugControl _dc = new DebugControl();
        StringBuilder _sb = new StringBuilder();
        Log _log = new Log(PROGRAM);
        ServerQuickFix tl;
        public QuickFixMain()
        {
            InitializeComponent();
            ContextMenu.MenuItems.Add("report", new EventHandler(report));
            _dc.Parent = this;
            _dc.Dock = DockStyle.Fill;
            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tls = new TradeLink.Common.TLServer_WM();
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);

            tl = new ServerQuickFix(tls,Properties.Settings.Default.SettingsPath);
            tl.SendDebugEvent+=new TradeLink.API.DebugDelegate(debug);
            if (tl.Start(string.Empty, string.Empty))
                debug("login succeeded.");
            else
                debug("connection failed.");
            FormClosing += new FormClosingEventHandler(QuickFixMain_FormClosing);
        }

        void report(object s, EventArgs e)
        {
            CrashReport.Report(PROGRAM,string.Empty,string.Empty,_sb.ToString(),null, new AssemblaTicketWindow.LoginSucceedDel(success),false);
        }

        void success(string u, string p)
        {
            _sb = new StringBuilder();
        }

        void debug(string msg)
        {
            _sb.AppendLine(msg);
            _dc.GotDebug(msg);
            _log.GotDebug(msg);
        }

        void QuickFixMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Stop();
        }
    }
}
