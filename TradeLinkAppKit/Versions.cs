using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.AppKit
{
    /// <summary>
    /// display a new version popup in tradelink, which allows user to download newer version easily.
    /// </summary>
    public partial class Versions : Form
    {
        public Versions(string ProgramName, string ProgramUrl) : this(ProgramName, ProgramUrl, "") { }
        public Versions(string ProgramName, string ProgramUrl, string msg)
        {
            InitializeComponent();
            Text = ProgramName + " Update Available";
            if (msg!="") statuslab.Text = msg;
            urlloclab.Links.Add(0, 100, ProgramUrl);
            urlloclab.LinkClicked += new LinkLabelLinkClickedEventHandler(urlloclab_LinkClicked);
        }
        public static void UpgradeAlert() { UpgradeAlert(null, null, true, false, null); }
        public static void UpgradeAlert(bool pause) { UpgradeAlert(null, null, true, false, null,pause); }
        public static void UpgradeAlert(TLClient tl) { UpgradeAlert(null, null, true, true, tl); }
        public static void UpgradeAlert(TLClient tl,bool pause) { UpgradeAlert(null, null, true, true, tl,pause); }
        public static void UpgradeAlert(string Program, string ProgramUrl, TLClient tl) { UpgradeAlert(Program, ProgramUrl, true, true, tl); }
        public static void UpgradeAlert(string Program, string ProgramUrl) {  UpgradeAlert(Program, ProgramUrl, true,false,null); }
        /// <summary>
        /// checks a url for all EXEs with version numbers.
        /// Finds highest version number and alerts if local version number is different
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="ProgramUrl"></param>
        /// <param name="path"></param>
        /// <param name="checktradelink"></param>
        /// <param name="checkbrokerserver"></param>
        public static void UpgradeAlert(string Program, string ProgramUrl, bool checktradelink, bool checkbrokerserver, TLClient tl) { UpgradeAlert(Program, ProgramUrl, checktradelink, checkbrokerserver, tl,false); }
        public static void UpgradeAlert(string Program, string ProgramUrl, bool checktradelink, bool checkbrokerserver, TLClient tl,bool pause)
        {
 
            if (Program != null)
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                int current = Util.BuildFromRegistry(Program);
                if (current == 0)
                {
                    string path = Util.ProgramPath(Program);
                    current = Util.BuildFromFile(path + "\\VERSION.txt");
                }
                if (current != 0)
                    wc.DownloadStringAsync(new Uri(ProgramUrl), new verstate(Program, ProgramUrl, current,pause));
            }
            if (checktradelink)
            {
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);

                int current = Util.BuildFromRegistry(Util.PROGRAM);
                if (current!=0)
                    wc.DownloadStringAsync(new Uri(TLSITEURL), new verstate(Util.PROGRAM,TLSITEURL, current,pause));
            }
        }
        public const string VERSIONFILE = @"\VERSION.txt";
        internal struct verstate
        {
            public bool pause;
            public string program;
            public int current;
            public string url;
            public verstate(string Program, string URL, int currentversion, bool pauseafter) { url = URL; program = Program; current = currentversion; pause = pauseafter; }
        }

        private static int latest(string res, string Program)
        {
            MatchCollection mc = Regex.Matches(res, Program + @"-([0-9]+).exe");
            int ver = 0;
            foreach (Match m in mc)
            {
                string r = m.Result("$1");
                int v = Convert.ToInt32(r);
                if (v > ver) ver = v;
            }
            return ver;
        }

        static void wc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            // if we don't have arguments, quit
            if (e.UserState==null) return;
            // if we had an error, quit
            if (e.Cancelled || (e.Error != null))
                return;
            // handle data
            verstate vs = (verstate)e.UserState;
            string res = e.Result;
            if ((res != null) && (res != string.Empty))
            {
                int ver = latest(res, vs.program);
                if ((ver > vs.current) && ((vs.current * ver) != 0))
                {
                    Versions nv = new Versions(vs.program, vs.url);
                    if (vs.pause)
                        nv.ShowDialog();
                    else
                        nv.Show();
                }
            }
        }

        void urlloclab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        public const string TLSITEURL = "http://code.google.com/p/tradelink/";

        /// <summary>
        /// Gets latest version number of an application's exe file, when version is embedded in file name like 'MyAppName-123.exe', and can be found at URL.
        /// </summary>
        /// <param name="URL">URL of the location to get versions from</param>
        /// <param name="Application">application name</param>
        /// <returns></returns>
        public static int LatestVersion(string Application)
        {
            
            return LatestVersion(TLSITEURL, Application);
        }
        public static int LatestVersion(string URL, string Application)
        {
            WebClient wc = new WebClient();
            int ver = 0;
            try
            {
                string res = wc.DownloadString(URL);
                MatchCollection mc = Regex.Matches(res, Application + @"-([0-9]+).exe");
                foreach (Match m in mc)
                {
                    string r = m.Result("$1");
                    int v = Convert.ToInt32(r);
                    if (v > ver) ver = v;
                }
            }
            catch (Exception) { return ver; }
            return ver;
        }




        [Obsolete("Use LatestVersion(Util.PROGRAM) instead.")]
        public static bool ExistsNewTLS()
        {
            int latest = LatestVersion(Util.PROGRAM);
            int build = Util.BuildFromRegistry(Util.PROGRAM);
            if (build == 0)
                build = Util.BuildFromFile(Util.TLProgramDir + @"\VERSION.txt");
            return latest > build;
        }

        [Obsolete("Brokerserver is part of TradeLink.  Use LatestVersion(Util.PROGRAM) instead.")]
        public static bool ExistsNewBS(TLClient_WM tl)
        {
            if (tl == null) return false;
            if (tl.ProvidersAvailable.Length==0) return false;
            int latest = LatestVersion(Util.PROGRAM);
            int thisver = tl.ServerVersion;
            return latest > thisver;
        }
    }
}
