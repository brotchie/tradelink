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
    public partial class PopupWindow : Form
    {
        public PopupWindow() : this("Notice") { }
        public PopupWindow(string caption)
        {
            Text = caption;
            InitializeComponent();
            
        }

        delegate void stringdel(string s);
        void GotDebug(string msg)
        {
            if (InvokeRequired)
                Invoke(new stringdel(GotDebug), new object[] { msg });
            {
                _msg.Text = msg;
                _msg.Invalidate();
            }
        }

        public static void Show(string msg) { Show("Notice", msg, true); }
        public static void Show(string title, string msg) { Show(title,msg, true); }
        public static void Show(string title,string msg, bool topmost)
        {
            PopupWindow tow = new PopupWindow(title);
            tow.TopMost = topmost;
            tow.Show();
            tow.GotDebug(msg);
        }
    }
}
