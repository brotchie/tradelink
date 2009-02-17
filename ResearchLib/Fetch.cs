using System;
using System.Collections.Generic;
using TradeLink.Common;
using System.Net;


namespace ResearchLib
{
    public class Fetch
    {
        public static BasketImpl NYSEFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.NYSE(wc.DownloadString(url));
        }
        public static BasketImpl NASDAQFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.NASDAQ(wc.DownloadString(url));
        }

        public static BasketImpl FromURL(string url)
        {
            BasketImpl b = NYSEFromURL(url);
            b.Add(NASDAQFromURL(url));
            return b;
        }

        public static BasketImpl LinkedNYSEFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.LinkedOnlyNYSE(wc.DownloadString(url));
        }

        public static BasketImpl LinkedNASDAQFromURL(string url)
        {
            WebClient wc = new WebClient();
            return ParseStocks.LinkedOnlyNASDAQ(wc.DownloadString(url));
        }



        public static BasketImpl RemoveDupe(BasketImpl input)
        {
            List<string> cache = new List<string>();
            BasketImpl output = new BasketImpl();
            for (int i = 0; i < input.Count; i++)
                if (!cache.Contains(input[i].Symbol))
                {
                    output.Add(input[i]);
                    cache.Add(input[i].Symbol);
                }
            return output;
        }

        public static BasketImpl RemoveUnlisted(BasketImpl input)
        {
            BasketImpl output = new BasketImpl();
            for (int i =0; i<input.Count; i++)
                if (NYSE.isListed(input[i].Symbol) || NASDAQ.isListed(input[i].Symbol))
                    output.Add(input[i]);
            return output;
        }

            
            


    }
}
