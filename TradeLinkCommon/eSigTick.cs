using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TradeLink.API;

namespace TradeLink.Common
{
    /// <summary>
    /// Marshalls eSignal-specific tickdata into and out of TradeLink's generic Tick type.
    /// </summary>
    public static class eSigTick
    {
        // eSignal record order definitions for quotes and trades
        enum Q { TYPE, DATE, TIME, BID, ASK, BIDSIZE, ASKSIZE, BIDEX, ASKEX };
        enum T { TYPE, DATE, TIME, PRICE, SIZE, EXCH };
        const string TRADE = "T";
        const string QUOTE = "Q";

        /// <summary>
        /// Loads a tick straight from an EPF file in the form of a StreamReader
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="sr">The sr.</param>
        /// <returns></returns>
        public static Tick FromStream(string symbol,StreamReader sr)
        {
            TickImpl t = new TickImpl();
            string line = "";
            try
            {
                line = sr.ReadLine();
            }
            catch (Exception) { return t; }
            string[] r = line.Split(',');
            if (r.Length < 6) return t;
            decimal td = 0;
            int ti = 0;

            if (r[(int)Q.TYPE] == TRADE)
            {
                if (decimal.TryParse(r[(int)T.PRICE], out td))
                    t.trade = td;
                if (int.TryParse(r[(int)T.SIZE], out ti))
                    t.size = ti;
                t.ex = r[(int)T.EXCH];
            }
            else
            {
                if (r.Length < 9) return t;
                if (decimal.TryParse(r[(int)Q.BID], out td))
                    t.bid = td;
                if (decimal.TryParse(r[(int)Q.ASK], out td))
                    t.ask = td;
                if (int.TryParse(r[(int)Q.BIDSIZE], out ti))
                    t.bs = ti;
                if (int.TryParse(r[(int)Q.ASKSIZE], out ti))
                    t.os = ti;
                t.be = r[(int)Q.BIDEX];
                t.oe = r[(int)Q.ASKEX];
            }
            t.symbol = symbol;
            if (int.TryParse(r[(int)Q.TIME], out ti))
            {
                t.time = ti;
            }
            if (int.TryParse(r[(int)Q.DATE], out ti))
                t.date = ti + 20000000;
            t.datetime = ((long)t.date * 1000000) + (long)t.time;
            return t;
        }

        static string epfdate(int d) { string s = d.ToString();  return s.Substring(2); }
        static string epftime(int time) { string s = time.ToString();  return s.PadLeft(6, '0'); }

        /// <summary>
        /// Converts the tick to a string-equivalent that can be written to an EPF file.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static string ToEPF(Tick t)
        {
            string s = "";
            if (!t.isTrade) s = "Q," + epfdate(t.date) + "," + epftime(t.time) + "," + t.bid + "," + t.ask + "," + t.bs + "," + t.os + "," + t.be + "," + t.oe;
            else s = "T," + epfdate(t.date) + "," + epftime(t.time) + "," + t.trade + "," + (t.size) + "," + t.ex;
            return s;
        }
        /// <summary>
        /// Create an epf file header.
        /// </summary>
        /// <param name="stock">The securities symbol</param>
        /// <param name="date">The date</param>
        /// <returns></returns>
        public static string EPFheader(string symbol, int date)
        {
            string s = "";
            s += "; Symbol=" + symbol + Environment.NewLine;
            s += "; Date=" + date.ToString() + Environment.NewLine;
            return s;
        }
        /// <summary>
        /// Create an epf file header
        /// </summary>
        /// <param name="sec">The security</param>
        /// <returns></returns>
        public static string EPFheader(SecurityImpl sec)
        {
        	return EPFheader(sec.Symbol,sec.Date);
        }

        /// <summary>
        /// Initilize the reading of an EPF file and return the header as a Stock object.
        /// </summary>
        /// <param name="EPFfile">The EP ffile.</param>
        /// <returns></returns>
        public static SecurityImpl InitEpf(StreamReader EPFfile) 
        {
            StreamReader cf = EPFfile; 
            string symline = cf.ReadLine();
            string dateline = cf.ReadLine();
            Regex se = new Regex("=[$^a-z-A-Z0-9]+");
            Regex dse = new Regex(@"; Date=([0-9]+)/([0-9]+)/([0-9]+).*$");
            MatchCollection r = se.Matches(symline, 0);
            string t = r[0].Value;
            string symbol = t.Substring(1, t.Length - 1);
            SecurityImpl s = SecurityImpl.Parse(symbol.ToUpper());
            string date = dateline.Contains("/") ? dse.Replace(dateline, "20$3$1$2") : Regex.Match(dateline, "[0-9]+").ToString();
            s.Date = Convert.ToInt32(date);
            return s;
        }
    }
}
