using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using TradeLink.API;
using TradeLink.Common;
using System.IO;
using SeasideResearch.LibCurlNet;

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
            return "http://www.assembla.com/spaces/" + space + "/documents";
        }
        public static bool Create(string space, string user, string password, string filepath) { return Create(space, user, password, filepath, 0); }
        public static bool Create(string space, string user, string password, string filename, int ticketid) { return Create(space, user, password, filename, 0, true); }
        public static bool Create(string space, string user, string password, string filename, int ticketid, bool prependdatetime) { return Create(space, user, password, filename, 0, prependdatetime,false); }
        public static bool Create(string space, string user, string password, string filename, int ticketid, bool prependdatetime, bool showprogress)
        {
            string url = GetDocumentsUrl(space);
            try
            {

                string unique = prependdatetime ? Util.ToTLDate(DateTime.Now).ToString() + Util.DT2FT(DateTime.Now) + Path.GetFileName(filename) : Path.GetFileName(filename);
                
                FileInfo fi = new FileInfo(filename);
                int maxwaitsec = (int)(fi.Length / (double)50000);
                string result = string.Empty;
                bool ok = qc.gomultipartpost(url, user, password, unique,filename,ticketid,maxwaitsec, showprogress,SendDebug, out result);
                string node = ok ? "document" : "error";
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(result);
                List<AssemblaDoc> docs = new List<AssemblaDoc>();
                XmlNodeList xnl = xd.GetElementsByTagName(node);
                foreach (XmlNode xn in xnl)
                {
                    AssemblaDoc doc = new AssemblaDoc();
                    doc.Space = space;
                    foreach (XmlNode dc in xn.ChildNodes)
                    {
                        string m = dc.InnerText;
                        if (ok)
                        {
                            if (dc.Name == "id")
                                doc.Id = m;
                            else if (dc.Name == "filesize")
                                doc.Size = Convert.ToInt32(m);
                            else if (dc.Name == "description")
                                doc.Desc = m;
                            else if (dc.Name == "name")
                                doc.Name = m;
                        }
                        else if (SendDebug!=null)
                        {
                            SendDebug(m);
                        }
                    }
                    if (doc.isValid)
                        docs.Add(doc);
                }

                
                
                return true;
            }
            catch (Exception ex)
            {
                if (SendDebug != null)
                    SendDebug("exception: " + ex.Message + ex.StackTrace);

                return false;
            }

        }

        public static bool DownloadDocument(AssemblaDoc doc, string user, string password) { return DownloadDocument(doc, Environment.CurrentDirectory, user, password); }
        public static bool DownloadDocument(AssemblaDoc doc, string path, string user, string password)
        {
            if (!doc.isValid) return false;
            string url = doc.Url;
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            hr.Credentials = new System.Net.NetworkCredential(user, password);
            hr.PreAuthenticate = true;
            hr.Method = "GET";
            hr.ContentType = "application/xml";
            HttpWebResponse wr = (HttpWebResponse)hr.GetResponse();
            Stream stream = wr.GetResponseStream();
            byte[] buff = new byte[(int)wr.ContentLength];
            int n = stream.Read(buff, 0, (int)wr.ContentLength);
            if (n == 0) return false;
            try
            {
                stream.Close();
                wr.Close();
                FileStream fs = new FileStream(path + "//" + doc.Name, FileMode.Create);
                fs.Write(buff, 0, (int)buff.Length);
                fs.Close();
                return true;
            }
            catch { }
            return false;
        }

        public static List<AssemblaDoc> GetDocuments(string space, string user, string password)
        {
            string url = GetDocumentsUrl(space);

            string result = string.Empty;
            List<AssemblaDoc> docs = new List<AssemblaDoc>();
            if (qc.goget(url, user, password, string.Empty,SendDebug, out result))
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(result);
                
                XmlNodeList xnl = xd.GetElementsByTagName("document");
                foreach (XmlNode xn in xnl)
                {
                    AssemblaDoc doc = new AssemblaDoc();
                    doc.Space = space;
                    foreach (XmlNode dc in xn.ChildNodes)
                    {
                        string m = dc.InnerText;
                        if (dc.Name == "id")
                            doc.Id = m;
                        else if (dc.Name == "filesize")
                            doc.Size = Convert.ToInt32(m);
                        else if (dc.Name == "description")
                            doc.Desc = m;
                        else if (dc.Name == "name")
                            doc.Name = m;
                    }
                    if (doc.isValid)
                        docs.Add(doc);
                }
            }
            return docs;

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
                    SendDebug(response.ReadToEnd());
            }
            catch (Exception ex)
            {
                if (SendDebug != null)
                    SendDebug("exception: " + ex.Message + ex.StackTrace);
                return false;
            }
            return true;
        }

        public static event DebugDelegate SendDebug;
    }

    
    


    public struct AssemblaDoc
    {
        public bool isValid { get { return (Space != null) && (Id != null); } }
        public string Url { get { return AssemblaDocument.GetDocumentsUrl(Space) + "/" + Id + "/download"; } }
        public string Desc;
        public string Space;
        public string Id;
        public int Size;
        public string Name;
        public override string ToString()
        {
            return !isValid ? "<empty>" : Name;
        }
    }
}
