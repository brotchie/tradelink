using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;
using TradeLink.Common;
using TradeLink.AppKit;





namespace GbTLServer
{
    public partial class GBTradeLink : AppTracker
    {
        GrayBox GB;
        public const string PROGRAM = "ServerGraybox";
        Log _log = new Log(PROGRAM);
        
       
        
        public GBTradeLink()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;

            InitializeComponent();
           


            TradeLink.API.TLServer tls;
            if (Properties.Settings.Default.TLClientAddress == string.Empty)
                tls = new TradeLink.Common.TLServer_WM();
            else
                tls = new TradeLink.Common.TLServer_IP(Properties.Settings.Default.TLClientAddress, Properties.Settings.Default.TLClientPort);
            try
            {
            GB = new GrayBox(tls,this.lstStatusList,this.bt_Connect);
            }
            catch (Exception ex)
            {
                const string URL = @"http://code.google.com/p/tradelink/wiki/HoldBrosGrayBox";
                debug("problem connecting to graybox...");
                debug("please check guide at: " + URL);
                System.Diagnostics.Process.Start(URL);
                debug(ex.Message + ex.StackTrace);
            }


            UpdateLoginDetails();
            FormClosing += new FormClosingEventHandler(GBTradeLink_FormClosing);
           

             
            GB.Start();

            this.txtPasword.Focus() ;
            
        }

        void GBTradeLink_FormClosing(object sender, FormClosingEventArgs e)
        {
            _log.Stop();
        }

        void debug(string msg)
        {
            debugControl1.GotDebug(msg);
            _log.GotDebug(msg);
        }

      
        private void bt_Connect_Click(object sender, EventArgs e)
        {
            if (txt_GBLoginName.Text == "")
            {
                MessageBox.Show("Please enter user id");
                return;
            }
            if (txtPasword.Text == "")
            {
                MessageBox.Show("Please enter password");
                return;
            }

            if (GB.GetconnectionStatus() == false)
            {
                MessageBox.Show("Network connection failed. Please check.");
                return;
            }
               this.Cursor = Cursors.WaitCursor;
                string strBookServer = cboBookServer.Text;
            if (strBookServer == "" || strBookServer == ",0" || strBookServer == ",32008")
            {
                strBookServer = "BOOKS1.HOLD.COM,32008";
            }
               bool bConnected = GB.ConnectQuotesServer(txt_GBLoginName.Text, txtPasword.Text, "",  cboQuoteServer.Text, strBookServer);
               this.Cursor = Cursors.Default;

           if (bConnected)
           {
               //bt_Connect.Enabled = false;
               txt_GBLoginName.Enabled = false;
               txtPasword.Enabled = false;
               cboQuoteServer.Enabled = false;
               cboBookServer.Enabled = false;
               }
               //else
               //{
               //    GB.Stop();
               //    Close();
               //}
           
        }

       

        private void btnClose_Click(object sender, EventArgs e)
        {
            GB.Stop();
            Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {

        }

        private void UpdateLoginDetails()
        {
            if ((GB!=null) && (GB.GetGrayboxPath() != null))
            {
                
                txt_GBLoginName.Text = GB.GetLastUserID();
                txtPasword.Text = "";
                cboQuoteServer.Items.AddRange(GB.GetQuotesServer());
                cboBookServer.Items.AddRange(GB.GetBookServer());
                cboQuoteServer.SelectedIndex = 0;
                cboBookServer.SelectedIndex = 0;
            }


        }

        private void PasswordKeyPressed(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                bt_Connect_Click(sender, e);
            }
        }

        private void PasswordKeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.ToString() == Keys.Enter.ToString())
            {
                bt_Connect_Click(sender, e);
            }
        }

        protected void OnClosed(EventArgs e)
        {
            GB.Stop();
            Close();
        }



        
    }
}
