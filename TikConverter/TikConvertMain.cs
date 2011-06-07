using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace TikConverter
{
    public partial class TikConvertMain : AppTracker
    {
        public const string PROGRAM = "TikConverter";
        BackgroundWorker bw = new BackgroundWorker();
        public TikConvertMain()
        {
            TrackEnabled = Util.TrackUsage();
            Program = PROGRAM;
            InitializeComponent();
            _cqgparseoptionsgroupbox.Visible = false;
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
            // see if we're converting from files or webservices
            switch (_conval)
            {
                    // webservice list
                case Converter.EuronextDaily:
                case Converter.YahooDaily:
                case Converter.GoogleDaily:
                    // reset progress
                    progress(0);
                    // get list of symbols from user
                    string symi = Microsoft.VisualBasic.Interaction.InputBox("Enter list of symbols to pull from " + _conval.ToString() + Environment.NewLine + "(eg LVS,GOOG,GE)", "Enter symbol list", string.Empty, 0, 0);
                    // remove spaces and capitalize
                    symi = symi.Replace(" ", string.Empty).ToUpper();
                    // parse
                    string[] syms = symi.Split(',');
                    int count = 0;
                    foreach (string sym in syms)
                    {
                        try
                        {
                            // get barlists for those symbols
                            BarList bl;
                            if (_conval == Converter.GoogleDaily)
                                bl = BarListImpl.DayFromGoogle(sym);
                            else if (_conval == Converter.YahooDaily)
                                bl = BarListImpl.DayFromYahoo(sym);
                            else if (_conval == Converter.EuronextDaily)
                            {
                                if (!System.Text.RegularExpressions.Regex.IsMatch(sym, "[A-Z0-9]{12}", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                                {
                                    debug("\"" + sym + "\" is not a valid ISIN.  Euronext expects ISINs!");
                                    continue;
                                }
                                bl = BarListImpl.DayFromEuronext(sym);
                            }
                            else
                                continue;
                            // convert to tick files
                            if (!TikUtil.TicksToFile(TikUtil.Barlist2Tick(bl), debug))
                                debug("Error saving downloaded bars.");
                            // notify
                            debug("downloaded " + bl.Count + " bars of daily data for " + sym + " from " + _conval.ToString());
                        }
                        catch (Exception ex)
                        {
                            debug(sym+" converter error: " + ex.Message + ex.StackTrace);
                        }
                        // update progress
                        progress((double)count++ / syms.Length);
                    }
                    debug("completed daily download.");
                    // we're done
                    return;
            }
            OpenFileDialog of = new OpenFileDialog();
            // allow selection of multiple inputs
            of.Multiselect = true;
            // keep track of bytes so we can approximate progress
            long bytes = 0;
            if (of.ShowDialog() == DialogResult.OK)
            {
                List<string> symbols = new List<string>();
                foreach (string file in of.FileNames)
                {
                    _path = Path.GetDirectoryName(file);
                    string sn = Path.GetFileName(file);
                    // get size of current file and append to total size
                    FileInfo fi = new FileInfo(file);
                    bytes += fi.Length;
                    string sym = string.Empty;
                    switch (_conval)
                    {
                        case Converter.QCollector_eSignal:
                            string [] r = Path.GetFileNameWithoutExtension(sn).Split('_');
                            if (r.Length != 2)
                            {
                                sym = Microsoft.VisualBasic.Interaction.InputBox("Symbol data represented by file: " + sn, "File's Symbol", string.Empty, 0, 0);
                            }
                            else
                                sym = r[0];
                            break;
                        default:
                            // guess symbol
                            string guess = Util.rxm(sn, "[^a-z]*([a-z]{1,6})[^a-z]+");
                            // remove extension
                            guess = Util.rxr(guess, "[.].*", string.Empty);
                            // see if it's a clean match, if not don't guess
                            if (!Util.rxmok(guess, "^[a-z]+$"))
                                guess = string.Empty;
                            sym = Microsoft.VisualBasic.Interaction.InputBox("Symbol data represented by file: " + sn, "File's Symbol", guess, 0, 0);
                            break;

                    }
                    if (sym != string.Empty)
                        symbols.Add(sym);

                }
                // estimate total ticks
                _approxtotal = (int)((double)bytes / 51);
                // reset progress bar
                progress(0);
                // start background thread to convert
                bw.RunWorkerAsync(new convargs(of.FileNames,symbols.ToArray()));
                debug("started conversion");

            }
        }

        internal class convargs
        {
            
            internal string[] files;
            internal string[] syms;
            internal bool hassyms { get { return (syms.Length == files.Length) && (syms.Length > 0); } }
            internal convargs(string[] filenames)
            {
                files = filenames;
            }
            internal convargs(string[] filenames, string[] symbols)
            {
                files = filenames;
                syms = symbols;
            }
        }
        Converter _conval = Converter.None;
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            convargs ca = (convargs)e.Argument;
            string[] filenames = ca.files;
            bool g = e.Result != null ? (bool)e.Result : true;
            int ds = (int)_defaultsize.Value;
            for (int i = 0; i<filenames.Length; i++)
            {
                string file = filenames[i];
                debug("input file: " + Path.GetFileNameWithoutExtension(file));
                if (!File.Exists(file))
                {
                    debug("file does not exist: " + file);
                    continue;
                }
                bool status = false;
                try
                {
                    // convert file
                    status = ca.hassyms ?
                        convert(_conval, file, ds, ca.syms[i]) :
                        convert(_conval, file, ds);
                }
                catch (Exception ex)
                {
                    debug("Is your file correct formated?  Exception received processing: " + file + " err: " + ex.Message + ex.StackTrace);
                    status = false;
                }

                // report progress
                if (!status) 
                    debug("error converting file: " + file);
                g &= status;
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
        bool convert(Converter con, string filename, int tradesize) { return convert(con, filename, tradesize, string.Empty); }
        bool convert(Converter con, string filename,int tradesize,string sym)
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
            int _date = 0;
            int cqgdecimalplaces = 2;

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
                        cqgdecimalplaces = (int) this._cqgdecimalplacesinput.Value;
                        // no header
                        break;
                    case Converter.TradingPhysicsTnS:
                    case Converter.TradingPhysicsTV:
                        string file = System.IO.Path.GetFileName(filename);
                        string[] date_sym = file.Split('_');
                        string[] sym_ext = date_sym[1].Split('.');
                        string datestr = date_sym[0];
                        int.TryParse(datestr, out _date);
                        _sym = sym_ext[0];
                        infile = new StreamReader(filename);
                        infile.ReadLine();//discard header line 
                        break;
                    case Converter.QCollector_eSignal:
                        infile = new StreamReader(filename);
                        // no header in file
                        break;
                    case Converter.MultiCharts:
                        // The symbol for data being imported is obtained from the filename
                        // Selected files for import must be named SYMBOL.ext - eg AAPL.txt, GOOG.txt
                        _sym = System.IO.Path.GetFileNameWithoutExtension(filename);
                        infile = new StreamReader(filename);
                        infile.ReadLine(); // ignore first line header of input file
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
                            k = CQG.parseline(infile.ReadLine(), tradesize, cqgdecimalplaces );
                            break;
                        case Converter.eSignal_EPF:
                            k = eSigTick.FromStream(_sym, infile);
                            break;
                        case Converter.TradeStation:
                            k = TradeStation.parseline(infile.ReadLine(), sym);
                            break;
                        case Converter.TradingPhysicsTnS:
                            k = TradingPhysicsTnS.parseline(infile.ReadLine(), _sym, _date);
                            break;
                        case Converter.TradingPhysicsTV:
                            k = TradingPhysicsTV.parseline(infile.ReadLine(), _sym, _date);
                            break;
                        case Converter.QCollector_eSignal:
                            k = QCollector.parseline(infile.ReadLine(), sym);
                            break;
                        case Converter.MultiCharts:
                            k = MultiCharts.parseline(infile.ReadLine(), _sym);
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
            if (outfile == null)
            {
                debug("Tick file was never opened, likely that input file in wrong format.");

            }
            else
            {
                debug("Saved: " + outfile.Filepath);
                outfile.Close();
            }
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

            if (_conval == Converter.CQG) _cqgparseoptionsgroupbox.Visible = true;
            else _cqgparseoptionsgroupbox.Visible = false;
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
