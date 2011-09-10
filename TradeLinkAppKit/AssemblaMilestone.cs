using System;
using System.Collections.Generic;
using System.Net;
using System.Xml;
using System.IO;
using System.Text;

namespace TradeLink.AppKit
{
    public struct AssemblaMilestone
    {

        string _space;
        string _name;

        /// <summary>
        /// name of milestone
        /// </summary>
        public string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// space that owns milestone
        /// </summary>
        public string Space { get { return _space; } set { _space = value; } }

        bool _completed;
        /// <summary>
        /// whether milestone is still open
        /// </summary>
        public bool isCompleted { get { return _completed; } set { _completed = value; } }

        int _id;
        /// <summary>
        /// id of milestone
        /// </summary>
        public int Id { get { return _id; } set { _id = value; } }

        string _sid ;
        public string SpaceId { get { return _sid; } set { _sid = value; } }

        string _des;
        /// <summary>
        /// description of milestone
        /// </summary>
        public string Desc { get { return _des; } set { _des = value; } }

        public bool isValid { get { return (_name!=null) && (Name != string.Empty); } }

        public static string GetMilestonesUrl(string space)
        {
            return "http://www.assembla.com/spaces/" + space + "/milestones/";
        }
        public static event TradeLink.API.DebugDelegate SendDebug;
        /// <summary>
        /// delete a milestone given a valid milestone instance
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="milestone"></param>
        /// <returns></returns>
        public static bool DeleteMilestone(string user, string password, AssemblaMilestone milestone)
        {
            string space = milestone.Space;
            string url = GetMilestonesUrl(space)+"//"+milestone.Id;
            string result = string.Empty;
            return qc.goget(url, user, password, string.Empty, SendDebug, out result);

        }

        /// <summary>
        /// get list of all milestones on a space
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static List<AssemblaMilestone> GetMilestones(string space, string user, string password)
        {
            string url = GetMilestonesUrl(space);
            string result = string.Empty;
            bool ok = qc.goget(url, user, password, string.Empty, SendDebug, out result);
            
            

            return getms(space,result);

        }
        internal static List<AssemblaMilestone> getms(string space,string result)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(result);
            List<AssemblaMilestone> docs = new List<AssemblaMilestone>();
            XmlNodeList xnl = xd.GetElementsByTagName("milestone");
            foreach (XmlNode xn in xnl)
            {
                AssemblaMilestone doc = new AssemblaMilestone();
                doc.Space = space;

                foreach (XmlNode dc in xn.ChildNodes)
                {
                    string m = dc.InnerText;
                    if (dc.Name == "id")
                        doc.Id = Convert.ToInt32(m);
                    else if (dc.Name == "space-id")
                        doc.SpaceId = m;
                    else if (dc.Name == "title")
                        doc.Name = m;
                    else if (dc.Name == "is-completed")
                        doc.isCompleted = Convert.ToBoolean(m);
                    else if (dc.Name == "description")
                        doc.Desc = m;
                }
                if (doc.isValid)
                    docs.Add(doc);
            }
            return docs;
        }
        /// <summary>
        /// create milestone
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AssemblaMilestone Create(string space, string user, string password, string name)
        {
            return Create(space, user, password, name, string.Empty, null);
        }
        /// <summary>
        /// create a milestone
        /// </summary>
        /// <param name="space"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="deb"></param>
        /// <returns></returns>

        public static AssemblaMilestone Create(string space, string user, string password, string name, string description, TradeLink.API.DebugDelegate deb)
        {
            string url = GetMilestonesUrl(space);

            StringBuilder data = new StringBuilder();
            data.AppendLine("<milestone>");
            data.AppendLine("<title>" + name + "</title>");
            data.AppendLine("<description>");
            data.AppendLine(description);
            data.AppendLine("</description>");
            data.AppendLine("</milestone>");

            string result = string.Empty;
            if (qc.gopost(url, user, password, data.ToString(), SendDebug, out result))
            {
                List<AssemblaMilestone> ms = getms(space, result);
                return ms[0];
            }
            return new AssemblaMilestone();
        }


    }


}
