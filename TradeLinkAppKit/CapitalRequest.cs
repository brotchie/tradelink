using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.ServiceModel.Web;
using TradeLink.Common;
using TradeLink.API;
using System.IO;
using System.Xml.Serialization;

namespace TradeLink.AppKit
{
    internal class XmlTextWriterFormattedNoDeclaration : System.Xml.XmlTextWriter
    {
        public XmlTextWriterFormattedNoDeclaration(System.IO.TextWriter w) :
            base(w) 
        {
            Formatting = System.Xml.Formatting.Indented;
        }
        public override void WriteStartDocument() { } // suppress
    }
    /// <summary>
    /// create and send capital connection requests
    /// </summary>
    public struct CapitalRequest
    {
        const string TESTEMAIL = "support@pracplay.com";
        const string PUBLICURL = "http://pracplay-capitalconnection.appspot.com" + POSTFIX;
        public static CapitalRequest CreateTest() { return CreateTest(TESTEMAIL); }
        public static CapitalRequest CreateTest(string email)
        {
            return new CapitalRequest(email, TESTRESULTTL()); 
        }
        bool usingresults;
        public Results SubmittedResults;
        public CapitalRequest(string email, TradeLink.AppKit.Results rs)
            : this(email, rs.ToString())
        {
            SubmittedResults = rs;
            usingresults = true;
            result_symbols = rs.Symbols;
            result = System.Text.RegularExpressions.Regex.Replace(result, "Symbols:.*\n", string.Empty);
            Source = CapitalRequestSource.TradeLink;
        }
        public CapitalRequest(string email, string results)
        {
            result = results;
            requestor_email = email;
            id = 0;
            requestor_id = string.Empty;
            viewers = new string[0];
            result_dates = string.Empty;
            result_symbols = string.Empty;
            result_symdates = string.Empty;
            result_params = string.Empty;
            date = null;
            tag = string.Empty;
            result_source = string.Empty;
            requesttype = 0;
            
            usingresults = false;
            SubmittedResults = new Results();
            Type = CapitalRequestType.Request;

        }
        public int id;
        public string requestor_id;
        public string requestor_email;
        public string[] viewers;
        public string result;
        public string result_symdates;
        public string result_dates;
        public string result_symbols;
        public string result_source;
        public string result_params;
        public string date;
        public string tag;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public DateTime Date_Nice { get { return DateTime.Parse(date); } }
        public int requesttype;
        [System.Xml.Serialization.XmlIgnoreAttribute]
        public CapitalRequestType Type { get { return (CapitalRequestType)requesttype; } set { requesttype = (int)value; }}

        [System.Xml.Serialization.XmlIgnoreAttribute]
        public CapitalRequestSource Source { get { return (CapitalRequestSource)Enum.Parse(typeof(CapitalRequestSource), result_source); } set { result_source = value.ToString(); } }

