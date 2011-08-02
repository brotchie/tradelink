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

        public ServerRithmicMain()
        {
            InitializeComponent();
            dw.Parent = this;
            FormClosing += new FormClosingEventHandler(ServerRithmicMain_FormClosing);
        }

        void ServerRithmicMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Stop();
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
    }
}
