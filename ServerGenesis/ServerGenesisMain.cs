using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace ServerGenesis
{
    public partial class ServerGenesisMain : Form
    {


        public const string PROGRAM = "GenesisServer-BETA";
        Log _log = new Log(PROGRAM);
        public DebugWindow _dw = new DebugWindow();

        GenesisServer gs = new GenesisServer();

        public ServerGenesisMain()
        {
            InitializeComponent();
            //
            this.ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("messages", new EventHandler(togmessage));

            FormClosing += new FormClosingEventHandler(ServerGenesisMain_FormClosing);


        }

        void togmessage(object sender, EventArgs e)
        {
            _dw.Toggle();
        }

        
        void ServerGenesisMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            gs.Stop();
            _log.Stop();
        }

        private void _login_Click(object sender, EventArgs e)
        {
            gs.Start(_user.Text, _pass.Text, string.Empty, 0);
            if (gs.isValid)
            {
                debug("logged in: " + _user.Text);
                BackColor = Color.Green;
                Invalidate(true);
            }
            else
                debug("login failed: " + _user.Text);


        }

        void debug(string msg)
        {
            _dw.GotDebug(msg);
            _log.GotDebug(msg);
        }


    }
}
