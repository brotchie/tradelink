using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLink.Common;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Update
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr HWND, int CMDSHOW);
        const int SW_MINIMIZE = 6;
        const string LOGFILE = "UpdateLog.txt";
        static StreamWriter logf = null;

        static void Main(string[] args)
        {
            if (File.Exists(LOGFILE))
            {
                FileInfo fi = new FileInfo(LOGFILE);
                if (fi.Length > 128000)
                    File.Delete(LOGFILE);
            }
            logf = new StreamWriter(LOGFILE, true);
            logf.AutoFlush = true;
            log("Update Started.");
            // minimize this window
            IntPtr curh = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(curh, SW_MINIMIZE);
            bool prompt = false;
            // save working directory, switch to temp location
            string od = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string path = Util.TLBaseDir;
            if (args.Length > 1)
                path = args[2];
            if (args.Length==1)
                prompt = true;
            if (prompt)
                if (MessageBox.Show("This application will uninstall your existing version of TradeLink and BrokerServer, installing the latest versions.", "Confirm update", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
            var wc = new WebClient();
            // build filenames
            const string URL = "http://tradelink.googlecode.com/files/";
            string bs = Util.BROKERSERVER+"-"+Util.LatestVersion(Util.BROKERSERVER).ToString()+".exe";
            string tls = Util.TRADELINKSUITE + "-" + Util.LatestVersion(Util.TRADELINKSUITE).ToString() + ".exe";
            log("removing existing installers");
            try
            {
                if (File.Exists(tls))
                    File.Delete(tls);

                if (File.Exists(bs))
                    File.Delete(bs);
            }
            catch (Exception ex) { log("unable to remove files: " + ex.Message); }


            log("downloading latest versions...");
            try
            {
                wc.DownloadFile(URL + bs, bs);
                wc.DownloadFile(URL + tls, tls);
            }
            catch (Exception ex) { log("error: " + ex.Message); return; }

            log("uninstalling existing versions");
            string ubs = path + Util.BROKERSERVER + "\\uninstall.exe";
            string utls = path + Util.TRADELINKSUITE + "\\uninstall.exe";
            if (File.Exists(ubs))
                Process.Start(ubs,"/S");
            if (File.Exists(tls))
                Process.Start(utls,"/S");

            log("installing downloaded versions");
            if (File.Exists(bs))
            {
                log("installing " + bs);
                Process.Start(bs, "/S");
            }
            else
                log("installer was missing: " + bs);
            if (File.Exists(tls))
            {
                log("installing " + tls);
                Process.Start(tls, "/S");
            }
            else
                log("installer was missing: " + tls);
            System.Threading.Thread.Sleep(3000);
            log("remove downloaded versions and restore working directory");
            try
            {
                if (File.Exists(tls))
                    File.Delete(tls);

                if (File.Exists(bs))
                    File.Delete(bs);
            }
            catch (Exception ex) { log("unable to remove files: " + ex.Message + Environment.NewLine + "any key to continue...");  }
            Environment.CurrentDirectory = od;
            log("Update finished.");
            logf.Close();
        }

        static void log(string msg)
        {
            if (logf == null)
                return;
            logf.WriteLine(DateTime.Now.ToString()+": "+msg);
        }
    }
}
