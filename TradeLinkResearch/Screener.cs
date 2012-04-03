using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.Research
{
    public struct Screener 
    {
        const int NUM = 0;
        const int TICKER = 1;
        const int COMPANY = 2;
        const int SECTOR = 3;
        const int INDUSTRY = 4;
        const int COUNTRY = 5;
        const int MARKETCAP = 6;
        const int PE = 7;
        const int PRICE = 8;
        const int CHANGE = 9;
        const int VOLUME = 10;
        public int id;
        public string symbol;
        public string company;
        public string sector;
        public string industry;
        public string country;
        public decimal marketcap;
        public decimal peratio;
        public decimal price;
        public decimal pctchange;
        public Int64 volume;

        public override string ToString()
        {
            return symbol + " screen";
        }

        static DebugDelegate d = null;
        static void debug(string msg)
        {
            if (d != null)
                d(msg);
        }
        static bool isstring(int pos, string line)
        {
            return line[pos]=='\"';
        }
        static int geteos(int start, string line)
        {
            return line.IndexOf('\"',start+1);
        }
        static int geteod(int start, string line)
        {
            int eod = line.IndexOf(',',start);
            if (eod==-1)
                eod = line.Length;
            return eod;
        }
        static string getdata(int start, int eod, string line)
        {
            return line.Substring(start, eod - start).Replace("%", string.Empty).Replace(",",string.Empty);
        }
        static string getstring(int start, int eos, string line)
        {
            return line.Substring(start,eos-start).Replace("\"",string.Empty).Replace(","," ");
        }
        static string[] getrec(string line)
        {
            // get record number
            int rn = 0;
            int lastrn = -1;
            List<string> r = new List<string>();
            // process every character
            for (int c = 0; c<line.Length; c++)
            {
                bool newrec = rn!=lastrn;
                lastrn = rn;
                if (newrec && isstring(c,line))
                {
                    int eos = geteos(c,line);
                    r.Add(getstring(c,eos,line));
                    rn++;
                    c = eos+1;
                }
                else if (newrec)
                {
                    int eod = geteod(c,line);
                    string data = getdata(c, eod, line);
                    data = data.Replace(" ",string.Empty);
                    if (data == string.Empty)
                        data = "0";
                    r.Add(data);
                    rn++;
                    c = eod;
                }

            }

            return r.ToArray();

            

        }
        static Screener getscreen(string[] rec)
        {
            Screener s = new Screener();
            s.id = Convert.ToInt32(rec[NUM]);
            s.symbol = rec[TICKER];
            s.company = rec[COMPANY];
            s.sector = rec[SECTOR];
            s.industry = rec[INDUSTRY];
            s.country = rec[COUNTRY];
            s.marketcap = Convert.ToDecimal(rec[MARKETCAP]);
            s.peratio = Convert.ToDecimal(rec[PE]);
            s.price = Convert.ToDecimal(rec[PRICE]);
            s.pctchange = Convert.ToDecimal(rec[CHANGE].Replace("%",string.Empty));
            s.volume = Convert.ToInt64(rec[VOLUME]);
            return s;
        }

        public static GenericTracker<Screener> fetchscreen() { return fetchscreen(finzurl, null); }
        public static GenericTracker<Screener> fetchscreen(DebugDelegate deb) { return fetchscreen(finzurl, deb); }
        public static GenericTracker<Screener> fetchscreen(string url, DebugDelegate deb)
        {
            // get raw list
            string[][] raw = fetchrawlist(url,deb);
            debug("beginning screen indexing of "+raw.GetLength(0)+" screens.");
            GenericTracker<Screener> ss = new GenericTracker<Screener>(raw.GetLength(0), "SCREENS", new Screener());
            foreach (string[] r in raw)
            {
                Screener s = getscreen(r);
                ss.addindex(s.symbol, s);
            }
            debug("completed index of "+ss.Count+" screens.");
            return ss;

        }
        const string finzurl = "http://finviz.com/export.ashx?v=111&ft=1&ta=1&p=d&r=1";
        static string[][] fetchrawlist(string url, DebugDelegate deb)
        {
            d = deb;
            
            debug("grabbing screen data from: "+url);
            string content = string.Empty;
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                content = wc.DownloadString(url);
                debug("obtained " + content.Length + " bytes of screen data.");
            }
            catch (Exception ex)
            {
                debug("error obtaining data from: " + url + " err: " + ex.Message + ex.StackTrace);
                return new string[0][];
            }
            string[] lines = content.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[][] final = new string[lines.Length - 1][];
            int err = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                // get line
                string line = lines[i];

                try
                {
                    // get records
                    string[] rec = getrec(line);
                    final[i - 1] = rec;
                }
                catch (Exception ex)
                {
                    err++;
                    debug("error parsing line: " + line+" err: "+ex.Message+ex.StackTrace);
                }
            }
            debug("retrieved " + final.GetLength(0) + " screen records with " + err + " errors.");
            return final;
        }

    }
}
