using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;

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


        private void symbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                recordbut_Click(null, null);
        }

        void refreshlist(int size)
        {
            if (size != (mb.Count + ib.Count))
            {
                if (ib.Count>0) tl.RegIndex(ib);
                if (mb.Count>0) tl.Subscribe(mb);

                stockslist.Items.Clear();
                for (int i = 0; i < mb.Count; i++)
                    stockslist.Items.Add(mb[i].Name);
                for (int i = 0; i < ib.Count; i++)
                    stockslist.Items.Add(ib[i].Name);
            }
        }

        private void recordbut_Click(object sender, EventArgs e)
        {
            int size = mb.Count + ib.Count;
            Security sec = Security.Parse(symbox.Text);
            if (sec.Type == SecurityType.IDX)
                ib.Add(new Index(symbox.Text));
            else
                mb.Add(new Stock(symbox.Text));
            refreshlist(size);

        }

    }
}