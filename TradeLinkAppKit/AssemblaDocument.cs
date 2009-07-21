using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.ServiceModel.Web;
using TradeLink.API;
using TradeLink.Common;
using System.IO;

namespace TradeLink.AppKit
{
    /// <summary>
    /// this class is for working with assembla documents.
    /// see : https://www.assembla.com/wiki/show/breakoutdocs/Document_REST_API
    /// </summary>
    public class AssemblaDocument
    {
        public static string GetDocumentsUrl(string space)
        {
            return "http://www.assembla.com/spaces/" + space + "/documents/";
        }
        public static bool Create(string space, string user, string password, string filename)
        {
            try 
            {
                string url = GetDocumentsUrl(space);
                WebClient wc = new WebClient();
                wc.Credentials = new System.Net.NetworkCredential(user, password);
                byte[] rb = wc.UploadFile(url, filename);
                string response = Encoding.UTF8.GetString(rb);
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create(response));
            }
            catch (Exception ex)
            {
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create("exception: " + ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        public static bool Delete(string space, string user, string password, string documentid)
        {
            string url = GetDocumentsUrl(space) + documentid;
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            hr.Credentials = new System.Net.NetworkCredential(user, password);
            hr.Method = "DELETE";
            hr.ContentType = "application/xml";
            try
            {
                // write it
                //System.IO.Stream post = hr.GetRequestStream();
                //post.Write(bytes, 0, 0);
                // get response
                System.IO.StreamReader response = new System.IO.StreamReader(hr.GetResponse().GetResponseStream());
                // display it
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create(response.ReadToEnd()));
            }
            catch (Exception ex)
            {
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create("exception: " + ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        public static event DebugFullDelegate SendDebug;
    }
}
