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
        public AssemblaTicketControl(string SPACE, string summary, string description)
        {
            InitializeComponent();
            _summ.Text = summary;
            _space.Text = SPACE;
            _desc.Text = description;
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
                status("failed.  check info, try again.");
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
    }
}
