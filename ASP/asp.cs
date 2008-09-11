using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;





namespace ASP
{
    public partial class ASP : Form
    {
        public ASP()
        {
            InitializeComponent();
            tl = new TradeLink_Client_WM("ASPclient", true);
            // don't save ticks from replay since they're already saved
            archivetickbox.Checked = tl.LinkType != TLTypes.HISTORICALBROKER;
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            this.FormClosing += new FormClosingEventHandler(ASP_FormClosing);
            status(Util.TLSIdentity());
        }

        void ASP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tl != null)
                tl.Disconnect();
            ta.CloseArchive();
        }

        TickArchiver ta = new TickArchiver();


       
        void tl_gotTick(Tick t)
        {

            if (archivetickbox.Checked)
                ta.Save(t);
            if (!barlist.ContainsKey(t.sym)) barlist.Add(t.sym, new BarList(BarInterval.FiveMin, t.sym));
            else barlist[t.sym].newTick(t);

            if (tradeboxlist.ContainsKey(t.sym) && (tradeboxlist[t.sym] != null))
            {

                Box b = tradeboxlist[t.sym];
                BoxInfo bi = new BoxInfo();
                Position p = new Position(t.sym);
                try
                {
                    p = poslist[t.sym];
                }
                catch (KeyNotFoundException) { }

                    
                Order o = b.Trade(t, barlist[t.sym], p, bi);
                tl.SendOrder(o);
            }
        }

        private int count = 0;

        void tl_gotFill(Trade t)
        {
            if(debugon.Checked)
                Debug(t.ToString());

            count++;
            if (!poslist.ContainsKey(t.symbol)) poslist.Add(t.symbol, new Position(t.symbol));
            poslist[t.symbol].Adjust(t);
            fillstatus("Fills: " + count);
        }


        void Debug(string message)
        {
            if (listBox1.InvokeRequired)
                listBox1.Invoke(new DebugDelegate(Debug), new object[] { message });
            else
                listBox1.Items.Add(message);
        }


        MarketBasket mb = new MarketBasket();

        
        string boxdll;
        List<Box> boxes = new List<Box>();

        Box workingbox = new Box();

        // name of dll of box names

        private void LoadDLL_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = ".dll";
            of.Filter = "BoxDLL|*.dll";
            of.Multiselect = false;
            if(of.ShowDialog() == DialogResult.OK) {
                boxdll = of.FileName;

                List <string> boxlist = Util.GetBoxList(boxdll);
                Boxes.Items.Clear();
                foreach (string box in boxlist)
                {
                    Boxes.Items.Add(box);
                }
                }

                tradeboxlist.Clear();
                boxes.Clear();
        }

        TradeLink_Client_WM tl;


        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string boxname = (string)Boxes.SelectedItem;
            workingbox = Box.FromDLL(boxname, boxdll);
            workingbox.GotDebug += new DebugFullDelegate(workingbox_GotDebug);
            workingbox.CancelOrderSource += new UIntDelegate(workingbox_CancelOrderSource);
            tl.gotOrder+=new OrderDelegate(workingbox.gotOrderSink);
            tl.gotOrderCancel+=new UIntDelegate(workingbox.gotCancelSink);
        }

        void workingbox_CancelOrderSource(uint number)
        {
            tl.CancelOrder((long)number);
        }

        void workingbox_GotDebug(Debug debug)
        {
            Debug(debug.Msg);
        }

        private void Trade_Click(object sender, EventArgs e)
        {
            string sym = stock.Text.ToUpper();
            if (!Stock.isStock(sym))
            {
                status("Please input a stock symbol.");
                return;
            }
            if (Boxes.SelectedIndex == -1)
            {
                status("Please select a box.");
                return;
            }

            
            mb.Add(new Stock(sym));
            tl.Subscribe(mb);
            string boxcrit = "Box on " + sym + " with " + (string)Boxes.SelectedItem;
            workingbox.Symbol = sym;
            boxcriteria.Items.Add(workingbox);
            tradeboxlist.Add(stock.Text, workingbox);
            boxes.Add(workingbox);
            

            stock.Text = "";
            status("");
            Boxes.SelectedIndex = -1;
            
        }

        
        Dictionary<string, Box> tradeboxlist = new Dictionary<string, Box>();
        Dictionary<string, BarList> barlist = new Dictionary<string, BarList>(); 
        Dictionary<string, Position> poslist = new Dictionary<string, Position>();
        

        private void boxcriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selbox = boxcriteria.SelectedIndex;
            if (boxes[selbox].Off)
                status("Box " + boxes[selbox].Name + " (" + boxes[selbox].Symbol + ")" + " is off.");
            else
                status("Box " + boxes[selbox].Name + " (" + boxes[selbox].Symbol + ")" + " is on.");

        }

        private void status(string status)
        {
            toolStripStatusLabel1.Text = status;
        }

        private void fillstatus(string fillstatus)
        {
            toolStripStatusLabel2.Text = fillstatus;
        }

        private void shutdown_Click(object sender, EventArgs e)
        {
            if (boxcriteria.SelectedIndex >= 0)
            {
                int selbox = boxcriteria.SelectedIndex;
                boxes[selbox].Shutdown("shutdown requested by user.");
                status("Box " + boxes[selbox].Name + " (" + boxes[selbox].Symbol + ")" + " shutdown.");
            }
            else
                status("select a box to shutdown");
        }

        private void activate_Click(object sender, EventArgs e)
        {
            if (boxcriteria.SelectedIndex >= 0)
            {
                int selbox = boxcriteria.SelectedIndex;
                boxes[selbox].Activate("activate requested by user.");
                status("Box " + boxes[selbox].Name + " (" + boxes[selbox].Symbol + ")" + " activated.");
            }
            else
                status("select a box to activate");
        }                                            
    }
}