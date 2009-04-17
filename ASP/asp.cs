using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;





namespace ASP
{
    public partial class ASP : Form
    {
        StreamWriter sw;
        public ASP()
        {
            InitializeComponent();
            try
            {
                new StreamWriter("ASPDebug." + Util.ToTLDate(DateTime.Now) + ".txt", true);
            }
            catch (Exception ex) { Debug("unable to open log file"); }
            TwitPopup tp = new TwitPopup();
            tp.Show();
            if (sw != null)
                sw.AutoFlush = true;
            tl = new TLClient_WM("ASPclient", true);
            // don't save ticks from replay since they're already saved
            archivetickbox.Checked = tl.LinkType != TLTypes.HISTORICALBROKER;
            tl.gotTick += new TickDelegate(tl_gotTick);
            tl.gotFill += new FillDelegate(tl_gotFill);
            tl.gotOrder += new OrderDelegate(tl_gotOrder);
            tl.gotOrderCancel += new UIntDelegate(tl_gotOrderCancel);
            boxcriteria.ContextMenu = new ContextMenu();
            boxcriteria.ContextMenu.MenuItems.Add("Activate/Shutdown", new EventHandler(toggleresponse));
            this.FormClosing += new FormClosingEventHandler(ASP_FormClosing);
            status(Util.TLSIdentity());
            Util.ExistsNewVersions(tl);
            LoadResponseDLL(Properties.Settings.Default.boxdll);
        }

        void toggleresponse(object sender, EventArgs e)
        {
            int selbox = boxcriteria.SelectedIndex;
            reslist[selbox].isValid = !reslist[selbox].isValid;
            if (!reslist[selbox].isValid)
                status("Response " + reslist[selbox].Name + " is off.");
            else
                status("Response " + reslist[selbox].Name + " is on.");            
        }

        void tl_gotOrderCancel(uint number)
        {
            foreach (string sym in symidx.Keys)
                foreach (int idx in symidx[sym])
                    if (reslist[idx].isValid)
                        reslist[idx].GotOrderCancel(number);
        }

        void tl_gotOrder(Order o)
        {
            int[] idxs = new int[0];
            if (!symidx.TryGetValue(o.Sec.FullName, out idxs))
                return;
            foreach (int idx in idxs)
                if (reslist[idx].isValid)
                    reslist[idx].GotOrder(o);
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
                if (reslist[idx].isValid) 
                    reslist[idx].GotTick(t);
        }

        private int count = 0;

        void tl_gotFill(Trade t)
        {
            count++;
            int[] idxs = new int[0];
            if (!symidx.TryGetValue(t.Sec.FullName, out idxs))
                return;
            foreach (int idx in idxs)
                if (reslist[idx].isValid)
                    reslist[idx].GotFill(t);
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


        BasketImpl mb = new BasketImpl();
        Response workingres = new InvalidResponse();

        // name of dll of response names

        void LoadResponseDLL(string filename)
        {
            if (!System.IO.File.Exists(filename))
            {
                status("file does not exist: " + filename);
                return;
            }

            Properties.Settings.Default.boxdll = filename;

            List<string> list = Util.GetResponseList(filename);
            Responses.Items.Clear();
            foreach (string res in list)
            {
                Responses.Items.Add(res);
            }
        }

        private void LoadDLL_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.DefaultExt = ".dll";
            of.Filter = "Response DLL|*.dll|All Files|*.*";
            of.Multiselect = false;
            if(of.ShowDialog() == DialogResult.OK) 
            {
                LoadResponseDLL(of.FileName);

            }

            reslist.Clear();
        }

        TLClient_WM tl;


        private void Boxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            string resname = (string)Responses.SelectedItem;
            workingres = ResponseLoader.FromDLL(resname, Properties.Settings.Default.boxdll);
            workingres.SendOrder += new OrderDelegate(workingbox_SendOrder);
            workingres.SendDebug+= new DebugFullDelegate(workingres_GotDebug);
            workingres.SendCancel+= new UIntDelegate(workingres_CancelOrderSource);
        }

        void workingbox_SendOrder(Order o)
        {
            o.Account = _account.Text;
            o.Security = seclist[o.symbol].Type;
            o.Exchange = seclist[o.symbol].DestEx;
            o.LocalSymbol = o.symbol;
            int res = tl.SendOrder(o);
            if (res != (int)MessageTypes.OK)
                Debug(Util.PrettyError(tl.BrokerName, res) + " " + o.ToString());
        }

        void workingres_CancelOrderSource(uint number)
        {
            tl.CancelOrder((long)number);
        }

        void workingres_GotDebug(Debug debug)
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
                SecurityImpl sec = SecurityImpl.Parse(sym);
                if (!sec.isValid)
                {
                    status("Security invalid: " + sec.ToString());
                    return;
                }
                if (Responses.SelectedIndex == -1)
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
            lock (reslist) // potentially used by other threads
            {
                reslist.Add(workingres);
            }
            int idx = reslist.Count -1;
            tl.Subscribe(mb);
            boxcriteria.Items.Add(workingres.Name+" ["+string.Join(",",valid.ToArray())+"]");
            foreach (SecurityImpl sec in seclist.Values)
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
            Responses.SelectedIndex = -1;
            
        }

        Dictionary<string,SecurityImpl> seclist = new Dictionary<string,SecurityImpl>();
        Dictionary<string, int[]> symidx = new Dictionary<string, int[]>();
        List<Response> reslist = new List<Response>();
        
        

        private void boxcriteria_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selbox = boxcriteria.SelectedIndex;
            if (!reslist[selbox].isValid)
                status("Resonse " + reslist[selbox].Name + " is off.");
            else
                status("Response " + reslist[selbox].Name +  " is on.");

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

        private void _togglemsgs_Click(object sender, EventArgs e)
        {
            listBox1.Visible = !listBox1.Visible;
        }                                            
    }
}