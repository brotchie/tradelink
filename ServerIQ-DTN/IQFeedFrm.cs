using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace IQFeedBroker
{
    public partial class IQFeedFrm : Form
    {
        private TLServer_WM _tl;
        private IQFeedHelper _helper;
        DebugWindow _dw = new DebugWindow();

        public bool IsConnected
        {
            get
            {
                return _helper.IsConnected;
            }
        }


        public IQFeedFrm()
        {
            InitializeComponent();
            this.FormClosing += IQFeedFrm_FormClosing;
            _helper = new IQFeedHelper();
            _helper.SendDebug += new DebugDelegate(_dw.GotDebug);
            _tl = new TLServer_WM();

            _helper.TickReceived += TickReceived;
            _helper.ConnectToAdmin();
            _helper.ConnectToLevelOne();
            _tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="basket"></param>
        public void AddBasket(Basket basket)
        {
            _helper.AddBasket(basket);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="security"></param>
        public void AddSecurity(Security security)
        {
            _helper.AddSecurity(security);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IQFeedFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();

            _helper.Close();
            this.Dispose();
        }




        private void tl_newRegisterStocks(string msg)
        {
            AddBasket(BasketImpl.Deserialize(msg));
        }


        private void TickReceived(Tick t)
        {
            _tl.newTick(t);
        }


        private void IQFeedFrm_Load(object sender, EventArgs e)
        {
            // auto login if username and password are already present
            if ((_user.Text != string.Empty) && (_pass.Text != string.Empty))
                _login_Click(null, null);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // toggle log message viewing
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            // report bugs
            CrashReport.Report(Global.PROGRAM_NAME, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        private void _login_Click(object sender, EventArgs e)
        {
            _helper.Start(_user.Text, _pass.Text);
            BackColor = _helper.IsConnected ? Color.Green : Color.Red;
        }
    }
}