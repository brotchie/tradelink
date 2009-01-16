using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradeLib;
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

        static void Main(string[] args)
        {
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
            // remove existing
            try
            {
                if (File.Exists(tls))
                    File.Delete(tls);

                if (File.Exists(bs))
                    File.Delete(bs);
            }
            catch (Exception ex) { Console.WriteLine("unable to remove files: " + ex.Message + Environment.NewLine + "any key to continue..."); Console.ReadLine(); }


            // get files
            try
            {
                wc.DownloadFile(URL + bs, bs);
                wc.DownloadFile(URL + tls, tls);
            }
            catch (Exception ex) { Console.WriteLine("error: " + ex.Message); Console.WriteLine("any key to continue...");  Console.ReadLine(); return; }

            // uninstall existing versions
            string ubs = path + Util.BROKERSERVER + "\\uninstall.exe";
            string utls = path + Util.TRADELINKSUITE + "\\uninstall.exe";
            if (File.Exists(ubs))
                Process.Start(ubs,"/S");
            if (File.Exists(tls))
                Process.Start(utls,"/S");

            // install downloaded versions
            if (File.Exists(bs))
                Process.Start(bs,"/S");
            if (File.Exists(tls))
                Process.Start(tls,"/S");

            // remove downloaded version and restore working directory
            System.Threading.Thread.Sleep(3000);
            try
            {
                if (File.Exists(tls))
                    File.Delete(tls);

                if (File.Exists(bs))
                    File.Delete(bs);
            }
            catch (Exception ex) { Console.WriteLine("unable to remove files: " + ex.Message + Environment.NewLine + "any key to continue..."); Console.ReadLine(); }
            Environment.CurrentDirectory = od;
        }
    }
}
