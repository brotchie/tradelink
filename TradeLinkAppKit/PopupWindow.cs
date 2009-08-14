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
        public PopupWindow()
        {
            InitializeComponent();
            
        }

        delegate void stringdel(string s);
        string _m = string.Empty;
        void GotDebug(string msg)
        {
            if (InvokeRequired)
                Invoke(new stringdel(GotDebug), new object[] { msg });
            {
                _msg.Text = msg;
                _msg.Invalidate();
            }
        }

        public void Title(string title)
        {
            if (InvokeRequired)
                Invoke(new stringdel(Title),new object[] { title} );
            else
            {
                Text = title;
                Invalidate(true);
            }
        }

        public static void Show(string msg) { Show("Notice", msg, true,false); }
        public static void Show(string title, string msg) { Show(title,msg, true,false); }
        public static void Show(string title,string msg, bool topmost, bool pause)
        {
            PopupWindow tow = new PopupWindow();

            tow.TopMost = topmost;
            
            tow.Title(title);
            tow.GotDebug(msg);
            if (pause)
                tow.ShowDialog();
            else
                tow.Show();
        }
    }
}
