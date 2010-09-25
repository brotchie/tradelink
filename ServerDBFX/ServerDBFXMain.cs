using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;
using TradeLink.Common;

namespace ServerDBFX
{
    public partial class ServerDBFXMain : AppTracker
    {
        ServerDBFX _dbfx;
        public const string PROGRAM = "ServerDBFX";
        Log _log = new Log(PROGRAM);
        public ServerDBFXMain()
        {
            TradeLink.API.TLServer tl;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tl = new TLServer_WM();
            else
                tl = new TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            _dbfx = new ServerDBFX(tl);

            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            _dbfx.SendDebug += new TradeLink.API.DebugFullDelegate(_dbfx_SendDebug);
            FormClosing += new FormClosingEventHandler(ServerDBFXMain_FormClosing);
        }

        void _dbfx_SendDebug(TradeLink.API.Debug debug)
        {
            _dw.GotDebug(debug);
            _log.GotDebug(debug);
        }


        void ServerDBFXMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            _dbfx.Stop();
            _log.Stop();
        }

        public DebugWindow _dw = new DebugWindow();
        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (_dbfx.Start(_un.Text, _pw.Text, _type.Text, 0))
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

        private void _report_Click(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }
    }
}
