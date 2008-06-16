using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLib;
using System.IO;

namespace Replay
{
    public partial class Replay : Form
    {
        TradeLink_Server_WM tl = new TradeLink_Server_WM(TLTypes.HISTORICALBROKER);
        Playback _playback = null;
        HistSim h = null;
        string tickfolder = Util.TLTickDir;

        public Replay()
        {
            InitializeComponent();
            tl.gotSrvFillRequest += new OrderDelegate(tl_gotSrvFillRequest);
            tl.PositionPriceRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_PositionPriceRequest);
            tl.PositionSizeRequest += new TradeLink_Server_WM.IntStringDelegate(tl_PositionSizeRequest);
            tl.DayHighRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_DayHighRequest);
            tl.DayLowRequest += new TradeLink_Server_WM.DecimalStringDelegate(tl_DayLowRequest);
        }

        decimal tl_DayLowRequest(string s)
        {
            decimal price = 0;
            lows.TryGetValue(s, out price);
            return price;
        }

        decimal tl_DayHighRequest(string s)
        {
            decimal price = 0;
            highs.TryGetValue(s, out price);
            return price;
        }

        int tl_PositionSizeRequest(string s)
        {
            if (h.SimBroker != null)
                return h.SimBroker.GetOpenPosition(s).Size;
            return 0;
        }

        decimal tl_PositionPriceRequest(string s)
        {
            if (h.SimBroker != null)
                return h.SimBroker.GetOpenPosition(s).AvgPrice;
            return 0;
        }


        private void inputbut_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.Description = "Choose folder containing tick or index archive files...";
            if (fd.ShowDialog() == DialogResult.OK)
            {
                tickfolder = fd.SelectedPath;
            }

        }

        void status(string msg)
        {
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new DebugDelegate(status), new object[] { msg });
            else
                statuslab.Text = msg;
        }

        private void playbut_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(tickfolder))
            {
                status("Tick folder " + tickfolder + " doesn't exist,  stopping.");
                return;
            }
            highs = new Dictionary<string, decimal>();
            lows = new Dictionary<string, decimal>();
            TickFileFilter tff = new TickFileFilter();
            tff.DateFilter(Util.ToTLDate(monthCalendar1.SelectionEnd),DateMatchType.Day|DateMatchType.Month|DateMatchType.Year);
            h = new HistSim(tickfolder, tff);
            h.GotTick += new TickDelegate(h_GotTick);
            h.GotIndex += new IndexDelegate(h_GotIndex);
            h.SimBroker.GotOrder += new OrderDelegate(SimBroker_GotOrder);
            h.SimBroker.GotFill += new FillDelegate(SimBroker_GotFill);
            _playback = new Playback(h);
            _playback.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_playback_RunWorkerCompleted);
            _playback.ProgressChanged+=new ProgressChangedEventHandler(_playback_ProgressChanged);
            _playback.RunWorkerAsync(new PlayBackArgs((int)trackBar1.Value/5,daystartpicker.Value));
            status("Playback started...");
            playbut.Enabled = false;
            stopbut.Enabled = true;
            trackBar1.Enabled = false;
        }

        void tl_gotSrvFillRequest(Order o)
        {
            // pass tradelink fill requests through to the histsim broker
            // (if histsim has been started)
            if (h!=null)
                h.SimBroker.sendOrder(o);
        }

        void _playback_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressbar.Value = e.ProgressPercentage;
            status("Playing... " + e.ProgressPercentage + "%");
        }

        void _playback_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                status("Playback was canceled.");
            else if (e.Error != null)
                status("Playback stopped: " + e.Error.ToString());
            else
                status("Playback completed successfully");
            progressbar.Value = 0;
        }

        void SimBroker_GotFill(Trade t)
        {
            tl.newFill(t);
        }

        void SimBroker_GotOrder(Order o)
        {
            tl.newOrder(o);
        }

        void h_GotIndex(Index idx)
        {
            tl.newIndexTick(idx);
        }

        Dictionary<string, decimal> highs = new Dictionary<string, decimal>();
        Dictionary<string, decimal> lows = new Dictionary<string, decimal>();
        void h_GotTick(Tick t)
        {
            if (t.isTrade)
            {
                decimal price = 0;
                if (highs.TryGetValue(t.sym, out price))
                {
                    if (t.trade > price)
                        highs[t.sym] = t.trade;
                }
                else highs.Add(t.sym, t.trade);
                if (lows.TryGetValue(t.sym, out price))
                {
                    if (t.trade < price)
                        lows[t.sym] = t.trade;
                }
                else lows.Add(t.sym, t.trade);
            }

            tl.newTick(t);
        }

        private void stopbut_Click(object sender, EventArgs e)
        {
            _playback.CancelAsync();
            status("Cancel requested.");
            stopbut.Enabled = false;
            playbut.Enabled = true;
            trackBar1.Enabled = true;
        }


    }
}