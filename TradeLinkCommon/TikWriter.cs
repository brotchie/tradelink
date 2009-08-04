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
        bool hasheader = false;
        string _realsymbol = string.Empty;
        public TikWriter(string sym) : this(sym, TradeLink.Common.Util.ToTLDate(DateTime.Now)) { }
        public TikWriter(string realsymbol, int date) : this(Environment.CurrentDirectory, realsymbol, date) { }
        public TikWriter(string path, string realsymbol, int date) 
        {
            _realsymbol = realsymbol;
            string filename = path + "//" + SafeSymbol(realsymbol) + date.ToString() + TikConst.DOT_EXT;
            hasheader = File.Exists(filename);
            OutStream = new FileStream(filename, FileMode.OpenOrCreate);
            if (!hasheader)
                Header(this,realsymbol);
        }

        public override void Close()
        {
            base.Close();
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

        public static bool Header(BinaryWriter bw, string realsymbol)
        {
            bw.Write(TikConst.Version);
            bw.Write(TikConst.VERSION);
            // fields follow
            bw.Write(realsymbol); // real symbol 
            // fields end
            bw.Write(TikConst.StartData);
            return true;
        }

        public void newTick(Tick k)
        {
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
                Write(k.ibid);
                Write(k.bs);
                Write(k.be);
                Write(k.depth);
            }
            else if (!fq && a) // ask only
            {
                Write(TikConst.TickAsk);
                Write(k.date);
                Write(k.time);
                Write(k.iask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);
            }
            else if ((t && !fq) || i) // trade or index
            {
                Write(TikConst.TickTrade);
                Write(k.date);
                Write(k.time);
                Write(k.itrade);
                Write(k.size);
                Write(k.ex);
            }
            else if (t && fq) // full quote
            {
                Write(TikConst.TickFull);
                Write(k.date);
                Write(k.time);
                Write(k.itrade);
                Write(k.size);
                Write(k.ex);
                Write(k.ibid);
                Write(k.bs);
                Write(k.be);
                Write(k.iask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);

            }
            else if (!t && fq) // quote only
            {
                Write(TikConst.TickQuote);
                Write(k.date);
                Write(k.time);
                Write(k.ibid);
                Write(k.bs);
                Write(k.be);
                Write(k.iask);
                Write(k.os);
                Write(k.oe);
                Write(k.depth);
            }
            // end tick
            Write(TikConst.EndTick);
            // write to disk
            Flush();
        }
    }
}
