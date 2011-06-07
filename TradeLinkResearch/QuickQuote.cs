using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
[assembly: CLSCompliant(true)]
namespace TradeLink.Research
{
    /// <summary>
    /// obtain a realtime quote from yahoo.
    /// (take caution not to overuse this.  yahoo will rate limit you.)
    /// </summary>
    public struct QuickQuote
    {
        /// <summary>
        /// base url for service
        /// </summary>
        const string baseurl = "http://download.finance.yahoo.com/d/quotes.csv?f=sl1d1t1c1ohgvj1pp2wern&s=";
        public string Symbol;
        public decimal price;
        public DateTime date;
        public DateTime time;
        public decimal open;
        public decimal high;
        public decimal low;
        public int vol;
        public decimal eps;
        public decimal marketcap;
        public decimal afterhours;
        public decimal changeper;
        public decimal high52;
        public decimal low52;
        public decimal change;
        public decimal pe;
        public string Company;
        public string symbol { get { return Symbol; } }

        public bool isValid { get { return (symbol!=null) && (Symbol != "") && (price != 0); } }
        /// <summary>
        /// fetch a quick quote from the yahoo service. returns a quick quote object.
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public static QuickQuote Fetch(string sym)
        {
            QuickQuote qq = new QuickQuote();
            if (sym == "") return qq;
            WebClient wc = new WebClient();
            string res  = wc.DownloadString(baseurl + sym);
            res = res.Replace("\"", "");
            string [] r = res.Split(',');
            qq.Symbol = r[(int)q.sym];
            qq.Company = r[(int)q.company];
            decimal v;
            try 
            {
                qq.time = DateTime.Parse(r[(int)q.time]);
                qq.date = DateTime.Parse(r[(int)q.date]);
                qq.price = Convert.ToDecimal(r[(int)q.last]);
                qq.vol = Convert.ToInt32(r[(int)q.vol]);
                qq.open = Convert.ToDecimal(r[(int)q.open]);
                qq.low = Convert.ToDecimal(r[(int)q.low]);
                qq.high = Convert.ToDecimal(r[(int)q.high]);
                if (decimal.TryParse(r[(int)q.change].Replace("%", string.Empty), out v))
                    qq.change = v;
                if (decimal.TryParse(r[(int)q.changeper].Replace("%", string.Empty), out v))
                    qq.changeper = v;
                if (decimal.TryParse(r[(int)q.eps], out v))
                    qq.eps = v;
                if (decimal.TryParse(r[(int)q.pe], out v))
                    qq.pe = v;
                if (decimal.TryParse(r[(int)q.marketcap], out v))
                    qq.marketcap = v;
                if (decimal.TryParse(r[(int)q.afterhour], out v))
                    qq.afterhours = v;
                
                // get 52week range
                string [] range = r[(int)q.fiftytworange].Split(',');
                // if we have two records, try to parse
                if (range.Length == 2)
                {
                    
                    if (decimal.TryParse(r[0], out v))
                        qq.low52 = v;
                    if (decimal.TryParse(r[1], out v))
                        qq.high52 = v;
                }
            }
            catch (Exception) {}
            return qq;

        }

        enum q
        {
            sym,
            last,
            date,
            time,
            change,
            open,
            high,
            low,
            vol,
            marketcap,
            afterhour,
            changeper,
            fiftytworange,
            eps,
            pe,
            company,
        }

    }
}
