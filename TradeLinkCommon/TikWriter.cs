using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// write tradelink tick files
    /// </summary>
    public class TikWriter : BinaryWriter
    {
        bool _hasheader = false;
        string _realsymbol = string.Empty;
        string _file = string.Empty;
        string _path = Environment.CurrentDirectory;
        int _date = 0;
        /// <summary>
        /// real symbol represented by tick file
        /// </summary>
        public string RealSymbol { get { return _realsymbol; } }
        /// <summary>
        /// path of this file
        /// </summary>
        public string Filepath { get { return _file; } }
        /// <summary>
        /// date represented by data
        /// </summary>
        public int Date { get { return _date; } }
        /// <summary>
        /// ticks written
        /// </summary>
        public int Count = 0;
        /// <summary>
        /// creates a tikwriter with no header, header is created from first tik
        /// </summary>
        public TikWriter()
        {

        }
        /// <summary>
        /// create a tikwriter for a specific symbol on todays date.
        /// auto-creates header
        /// </summary>
        /// <param name="realsymbol"></param>
        public TikWriter(string realsymbol) : this(realsymbol, TradeLink.Common.Util.ToTLDate(DateTime.Now)) { }
        /// <summary>
        /// create a tikwriter for specific symbol on specific date
        /// auto-creates header
        /// </summary>
        /// <param name="realsymbol"></param>
        /// <param name="date"></param>
        public TikWriter(string realsymbol, int date) : this(Environment.CurrentDirectory, realsymbol, date) { }
        /// <summary>
        /// create tikwriter with specific location, symbol and date.
        /// auto-creates header
        /// </summary>
        /// <param name="path"></param>
        /// <param name="realsymbol"></param>
        /// <param name="date"></param>
        public TikWriter(string path, string realsymbol, int date) 
        {

            init(realsymbol, date, path);

        }

        private void init(string realsymbol, int date, string path)
        {
            // if file exists, assume it has a header
            _hasheader = File.Exists(_file);

            // store important stuff
            _realsymbol = realsymbol;
            _path = path;
            _date = date;
            // get filename from path and symbol
            _file = SafeFilename(_realsymbol, _path, _date);

            if (!_hasheader)
                Header(this, realsymbol);
            else
                OutStream = new FileStream(_file, FileMode.Open,  FileAccess.Write, FileShare.Read);

        }

        public override void Close()
        {
            base.Close();
        }

        public static string SafeFilename(string realsymbol, string path, int date)
        {
            return path + "\\" + SafeSymbol(realsymbol) + date.ToString() + TikConst.DOT_EXT;
        }

        public static string SafeSymbol(string realsymbol)
        {
            foreach (char c in Path.GetInvalidPathChars())
            {
                int p = 0;
                while (p != -1)
                {
                    p = realsymbol.IndexOf(c);
                    if (p != -1)
                        realsymbol = realsymbol.Remove(p, 1);
                }
            }
            return realsymbol;
        }

        public static bool Header(TikWriter bw, string realsymbol)
        {
            bw.OutStream = new FileStream(bw.Filepath, FileMode.Create, FileAccess.Write, FileShare.Read);
            // version
            bw.Write(TikConst.Version);
            bw.Write(TikConst.VERSION);
            // full symbol name
            bw.Write(realsymbol); // 
            // fields end
            bw.Write(TikConst.StartData);
            // flag header as created
            bw._hasheader = true;
            return true;
        }

        public void newTick(Tick k)
        {
            // make sure we have a header
            if (!_hasheader) init(k.symbol, k.date, _path);
            // get types
            bool t = k.isTrade;
            bool fq = k.isFullQuote;
            bool b = k.hasBid;
            bool a = k.hasAsk;
            bool i = k.isIndex;

            // next we write tick type and the data
            if (!fq && b) // bid only
            {
                Write(TikConst.TickBid);
                Write(k.date);
                Write(k.time);
                Write(k.lbid);
                Write(k.bs);
                Write(k.be);
                Write(k.depth);
            }
            else if (!fq && a) // ask only
            {
                Write(TikConst.TickAsk);
                Write(k.date);
                Write(k.time);
                Write(k.lask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);
            }
            else if ((t && !fq) || i) // trade or index
            {
                Write(TikConst.TickTrade);
                Write(k.date);
                Write(k.time);
                Write(k.ltrade);
                Write(k.size);
                Write(k.ex);
            }
            else if (t && fq) // full quote
            {
                Write(TikConst.TickFull);
                Write(k.date);
                Write(k.time);
                Write(k.ltrade);
                Write(k.size);
                Write(k.ex);
                Write(k.lbid);
                Write(k.bs);
                Write(k.be);
                Write(k.lask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);

            }
            else if (!t && fq) // quote only
            {
                Write(TikConst.TickQuote);
                Write(k.date);
                Write(k.time);
                Write(k.lbid);
                Write(k.bs);
                Write(k.be);
                Write(k.lask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);
            }
            // end tick
            Write(TikConst.EndTick);
            // write to disk
            Flush();
            // count it
            Count++;

        }
    }
}
