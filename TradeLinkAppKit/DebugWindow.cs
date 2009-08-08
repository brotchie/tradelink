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
        public DebugWindow()
        {
            InitializeComponent();

        }
        public string Content { get { return _msg.ToString(); } }
        StringBuilder _msg = new StringBuilder();
        public bool isValid = true;
        public bool TimeStamps { get { return debugControl1.TimeStamps; } set { debugControl1.TimeStamps = value; } }

        public void GotDebug(TradeLink.API.Debug deb)
        {
            _msg.AppendLine(deb.Msg);
            debugControl1.GotDebug(deb);
        }

        public void GotDebug(string msg)
        {
            _msg.AppendLine(msg);
            debugControl1.GotDebug(msg);
        }

        public void Toggle()
        {
            Visible = !Visible;
            Invalidate(true);
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
