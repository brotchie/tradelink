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

namespace IQFeedBroker
{
    public partial class IQFeedFrm : Form
    {
        private TLServer_WM _tl;
        private IQFeedHelper _helper;

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
#if DEBUG
            btnDebug.Enabled = true;
#endif
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


        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            _tl = new TLServer_WM();

            string uID = Properties.Settings.Default.UserID;
            string pass = Properties.Settings.Default.Password;           
            _helper = new IQFeedHelper(uID, pass);

            _helper.TickReceived += TickReceived;
            _helper.ConnectToAdmin();
            _helper.ConnectToLevelOne();
            _tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
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
            Init();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Basket basket = new BasketImpl();
            basket.Add("IBM");
            basket.Add("JNJ");
            basket.Add("GOOG");
            AddBasket(basket);
        }
    }
}