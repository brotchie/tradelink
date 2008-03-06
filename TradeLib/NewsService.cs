using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // this class sends our news to subscribers
    public class NewsService
    {
        public event NewsDelegate NewsEventSubscribers;
        public event KadDelegate KadListeners;

        public void newNews(string news)
        {
            if (NewsEventSubscribers != null)
            {
                NewsEventSubscribers(new News(news));
            }
        }
        public void newKad(Kad k)
        {
            if (KadListeners != null) KadListeners(k);
        }
        public NewsService() { }
    }

}
