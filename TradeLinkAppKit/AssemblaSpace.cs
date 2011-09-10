using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;
using System.Text;

namespace TradeLink.AppKit
{
    /// <summary>
    /// obtain information on portal spaces
    /// </summary>
    public struct AssemblaSpace
    {

        string _sn;
        public string Space { get { return _sn; } set { _sn = value; } }
        public string SpaceName { get { return _sn; } set { _sn = value; } }
        bool _commercial;
        public bool isCommercial { get { return _commercial; } set { _commercial = value; } }
        string _desc;
        public string Desc { get { return _desc; } set { _desc = value; } }
        /// <summary>
        /// whether space is valid
        /// </summary>
        public bool isValid { get { return (_sn != null) && (_sn != string.Empty); } }
        public static event TradeLink.API.DebugDelegate SendDebug;
        /// <summary>
        /// get list of spaces for an account
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<AssemblaSpace> GetSpaces(string user, string password)
        {
            string url = @"http://www.assembla.com/spaces/my_spaces";
            List<AssemblaSpace> docs = new List<AssemblaSpace>();
            string result = string.Empty;
            if (qc.goget(url, user, password, string.Empty, SendDebug, out result))
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(result);
                
                XmlNodeList xnl = xd.GetElementsByTagName("space");
                foreach (XmlNode xn in xnl)
                {
                    AssemblaSpace doc = new AssemblaSpace();

                    foreach (XmlNode dc in xn.ChildNodes)
                    {
                        string m = dc.InnerText;
                        if (dc.Name == "name")
                            doc.Space = m;
                        else if (dc.Name == "is-commercial")
                            doc.isCommercial = Convert.ToBoolean(m);
                        else if (dc.Name == "description")
                            doc.Desc = m;
                    }
                    if (doc.isValid)
                        docs.Add(doc);
                }


            }

            return docs;



        }

        
    }
}
