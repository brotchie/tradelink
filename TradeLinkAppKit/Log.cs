using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeLink.API;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    public class Log
    {

        StreamWriter _log = null;
        public bool isEnabled = true;
        public Log(string name) : this(name, true, true, Environment.CurrentDirectory) { }
        public Log(string logname, bool dateinlogname,bool appendtolog,string path)
        {
            string fn = path+"\\"+logname+(dateinlogname ? "."+TradeLink.Common.Util.ToTLDate(DateTime.Now): "") + ".txt";
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
