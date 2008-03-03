using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // methods receiving news must have this signature
    public delegate void NewsDelegate(News news);

    // heres our news object
    public class News
    {
        private string news;
        private string type = "Info";
        public News(string news) { this.news = news; }
        public News(string news, string type) { this.news = news; this.type = type; }
        public News(News n) { this.news = n.news; this.type = n.type; }
        public string Msg { get { return news; } }
        public string Type { get { return type; } set { type = value; } }
    }

    // this class sends our news to subscribers
    public class NewsService
    {
        public event NewsDelegate NewsEventSubscribers;

        public void newNews(string news)
        {
            if (NewsEventSubscribers != null)
            {
                NewsEventSubscribers(new News(news));
            }
        }
        public NewsService() { }
    }

    public class Subscriber
    {
        public string name;
        NewsDelegate delNews;

        public void SubscribeNewsService(NewsService ns)
        {
            // newsdelegate's parameter is the method
            // to notify when the event occurs.
            // if method to notify is in another class,
            // parameter is INSTANCEofOtherClass.method
            delNews = new NewsDelegate(WriteNews);

            ns.NewsEventSubscribers += delNews;
        }

        public void UnsubscribeNewsService(NewsService ns)
        {
            ns.NewsEventSubscribers -= delNews;
        }

        // this method has same signature as NewsDelegate,
        // that is, have public void and receive News object
        public void WriteNews(News e)
        {
            Console.WriteLine(name + "received news: " + e.Msg);
        }

        public Subscriber(string name) { this.name = name; }
    }

    class MakeNews
    {
        static void Test(string[] args)
        {
            NewsService ns = new NewsService();
            Subscriber julie = new Subscriber("Julie");
            julie.SubscribeNewsService(ns);
            Subscriber matthew = new Subscriber("Mathew");
            matthew.SubscribeNewsService(ns);
            ns.newNews("more money for schools");
            julie.UnsubscribeNewsService(ns);
            ns.newNews("mayor");
        }
    }


         





}
