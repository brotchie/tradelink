using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;

namespace TradeLink.AppKit
{
    public class Auth 
    {
        public Auth(string authUrl) { url = authUrl; }
        string url = "";
        public bool isAuthorized() { return isAuthorized(Auth.GetCPUId()); }
        public bool isAuthorized(string key)
        {
            return isAuthorized(key, true, false, true, false);
        }
        public bool isAuthorized(string key,bool pause)
        {
            return isAuthorized(key, true, false, true,pause);
        }
        public bool isAuthorized(string key, bool appendrandom, bool throwexception, bool showerrorbox, bool pause)
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
                PopupWindow.Show("Registration Error", err,true,pause);
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

