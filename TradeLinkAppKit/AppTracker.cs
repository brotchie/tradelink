using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;
using System.Net;

namespace TradeLink.AppKit
{
    /// <summary>
    /// record application custom application events, mouse-clicks, etc to a URL
    /// </summary>
    
    public partial class AppTracker : System.Windows.Forms.Form
    {
        public AppTracker() :base() { }
        public AppTracker(string Program, string url) :this()
        {
            _URL = url;
            _TAG = Program;
        }
        string _URL = @"http://tradelinkappstore.appspot.com/apptracker";
        /// <summary>
        /// url to post requests
        /// </summary>
        public string TrackUrl { get { return _URL; } set { _URL = value; } }
        string _TAG = string.Empty;
        /// <summary>
        /// program to post as
        /// </summary>
        public string Program { get { return _TAG; } set { _TAG = value; } }

        bool _autostartstop = true;
        /// <summary>
        /// send start and stop actions automatically at load and form close
        /// </summary>
        public bool AutoStartStop { get { return _autostartstop; } set { _autostartstop = value; } }

        bool _track = true;
        /// <summary>
        /// enable or disable server tracking (debugging still tracked)
        /// </summary>
        public bool TrackEnabled { get { return _track; } set { _track = value; } }
        BackgroundWorker _bw = new BackgroundWorker();
        bool _go = true;
        public event DebugDelegate SendDebug;

        protected override void OnClosed(EventArgs e)
        {
            UnhookControl(this as Control);
            base.OnClosed(e);
        }

        void debug(string msg)
        {
            if (SendDebug != null)
            {
                try
                {
                    SendDebug(msg);
                }
                catch { }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            debug("loading apptracker for " + Program);
            base.OnLoad(e);
            
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);
            _bw.RunWorkerAsync();
            if (_autostartstop)
                Track(TrackType.AppStart);
            HookControl(this as Control);
        }

        bool _postallonclose = true;
        /// <summary>
        /// push any untracked events when form is closed
        /// </summary>
        public bool PushTracksOnClose { get { return _postallonclose; } set { _postallonclose = value; } }

        int _maxattemps = 6;
        /// <summary>
        /// maximum number of attempts to push tracks on close
        /// </summary>
        public int PushTracksCloseMax { get { return _maxattemps; } set { _maxattemps = value; } }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            debug("apptracker stopping for " + Program);
            if (_autostartstop)
                Track(TrackType.AppStop);
            if (PushTracksOnClose)
                debug("Pushing tracks on close and waiting...");
            int closeattempts = 0;
            while (PushTracksOnClose && _untrackedqueue.hasItems && (((_maxattemps!=0) && (closeattempts++<_maxattemps)) || (_maxattemps==0)))
            {
                
                _SLEEP = 10;
                System.Threading.Thread.Sleep(100);
            }
            debug("Canceling background threads");
            _go = false;
            _bw.CancelAsync();
            debug("apptracker closed");
            base.OnFormClosing(e);
            
        }
        int _SLEEP = 5000;
        /// <summary>
        /// wait between tracks
        /// </summary>
        public int InterTrackSleep { get { return _SLEEP; } set { _SLEEP = value; } }
        RingBuffer<Track> _untrackedqueue = new RingBuffer<Track>(1000);

