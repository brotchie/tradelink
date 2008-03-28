using System;
using System.Collections.Generic;
using System.Text;

namespace TradeLib
{
    // methods receiving news must have this signature
    /// <summary>
    /// Safely pass around News instances between TradeLink members.
    /// </summary>
    public delegate void NewsDelegate(News news);

    // heres our news object
    /// <summary>
    /// Create news for NewsService instances
    /// </summary>
    [Serializable]
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
}
