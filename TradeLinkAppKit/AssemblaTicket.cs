using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.ServiceModel.Web;
using TradeLink.API;
using TradeLink.Common;
namespace TradeLink.AppKit
{
    /// <summary>
    /// to create and update assembla tickets
    /// </summary>
    public class AssemblaTicket
    {
        public static string GetTicketsUrl(string space)
        {
            return "http://www.assembla.com/spaces/" + space + "/tickets";
        }
        /// <summary>
        /// returns global id of ticket if successful, zero if not successful
        /// (global id does not equal space's ticket id)
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="summary"></param>
        /// <returns></returns>
        public static int Create(string space, string user, string password, string summary) { return Create(space, user, password, summary, string.Empty, AssemblaStatus.New, AssemblaPriority.Normal); }
        public static int Create(string space, string user, string password, string summary, string description, AssemblaStatus status, AssemblaPriority priority)
        {
            int stat = (int)status;
            int pri = (int)priority;
            string url = GetTicketsUrl(space);
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            hr.Credentials = new System.Net.NetworkCredential(user, password);
            hr.PreAuthenticate = true;
            hr.Method = "POST";
            hr.ContentType = "application/xml";
            StringBuilder data = new StringBuilder();
            data.AppendLine("<ticket>");
            data.AppendLine("<status>"+stat.ToString()+"</status>");
            data.AppendLine("<priority>" + pri.ToString()+"</priority>");
            data.AppendLine("<summary>");
            data.AppendLine(System.Web.HttpUtility.HtmlEncode(summary));
            data.AppendLine("</summary>");
            data.AppendLine("<description>");
            data.AppendLine(System.Web.HttpUtility.HtmlEncode(description));
            data.AppendLine("</description>");
            data.AppendLine("</ticket>");
            // encode
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(data.ToString());
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
                string rs = response.ReadToEnd();

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(rs);
                XmlNodeList xnl = xd.GetElementsByTagName("id");
                string val = xnl[0].InnerText;
                if ((val!=null) && (val!=string.Empty))
                    id = Convert.ToInt32(val);
                // display it
                if (SendDebug!=null)
                    SendDebug(DebugImpl.Create(rs));
            }
            catch (Exception ex) 
            {
                if (SendDebug != null)
                    SendDebug(DebugImpl.Create("exception: " + ex.Message+ex.StackTrace)); 
                return 0; 
            }
            return id;
        }

        /// <summary>
        /// see https://www.assembla.com/wiki/show/breakoutdocs/Ticket_REST_API
        /// for example of valid xml updates
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="ticket"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static bool UpdateComment(string space, string user, string password, int ticket, string comment)
        {
            string xml = "<user-comment>" + comment + "</user-comment>";
            return Update(space,user,password,ticket,xml);
        }

        public static bool UpdateStatus(string space, string user, string password, int ticket, AssemblaStatus status)
        {
            int stat = (int)status;
            string xml = "<status>"+stat.ToString()+"</status>";
            return Update(space,user,password,ticket,xml);
        }
        public static bool Update(string space, string user, string password, int ticket, string xml)
        {
            string url = "http://www.assembla.com/spaces/" + space + "/tickets/"+ticket.ToString();
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            hr.Credentials = new System.Net.NetworkCredential(user, password);
            hr.Method = "PUT";
            hr.ContentType = "application/xml";
            StringBuilder data = new StringBuilder();
            data.AppendLine(System.Web.HttpUtility.HtmlEncode(xml));
            // encode
            byte[] bytes = UTF8Encoding.UTF8.GetBytes(data.ToString());
            hr.ContentLength = bytes.Length;
            try
            {
                // write it
                System.IO.Stream post = hr.GetRequestStream();
                post.Write(bytes, 0, bytes.Length);
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

    public enum AssemblaPriority
    {
        Highest = 1,
        High = 2,
        Normal = 3,
        Low = 5,
        Lowest = 5,
    }

    public enum AssemblaStatus
    {
        New,
        Accepted,
        ClosedInvalid,
        ClosedFixed,
        Test,
    }
}
