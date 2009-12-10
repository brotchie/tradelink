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
        
        private delegate void DisplayStatusHandler(bool bConnected);
        private DisplayStatusHandler StatusHandler;

        ServerBlackwood _con = new ServerBlackwood();

        public ServerBlackwoodMain()
        {
            InitializeComponent();
            _con.BWConnectedEvent += new BWConnectedEventHandler(_con_BWConnectedEvent);
            _con.SendDebug+=new DebugFullDelegate(_dw.GotDebug);

            StatusHandler = new DisplayStatusHandler(DisplayStatus);
        }

        void _con_BWConnectedEvent(object sender, bool BWConnected)
        {
            object[] args = { BWConnected };
            this.BeginInvoke(StatusHandler, args);
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (this._login.Text == "login")
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
            else
            {
                _con.Stop();
                _dw.GotDebug("logoff successful.");
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

        private void ServerBlackwoodMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            _con.Stop();
        }

        private void DisplayStatus(bool bConnected)
        {
            if (bConnected)
            {
                this.BackColor = Color.Green ;
                this._login.Text = "logoff";
                this._ipaddress.Enabled = false;
                this._un.Enabled = false;
                this._pw.Enabled = false;
            }
            else
            {
                this.BackColor = Color.Red;
                this._login.Text = "login";
                this._ipaddress.Enabled = true;
                this._un.Enabled = true;
                this._pw.Enabled = true;
            }
        }
    }
}
