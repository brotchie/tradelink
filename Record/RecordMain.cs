using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using ResearchLib;

namespace Record
{
    public partial class RecordMain : Form
    {
        TickArchiver ta = new TickArchiver();
        TickWatcher tw = new TickWatcher();
        TradeLink_Client_WM tl = new TradeLink_Client_WM();
        MarketBasket mb = new MarketBasket();
        IndexBasket ib = new IndexBasket();

        public RecordMain()
        {
            InitializeComponent();
            if ((tl.LinkType == TLTypes.LIVEBROKER) || (tl.LinkType == TLTypes.SIMBROKER))
            {
                tw.Alerted += new StockDelegate(tw_Alerted);
                tl.gotTick += new TickDelegate(tl_gotTick);
                tl.gotIndexTick += new IndexDelegate(tl_gotIndexTick);
            }
        }

        void tl_gotIndexTick(Index idx)
        {
            ta.Save(idx);
        }

        void tl_gotTick(Tick t)
        {
            ta.Save(t);
        }

        void tw_Alerted(Stock stock)
        {
            if (emailbox.Text != "")
                Email.Send(emailbox.Text, emailbox.Text, "TickDelay " + stock.Symbol, "No ticks received for " + tw.DefaultWait + " seconds." + stock.ToString());
        }

        private void symbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                int size = mb.Count + ib.Count;
                if (Stock.isStock(symbox.Text))
                    mb.Add(new Stock(symbox.Text));
                else if (Index.isIdx(symbox.Text))
                    ib.Add(new Index(symbox.Text));
                refreshlist(size);
            }
        }

        void refreshlist(int size)
        {
            if (size != (mb.Count + ib.Count))
            {
                tl.RegIndex(ib);
                tl.Subscribe(mb);

                stockslist.Items.Clear();
                for (int i = 0; i < mb.Count; i++)
                    stockslist.Items.Add(mb[i].Name);
                for (int i = 0; i < ib.Count; i++)
                    stockslist.Items.Add(ib[i].Name);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                string t = Clipboard.GetText();
                int size = mb.Count + ib.Count;
                mb.Add(ParseStocks.NYSE(t));
                mb.Add(ParseStocks.NASDAQ(t));
                refreshlist(size);
            }
        }
    }
}