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

 



namespace TradeLinkTest
{
    public partial class GBTradeLink : AppTracker
    {
        GrayBox GB;
        public const string PROGRAM = "Graybox Server";
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

            GB = new GrayBox(tls,this.lstStatusList);


            UpdateLoginDetails();
            
           

             
            

            
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

               this.Cursor = Cursors.WaitCursor;
               bool bConnected = GB.ConnectQuotesServer(txt_GBLoginName.Text, txtPasword.Text, "",  cboQuoteServer.Text, cboBookServer.Text);
               this.Cursor = Cursors.Default;

           if (bConnected)
           {
               bt_Connect.Enabled = false;
               txt_GBLoginName.Enabled = false;
               txtPasword.Enabled = false;
               cboQuoteServer.Enabled = false;
               cboBookServer.Enabled = false;
           }
           
        }

       

        private void btnClose_Click(object sender, EventArgs e)
        {
            GB.DisconnectServer();
            Close();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {

        }

        private void UpdateLoginDetails()
        {
            if (GB.GetGrayboxPath() != null)
            {
                
                txt_GBLoginName.Text = GB.GetLastUserID();
                txtPasword.Text = "";
                cboQuoteServer.Items.AddRange(GB.GetQuotesServer());
                cboBookServer.Items.AddRange(GB.GetBookServer());
                cboQuoteServer.SelectedIndex = 0;
                cboBookServer.SelectedIndex = 0;
            }


        }

        
    }
}
