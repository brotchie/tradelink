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

namespace ServerMB
{
    public partial class ServerMBMain : Form
    {
        TLServer_WM tl = new TLServer_WM();
        public ServerMBMain()
        {
            InitializeComponent();
            tl.newProviderName = Providers.MBTrading;
            tl.newFeatureRequest += new MessageArrayDelegate(tl_newFeatureRequest);
            tl.newSendOrderRequest += new OrderDelegate(tl_newSendOrderRequest);
            tl.newRegisterStocks += new DebugDelegate(tl_newRegisterStocks);
            tl.newOrderCancelRequest += new UIntDelegate(tl_newOrderCancelRequest);
            tl.newAcctRequest += new StringDelegate(tl_newAcctRequest);
        }

        string tl_newAcctRequest()
        {
            throw new NotImplementedException();
        }

        void tl_newOrderCancelRequest(uint number)
        {
            throw new NotImplementedException();
        }

        void tl_newRegisterStocks(string msg)
        {
            throw new NotImplementedException();
        }

        void tl_newSendOrderRequest(Order o)
        {
            throw new NotImplementedException();
        }

        MessageTypes[] tl_newFeatureRequest()
        {
            List<MessageTypes> f = new List<MessageTypes>();
            f.Add(MessageTypes.FEATUREREQUEST);
            f.Add(MessageTypes.FEATURERESPONSE);
            f.Add(MessageTypes.BROKERNAME);
            return f.ToArray();
        }

        void debug(string msg)
        {
            if (_msg.InvokeRequired)
                Invoke(new DebugDelegate(debug), new object[] { msg });
            else
            {
                _msg.Items.Add(DateTime.Now.ToShortTimeString() + " " + msg);
                _msg.SelectedIndex = _msg.Items.Count - 1;
            }
        }
    }
}
