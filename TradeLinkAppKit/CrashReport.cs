using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GCodeIssueTracker;
using GCore;
using TradeLink.API;
using TradeLink.Common;

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
        public CrashReport(string program, string username, string password, Exception ex) : this(program, username,password,ex, string.Empty) { }
        public CrashReport(string program, string username, string password, Exception ex, string data) : this(program, username, password, ex, data, Desc(program)) { }
        public CrashReport(string program, string username, string password, Exception ex, string data, string desc)
        {
            InitializeComponent();
            PROGRAM = program;
            DATA = data;
            EX = ex;
            user(username);
            pw(password);
            descrip(desc);
            body(DecodedBody(PROGRAM, EX, DATA, true));
            FormClosing += new FormClosingEventHandler(CrashReport_FormClosing);
            SizeChanged += new EventHandler(CrashReport_SizeChanged);
            Load += new EventHandler(CrashReport_Load);
            Invalidate(true);
        }

        void body(string txt)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(body), new object[] { txt });
            else
            {
                _body.Text = txt;
            }
        }

        void user(string txt)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(user), new object[] { txt });
            else
            {
                _user.Text = txt;
            }
        }

        void pw(string txt)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(pw), new object[] { txt });
            else
            {
                _pass.Text = txt;
            }
        }
        void descrip(string txt)
        {
            if (InvokeRequired)
                Invoke(new DebugDelegate(descrip), new object[] { txt });
            else
            {
                _desc.Text = txt;
            }
        }

        void CrashReport_Load(object sender, EventArgs e)
        {
            if (_body.Height != 0)
                hdelta = (double)ClientRectangle.Height - _body.Height;
            delta = (double)_body.Width / ClientRectangle.Width;
        }
        double delta = 0;
        double hdelta = 0;
        void CrashReport_SizeChanged(object sender, EventArgs e)
        {
            int neww = (int)(ClientRectangle.Width * delta);
            if (neww != 0)
                _body.Width = neww;
            int newh = (int)(ClientRectangle.Height - hdelta);
            if (newh != 0)
                _body.Height = newh - (int)(_desc.Height * 1.5);
            Invalidate(true);
        }

        public event AssemblaTicketWindow.LoginSucceedDel TicketSucceed;

        void CrashReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        public static string Desc(string program)
        {
            return program+ " Bug (" + TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString() + ")";
        }
        private void _email_Click(object sender, EventArgs e)
        {
            ProjectHostingService service = new ProjectHostingService("tradelink", _user.Text, _pass.Text);
            var issue = new IssuesEntry();
            issue.Author = new Author { Name = _user.Text };
            issue.Title = _desc.Text;
            issue.Content = new Content{ Type = "text", Description =  _body.Text };
            issue.Status = "New";
            try
            {
                int id = service.SubmitNewIssue(issue, PROGRAM).Id;
                System.Diagnostics.Process.Start("http://code.google.com/p/tradelink/issues/detail?id=" + id);
                if (TicketSucceed != null)
                    TicketSucceed(_user.Text, _pass.Text);
                Close();
            }
            catch (Exception)
            {
                Text = "Authentication failed.";
                Invalidate(true);
            }
        }



        static string template()
        {
            return "What did you expect to see, what did you see instead?"+Environment.NewLine+Environment.NewLine+"What steps led to seeing this error?" + Environment.NewLine + Environment.NewLine + "1. " + Environment.NewLine + "2." + Environment.NewLine + "3." + Environment.NewLine + Environment.NewLine + "---------------------------------------------------------" + Environment.NewLine;
        }

        static string DecodedBody(string program, Exception ex, string data, bool addtemplate)
        {


            string[] r = new string[] { (addtemplate ? template() : string.Empty), "App:" + program, "Err:" + (ex != null ? ex.Message : "n/a"), "Trace:" + (ex != null ? ex.StackTrace : "n/a"), "OS:" + Environment.OSVersion.VersionString + " " + (IntPtr.Size * 8).ToString() + "bit", "CLR:" + Environment.Version.ToString(4), "TL:" + TradeLink.Common.Util.TLSIdentity(), "Mem:" + Environment.WorkingSet.ToString(), "Proc:" + Environment.ProcessorCount.ToString(), "MID:" + Auth.GetNetworkAddress(), "Date/Time: " + Util.ToTLDate() + "/" + Util.ToTLTime(), "Culture: " + System.Globalization.CultureInfo.CurrentCulture.EnglishName,data };

            string decoded = string.Join(Environment.NewLine, r);
            return decoded;

        }


        static string d = "74726164656C696E6B6D61696C21";
        static string D = "74726164656C696E6B6D61696C";

        public static void Report(string PROGRAM, string username, string password, System.Threading.ThreadExceptionEventArgs e, AssemblaTicketWindow.LoginSucceedDel success) { Report(PROGRAM, username, password, string.Empty,e.Exception,success,true); }
        public static void Report(string PROGRAM, System.Threading.ThreadExceptionEventArgs e) { Report(PROGRAM, Util.decode(D), Util.decode(d), string.Empty, e.Exception, null, true); }
        public static void Report(string PROGRAM, Exception ex) { Report(PROGRAM, Util.decode(D), Util.decode(d) , string.Empty, ex, null, true); }
        public static void Report(string PROGRAM, string username, string password, string data, Exception ex, AssemblaTicketWindow.LoginSucceedDel success, bool pause)
        {
            Report(PROGRAM, username, password, data, ex, success, pause, Desc(PROGRAM));
        }
        public static void Report(string PROGRAM, string data, Exception ex, AssemblaTicketWindow.LoginSucceedDel success, bool pause, string desc) { Report(PROGRAM, Util.decode(D), Util.decode(d), data, ex, success, pause, desc); }
        public static void Report(string PROGRAM, string username, string password, string data, Exception ex,AssemblaTicketWindow.LoginSucceedDel success,bool pause,string desc)
        {
            CrashReport cr = new CrashReport(PROGRAM, username, password, ex,data,desc);
            if (success!=null)
                cr.TicketSucceed+=new AssemblaTicketWindow.LoginSucceedDel(success);
            if (pause)
                cr.ShowDialog();
            else
                cr.Show();
        }

        private void _user_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.com/accounts/NewAccount");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.google.com/accounts/NewAccount");
        }



    }
}