        /// <summary>
        /// count of unprocessed items
        /// </summary>
        public int UnprocessedItemCount { get { return _untrackedqueue.Count; } }

        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient wc = new WebClient();
            while (_go)
            {
                if (e.Cancel) break;
                
                if (TrackUrl == string.Empty) continue;
                while (!_untrackedqueue.isEmpty && TrackEnabled)
                {
                    if (e.Cancel) break;
                    // get item
                    Track t = _untrackedqueue.Read();
                    try
                    {
                        wc.UploadValues(TrackUrl, "POST", t.ToQuery());
                    }
                    catch (Exception ex)
                    {
                        debug("error uploading: " + t.ToQuery() + " " + ex.Message + ex.StackTrace);
                    }
                    System.Threading.Thread.Sleep((int)((double)_SLEEP / 10));
                }
                System.Threading.Thread.Sleep(_SLEEP);
                
            }
        }

        private void HookControl(Control controlToHook)
        {
            controlToHook.MouseClick += AllControlsMouseClick;
            foreach (Control ctl in controlToHook.Controls)
            {
                HookControl(ctl);
            }
        }

        private void UnhookControl(Control controlToUnhook)
        {
            controlToUnhook.MouseClick -= AllControlsMouseClick;
            foreach (Control ctl in controlToUnhook.Controls)
            {
                UnhookControl(ctl);
            }
        }

        bool _trackclicks = true;
        /// <summary>
        /// track mouse clicks as apptracker events
        /// </summary>
        public bool TrackClicks { get { return _trackclicks; } set { _trackclicks = value; } }

        void AllControlsMouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Control f = (Control)sender;
                Track(TrackType.ClickAction, f.Name);
            }
            catch (InvalidCastException)
            {
                try
                {
                    Form f = (Form)sender;
                    Track(TrackType.ClickAction, f.Name);
                }
                catch (InvalidCastException ex)
                {
                    debug("can't cast: " + sender.ToString() + " " + ex.Message + ex.StackTrace);
                }
            }
            

        }

        int _tc = 0;
        /// <summary>
        /// count of how many events/actions have been tracked in total for this session
        /// </summary>
        public int TrackCount { get { return _tc; } }

        /// <summary>
        /// track an event
        /// </summary>
        /// <param name="type"></param>
        public void Track(TrackType type) { Track(type, string.Empty); }
        /// <summary>
        /// track an event with custom data
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        public void Track(TrackType type, string data)
        {
            // get clicked item name
            Track t = new Track(type, data);
            t.id = Auth.GetNetworkAddress();
            t.tag = Program;
            debug(_tc.ToString() + " " + t.ToString());
            if (SendTrackEvent != null)
                SendTrackEvent(t);
            if (!TrackEnabled) return;
            _tc++;

            
            _untrackedqueue.Write(t);
        }

        public event TrackDelegate SendTrackEvent;



        /// <summary>
        /// track a numeric event
        /// </summary>
        /// <param name="t"></param>
        /// <param name="val"></param>
        public void Track(TrackType t, decimal val)
        {
            Track(t, val.ToString("F2"));
        }
        /// <summary>
        /// track a score update
        /// </summary>
        /// <param name="points"></param>
        public void Score(decimal points)
        {
            Track(TrackType.ScorePoints, points);
        }
    }

    public delegate void TrackDelegate(Track t);

    public struct Track
    {
        public string id;
        public string tag;
        public string data;
        public int date;
        public int time;
        public TrackType type;
        public Track(TrackType t)
        {
            id = string.Empty;
            tag = string.Empty;
            data = string.Empty;
            date = Util.ToTLDate();
            time = Util.ToTLTime();
            type = t;
        }

        public Track(TrackType t,string datamsg)
        {
            id = string.Empty;
            tag = string.Empty;
            data = datamsg;
            date = Util.ToTLDate();
            time = Util.ToTLTime();
            type = t;
        }

        public System.Collections.Specialized.NameValueCollection ToQuery()
        {
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
            nvc.Add("ID", id);
            nvc.Add("TAG", tag);
            nvc.Add("DATA", data);
            nvc.Add("TYPE", type.ToString());
            nvc.Add("DATE", date.ToString());
            nvc.Add("TIME", time.ToString());
            return nvc;
        }
        public override string ToString()
        {
            return date.ToString() + time.ToString() + " " + id + " " + tag + " " + data;
        }
    }

    public enum TrackType
    {
        Unknown = 0,
        AppLog,
        AppStart,
        AppStop,
        AppCrash,
        AppBugReport,
        ClickAction,
        ScorePoints,
        AppAuthOk,
        AppAuthFail,
    }
}
