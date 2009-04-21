using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// archive ticks as they arrive.   Once archived, ticks can be replayed, viewed or analyzed in any of the tradelink programs.
    /// </summary>
    public class TickArchiver
    {
        string _path;
        System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();

        Dictionary<string,StreamWriter> filedict = new Dictionary<string,StreamWriter>();
        public TickArchiver() : this(Util.TLTickDir,60) { }
        public TickArchiver(string folderpath, int FlushAfterSeconds)
        {
            _path = folderpath;
            _timer.Interval = 1000 * FlushAfterSeconds;
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }
        public void CloseArchive()
        {
            foreach (string file in filedict.Keys)
                filedict[file].Close();
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            foreach (string sym in filedict.Keys)
                filedict[sym].Flush();
        }

        bool SaveTick(Tick t)
        {
            if ((t.symbol==null) || (t.symbol=="")) return false;
            if (filedict.ContainsKey(t.symbol))
            {
                try 
                {
                   filedict[t.symbol].WriteLine(eSigTick.ToEPF(t));
                }
                catch (IOException) { return false; }
            }
            else
            {
                string fn = _path + @"/" + t.symbol + t.date + ".EPF";
                bool hasheader = false;
                try 
                {
                    if (File.Exists(fn))
                    {
                        StreamReader sr = new StreamReader(fn);
                        SecurityImpl s = eSigTick.InitEpf(sr);
                        if (s.isValid)
                            hasheader = true;
                        sr.Close();
                    }
                    filedict.Add(t.symbol, new StreamWriter(fn, true));
                    if (!hasheader)
                        filedict[t.symbol].Write(eSigTick.EPFheader(t.symbol, t.date));
                    filedict[t.symbol].WriteLine(eSigTick.ToEPF(t));
                }
                catch (IOException) { return false; }
                catch (Exception) { return false; }
            }

            return true;
        }


        public bool Save(Tick t) { return SaveTick(t); }

    }
}
