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
        string _logname = string.Empty;
        public string Program { get { return _logname; } }
        int _date = Util.ToTLDate();
        /// <summary>
        /// gets current date associated with log
        /// </summary>
        public int Date { get { return _date; } }

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
            _dateinname = dateinlogname;
            _path = path;
            _append = appendtolog;
            _logname = logname;
            setfile();
            
        }

        bool _dateinname = true;

        void setfile()
        {
            fn = getfn(_path, _logname, _dateinname);
            try
            {
                try
                {
                    if (_log != null)
                        _log.Close();
                }
                catch { }
                _log = new StreamWriter(fn, _append);
                _log.AutoFlush = true;
            }
            catch (Exception) { _log = null; }
        }

        string _path = string.Empty;
        bool _append = true;

        string getfn(string path, string logname, bool dateinlogname)
        {
            string fn = string.Empty;
            int inst = -1;
            do
            {
                inst++;
                string inststring = inst < 0 ? string.Empty : "." + inst.ToString(); 
                fn = path + "\\" + logname + (dateinlogname ? "." + _date : "") + inststring + ".txt";
            } while (!TikUtil.IsFileWritetable(fn));
            return fn;
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
            if (!isEnabled) 
                return;
            try
            {
                if (_log != null)
                {
                    StringBuilder sb = new StringBuilder();
                    if (_timestamps)
                    {
                        DateTime now = DateTime.Now;
                        // see if date changed
                        int newdate = Util.ToTLDate(now);
                        // if date has changed, start new file
                        if (newdate != _date)
                        {
                            _date = newdate;
                            setfile();
                        }
                        sb.Append(now.ToString("HHmmss"));
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
