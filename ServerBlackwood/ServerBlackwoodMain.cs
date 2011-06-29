using System;
using System.Drawing;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;

namespace ServerBlackwood
{
    public partial class ServerBlackwoodMain : AppTracker
    {
        public const string PROGRAM = "ServerBlackwood";
        public DebugWindow _dw = new DebugWindow();
        
        private delegate void DisplayStatusHandler(bool bConnected);
        private DisplayStatusHandler StatusHandler;

        ServerBlackwood _con ;

        public ServerBlackwoodMain()
        {
            TLServer tl;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tl = new TLServer_WM();
            else
                tl = new TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            _con = new ServerBlackwood(tl);
            _con.VerbuseDebugging = Properties.Settings.Default.VerboseDebugging;
            
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            _con.BWConnectedEvent += new BWConnectedEventHandler(_con_BWConnectedEvent);
            _con.SendDebug += new DebugFullDelegate(_dw.GotDebug);

            StatusHandler = new DisplayStatusHandler(DisplayStatus);

            Location = Properties.Settings.Default.wlocation;
            
            //Avoid painting window off screen.
            Point screen = new Point (SystemInformation.VirtualScreen.Bottom,SystemInformation.VirtualScreen.Right) ;
            if (this.Top > screen.X - this.Height | this.Left > screen.Y - this.Width) 
            {
                this.Location = new Point(300, 300);
            }

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
                    _dw.GotDebug("logging in...");
                    Invalidate();
                }
                else
                { 
                    BackColor = Color.Red;
                    _dw.GotDebug("Problem connecting to Blackwood!");
                }
            }
            else
            {
                _dw.GotDebug("logging off...");
                _con.Stop();
            }
        }

        private void _togmsg_Click(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            CrashReport.Report(PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        private void ServerBlackwoodMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.wlocation = this.Location ;
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
