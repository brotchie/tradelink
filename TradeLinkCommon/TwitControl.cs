using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RareEdge.Twitteroo;

using TradeLink.API;
/*
 * to use this control it's recommended to add following to your application app.config file
 * 
<configuration>
 <settings>
            <servicePointManager expect100Continue="false" />
</settings>
</system.net>
</configuration>
 * 
 * 
 */


namespace TradeLink.Common
{
    /// <summary>
    /// allows user to communicate with other tradelink users via twitter
    /// (public communications only)
    /// </summary>
    public partial class TwitControl : UserControl
    {
        Timer _t = new Timer();
        string _nick = string.Empty;
        public TwitControl()
        {
            InitializeComponent();

            _nick = Environment.MachineName;
            _t.Interval = 3000;
            _t.Tick += new EventHandler(_t_Tick);
            _t.Start();
            status("send a twit, "+_nick);
            tc = new TwitterooCore(Util.decode(DATA),Util.decode(DATA));
        }

        void _t_Tick(object sender, EventArgs e)
        {
            updatestatus();
        }

        TwitterooCore tc;

        string _last = string.Empty;

        void status(string msg)
        {
            try
            {
                if (InvokeRequired)
                    Invoke(new DebugDelegate(status), new object[] { msg });
                else
                {
                    _msg.AppendText(Environment.NewLine + msg);
                    _msg.Invalidate(true);
                }
            }
            catch (Exception) { }
        }

        private void _twit_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Return)
            {
                if (_twit.Text.Length > 0)
                {
                    string msg = _nick + "> " + _twit.Text;
                    try
                    {
                        tc.ChangeStatus(msg);
                        _last = msg;
                    }
                    catch (Exception) { status("error occured."); }
                    _twit.Clear();
                }
                updatestatus();
            }
            

        }
        void updatestatus()
        {
            try
            {
                Users us = tc.GetTimeline(Timeline.Friends);
                for (int i = 0; i < us.Count; i++)
                {
                    Status s = us[i].Status;
                    if (!s.Text.Contains(_nick))
                    {
                        DateTime dt = s.CreatedAt;
                        status(dt.ToShortTimeString()+" "+s.Text);
                        break;
                    }
                }
            }
            catch (Exception) { status("unable to retrieve last message."); }
        }

        const string DATA = "7573655F74726164656C696E6B0D";

    }
}
