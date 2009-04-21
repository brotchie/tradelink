using System;
using TradeLink.Common;
using System.Text.RegularExpressions;

namespace TradeLink.Research
{
    /// <summary>
    /// parse symbols (most likely to be stocks) from a string
    /// </summary>
    public class ParseStocks
    {
        /// <summary>
        /// gets nyse symbols
        /// </summary>
        /// <param name="ParseStocks"></param>
        /// <returns></returns>
        public static BasketImpl NYSE(string ParseStocks)
        {
            BasketImpl mb = new BasketImpl();
            MatchCollection mc = Regex.Matches(ParseStocks, @"\b[A-Z]{1,3}\b");
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new SecurityImpl(mc[i].Value.ToUpper()));
            return mb;
        }
        /// <summary>
        /// gets nasdaq symbols
        /// </summary>
        /// <param name="ParseStocks"></param>
        /// <returns></returns>
        public static BasketImpl NASDAQ(string ParseStocks)
        {
            BasketImpl mb = new BasketImpl();
            string regexp = @"\b[A-Z]{4}\b";
            MatchCollection mc = Regex.Matches(ParseStocks, regexp);
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new SecurityImpl(mc[i].Value.ToUpper()));
            return mb;
        }
        /// <summary>
        /// gets clickable symbols found in a string (eg html)
        /// </summary>
        /// <param name="parsestring"></param>
        /// <returns></returns>
        public static BasketImpl LinkedOnlyNYSE(string parsestring)
        {
            BasketImpl mb = new BasketImpl();
            string regexp = @">[A-Z]{1,3}</a>";
            MatchCollection mc = Regex.Matches(parsestring, regexp);
            for (int i = 0; i < mc.Count; i++)
            {
                string chunk = mc[i].Value;
                chunk = chunk.Replace("</a>", "");
                chunk = chunk.TrimStart('>');
                mb.Add(new SecurityImpl(chunk.ToUpper()));
            }
            return mb;
        }
        /// <summary>
        /// gets clickable nasdaq symbols found in a string (eg html)
        /// </summary>
        /// <param name="parsestring"></param>
        /// <returns></returns>
        public static BasketImpl LinkedOnlyNASDAQ(string parsestring)
        {
            BasketImpl mb = new BasketImpl();
            string regexp = @">[A-Z]{4}</a>";
            MatchCollection mc = Regex.Matches(parsestring, regexp);
            for (int i = 0; i < mc.Count; i++)
            {
                string chunk = mc[i].Value;
                chunk = chunk.Replace("</a>", "");
                chunk = chunk.TrimStart('>');
                mb.Add(new SecurityImpl(chunk.ToUpper()));
            }
            return mb;
        }
    }


}
