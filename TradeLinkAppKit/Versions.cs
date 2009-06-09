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

        public static void UpgradeAlert(TLClient_WM tl) { UpgradeAlert(null, null, null,true, true, tl); }
        public static void UpgradeAlert(string Program, string ProgramUrl) {  UpgradeAlert(Program, ProgramUrl, Environment.CurrentDirectory,true,false,null); }
        /// <summary>
        /// checks a url for all EXEs with version numbers.
        /// Finds highest version number and alerts if local version number is different
        /// </summary>
        /// <param name="Program"></param>
        /// <param name="ProgramUrl"></param>
        /// <param name="path"></param>
        /// <param name="checktradelink"></param>
        /// <param name="checkbrokerserver"></param>
        public static void UpgradeAlert(string Program, string ProgramUrl, string path, bool checktradelink, bool checkbrokerserver, TLClient_WM tl)
        {
            
            if (Program != null)
            {
                
                int current = Util.BuildFromFile(path + "\\VERSION.txt");
                verstate vs = new verstate(Program, ProgramUrl, current, checktradelink, checkbrokerserver, tl);
                WebClient wc = new WebClient();
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wc_DownloadStringCompleted);
                try
                {
                    wc.DownloadStringAsync(new Uri(ProgramUrl), vs);

                }
                catch { }
            }
            else
            {
                verstate vs = new verstate(Program, ProgramUrl, 0, checktradelink, checkbrokerserver, tl);
                processver(null, vs);
            }




        }

        internal struct verstate
        {
            public TLClient_WM tl;
            public bool ctl;
            public bool cbs;
            public string program;
            public int current;
            public string url;
            public verstate(string Program, string URL, int currentversion, bool TL, bool BS, TLClient_WM TLwm) { url = URL; program = Program; current = currentversion; ctl = TL; cbs = BS; tl = TLwm;}
        }
        private  static void processver(string res, verstate vs)
        {
            if ((res != null) && (res != string.Empty))
            {
                int current = vs.current;
                string program = vs.program;
                int ver = 0;

                MatchCollection mc = Regex.Matches(res, program + @"-([0-9]+).exe");
                foreach (Match m in mc)
                {
                    string r = m.Result("$1");
                    int v = Convert.ToInt32(r);
                    if (v > ver) ver = v;
                }

                if ((ver > current) && (current * ver != 0))
                {
                    Versions nv = new Versions(program, vs.url);
                    nv.Show();
                }
            }
            bool t = false;
            bool b = false;
            if (vs.ctl)
                t = ExistsNewTLS();
            if ((vs.cbs) && (vs.tl != null) && (vs.tl.LinkType != TLTypes.NONE))
                b = ExistsNewBS(vs.tl);
            if ((t || b))
            {
                string ps = (t ? "TradeLinkSuite" : " ") + (b ? "BrokerServer":"");
                Versions nv = new Versions(ps, "http://tradelink.googlecode.com", "Grab new versions of: " + ps + Environment.NewLine + "Or run TradeLink Update.");
                nv.Show();
            }
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
            processver(res, vs);
        }

        void urlloclab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        public const string TLSITEURL = "http://code.google.com/p/tradelink/";
        public const string BROKERSERVER = "BrokerServer";
        public const string TRADELINKSUITE = "TradeLinkSuite";
        /// <summary>
        /// Gets latest version number of an application's exe file, when version is embedded in file name like 'MyAppName-123.exe', and can be found at URL.
        /// </summary>
        /// <param name="URL">URL of the location to get versions from</param>
        /// <param name="Application">Util.BROKERSERVER|Util.TRADELINKSUITE</param>
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



        /// <summary>
        /// Returns true if a newer version of suite exists on website
        /// </summary>
        /// <returns></returns>
        public static bool ExistsNewTLS()
        {
            int latest = LatestVersion(TRADELINKSUITE);
            int build = Util.BuildFromFile(Util.TLProgramDir + @"\VERSION.txt");
            return latest > build;
        }
        /// <summary>
        /// Returns true if a newer version of brokerserver exists.
        /// </summary>
        /// <param name="tl"></param>
        /// <returns></returns>
        public static bool ExistsNewBS(TLClient_WM tl)
        {
            if (tl == null) return false;
            if (tl.LinkType == TLTypes.NONE) return false;
            int latest = 0;
            if (tl.LinkType == TLTypes.HISTORICALBROKER)
                latest = LatestVersion(TRADELINKSUITE);
            else
                latest = LatestVersion(BROKERSERVER);
            int thisver = tl.ServerVersion;
            return latest > thisver;
        }
    }
}
