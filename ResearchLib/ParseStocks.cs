using System;
using TradeLink.Common;
using System.Text.RegularExpressions;

namespace ResearchLib
{
    public class ParseStocks
    {
        public static BasketImpl NYSE(string ParseStocks)
        {
            BasketImpl mb = new BasketImpl();
            MatchCollection mc = Regex.Matches(ParseStocks, @"\b[A-Z]{1,3}\b");
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new SecurityImpl(mc[i].Value.ToUpper()));
            return mb;
        }

        public static BasketImpl NASDAQ(string ParseStocks)
        {
            BasketImpl mb = new BasketImpl();
            string regexp = @"\b[A-Z]{4}\b";
            MatchCollection mc = Regex.Matches(ParseStocks, regexp);
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new SecurityImpl(mc[i].Value.ToUpper()));
            return mb;
        }

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
