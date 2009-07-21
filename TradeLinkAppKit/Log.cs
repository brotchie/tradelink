using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    /// <summary>
    /// TradeLink logging to a file
    /// </summary>
    public class Log
    {

        StreamWriter _log = null;
        public bool isEnabled = true;
        private string fn = string.Empty;
        public string FullName { get { return fn; } }
        public string Content
        {
            get
            {
                if (fn == string.Empty) return string.Empty;
                try
                {
                    StreamReader sr = new StreamReader(fn);
                    return sr.ReadToEnd();
                }
                catch { return string.Empty; }
            }
        }
        public Log(string name) : this(name, true, true, Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) { }
        public Log(string logname, bool dateinlogname,bool appendtolog,string path)
        {
            fn = path+"\\"+logname+(dateinlogname ? "."+TradeLink.Common.Util.ToTLDate(DateTime.Now): "") + ".txt";
            try
            {
                _log = new StreamWriter(fn, appendtolog);
                _log.AutoFlush = true;
            }
            catch (Exception ex) { _log = null; }
        }
        public event DebugFullDelegate SendDebug;

        public void GotDebug(Debug msg) 
        {
            if (SendDebug != null)
                SendDebug(msg);
            if (!isEnabled) return;
            try
            {
                if (_log != null)
                    _log.WriteLine(DateTime.Now.ToString("HHmmss") + ": " + msg.Msg);
            }
            catch { }
        }
        public void GotDebug(string msg)
        {
            GotDebug(DebugImpl.Create(msg));
        }
        public void Stop()
        {
            try
            {
                if (_log != null) _log.Close();
            }
            catch { }
        }
    }
}
