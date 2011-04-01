using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;


namespace TradeLink.AppKit
{
    public partial class AssemblaTicketControl : UserControl
    {
        public AssemblaTicketControl()
        {
            InitializeComponent();
        }

        public void Update(string SPACE, string summary, string description) { Update(SPACE, summary, description, string.Empty, string.Empty); }
        public void Update(string SPACE, string summary, string description, string user, string pass) { Update(SPACE, summary, null, description, user, pass,string.Empty,string.Empty); }
        public void Update(string SPACE, string summary,  string data, string description, string username, string password, string succeedurl, string failurl)
        {
            summ.Text = summary;
            space.Text = SPACE;
            desc.Text = description;
            user.Text = username;
            pass.Text = password;
            if (data != null)
            {
                try
                {
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path+ DATAFILE, false);
                    sw.WriteLine(data);
                    sw.Close();
                    _attachdataasfile = true;
                }
                catch 
                {
                    _attachdataasfile = false;
                }
            }

            Invalidate(true);
        }

        string path { get { return TradeLink.Common.Util.ProgramData(space.Text); } }

        public event VoidDelegate TicketSucceed;
        public event VoidDelegate TicketFailed;
        const string SSFILE = "ScreenShot.jpg";
        public string FailUrl = string.Empty;
        public string SucceedUrl = string.Empty;
        private void _create_Click(object sender, EventArgs e)
        {
            if (summ.Text.Length == 0)
            {
                status("missing subject.");
                return;
            }
            if (SucceedUrl==string.Empty)
                SucceedUrl = AssemblaTicket.GetTicketUrl(space.Text);
            if (FailUrl == string.Empty)
                FailUrl = null;
            
            int id = AssemblaTicket.Create(space.Text, user.Text, pass.Text, summ.Text,desc.Text, TicketStatus.New, Priority.Normal);
            if (id!=0)
            {
                if (attach)
                    if (!AssemblaDocument.Create(space.Text, user.Text, pass.Text, path+SSFILE,id))
                        status("screenshot failed.");
                if (_attachdataasfile)
                    if (!AssemblaDocument.Create(space.Text, user.Text, pass.Text, path + DATAFILE, id))
                        status("data attach failed.");
                if (SucceedUrl!=null)
                    System.Diagnostics.Process.Start(SucceedUrl);
                if (TicketSucceed != null)
                    TicketSucceed();

            }
            else
            {
                status("login failed.");
                if (TicketFailed != null)
                    TicketFailed();
                if (FailUrl!=null)
                    System.Diagnostics.Process.Start(FailUrl);

            }

        }

        void status(string msg)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(status), new object[] { msg });
            else
            {
                _stat.Text = msg;
                _stat.Invalidate();
            }
        }

        double delta = 0;
        double  hdelta = 0;
        private void AssemblaTicketControl_SizeChanged(object sender, EventArgs e)
        {
            int neww = (int)(ClientRectangle.Width * delta);
            if (neww != 0)
                desc.Width = neww;
            int newh = (int)(ClientRectangle.Height - hdelta);
            if (newh != 0)
                desc.Height = newh - (int)(summ.Height*1.5);
            Invalidate(true);
        }

        private void AssemblaTicketControl_Load(object sender, EventArgs e)
        {
            if (desc.Height!=0)
                hdelta = (double)ClientRectangle.Height - desc.Height;
            delta = (double)desc.Width / ClientRectangle.Width;
        }
        bool attach = false;
        private void _ss_Click(object sender, EventArgs e)
        {
            TradeLink.Common.ScreenCapture sc = new TradeLink.Common.ScreenCapture();
            sc.CaptureScreenToFile(path+SSFILE, System.Drawing.Imaging.ImageFormat.Jpeg);
            attach = true;
            _ss.BackColor = Color.Blue;
            _ss.Invalidate();
        }

        bool _attachdataasfile = false;
        const string DATAFILE = "Log.txt";

        private void _screencast_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://screenr.com/record");
            status("remember to place video link in ticket.");
        }
    }
}
