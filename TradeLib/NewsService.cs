using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // this class sends our news to subscribers
    /// <summary>
    /// NewsService is used to receive news events from subscribers.
    /// 
    /// For example, Boxes you write that send debug messages through the
    /// Box.D(string msg) method call... these are passed to a NewsService
    /// instance as type News.
    /// 
    /// So if you want to handle news, instantiate a NewsService object.
    /// </summary>
    [Serializable]
    public class NewsService
    {
        public event NewsDelegate NewsEventSubscribers;
        public event KadDelegate KadListeners;

        /// <summary>
        /// Send some news
        /// </summary>
        /// <param name="news">The news.</param>
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
