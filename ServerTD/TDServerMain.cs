using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using AMTD_API;

namespace TDServer
{
    public partial class TDServerMain : Form
    {
        AmeritradeBrokerAPI api = new AmeritradeBrokerAPI();
        TLServer_WM tl = new TLServer_WM(TLTypes.LIVEBROKER);
        public TDServerMain()
        {
            InitializeComponent();
        }
    }
}
