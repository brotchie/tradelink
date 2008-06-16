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

        void h_GotTick(Tick t)
        {
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