        static TradeLink.API.DebugDelegate d;
        public static TradeLink.AppKit.Results TESTRESULTTL()
        {
            TradeLink.AppKit.Results r = new TradeLink.AppKit.Results();
            List<Trade> f = new List<Trade>();
            f.Add(new TradeImpl("IBM", 100, 400));
            f.Add(new TradeImpl("IBM", 120, -400));
            f.Add(new TradeImpl("CLV8 FUT NYMEX", 80.5m, -5));
            f.Add(new TradeImpl("CLV8 FUT NYMEX", 82.1m, 5));
            f.Add(new TradeImpl("MSFT 201102 CALL 20", 1, 5));
            f.Add(new TradeImpl("MSFT 201102 CALL 20", 2, -5));
            f.Add(new TradeImpl("EUR.USD", 1.25m, 10000));
            f.Add(new TradeImpl("EUR.USD", 1.20m, 10000));
            r = TradeLink.AppKit.Results.ResultsFromTradeList(f, .01m, .01m, null);
            return r;
        }
        static string TESTRESULT() 
        {
            Random r = new Random();
            return @"Sharp: " + (r.NextDouble() + r.Next(0, 6)).ToString();
        }
        const string POSTFIX = @"/request";
        public const string LOCALURL = @"http://localhost:8080"+POSTFIX;
        static void debug(string m) { if (d != null) d(m); }
        public static bool ResultsSubmittable(Results rs)
        {
            bool ok = (rs.GrossPL - (rs.ComPerShare * rs.Trades) > 0) && (rs.Trades > 1) && (rs.DaysTraded + rs.SymbolCount > 2);
            return ok;
        }
        public static bool SubmitTest(TradeLink.API.DebugDelegate del) { return Submit(LOCALURL,CreateTest() , del); }
        public static bool SubmitTest() { return Submit(LOCALURL, CreateTest(), null); }
        public static bool Submit(string email, TradeLink.AppKit.Results rs ) { return Submit(PUBLICURL, new CapitalRequest(email,rs), null); }
        public static bool Submit(string email, TradeLink.AppKit.Results rs,DebugDelegate deb) { return Submit(PUBLICURL, new CapitalRequest(email, rs), deb); }
        public static bool Submit(CapitalRequest cr) { return Submit(PUBLICURL, cr, null); }
        public static bool Submit(CapitalRequest cr, DebugDelegate deb) { return Submit(PUBLICURL, cr, deb); }
        public static bool Submit(string url, CapitalRequest cr) { return Submit(url, cr, null); }
        public static bool Submit(string url, CapitalRequest cr, TradeLink.API.DebugDelegate deb)
        {
            d = deb;
            if (cr.usingresults && !ResultsSubmittable(cr.SubmittedResults))
            {
                debug("To submit capital connection request, results must be profitable over at least two symbol days.");
                return false;
            }
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            //hr.Credentials = new System.Net.NetworkCredential(user, password);
            //hr.PreAuthenticate = true;
            //hr.Headers.Add(
            hr.Method = "POST";
            //hr.ContentType = "application/xml";

            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.OmitXmlDeclaration = true;
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(CapitalRequest));

            XmlWriter xmlWriter = XmlTextWriterFormattedNoDeclaration.Create(stringWriter, writerSettings);
            System.Xml.Serialization.XmlSerializerNamespaces ns = new System.Xml.Serialization.XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(xmlWriter, cr, ns);
            string xml = stringWriter.ToString();

            System.Xml.Linq.XDocument xd = System.Xml.Linq.XDocument.Parse(xml);
            StringBuilder sb = new StringBuilder();
            // get parameter list
            int i = 0;
            foreach (System.Xml.Linq.XElement xe in xd.Descendants())
            {
                if (xe.HasElements) continue;
                if (i++ > 0)
                    sb.Append("&");
                if (xe.Name.LocalName.ToLower() == "result")
                    sb.Append(xe.Name.LocalName + "=" + System.Web.HttpUtility.HtmlEncode(xe.Value));
                else
                    sb.Append(xe.Name.LocalName + "=" + xe.Value);
            }
            string data = sb.ToString();
          
            // encode
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(data);
            hr.ContentLength = bytes.Length;
            // prepare id
            int id = 0;
            try
            {
                // write it
                System.IO.Stream post = hr.GetRequestStream();
                post.Write(bytes, 0, bytes.Length);
                // get response
                System.IO.StreamReader response = new System.IO.StreamReader(hr.GetResponse().GetResponseStream());
                // get string version
                string key = response.ReadToEnd();
                // display it
                debug("added request: "+key);
                return true;
            }
            catch (Exception ex)
            {
                debug("error adding: "+Util.Serialize<CapitalRequest>(cr)+" "+ex.Message + ex.StackTrace);
                return false;
            }
        }
    }

    public enum CapitalRequestType
    {
        None,
        Request,
        ArchiveOnly,
    }

    public enum CapitalRequestSource
    {
        None,
        TradeLink,
        TradeStation,
        Other,
    }
}
