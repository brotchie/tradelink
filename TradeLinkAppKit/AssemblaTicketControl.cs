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
        public void Update(string SPACE, string summary, string description, string user, string pass)
        {
            _summ.Text = summary;
            _space.Text = SPACE;
            _desc.Text = description;
            _user.Text = user;
            _pass.Text = pass;
            Invalidate(true);
        }

        public event VoidDelegate TicketSucceed;
        public event VoidDelegate TicketFailed;

        private void _create_Click(object sender, EventArgs e)
        {
            if (AssemblaTicket.Create(_space.Text, _user.Text, _pass.Text, _summ.Text,_desc.Text, AssemblaStatus.New, AssemblaPriority.Normal))
            {
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
    }
}
