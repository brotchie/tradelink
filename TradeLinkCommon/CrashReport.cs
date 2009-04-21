using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.Common
{
    /// <summary>
    /// display a tradelink crash window rather than standard windows form.
    /// </summary>
    public partial class CrashReport : Form
    {
        string PROGRAM = string.Empty;
        string BODY = string.Empty;
        public CrashReport(string program, Exception ex)
        {
            PROGRAM = program;
            string[] r = new string[] { "APP", ex.Message, ex.StackTrace, Environment.CommandLine, "SYSTEM", Environment.OSVersion.VersionString, Environment.Version.ToString(4), Environment.WorkingSet.ToString(), Environment.ProcessorCount.ToString() };
            BODY = string.Join(Environment.NewLine, r);
            InitializeComponent();
            ShowDialog();
        }
        const string email = "tradelink-contribute@googlegroups.com";
        private void _email_Click(object sender, EventArgs e)
        {
            if (BODY!=string.Empty)
                Email.Send(email, PROGRAM + "." + Util.ToTLDate(DateTime.Now), BODY);
            Close();
        }

        private void _ignore_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static void Report(string PROGRAM, Exception ex)
        {
            CrashReport cr = new CrashReport(PROGRAM, ex);
        }
    }
}
