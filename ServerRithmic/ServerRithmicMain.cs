using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.AppKit;
using TradeLink.Common;

namespace ServerRithmic
{
    public partial class ServerRithmicMain : Form
    {
        public const string PROGRAM = "RithmicConnector";
        Log _log = new Log(PROGRAM);
        DebugWindow dw = new DebugWindow();

        ServerRithmic sr;

        public ServerRithmicMain()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(ServerRithmicMain_FormClosing);

            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
            {
                tls = new TradeLink.Common.TLServer_WM();
            }
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            sr = new ServerRithmic(tls, debug);
            sr.AdmPt = Properties.Settings.Default.sAdmCnnctPt;
            sr.TsConnectPt = Properties.Settings.Default.sTsCnnctPt;
            sr.MarketDataPt = Properties.Settings.Default.sMdCnnctPt;

            if ((user.Text!=string.Empty) && (pass.Text!=string.Empty))
                go();
        }

        void ServerRithmicMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Stop();
            if (sr != null)
                sr.Stop();
            Properties.Settings.Default.Save();
        }

        void debug(string msg)
        {
            dw.GotDebug(msg);
            _log.GotDebug(msg);
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            dw.Toggle();
        }

        void go()
        {
            if (sr.Start(user.Text, pass.Text))
                BackColor = Color.Green;
            else
                BackColor = Color.Red;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            go();
        }
    }
}
