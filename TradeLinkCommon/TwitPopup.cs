using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.Common
{
    /// <summary>
    /// popup window that allows users to communicate with other tradleink users via twitter in real time.
    /// </summary>
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
