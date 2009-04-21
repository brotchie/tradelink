using System;
using System.Collections.Generic;
using System.Text;
using TradeLink.Common;
using System.Web;
using TradeLink.API;

namespace TradeLink.Research
{
    /// <summary>
    /// used to serialize and store parameters selected in a FetchBasket popup.
    /// </summary>
    public class FetchTarget
    {
        public FetchTarget() 
        {
         
        }
        string _url = "";
        string _name = "";
        bool _nyse = true;
        bool _nasd = false;
        bool _xdupe = true;
        bool _linkedonly = true;
        string _file = "";
        public string File { get { return _file; } set { _file = value; } }
        public string Url { get { return _url; } set { _url = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public bool ClickableOnly { get { return _linkedonly; } set { _linkedonly = value; } }
        public bool ParseNYSE { get { return _nyse; } set { _nyse = value; } }
        public bool ParseNASD { get { return _nasd; } set { _nasd = value; } }
        public bool RemoveDupes { get { return _xdupe; } set { _xdupe = value; } }
        /// <summary>
        /// fetches the basket
        /// </summary>
        /// <returns></returns>
        public Basket Go()
        {
            if (_file != "") return FILE();
            return URL();
        }
        /// <summary>
        /// unused.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Basket Go(string username)
        {
            return Go();
        }
        /// <summary>
        /// gets a basket from a url
        /// </summary>
        /// <returns></returns>
        public Basket URL()
        {
            Basket mb = new BasketImpl();
            if (!Uri.IsWellFormedUriString(_url, UriKind.RelativeOrAbsolute)) return mb;
            if (_nyse && _linkedonly) mb.Add(Fetch.LinkedNYSEFromURL(_url));
            else if (_nyse) mb.Add(Fetch.NYSEFromURL(_url));
            if (_nasd && _linkedonly) mb.Add(Fetch.LinkedNASDAQFromURL(_url));
            else if (_nasd) mb.Add(Fetch.NASDAQFromURL(_url));
            if (_xdupe) mb = Fetch.RemoveDupe(mb);
            return mb;
        }
        /// <summary>
        /// gets a basket from a file
        /// </summary>
        /// <returns></returns>
        public Basket FILE()
        {
            Basket mb = new BasketImpl();
            if ((_file == "") || (_file == null)) return mb;
            System.IO.StreamReader sr = null;
            try
            {
                sr = new System.IO.StreamReader(_file);
            }
            catch (Exception) { return mb; }
            string file = sr.ReadToEnd();
            if (_nyse && _linkedonly) mb.Add(ParseStocks.LinkedOnlyNYSE(file));
            else if (_nyse) mb.Add(ParseStocks.NYSE(file));
            if (_nasd && _linkedonly) mb.Add(ParseStocks.LinkedOnlyNASDAQ(file));
            else if (_nasd) mb.Add(ParseStocks.NASDAQ(file));
            if (_xdupe) mb = Fetch.RemoveDupe(mb);
            return mb;
            
            
        }
        /// <summary>
        /// serialize these parameters for later use
        /// </summary>
        /// <returns></returns>
        public string Serialize()
        {
            string[] s = new string[] { _url, _name, _nyse.ToString(), _nasd.ToString(),_xdupe.ToString(),_linkedonly.ToString() };
            return string.Join(",", s);
        }
        /// <summary>
        /// restore parameters from file or network location.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static FetchTarget Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            FetchTarget ft = new FetchTarget();
            if (r.Length != Enum.GetNames(typeof(ftfield)).Length) return ft;
            ft.ParseNASD = Convert.ToBoolean(r[(int)ftfield.nasd]);
            ft.ParseNYSE = Convert.ToBoolean(r[(int)ftfield.nyse]);
            ft.Url = r[(int)ftfield.url];
            ft.Name = r[(int)ftfield.name];
            ft.RemoveDupes = Convert.ToBoolean(r[(int)ftfield.xdupe]);
            ft.ClickableOnly = Convert.ToBoolean(r[(int)ftfield.linked]);
            return ft;
        }

        public override int GetHashCode()
        {
            return _url.GetHashCode() + _name.GetHashCode() + _nyse.GetHashCode() + _nasd.GetHashCode() + _xdupe.GetHashCode()+_linkedonly.GetHashCode();
        }
        public override string ToString()
        {
            return _name;
        }


        enum ftfield
        {
            url,
            name,
            nyse,
            nasd,
            xdupe,
            linked,
        }

    }
}
