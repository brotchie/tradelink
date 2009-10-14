using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using TradeLink.API;

namespace TikConverter
{
    public partial class TikConvertMain : Form
    {
        public const string PROGRAM = "TikConverter";
        BackgroundWorker bw = new BackgroundWorker();
        public TikConvertMain()
        {
            InitializeComponent();
            _con.Items.AddRange(Enum.GetNames(typeof(Converter)));
            bw.WorkerReportsProgress = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }




        Dictionary<string, string> _filesyms = new Dictionary<string, string>();
        string _path = string.Empty;
        private void _inputbut_Click(object sender, EventArgs e)
        {
            // make sure we only convert one group at a time
            if (bw.IsBusy) { debug("wait until conversion completes..."); return; }
            OpenFileDialog of = new OpenFileDialog();
            // allow selection of multiple inputs
            of.Multiselect = true;
            // keep track of bytes so we can approximate progress
            long bytes = 0;
            if (of.ShowDialog() == DialogResult.OK)
            {
                bool g = true;
                foreach (string file in of.FileNames)
                {
                    _path = Path.GetDirectoryName(file);
                    // get size of current file and append to total size
                    FileInfo fi = new FileInfo(file);
                    bytes += fi.Length;
                }
                // estimate total ticks
                _approxtotal = (int)((double)bytes / 51);
                // reset progress bar
                progress(0);
                // start background thread to convert
                bw.RunWorkerAsync(of.FileNames);
                debug("started conversion");

            }
        }
        Converter _conval = Converter.None;
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] filenames = (string[])e.Argument;
            bool g = e.Result != null ? (bool)e.Result : true;
            foreach (string file in filenames)
            {
                debug("input file: " + Path.GetFileNameWithoutExtension(file));
                // convert file
                bool fg = convert(_conval,file, (int)_defaultsize.Value);
                // report progress
                if (!fg) debug("error converting file: " + file);
                g &= fg;
            }
            e.Result = g;

        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // status back to user
            bool g = (bool)e.Result;
            debug("processed ticks: " + _ticksprocessed.ToString("N0"));
            if (g) debug("converted files successfully.");
            else debug("errors converting files.");
            // reset progress bar
            progress(0);
            // reset ticks processed
            _ticksprocessed = 0;
        }

        int _ticksprocessed = 0;
        int _approxtotal = 0;
        string _sym;
        bool convert(Converter con, string filename,int tradesize)
        {
            int bads = 0;
            int thistotal = _ticksprocessed;
            bool g = true;
            // get output filename
            string convertname = string.Empty;
            // setup writing to output
            TikWriter outfile = null;
            // setup input file
            StreamReader infile = null;
            try
            {
                // open input file
                switch (con)
                {
                    case Converter.TradeStation:
                        infile = new StreamReader(filename);
                        // read in and ignore header of input file
                        infile.ReadLine();
                        break;
                    case Converter.eSignal_EPF:
                        infile = new StreamReader(filename);
                        // ignore header
                        SecurityImpl esec = eSigTick.InitEpf(infile);
                        _sym = esec.Symbol;
                        break;
                    case Converter.CQG:
                        infile = new StreamReader(filename);
                        // no header
                        break;
                }

            }
            catch (Exception ex) { debug("error reading input header:" + ex.Message); g = false; }
            // setup previous tick and current tick
            Tick pk = new TickImpl();
            Tick k = null;
            do
            {
                try
                {
                    // get next tick from the file

                    switch (con)
                    {
                        case Converter.CQG:
                            k = CQG.parseline(infile.ReadLine(), tradesize);
                            break;
                        case Converter.eSignal_EPF:
                            k = eSigTick.FromStream(_sym, infile);
                            break;
                        case Converter.TradeStation:
                            k = TradeStation.parseline(infile.ReadLine(), _sym, tradesize);
                            break;
                    }
                }
                catch (Exception ex) { bads++;  continue; }
                if (k == null)
                {
                    debug("Invalid converter: " + con.ToString());
                    return false;
                }
                // bad tick
                if (k.date == 0) { bads++; continue; }
                // if dates don't match, we need to write new output file
                if (k.date != pk.date)
                {
                    try
                    {
                    // if the outfile was open previously, close it
                    if (outfile != null) 
                        outfile.Close();
                        // get path from input
                        string path = Path.GetDirectoryName(filename) + "\\";
                        // setup new file
                        outfile = new TikWriter(path,k.symbol, k.date);
                        // report progress
                        progress((double)_ticksprocessed / _approxtotal);
                    }
                    catch (Exception ex) { debug(ex.Message); g = false; }
                }
                try
                {
                    // write the tick
                    outfile.newTick(k);
                    // save this tick as previous tick
                    pk = k;
                    // count the tick as processed
                    _ticksprocessed++;
                }
                catch (Exception ex) { debug("error writing output tick: " + ex.Message); g = false; }

            }
            // keep going until input file is exhausted
            while (!infile.EndOfStream);
            // close output file
            outfile.Close();
            // close input file
            infile.Close();
            // get percentage of good ticks
            double goodratio = (_ticksprocessed - thistotal - bads) / (_ticksprocessed - (double)thistotal);
            if (goodratio < GOODRATIO)
            {
                debug("Less than " + GOODRATIO.ToString("P0") + " good ticks");
                g = false;
            }
            // return status
            return g;
        }

        const double GOODRATIO = .95;


        delegate void pdouble(double p);
        void progress(double percent)
        {
            int p = (int)(percent * 100);
            if (p > 100) p = 100;
            if (p < 0) p = 0;
            // if being called from a background thread, 
            // invoke UI thread to update screen
            if (statusStrip1.InvokeRequired)
                statusStrip1.Invoke(new pdouble(progress), new object[] { percent });
            else
            {
                _progress.Value = p;
                _progress.Invalidate();
            }
        }

        void debug(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(debug), new object[] { msg });
            else
            {
                msg = DateTime.Now.ToShortTimeString() + " " + msg;
                _msg.Items.Add(msg);
                _msg.SelectedIndex = _msg.Items.Count - 1;
            }

        }

        private void _con_SelectedIndexChanged(object sender, EventArgs e)
        {
            // get converter
            _conval = (Converter)Enum.Parse(typeof(Converter), _con.Text, true);
            // esignal convert does not use default size
            _defaultsize.Enabled = _conval != Converter.eSignal_EPF;
            _defaultsize.Invalidate();
                

        }

        private void TikConvertMain_SizeChanged(object sender, EventArgs e)
        {
            int neww = (int)(ClientRectangle.Width * delta);
            if (neww != 0)
                _msg.Width = neww;
            int newh = (int)(ClientRectangle.Height - hdelta);
            if (newh != 0)
                _msg.Height = newh - (int)(_con.Height * 1.5);
            Invalidate(true);
        }
        double hdelta, delta;

        private void TikConvertMain_Load(object sender, EventArgs e)
        {
            if (_msg.Height != 0)
                hdelta = (double)ClientRectangle.Height - _msg.Height;
            delta = (double)_msg.Width / ClientRectangle.Width;
        }


    }
}
