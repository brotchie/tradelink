using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using TradeLink.Common;
using System.Net;

namespace TradeLink.AppKit
{
    public partial class AppTracker : System.Windows.Forms.Form
    {
        string _URL = string.Empty;
        public string TrackUrl { get { return _URL; } set { _URL = value; } }
        BackgroundWorker _bw = new BackgroundWorker();
        bool _go = true;

        protected override void OnClosed(EventArgs e)
        {
            UnhookControl(this as Control);
            base.OnClosed(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _bw.WorkerSupportsCancellation = true;
            _bw.DoWork += new DoWorkEventHandler(_bw_DoWork);

            HookControl(this as Control);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _go = false;
            _bw.CancelAsync();
            base.OnFormClosing(e);
        }
        int _SLEEP = 5000;
        public int TrackSleep { get { return _SLEEP; } set { _SLEEP = value; } }
        RingBuffer<Track> _untrackedqueue = new RingBuffer<Track>(1000);
        void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            WebClient wc = new WebClient();
            while (_go)
            {
                if (e.Cancel) break;
                System.Threading.Thread.Sleep(TrackSleep);
                if (TrackUrl == string.Empty) continue;
                while (!_untrackedqueue.isEmpty)
                {
                    if (e.Cancel) break;
                    // get item
                    Track t = _untrackedqueue.Read();
                    wc.UploadValues(TrackUrl, t.ToQuery());
                    System.Threading.Thread.Sleep((int)((double)TrackSleep / 10));
                }
                
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

        void AllControlsMouseClick(object sender, MouseEventArgs e)
        {
            Form f = (Form)sender;
            // get clicked item name
            Track t;
            t.name = f.Name;
            t.date = Util.ToTLDate();
            t.time = Util.ToTLTime();
            _untrackedqueue.Write(t);
        }
    }

    public struct Track
    {
        public string name;
        public int date;
        public int time;
        public System.Collections.Specialized.NameValueCollection ToQuery()
        {
            System.Collections.Specialized.NameValueCollection nvc = new System.Collections.Specialized.NameValueCollection();
            nvc.Add("name", name);
            nvc.Add("date", date.ToString());
            nvc.Add("time", time.ToString());
            return nvc;
        }
    }
}
