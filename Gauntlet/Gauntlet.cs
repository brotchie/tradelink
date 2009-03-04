using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using TradeLink.Common;
using System.IO;
using TradeLink.API;

    namespace WinGauntlet
{
    public partial class Gauntlet : Form
    {
        Response myres;
        BackTest bt;
        public Gauntlet()
        {
            InitializeComponent();
            FindStocks((WinGauntlet.Properties.Settings.Default.tickfolder == null) ? @"c:\program files\tradelink\tickdata\" : WinGauntlet.Properties.Settings.Default.tickfolder);
            string fn = (WinGauntlet.Properties.Settings.Default.boxdll== null) ? "box.dll" : WinGauntlet.Properties.Settings.Default.boxdll;
            if (File.Exists(fn))
                UpdateResponses(Util.GetResponseList(fn));
            exchlist.SelectedItem = "NYS";
            ProgressBar1.Enabled = false;
            FormClosing+=new FormClosingEventHandler(Gauntlet_FormClosing);
            debug(Util.TLSIdentity());
        }


        void FindStocks(string path)
        {
            // build list of available stocks and dates available
            stocklist.Items.Clear();
            yearlist.Items.Clear();
            daylist.Items.Clear();
            monthlist.Items.Clear();
            DirectoryInfo di;
            FileInfo[] fi;

            try
            {
                di = new DirectoryInfo(path);
                fi = di.GetFiles("*.epf");
            }
            catch (Exception ex) { show("exception loading stocks: " + ex.ToString()); return; }

            int[] years = new int[200];
            int[] days = new int[31];
            int[] months = new int[12];
            int yc = 0;
            int dc = 0;
            int mc = 0;

            for (int i = 0; i < fi.Length; i++)
            {
                SecurityImpl s = StockFromFileName(fi[i].Name);
                if (!s.isValid) continue;
                DateTime d = Util.TLD2DT(s.Date);
                if (!stocklist.Items.Contains(s.Symbol))
                    stocklist.Items.Add(s.Symbol);
                if (!contains(d.Year,years))
                    years[yc++] = d.Year;
                if (!contains(d.Month, months))
                    months[mc++] = d.Month;
                if (!contains(d.Day, days))
                    days[dc++] = d.Day;
            }
            Array.Sort(years);
            Array.Sort(days);
            Array.Sort(months);
            for (int i = 0; i<years.Length; i++)
                if (years[i]==0) continue;
                else yearlist.Items.Add(years[i]);
            for (int i = 0; i < months.Length; i++)
                if (months[i] == 0) continue;
                else monthlist.Items.Add(months[i]);
            for (int i = 0; i < days.Length; i++)
                if (days[i] == 0) continue;
                else daylist.Items.Add(days[i]);
        }

        bool contains(int number, int[] array) { for (int i = 0; i < array.Length; i++) if (array[i] == number) return true; return false; }

        

        SecurityImpl StockFromFileName(string filename)
        {
            try
            {
                string ds = System.Text.RegularExpressions.Regex.Match(filename, "([0-9]{8})[.]", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Result("$1");
                string sym = filename.Replace(ds, "").Replace(".EPF", "");
                SecurityImpl s = new SecurityImpl(sym);
                s.Date = Convert.ToInt32(ds);
                return s;
            }
            catch (Exception) { }
            return new SecurityImpl();
            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // tick folder
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Select the folder containing tick files";
            fd.SelectedPath = (WinGauntlet.Properties.Settings.Default.tickfolder == null) ? @"c:\program files\tradelink\tickdata\" : WinGauntlet.Properties.Settings.Default.tickfolder;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                WinGauntlet.Properties.Settings.Default.tickfolder = fd.SelectedPath;
                FindStocks(fd.SelectedPath);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // box DLL selection
            OpenFileDialog of = new OpenFileDialog();
            of.CheckPathExists = true;
            of.CheckFileExists = true;
            of.DefaultExt = ".dll";
            of.Filter = "Box DLL|*.dll";
            of.Multiselect = false;
            if (of.ShowDialog() == DialogResult.OK)
            {
                WinGauntlet.Properties.Settings.Default.boxdll = of.FileName;
                UpdateResponses(Util.GetResponseList(of.FileName));
            }
        }

        void UpdateResponses(List<string> responses)
        {
            reslist.Items.Clear();
            for (int i = 0; i < responses.Count; i++)
                reslist.Items.Add(responses[i]);
        }

        private void messages_DoubleClick(object sender, EventArgs e)
        {
            messages.Clear();
        }

        delegate void ShowCallBack(string msg);
        void debug(string message) { show(message + Environment.NewLine); }
        void show(string message)
        {
            if (messages.InvokeRequired)
                Invoke(new ShowCallBack(show), new object[] { message });
            else
            {
                messages.AppendText(message);
                if (message.Contains(Environment.NewLine)) lastmessage.Text = message.Replace(Environment.NewLine,"");
                else lastmessage.Text = lastmessage.Text + message;
            }
        }

        

        private void queuebut_Click(object sender, EventArgs e)
        {
            bt = new BackTest();
            bt.BTStatus += new DebugFullDelegate(bt_BTStatus);
            bt.mybroker.GotOrder += new OrderDelegate(myres.GotOrder);
            bt.mybroker.GotOrderCancel += new OrderCancelDelegate(mybroker_GotOrderCancel);
            bt.mybroker.GotFill+=new FillDelegate(myres.GotFill);

            List<FileInfo> tf = new List<FileInfo>();
            string dir = WinGauntlet.Properties.Settings.Default.tickfolder;
            if (!Directory.Exists(dir))
            {
                string msg = "You must select a valid tick directory containing historical tick files.";
                MessageBox.Show(msg,"Nonexistent tick directory");
                show(msg);
                return;
            }
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo [] fi = di.GetFiles("*.EPF");
            for (int i = 0; i < fi.Length; i++)
            {
                bool datematch = true;
                bool symmatch = true;
                bool ud = usedates.Checked;
                bool us = usestocks.Checked;
                SecurityImpl s = StockFromFileName(fi[i].Name);
                if (!s.isValid)
                {
                    continue;
                }
                DateTime d = Util.TLD2DT(s.Date);
                if (ud)
                {
                    for (int j = 0; j < yearlist.SelectedItems.Count; j++)
                        if ((int)yearlist.SelectedItems[j] == d.Year) { datematch &= true; break; }
                        else datematch &= false;
                    for (int j = 0; j < monthlist.SelectedItems.Count; j++)
                        if ((int)monthlist.SelectedItems[j] == d.Month) { datematch &= true; break; }
                        else datematch &= false;
                    for (int j = 0; j < daylist.SelectedItems.Count; j++)
                        if ((int)daylist.SelectedItems[j] == d.Day) { datematch &= true; break; }
                        else datematch &= false;
                }
                if (us)
                {
                    for (int j = 0; j < stocklist.SelectedItems.Count; j++)
                        if (fi[i].Name.Contains((string)stocklist.SelectedItems[j])) { symmatch = true; break; }
                        else symmatch = false;
                }


                if ((ud && us && datematch && symmatch) ||
                    (ud && datematch) ||
                    (us && symmatch))
                {
                    debug("added to run: " + fi[i]);
                    tf.Add(fi[i]);
                }
            }
            if (tf.Count == 0)
            {
                string msg = "You didn't select any valid tick files, or none were available.";
                MessageBox.Show(msg, "No tick files selected.");
                show(msg);
                return;
            }

            bt.name = DateTime.Now.ToString("yyyMMdd.HHmm");
            if (myres==null)
            { 
                show("You must select a response to run the gauntlet."); 
                return; 
            } 
            string exfilt = "";
            if ((exchlist.SelectedIndices.Count > 0) && 
                !exchlist.SelectedItem.ToString().Contains("NoFilter")) 
                exfilt = (string)exchlist.SelectedItem;
            bt.ExchFilter(exfilt);
            bt.Path = WinGauntlet.Properties.Settings.Default.tickfolder+"\\";
            bt.ProgressChanged += new ProgressChangedEventHandler(bt_ProgressChanged);
            bt.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bt_RunWorkerCompleted);
            ProgressBar1.Enabled = true;
            queuebut.Enabled = false;
            bt.RunWorkerAsync(new BackTestArgs(tf,myres));
        }

        void bt_BTStatus(Debug debug)
        {
            show(debug.Msg);
        }


        void bt_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string unique = csvnamesunique.Checked ? "."+bt.name : "";
            if (tradesincsv.Checked)
                Util.ClosedPLToText(bt.mybroker.GetTradeList(),',',Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"\\Gauntlet.Trades"+unique+".csv");
            if (ordersincsv.Checked)
            {
                StreamWriter sw = new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gauntlet.Orders" + unique + ".csv", false);
                for (int i = 0; i < bt.mybroker.GetOrderList().Count; i++)
                    sw.WriteLine(OrderImpl.Serialize(bt.mybroker.GetOrderList()[i]));
                sw.Close();
            }
            if (indf!=null)
                indf.Close();
            bt = null;
            
            ProgressBar1.Enabled = false;
            ProgressBar1.Value = 0;
            queuebut.Enabled = true;
        }

