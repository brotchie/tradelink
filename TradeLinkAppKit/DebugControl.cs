using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.AppKit
{
    public partial class DebugControl : UserControl
    {
        bool _timestamp = true;
        public bool TimeStamps { get { return _timestamp; } set { _timestamp = value; } }
        public DebugControl() : this(true) { }
        public DebugControl(bool timestamp)
        {
            InitializeComponent();
        }

        public void GotDebug(Debug msg)
        {
            debug(msg.Msg);
            
        }

        public void GotDebug(string msg)
        {
            debug(msg);
        }

        delegate void stringdel(string msg);
        void debug(string msg)
        {
            if (_msg.InvokeRequired)
                _msg.Invoke(new stringdel(debug), new object[] { msg });
            else
            {
                if (!TimeStamps)
                    _msg.Items.Add(msg);
                else
                    _msg.Items.Add(DateTime.Now.ToString("HHmmss") + ": " + msg);
                _msg.SelectedIndex = _msg.Items.Count - 1;
                _msg.Invalidate(true);
            }
        }

    }
}
