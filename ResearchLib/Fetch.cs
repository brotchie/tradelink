using System;
using System.Collections.Generic;
using TradeLib;
using System.Net;


namespace ResearchLib
{
    public class Fetch
    {
        public static MarketBasket NYSEFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.NYSE(wc.DownloadString(url));
        }
        public static MarketBasket NASDAQFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.NASDAQ(wc.DownloadString(url));
        }

        public static MarketBasket FromURL(string url)
        {
            MarketBasket b = NYSEFromURL(url);
            b.Add(NASDAQFromURL(url));
            return b;
        }

        public static MarketBasket LinkedNYSEFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.LinkedOnlyNYSE(wc.DownloadString(url));
        }

        public static MarketBasket LinkedNASDAQFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.LinkedOnlyNASDAQ(wc.DownloadString(url));
        }



        public static MarketBasket RemoveDupe(MarketBasket input)
        {
            List<string> cache = new List<string>();
            MarketBasket output = new MarketBasket();
            for (int i = 0; i < input.Count; i++)
                if (!cache.Contains(input[i].Symbol))
                {
                    output.Add(input[i]);
                    cache.Add(input[i].Symbol);
                }
            return output;
        }

        public static MarketBasket RemoveUnlisted(MarketBasket input)
        {
            MarketBasket output = new MarketBasket();
            for (int i =0; i<input.Count; i++)
                if (NYSE.isListed(input[i].Symbol) || NASDAQ.isListed(input[i].Symbol))
                    output.Add(input[i]);
            return output;
        }

            
            


    }
}
