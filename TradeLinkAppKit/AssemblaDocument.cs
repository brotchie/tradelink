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
            string url = GetDocumentsUrl(space);
            try
            {
                StreamReader sr = new StreamReader(filename);
                string content = sr.ReadToEnd();
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(url);
                hr.Credentials = new System.Net.NetworkCredential(user, password);
                hr.Method = "POST";
                hr.ContentType = "multipart/form-data";
                PostData pd = new PostData();
                pd.Params.Add(new PostDataParam("document[file]", filename,content, PostDataParamType.File));

                byte[] buffer = Encoding.UTF8.GetBytes(pd.GetPostData());
                hr.ContentLength = buffer.Length;
                Stream ds = hr.GetRequestStream();
                ds.Write(buffer, 0, buffer.Length);
                ds.Close();
                string res = new StreamReader(hr.GetResponse().GetResponseStream()).ReadToEnd();
                

                return true;
            }
            catch (Exception ex)
            {
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create("exception: " + ex.Message + ex.StackTrace));

                return false;
            }

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

    public class PostData
    {

        private List<PostDataParam> m_Params;

        public List<PostDataParam> Params
        {
            get { return m_Params; }
            set { m_Params = value; }
        }

        public PostData()
        {
            m_Params = new List<PostDataParam>();

            // Add sample param
        }


        /// <summary>
        /// Returns the parameters array formatted for multi-part/form data
        /// </summary>
        /// <returns></returns>
        public string GetPostData()
        {
            // Get boundary, default is --AaB03x
            string boundary = "--AaB03x";

            StringBuilder sb = new StringBuilder();
            foreach (PostDataParam p in m_Params)
            {
                sb.AppendLine(boundary);

                if (p.Type == PostDataParamType.File)
                {
                    sb.AppendLine(string.Format("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", p.Name, p.FileName));
                    sb.AppendLine("Content-Type: text/plain");
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
                else
                {
                    sb.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", p.Name));
                    sb.AppendLine();
                    sb.AppendLine(p.Value);
                }
            }

            sb.AppendLine(boundary);

            return sb.ToString();
        }
    }

    public enum PostDataParamType
    {
        Field,
        File
    }

    public class PostDataParam
    {


        public PostDataParam(string name, string value, PostDataParamType type)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public PostDataParam(string name, string filename, string value, PostDataParamType type)
        {
            Name = name;
            Value = value;
            FileName = filename;
            Type = type;
        }

        public string Name;
        public string FileName;
        public string Value;
        public PostDataParamType Type;
    }
}
