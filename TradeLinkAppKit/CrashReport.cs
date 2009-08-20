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
        Exception EX = null;
        string DATA = string.Empty;
        public CrashReport(string program, Exception ex) : this(program, ex, string.Empty) { }
        public CrashReport(string program, Exception ex, string data)
        {
            InitializeComponent();
            PROGRAM = program;
            DATA = data;
            EX = ex;
            ShowDialog();
        }

        public const string DEVELOPERURL = "http://groups.google.com/group/tradelink-contribute/post?";
        public static string Desc(string program)
        {
            return program+ " Bug (" + TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString() + ")";
        }
        private void _email_Click(object sender, EventArgs e)
        {
            BugReport(PROGRAM, Desc(PROGRAM), EX, DATA);
            Close();
        }

        public static string DeveloperIssuePostURL(string program, string desc, Exception ex, string data)
        {
            string bodystring = Body(program, ex, data,true);
            return DEVELOPERURL + string.Format("subject={0}&body={1}", desc, bodystring);
        }

        public static void BugReport(string program, string data)
        {
            BugReport(program, Desc(program), null, data);
        }
        public static void BugReport(string program, string desc, Exception ex, string data)
        {
            System.Diagnostics.Process.Start(DeveloperIssuePostURL(program, desc, ex, data));

        }

        static string template()
        {
            return "What did you expect to see, what did you see instead?"+Environment.NewLine+Environment.NewLine+"What steps led to seeing this error?" + Environment.NewLine + Environment.NewLine + "1. " + Environment.NewLine + "2." + Environment.NewLine + "3." + Environment.NewLine + Environment.NewLine + "---------------------------------------------------------" + Environment.NewLine;
        }

        static string Body(string program) { return Body(program, (Exception)null); }
        static string Body(string program, string data) { return Body(program, null, data,true); }
        static string Body(string program, Exception ex) { return Body(program, ex, string.Empty,true); }
        static string Body(string program, Exception ex,string data, bool addtemplate)
        {

            string[] r = new string[] { (addtemplate ? template() : string.Empty), "Product:" + program, "Exception:" + (ex != null ? ex.Message : "n/a"), "StackTrace:" + (ex != null ? ex.StackTrace : "n/a"), "CommandLine:" + Environment.CommandLine, "OS:" + Environment.OSVersion.VersionString+" "+(IntPtr.Size*8).ToString()+"bit", "CLR:" + Environment.Version.ToString(4), "TradeLink:" + TradeLink.Common.Util.TLSIdentity(), "Memory:" + Environment.WorkingSet.ToString(), "Processors:" + Environment.ProcessorCount.ToString(), data };

            string decoded = string.Join(Environment.NewLine, r);
            return Uri.EscapeUriString(decoded);

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
