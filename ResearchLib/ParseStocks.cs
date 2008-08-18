using System;
using TradeLib;
using System.Text.RegularExpressions;

namespace ResearchLib
{
    public class ParseStocks
    {
        public static MarketBasket NYSE(string ParseStocks)
        {
            MarketBasket mb = new MarketBasket();
            MatchCollection mc = Regex.Matches(ParseStocks, @"\b[A-Z]{1,3}\b");
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new Stock(mc[i].Value.ToUpper()));
            return mb;
        }

        public static MarketBasket NASDAQ(string ParseStocks)
        {
            MarketBasket mb = new MarketBasket();
            string regexp = @"\b[A-Z]{4}\b";
            MatchCollection mc = Regex.Matches(ParseStocks, regexp);
            for (int i = 0; i < mc.Count; i++)
                mb.Add(new Stock(mc[i].Value.ToUpper()));
            return mb;
        }

        public static MarketBasket LinkedOnlyNYSE(string parsestring)
        {
            MarketBasket mb = new MarketBasket();
            string regexp = @">[A-Z]{1,3}</a>";
            MatchCollection mc = Regex.Matches(parsestring, regexp);
            for (int i = 0; i < mc.Count; i++)
            {
                string chunk = mc[i].Value;
                chunk = chunk.Replace("</a>", "");
                chunk = chunk.TrimStart('>');
                mb.Add(new Stock(chunk.ToUpper()));
            }
            return mb;
        }

        public static MarketBasket LinkedOnlyNASDAQ(string parsestring)
        {
            MarketBasket mb = new MarketBasket();
            string regexp = @">[A-Z]{4}</a>";
            MatchCollection mc = Regex.Matches(parsestring, regexp);
            for (int i = 0; i < mc.Count; i++)
            {
                string chunk = mc[i].Value;
                chunk = chunk.Replace("</a>", "");
                chunk = chunk.TrimStart('>');
                mb.Add(new Stock(chunk.ToUpper()));
            }
            return mb;
        }
    }


}
