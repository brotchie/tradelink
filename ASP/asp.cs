using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using System.IO;





namespace ASP
{
    public partial class ASP : Form
    {
        StreamWriter sw = new StreamWriter("ASPDebug." + Util.ToTLDate(DateTime.Now) + ".txt", true);
        public ASP()
        {
            InitializeComponent();
            if (sw != null)
                sw.AutoFlush = true;
            tl = new TLClient_WM("ASPclient", true);
            // don't save ticks from replay since they're already saved
            archivetickbox.Checked = tl.LinkType != TLTypes.HISTORICALBROKER;
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotOrder += new OrderDelegate(tl_gotOrder);
            tl.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            this.FormClosing += new FormClosingEventHandler(ASP_FormClosing);
            status(Util.TLSIdentity());
            LoadBoxDLL(Properties.Settings.Default.boxdll);
        }

        void tl_gotOrderCancel(uint number)
        {
            foreach (string sym in symidx.Keys)
                foreach (int idx in symidx[sym])
                    boxlist[idx].GotOrderCancel(number);
        }

        void tl_gotOrder(Order o)
        {
            int[] idxs = new int[0];
            if (!symidx.TryGetValue(o.Sec.FullName, out idxs))
                return;
            foreach (int idx in idxs)
                boxlist[idx].GotOrder(o);
        }

        void ASP_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (sw != null)
            {
                sw.Close();
                sw = null;
            }
            Properties.Settings.Default.Save();
            if (tl != null)
                tl.Disconnect();
            ta.CloseArchive();
        }

        TickArchiver ta = new TickArchiver();




       
        void tl_gotTick(Tick t)
        {

            if (archivetickbox.Checked)
                ta.Save(t);

            int[] idxs = new int[0];
            if (!symidx.TryGetValue(t.Sec.FullName, out idxs))
                return;
            foreach (int idx in idxs)
                boxlist[idx].GotTick(t);
        }

        private int count = 0;

        void tl_gotFill(Trade t)
        {
            count++;
            int[] idxs = new int[0];
            if (!symidx.TryGetValue(t.Sec.FullName, out idxs))
                return;
            foreach (int idx in idxs)
                boxlist[idx].GotFill(t);
        }


        void Debug(string message)
        {
            if (listBox1.InvokeRequired)
                listBox1.Invoke(new DebugDelegate(Debug), new object[] { message });
            else
            {
                listBox1.Items.Add(message);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }


        MarketBasket mb = new MarketBasket();
        Response workingbox = new InvalidResponse();

        // name of dll of box names

        void LoadBoxDLL(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                status("file does not exist: " + filename);
                return;
            }

            Properties.Settings.Default.boxdll = filename;

            List<string> list = Util.GetBoxList(filename);
            Boxes.Items.Clear();
            foreach (string box in list)
            {
                Boxes.Items.Add(box);
            }
        }

        private void LoadDLL_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = ".dll";
            of.Filter = "BoxDLL|*.dll";
            of.Multiselect = false;
            if(of.ShowDialog() == DialogResult.OK) 
            {
                LoadBoxDLL(of.FileName);

            }

            boxlist.Clear();
        }

        TLClient_WM tl;


        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string boxname = (string)Boxes.SelectedItem;
            workingbox = ResponseLoader.FromDLL(boxname, Properties.Settings.Default.boxdll);
            workingbox.SendOrder += new OrderDelegate(workingbox_SendOrder);
            workingbox.SendDebug+= new DebugFullDelegate(workingbox_GotDebug);
            workingbox.SendCancel+= new UIntDelegate(workingbox_CancelOrderSource);
        }

        void workingbox_SendOrder(Order o)
        {
            o.Security = seclist[o.symbol].Type;
            o.Exchange = seclist[o.symbol].DestEx;
            o.LocalSymbol = o.symbol;
            tl.SendOrder(o);
        }

        void workingbox_CancelOrderSource(uint number)
        {
            tl.CancelOrder((long)number);
        }

        void workingbox_GotDebug(Debug debug)
        {
            if (!debugon.Checked) return;
            if (sw != null)
                sw.WriteLine(debug.Msg);
            Debug(debug.Msg);
        }

        private void Trade_Click(object sender, EventArgs e)
        {
            string[] syms = stock.Text.Split(',');
            List<string> valid = new List<string>();
            foreach (string symt in syms)
            {
                string sym = symt.ToUpper();
                Security sec = Security.Parse(sym);
                if (!sec.isValid)
                {
                    status("Security invalid: " + sec.ToString());
                    return;
                }
                if (Boxes.SelectedIndex == -1)
                {
                    status("Please select a box.");
                    return;
                }
                mb.Add(sec);
                valid.Add(sec.Symbol);
                if (!seclist.ContainsKey(sec.Symbol))
                {
                    lock (seclist) // potentially used by other threads, so we lock it
                    {
                        seclist.Add(sec.Symbol,sec);
                    }
                }

            }
            lock (boxlist) // potentially used by other threads
            {
                boxlist.Add(workingbox);
            }
            int idx = boxlist.Count -1;
            tl.Subscribe(mb);
            boxcriteria.Items.Add(workingbox.Name+" ["+string.Join(",",valid.ToArray())+"]");
            foreach (Security sec in seclist.Values)
                if (symidx.ContainsKey(sec.FullName))
                {
                    // add this boxes' index to subscriptions for this symbol
                    int len = symidx[sec.FullName].Length;
                    int[] a = new int[len + 1];
                    a[len] = idx;
                }
                else symidx.Add(sec.FullName, new int[] { idx });
            stock.Text = "";
            status("");
            Boxes.SelectedIndex = -1;
            
        }

        Dictionary<string,Security> seclist = new Dictionary<string,Security>();
        Dictionary<string, int[]> symidx = new Dictionary<string, int[]>();
        List<Response> boxlist = new List<Response>();
        
        

        private void boxcriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selbox = boxcriteria.SelectedIndex;
            if (!boxlist[selbox].isValid)
                status("Box " + boxlist[selbox].Name + " is off.");
            else
                status("Box " + boxlist[selbox].Name +  " is on.");

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