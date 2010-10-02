using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using TradeLink.API;
using TradeLink.Common;
using System.IO;

namespace TradeLink.AppKit
{
    public struct AssemblaUser
    {
        public string Id;
        public string Username;
        public string Email;
        public string Organization;
        public string Website;
        public string CurrentSpace;
        public bool isValid { get { return (Username != null) && (Username != string.Empty); } }

        public static string GetUrl(string space)
        {
            return @"http://www.assembla.com/spaces/" + space + @"/users";
        }

        private static List<AssemblaUser> getdata(string url, bool auth, string user, string password)
        {
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            if (auth)
            {
                hr.Credentials = new System.Net.NetworkCredential(user, password);
                hr.PreAuthenticate = true;
            }
            hr.Method = "GET";
            hr.ContentType = "application/xml";
            HttpWebResponse wr = (HttpWebResponse)hr.GetResponse();
            StreamReader sr = new StreamReader(wr.GetResponseStream());
            XmlDocument xd = new XmlDocument();
            string result = sr.ReadToEnd();
            xd.LoadXml(result);
            List<AssemblaUser> docs = new List<AssemblaUser>();
            XmlNodeList xnl = xd.GetElementsByTagName("user");
            foreach (XmlNode xn in xnl)
            {
                AssemblaUser doc = new AssemblaUser();
                foreach (XmlNode dc in xn.ChildNodes)
                {
                    string m = dc.InnerText;
                    if (dc.Name == "id")
                        doc.Id = m;
                    else if (dc.Name == "email")
                        doc.Email = m;
                    else if (dc.Name == "organization")
                        doc.Organization = m;
                    else if (dc.Name == "login_name")
                        doc.Username = m;
                    else if (dc.Name == "website")
                        doc.Website = m;
                }
                if (doc.isValid)
                    docs.Add(doc);
            }
            return docs;
        }

        public static List<AssemblaUser> GetUsers(string space, string user, string password)
        {
            string url = GetUrl(space);
            return getdata(url, true,user, password);

        }

        public static AssemblaUser GetUserFromUsername(string user)
        {
            string url = @"http://www.assembla.com/user/best_profile?login=" + user;
            List<AssemblaUser> aus = getdata(url, false, string.Empty, string.Empty);
            if (aus.Count > 0)
                return aus[0];
            return new AssemblaUser();
        }
        public static AssemblaUser GetUserFromId(string id)
        {
            string url = @"http://www.assembla.com/user/best_profile/" + id;
            List<AssemblaUser> aus = getdata(url, false, string.Empty, string.Empty);
            if (aus.Count > 0)
                return aus[0];
            return new AssemblaUser();
        }
    }
}
