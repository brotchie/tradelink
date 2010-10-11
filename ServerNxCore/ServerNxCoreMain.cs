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
            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress== string.Empty)
                tls = new TradeLink.Common.TLServer_WM() ;
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            string start = Properties.Settings.Default.HistoricalFile == string.Empty ? ServerNxCore.LIVEFEED : Properties.Settings.Default.HistoricalFile;
            tl = new ServerNxCore(tls, start, Properties.Settings.Default.StateSaveInterval, Properties.Settings.Default.VerboseDebugging, debug);
            debug((tl.VerboseDebugging ? "Verbose is on" : "Verbose is off"));
            debug("save state interval: " + tl.SaveStateIntervalSec);
            debug("save state interval: " + tl.SaveStateIntervalSec);
            FormClosing += new FormClosingEventHandler(ServerNxCoreMain_FormClosing);
            tl.Start();
        }

        void ServerNxCoreMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (tl != null)
                {
                    tl.Stop();
                }
            }
            catch { }
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
