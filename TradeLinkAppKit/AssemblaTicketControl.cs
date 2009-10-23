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
        public void Update(string SPACE, string summary, string description, string user, string pass) { Update(SPACE, summary, null, description, user, pass); }
        public void Update(string SPACE, string summary,  string data, string description, string user, string pass)
        {
            _summ.Text = summary;
            _space.Text = SPACE;
            _desc.Text = description;
            _user.Text = user;
            _pass.Text = pass;
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

        string path { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\"; } }

        public event VoidDelegate TicketSucceed;
        public event VoidDelegate TicketFailed;
        const string SSFILE = "ScreenShot.jpg";
        private void _create_Click(object sender, EventArgs e)
        {
            if (_summ.Text.Length == 0)
            {
                status("missing subject.");
                return;
            }
            int id = AssemblaTicket.Create(_space.Text, _user.Text, _pass.Text, _summ.Text,_desc.Text, AssemblaStatus.New, AssemblaPriority.Normal);
            if (id!=0)
            {
                if (attach)
                    if (!AssemblaDocument.Create(_space.Text, _user.Text, _pass.Text, path+SSFILE,id))
                        status("screenshot failed.");
                if (_attachdataasfile)
                    if (!AssemblaDocument.Create(_space.Text, _user.Text, _pass.Text, path + DATAFILE, id))
                        status("data attach failed.");
                System.Diagnostics.Process.Start(AssemblaTicket.GetTicketsUrl(_space.Text));
                if (TicketSucceed != null)
                    TicketSucceed();

            }
            else
            {
                status("login failed.");
                if (TicketFailed != null)
                    TicketFailed();
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
                _desc.Width = neww;
            int newh = (int)(ClientRectangle.Height - hdelta);
            if (newh != 0)
                _desc.Height = newh - (int)(_summ.Height*1.5);
            Invalidate(true);
        }

        private void AssemblaTicketControl_Load(object sender, EventArgs e)
        {
            if (_desc.Height!=0)
                hdelta = (double)ClientRectangle.Height - _desc.Height;
            delta = (double)_desc.Width / ClientRectangle.Width;
        }
        bool attach = false;
        private void _ss_Click(object sender, EventArgs e)
        {
            TradeLink.Common.ScreenCapture sc = new TradeLink.Common.ScreenCapture();
            sc.CaptureScreenToFile(path+SSFILE, System.Drawing.Imaging.ImageFormat.Jpeg);
            attach = true;
            
        }

        bool _attachdataasfile = false;
        const string DATAFILE = "Log.txt";
    }
}
