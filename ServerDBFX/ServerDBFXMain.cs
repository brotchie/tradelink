using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.AppKit;

namespace ServerDBFX
{
    public partial class ServerDBFXMain : Form
    {
        ServerDBFX _dbfx = new ServerDBFX();

        public ServerDBFXMain()
        {
            InitializeComponent();
            _dbfx.SendDebug += new TradeLink.API.DebugFullDelegate(_dw.GotDebug);
            FormClosing += new FormClosingEventHandler(ServerDBFXMain_FormClosing);
        }


        void ServerDBFXMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _dbfx.Stop();
        }

        DebugWindow _dw = new DebugWindow();
        private void _togmsg_Click(object sender, EventArgs e)
        {

            _dw.Toggle();
        }

        private void _login_Click(object sender, EventArgs e)
        {
            if (_dbfx.Start(_un.Text, _pw.Text, "Demo", 0))
            {
                BackColor = Color.Green;
                _dw.GotDebug("login successful");
                Invalidate();
            }
            else
            {
                _dw.GotDebug("login failed.");
            }
        }
    }
}
