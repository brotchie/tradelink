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
using IESignal;


namespace ServerEsignal
{
    public partial class EsignalMain : AppTracker
    {

        EsignalServer tl;
        public const string PROGRAM = "ServerEsignal";
        Log _log = new Log(PROGRAM);
        
        public EsignalMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            tl = new EsignalServer();
            // send debug messages to log file
            tl.GotDebug += new DebugFullDelegate(tl_GotDebug);
            // attempt to connect to esignal
            _ok_Click(null, null);
            // handle connector exits
            FormClosing += new FormClosingEventHandler(EsignalMain_FormClosing);
        }

        void tl_GotDebug(Debug debug)
        {
            _log.GotDebug(debug);
        }

        void EsignalMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            tl.Stop();
            _log.Stop();
        }


        // attempt to connect to esignal
        private void _ok_Click(object sender, EventArgs e)
        {
            if (_acctapp.Text == string.Empty) return;
            tl.Start(_acctapp.Text,null,null,0);
            if (tl.isValid) { BackColor = Color.Green; Invalidate(true); }
        }
    }
}
