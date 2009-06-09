using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.AppKit
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
            string[] r = new string[] { "Product:"+program, "Exception:"+ex.Message, "StackTrace:"+ex.StackTrace, "CommandLine:"+Environment.CommandLine, "OS:"+Environment.OSVersion.VersionString, "CLR:"+Environment.Version.ToString(4), "TradeLink:"+TradeLink.Common.Util.TLSIdentity(),"Memory:"+Environment.WorkingSet.ToString(), "Processors:"+Environment.ProcessorCount.ToString() };
            BODY = string.Join(Environment.NewLine, r);
            InitializeComponent();
            ShowDialog();
        }
        const string email = "tradelink-contribute@googlegroups.com";
        const string webase = "http://groups.google.com/group/tradelink-contribute/post?";
        private void _email_Click(object sender, EventArgs e)
        {
            if (BODY != string.Empty)
            {
                BODY = "what steps led to seeing this error?" + Environment.NewLine + Environment.NewLine+"1. " + Environment.NewLine + "2." + Environment.NewLine +"3."+Environment.NewLine+ Environment.NewLine + "---------------------------------------------------------"+Environment.NewLine+BODY;
                string bodystring = Uri.EscapeUriString(BODY);
                string url = webase + string.Format("subject={0}&body={1}", PROGRAM + " Crash ("+TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString()+")", bodystring);
                System.Diagnostics.Process.Start(url);
            }
                //Email.Send(email, PROGRAM + "." + Util.ToTLDate(DateTime.Now), BODY);
            Close();
        }

        private void _ignore_Click(object sender, EventArgs e)
        {
            Close();
        }
        public static void Report(string PROGRAM, System.Threading.ThreadExceptionEventArgs e) { Report(PROGRAM, e.Exception); }
        public static void Report(string PROGRAM, Exception ex)
        {
            CrashReport cr = new CrashReport(PROGRAM, ex);
        }
    }
}
