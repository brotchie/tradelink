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

            if (boxlist.ContainsKey(t.sym) && (boxlist[t.sym] != null))
                boxlist[t.sym].GotTick(t);
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
        List<Response> boxes = new List<Response>();

        Response workingbox = new InvalidResponse();

        // name of dll of box names

        private void LoadDLL_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = ".dll";
            of.Filter = "BoxDLL|*.dll";
            of.Multiselect = false;
            if(of.ShowDialog() == DialogResult.OK) 
            {
                boxdll = of.FileName;

                List <string> list = Util.GetBoxList(boxdll);
                Boxes.Items.Clear();
                foreach (string box in list)
                {
                    Boxes.Items.Add(box);
                }

            }

            boxlist.Clear();
            boxes.Clear();
        }

        TradeLink_Client_WM tl;


        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string boxname = (string)Boxes.SelectedItem;
            workingbox = ResponseLoader.FromDLL(boxname, boxdll);
            workingbox.SendOrder += new OrderDelegate(workingbox_SendOrder);
            workingbox.SendDebug+= new DebugFullDelegate(workingbox_GotDebug);
            workingbox.SendCancel+= new UIntDelegate(workingbox_CancelOrderSource);
            tl.gotOrder+=new OrderDelegate(workingbox.GotOrder);
            tl.gotOrderCancel+=new UIntDelegate(workingbox.GotOrderCancel);
            tl.gotFill += new FillDelegate(workingbox.GotFill);
        }

        void workingbox_SendOrder(Order o)
        {
            o.Security = seclist[o.symbol].Type;
            o.Exchange = seclist[o.symbol].DestEx;
            o.LocalSymbol = seclist[o.symbol].Name;
            tl.SendOrder(o);
        }

        void workingbox_CancelOrderSource(uint number)
        {
            tl.CancelOrder((long)number);
        }

        void workingbox_GotDebug(Debug debug)
        {
            if (!debugon.Checked) return;
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
            Security sec = Security.Parse(sym);
            string shortsym = sec.Symbol;
            string boxcrit = "Box on " + sym + " with " + (string)Boxes.SelectedItem;
            if (boxlist.ContainsKey(shortsym))
            {

                if (MessageBox.Show("For safety, you're only allowed one box per symbol.  Would you like to replace existing box?", "Confirm box replace", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
                {
                    status("flatting symbol " + shortsym);
                    tl.SendOrder(new MarketOrder(shortsym, poslist[shortsym].Size * -1));
                    string name = boxlist[shortsym].Name;
                    boxlist.Remove(shortsym);
                    status("Removed strategy " + name + " from " + shortsym);
                }
                else
                {
                    status("Box activation canceled because of too many boxes.");
                    return;

                }
            }
            mb.Add(new Stock(sym));
            tl.Subscribe(mb);
            boxcriteria.Items.Add(workingbox.Name+" ["+shortsym+"]");
            boxlist.Add(shortsym,workingbox);
            seclist.Add(shortsym, sec);
            boxes.Add(workingbox);
            

            stock.Text = "";
            status("");
            Boxes.SelectedIndex = -1;
            
        }

        Dictionary<string, Security> seclist = new Dictionary<string, Security>();


        Dictionary<string, Response> boxlist = new Dictionary<string, Response>();
        Dictionary<string, BarList> barlist = new Dictionary<string, BarList>(); 
        Dictionary<string, Position> poslist = new Dictionary<string, Position>();
        

        private void boxcriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selbox = boxcriteria.SelectedIndex;
            if (!boxes[selbox].isValid)
                status("Box " + boxes[selbox].Name + " is off.");
            else
                status("Box " + boxes[selbox].Name +  " is on.");

        }

        private void status(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { msg });
            else 
                toolStripStatusLabel1.Text = msg;
        }

        private void fillstatus(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(fillstatus), new object[] { msg});
            else 
                toolStripStatusLabel2.Text = msg;
        }



        private void stock_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                Trade_Click(null, null);
        }                                            
    }
}