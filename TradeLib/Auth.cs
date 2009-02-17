using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;

namespace TradeLink.Common
{
    public class Auth
    {
        public Auth(string authUrl) { url = authUrl; }
        string url = "";
        public bool isAuthorized(string key)
        {
            return isAuthorized(key, true, false, true);
        }
        public bool isAuthorized(string key, bool appendrandom, bool throwexception, bool showerrorbox)
        {
            WebClient wc = new WebClient();
            string rurl = url;
            if (appendrandom)
            {
                if (!rurl.Contains("?"))
                    rurl += "?";
                Random rand = new Random((int)DateTime.Now.Ticks);
                string last = rurl.Substring(url.Length-1,1);
                if ((last != "?") || (last != "&"))
                    rurl += "&";
                rurl += "r=" + rand.Next().ToString();
            }
            string res = "";
            try 
            {
                res = wc.DownloadString(rurl);
            }
            catch (WebException ex) { res = ex.Message; }
            bool reg = res.Contains(key);
            string err = "Registration Key " + key + " not found. Contact your software provider to register software.";
            if (throwexception && !reg)
                throw new UnregisteredException(err);
            if (showerrorbox && !reg)
                System.Windows.Forms.MessageBox.Show(err, "Registration Error");
            return reg;
        }

        /// <summary>
        /// Return processorId from first CPU in machine
        /// </summary>
        /// <returns>[string] ProcessorId</returns>
        public static string GetCPUId()
        {
            string cpuInfo = String.Empty;
            string temp = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == String.Empty)
                {// only return cpuInfo from first CPU
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
            }
            return cpuInfo;
        }
    }


    public class UnregisteredException : Exception
    {
        public UnregisteredException(string message) : base(message) { }
    }



}

