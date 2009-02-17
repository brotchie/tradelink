using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.Common
{
    public partial class NewVersion : Form
    {
        public NewVersion(string ProgramName, string ProgramUrl) : this(ProgramName, ProgramUrl, "") { }
        public NewVersion(string ProgramName, string ProgramUrl, string msg)
        {
            InitializeComponent();
            Text = ProgramName + " Update Available";
            if (msg!="") statuslab.Text = msg;
            urlloclab.Links.Add(0, 100, ProgramUrl);
            urlloclab.LinkClicked += new LinkLabelLinkClickedEventHandler(urlloclab_LinkClicked);
        }

        void urlloclab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }
    }
}
