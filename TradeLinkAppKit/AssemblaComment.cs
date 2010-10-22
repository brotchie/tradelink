using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;

namespace TradeLink.AppKit
{
    public struct AssemblaComment
    {
        public bool isAssignmentChange { get { return ChangesRaw.Contains("assigned_to"); } }
        public bool isMilestoneChange { get { return ChangesRaw.Contains("milestone"); } }
        public bool isStatusChange { get { return ChangesRaw.Contains("status"); } }
        public bool isDescChange { get { return ChangesRaw.Contains("description"); } }
        public bool isSummaryChange { get { return ChangesRaw.Contains("summary"); } }
        public bool isPriorityChange { get { return ChangesRaw.Contains("priority"); } }
        public string Space;
        public string ChangesRaw;
        public int TicketId;
        public string CreatedOnRaw;
        public DateTime CreatedOn { get { return DateTime.Parse(CreatedOnRaw); } }
        public string UpdatedOnRaw;
        public DateTime UpdatedOn { get { return DateTime.Parse(UpdatedOnRaw); } }
        public string UserIdRaw;
        public string Username;
        public string Comment;
        public bool isValid { get { return (UserIdRaw != null) && (UserIdRaw != string.Empty) && (TicketId != 0); } }
        /// <summary>
        /// get comments on a ticket
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="pw"></param>
        /// <param name="ticketnum"></param>
        /// <returns></returns>
        public static List<AssemblaComment> GetComments(string space, string user, string pw, int ticketnum)
        {
            string url = GetCommentsUrl(space, ticketnum);
            HttpWebRequest hr = WebRequest.Create(url) as HttpWebRequest;
            hr.Credentials = new System.Net.NetworkCredential(user, pw);
            hr.PreAuthenticate = true;
            hr.Method = "GET";
            hr.ContentType = "application/xml";
            HttpWebResponse wr = (HttpWebResponse)hr.GetResponse();
            StreamReader sr = new StreamReader(wr.GetResponseStream());

            string result = sr.ReadToEnd();

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(result);
            List<AssemblaComment> docs = new List<AssemblaComment>();
            XmlNodeList xnl = xd.GetElementsByTagName("comment");
            foreach (XmlNode xn in xnl)
            {
                AssemblaComment doc = new AssemblaComment();

                doc.Space = space;
                foreach (XmlNode dc in xn.ChildNodes)
                {
                    string m = dc.InnerText;
                    if (dc.Name == "comment")
                        doc.Comment = m;
                    else if (dc.Name == "created-on")
                        doc.CreatedOnRaw = m;
                    else if (dc.Name == "ticket-id")
                        doc.TicketId = Convert.ToInt32(m);
                    else if (dc.Name == "updated-at")
                        doc.UpdatedOnRaw = m;
                    else if (dc.Name == "user-id")
                        doc.UserIdRaw = m;
                    else if (dc.Name == "changes")
                        doc.ChangesRaw = m;

                }
                if (doc.isValid)
                    docs.Add(doc);
            }
            return docs;
        }

        public static string GetCommentsUrl(string space, int ticketnum)
        {
            return "http://www.assembla.com/spaces/" + space + "/tickets/" + ticketnum + "/comments";
        }
    }
}
