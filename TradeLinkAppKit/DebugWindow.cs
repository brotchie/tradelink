using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.AppKit
{
    public partial class DebugWindow : Form
    {
        string preface = string.Empty;
        public DebugWindow()
        {
            InitializeComponent();
            debugControl1.NewSearchEvent += new TradeLink.API.DebugDelegate(debugControl1_NewSearchEvent);
            debugControl1.NewCreateTicketEvent += new TradeLink.API.DebugDelegate(debugControl1_NewCreateTicketEvent);
        }

        void debugControl1_NewCreateTicketEvent(string msg)
        {
            if (NewCreateTicketEvent != null)
                NewCreateTicketEvent(msg);
        }

        public void BeginUpdate()
        {
            debugControl1.BeginUpdate();
        }

        public void EndUpdate()
        {
            debugControl1.EndUpdate();
        }

        bool _useexttime = false;
        /// <summary>
        /// toggle whether an external time stamp is used (timestamps must be enabled)
        /// </summary>
        public bool UseExternalTimeStamp { get { return debugControl1.UseExternalTimeStamp; } set { debugControl1.UseExternalTimeStamp = value; } }
        int _exttime = 0;
        /// <summary>
        /// set an external time stamp
        /// </summary>
        public int ExternalTimeStamp { get { return debugControl1.ExternalTimeStamp; } set { debugControl1.ExternalTimeStamp = value; } }


        public event TradeLink.API.DebugDelegate NewCreateTicketEvent;

        void debugControl1_NewSearchEvent(string msg)
        {
            if (preface == string.Empty)
                preface = Text;
            Text = preface + " " + msg;
            Invalidate(true);
        }
        public string Content { get { return debugControl1.Content; } }

        public bool isValid = true;
        public bool TimeStamps { get { return debugControl1.TimeStamps; } set { debugControl1.TimeStamps = value; } }

        public void GotDebug(TradeLink.API.Debug deb)
        {
            debugControl1.GotDebug(deb);
        }

        public void GotDebug(string msg)
        {
            debugControl1.GotDebug(msg);
        }

        public void Toggle()
        {
            Visible = !Visible;
            Invalidate(true);
        }

        public void Toggle(object sender, EventArgs e)
        {
            Toggle();
        }

        public void Clear()
        {
            debugControl1.Clear();
        }

        private void DebugWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            // hide window
            Toggle();
            // cancel the close
            e.Cancel = true;
        }


    }
}