        void bt_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar1.Value = e.ProgressPercentage > 100 ? 100 : e.ProgressPercentage;
        }


        private void savesettings_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Save();
        }

        private void Gauntlet_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (saveonexit.Checked) WinGauntlet.Properties.Settings.Default.Save();
        }

        private void usestocks_CheckedChanged(object sender, EventArgs e)
        {
            stocklist.Enabled = !stocklist.Enabled;
        }

        private void usedates_CheckedChanged(object sender, EventArgs e)
        {
            yearlist.Enabled = !yearlist.Enabled;
            monthlist.Enabled = !monthlist.Enabled;
            daylist.Enabled = !daylist.Enabled;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Reset();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            WinGauntlet.Properties.Settings.Default.Reload();
        }

        private void boxlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                myres = ResponseLoader.FromDLL((string)reslist.SelectedItem, WinGauntlet.Properties.Settings.Default.boxdll);
            }
            catch (Exception ex) { show("Response failed to load, quitting... (" + ex.Message + (ex.InnerException != null ? ex.InnerException.Message.ToString() : "") + ")"); }
            if (!myres.isValid) { show("Response did not load or loaded in a shutdown state. "+myres.Name+ " "+myres.FullName); return; }
            myres.SendIndicators += new ObjectArrayDelegate(myres_IndicatorUpdate);
            myres.SendDebug += new DebugFullDelegate(myres_GotDebug);
            myres.SendCancel+= new UIntDelegate(myres_CancelOrderSource);
            myres.SendOrder += new OrderDelegate(myres_SendOrder);
        }

        void myres_SendOrder(Order o)
        {
            if ((bt != null) && (bt.mybroker != null))
                bt.mybroker.sendOrder(o);
        }

        void mybroker_GotOrderCancel(string sym, bool side, uint id)
        {
            if (myres != null)
                myres.GotOrderCancel(id);
        }

        void myres_CancelOrderSource(uint number)
        {
            if ((bt != null) && (bt.mybroker != null))
                bt.mybroker.CancelOrder(number);
            
        }

        void myres_GotDebug(Debug debug)
        {
            if (!showdebug.Checked) return;
            show(debug.Msg);
        }
        StreamWriter indf = null;
        void myres_IndicatorUpdate(object[] parameters)
        {
            if (indicatorscsv.Checked)
            {
                if (indf== null)
                {
                    string unique = csvnamesunique.Checked ? "." + bt.name : "";
                    indf= new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Gauntlet.Indicators" + unique + ".csv", false);
                    indf.WriteLine(string.Join(",", myres.Indicators));
                    indf.AutoFlush = true;
                }
                string[] ivals = new string[parameters.Length];
                for (int i = 0; i < parameters.Length; i++) ivals[i] = parameters[i].ToString();
                indf.WriteLine(string.Join(",", ivals));
            }
        }
    }
}
