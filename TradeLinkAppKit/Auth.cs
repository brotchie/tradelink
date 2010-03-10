using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Net;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    /// <summary>
    /// authenticate username/url/key to 'allow list' url
    /// </summary>
    public sealed class Auth 
    {
        /// <summary>
        /// authenicate using given url
        /// </summary>
        /// <param name="authurl"></param>
        public Auth(string authurl) : this(authurl, 0) { }
        /// <summary>
        /// authenticate using given url and expire authenticates 
        /// </summary>
        /// <param name="authUrl"></param>
        /// <param name="expireAfterSeconds"></param>
        public Auth(string authUrl, int expireAfterSeconds) 
        { 
            url = authUrl;
            _expiresec = expireAfterSeconds;
        }
        string url = "";
        bool _lastauth = false;
        int _authtime = 0;
        int _expiresec = 0;
        /// <summary>
        /// number of seconds this authorization is good for before must be reauthorized
        /// (0 if not used)
        /// </summary>
        public int ExpireSeconds { get { return _expiresec; } }
        /// <summary>
        /// returns true if authorization has passed and not expired
        /// </summary>
        public bool isValid { get { return (_lastauth && (_expiresec==0)) || (_lastauth && (Util.FTDIFF(_authtime, Util.ToTLTime()) < _expiresec)); } }
        /// <summary>
        /// see if we're authorized on this machine
        /// </summary>
        /// <returns></returns>
        public bool isAuthorized() { return isAuthorized(Auth.GetCPUId()); }
        /// <summary>
        /// see if a given key is authorized
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool isAuthorized(string key)
        {
            return isAuthorized(key, true, false, true, false);
        }
        /// <summary>
        /// see if a given key is authorized, display dialog box and determine whether dialog box should pause program execution (eg if running on background thread which might terminate otherwise)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pause"></param>
        /// <returns></returns>
        public bool isAuthorized(string key,bool pause)
        {
            return isAuthorized(key, true, false, true,pause);
        }
        /// <summary>
        /// see if given key is authorized
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="appendrandom">whether random parameter is appended to url to prevent results caching</param>
        /// <param name="throwexception">throw exceptions on errors</param>
        /// <param name="showerrorbox">show registration dialog if not authorized</param>
        /// <param name="pause">pause execution when showing dialog</param>
        /// <returns></returns>
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
            _lastauth = reg;
            _authtime = Util.ToTLTime();
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

