using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradeLink.API;
using TradeLink.Common;
[assembly: CLSCompliant(true)]

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
        /// <summary>
        /// full path of log file
        /// </summary>
        public string FullName { get { return fn; } }
        private StringBuilder _content = new StringBuilder();
        /// <summary>
        /// contents of log file
        /// </summary>
        public string Content  { get { return _content.ToString(); } }
        /// <summary>
        /// create a log
        /// </summary>
        /// <param name="program"></param>
        public Log(string program) : this(program, true, true, Util.ProgramData(program),true) { }
        bool _timestamps = true;
        /// <summary>
        /// create a log
        /// </summary>
        /// <param name="logname"></param>
        /// <param name="dateinlogname"></param>
        /// <param name="appendtolog"></param>
        /// <param name="path"></param>
        /// <param name="timestamps"></param>
        public Log(string logname, bool dateinlogname,bool appendtolog,string path, bool timestamps)
        {
            _timestamps = timestamps;
            fn = path+"\\"+logname+(dateinlogname ? "."+TradeLink.Common.Util.ToTLDate(DateTime.Now): "") + ".txt";
            try
            {
                _log = new StreamWriter(fn,true);
                _log.AutoFlush = true;
            }
            catch (Exception ex) { _log = null; }
        }
        public event DebugFullDelegate SendDebug;
        /// <summary>
        /// log something
        /// </summary>
        /// <param name="msg"></param>
        public void GotDebug(Debug msg) 
        {
            if (SendDebug != null)
                SendDebug(msg);
            if (!isEnabled) return;
            try
            {
                if (_log != null)
                {
                    StringBuilder sb = new StringBuilder();
                    if (_timestamps)
                    {
                        sb.Append(DateTime.Now.ToString("HHmmss"));
                        sb.Append(": ");
                    }
                    sb.Append(msg.Msg);
                    _log.WriteLine(sb.ToString());
                    _content.Append(sb.ToString());
                }
            }
            catch { }
        }
        /// <summary>
        /// log something
        /// </summary>
        /// <param name="msg"></param>
        public void GotDebug(string msg)
        {
            GotDebug(DebugImpl.Create(msg));
        }
        /// <summary>
        /// close the log
        /// </summary>
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
