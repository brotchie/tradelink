using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TradeLib
{
    /// <summary>
    /// Marshalls eSignal-specific tickdata into and out of TradeLink's generic Tick type.
    /// </summary>
    public class eSigTick : Tick
    {
        // eSignal record order definitions for quotes and trades
        enum Q { TYPE, DATE, TIME, BID, ASK, BIDSIZE, ASKSIZE, BIDEX, ASKEX };
        enum T { TYPE, DATE, TIME, PRICE, SIZE, EXCH };
        const string TRADE = "T";
        const string QUOTE = "Q";
        public eSigTick() : base() {}
        public eSigTick(Tick t) : base(t) { }

        /// <summary>
        /// Loads a tick straight from an EPF file in the form of a StreamReader
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="sr">The sr.</param>
        /// <returns></returns>
        public static eSigTick FromStream(string symbol, System.IO.StreamReader sr)
        {
            eSigTick e = new eSigTick();
            e.Load(sr.ReadLine());
            e.sym = symbol;
            return e;
        }
        public static eSigTick FromStream(System.IO.StreamReader sr)
        { return FromStream("",sr); }


        /// <summary>
        /// Loads the specified tickfile from a line(string).
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>true if worked, false if not or if EOF</returns>
        /// <exception cref="AlreadySmallSizeException"></exception>
        public bool Load(string line)
        {
            if ((line != null) && (line != ""))
            {
                try
                {
                    string[] r = line.Split(',');
                    this.time = TickTime(r[(int)Q.TIME]);
                    this.date = TickDate(r[(int)Q.DATE]);
                    this.sec = TickSec(r[(int)Q.TIME]);
                    // we divide trade-size by 100 to match bid and offer size units (hundreds of shares)
                    if (r[(int)Q.TYPE] == TRADE)
                    {
                        int isize = Convert.ToInt32(r[(int)T.SIZE]);
                        
                        this.SetTrade(date, time, sec, Convert.ToDecimal(r[(int)T.PRICE]), isize, r[(int)T.EXCH]);
                    }
                    else this.SetQuote(date, time, sec, Convert.ToDecimal(r[(int)Q.BID]), Convert.ToDecimal(r[(int)Q.ASK]), Convert.ToInt32(r[(int)Q.BIDSIZE]), Convert.ToInt32(r[(int)Q.ASKSIZE]), r[(int)Q.BIDEX], r[(int)Q.ASKEX]);
                }
                catch (Exception ex) { string s = ex.Message; return false; }
                return true;
            }
            return false;
        }

        private int TickSec(string ticktime)
        {
            string t = ticktime.Substring(4, 2);
            return Convert.ToInt32(t);
        }

        private int TickTime(string ticktime)
        {
            string t = ticktime.Substring(0, 4);
            return Convert.ToInt32(t);
        }

        private int TickDate(string tickdate)
        {
            int yr = Convert.ToInt32(tickdate.Substring(0, 2));
            int mo = Convert.ToInt32(tickdate.Substring(2, 2));
            int dy = Convert.ToInt32(tickdate.Substring(4, 2));
            yr += 2000;
            yr *= 10000;
            mo *= 100;
            yr += mo + dy;
            return yr;
        }
        static string epfdate(int d) { string s = d.ToString();  return s.Substring(2); }
        static string epftime(int time, int sec) { int num = time * 100 + sec; string s = num.ToString();  return s.PadLeft(6, '0'); }

        /// <summary>
        /// Converts the tick to a string-equivalent that can be written to an EPF file.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static string ToEPF(Tick t)
        {
            string s = "";
            if (!t.isTrade) s = "Q," + epfdate(t.date) + "," + epftime(t.time, t.sec) + "," + t.bid + "," + t.ask + "," + t.bs + "," + t.os + "," + t.be + "," + t.oe;
            else s = "T," + epfdate(t.date) + "," + epftime(t.time, t.sec) + "," + t.trade + "," + (t.size) + "," + t.ex;
            return s;
        }
        /// <summary>
        /// Create an epf file header.
        /// </summary>
        /// <param name="stock">The stock.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string EPFheader(string stock, int date)
        {
            string s = "";
            s += "; Symbol=" + stock + Environment.NewLine;
            s += "; Date=" + date.ToString() + Environment.NewLine;
            return s;
        }
        /// <summary>
        /// Create an epf file header.
        /// </summary>
        /// <param name="s">The stock.</param>
        /// <returns></returns>
        public static string EPFheader(Stock s)
        {
        	return EPFheader(s.Symbol,s.Date);
        }

        /// <summary>
        /// Initilize the reading of an EPF file and return the header as a Stock object.
        /// </summary>
        /// <param name="EPFfile">The EP ffile.</param>
        /// <returns></returns>
        public static Stock InitEpf(StreamReader EPFfile) 
        {
            StreamReader cf = EPFfile; 
            string symline = cf.ReadLine();
            string dateline = cf.ReadLine();
            Regex se = new Regex("=[a-z-A-Z]+");
            Regex dse = new Regex(@"; Date=([0-9]+)/([0-9]+)/([0-9]+).*$");
            MatchCollection r = se.Matches(symline, 0);
            string t = r[0].Value;
            string symbol = t.Substring(1, t.Length - 1);
            Stock s = new Stock(symbol.ToUpper());
            string date = dateline.Contains("/") ? dse.Replace(dateline, "20$3$1$2") : Regex.Match(dateline, "[0-9]+").ToString();
            s.Date = Convert.ToInt32(date);
            return s;
        }
    }

    public class AlreadySmallSizeException : Exception 
    
    {
    }

}
