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

        Dictionary<string, TikWriter> filedict = new Dictionary<string, TikWriter>();
        public TickArchiver() : this(Util.TLTickDir) { }
        public TickArchiver(string folderpath)
        {
            _path = folderpath;
        }
        public void Stop()
        {
            foreach (string file in filedict.Keys)
                filedict[file].Close();
        }

        public bool newTick(Tick t)
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
                    tw.newTick(t);
                    filedict.Add(t.symbol, tw);
                }
                catch (IOException) { return false; }
                catch (Exception) { return false; }
            }

            return true;
        }

    }
}
