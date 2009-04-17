using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.Common
{
    public partial class TwitPopup : Form
    {
        public TwitPopup()
        {
            InitializeComponent();
            Show();
        }
        public static void Twit() { TwitPopup tp = new TwitPopup(); }
    }
}
