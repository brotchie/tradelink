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

        Dictionary<string, TikWriter> filedict = new Dictionary<string, TikWriter>();
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
            TikWriter tw;
            if (filedict.TryGetValue(t.symbol, out tw))
            {
                try 
                {
                    tw.newTick(t);
                }
                catch (IOException) { return false; }
            }
            else
            {
                try 
                {
                    tw = new TikWriter(_path, t.symbol, t.date);
                }
                catch (IOException) { return false; }
                catch (Exception) { return false; }
            }

            return true;
        }


        public bool Save(Tick t) { return SaveTick(t); }

    }
}
