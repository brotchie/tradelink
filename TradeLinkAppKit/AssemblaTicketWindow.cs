using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.AppKit
{
    public partial class AssemblaTicketWindow : Form
    {
        public AssemblaTicketWindow()
        {
            InitializeComponent();
        }
        public AssemblaTicketWindow(string space, string user, string pass, string description, string data)
        {
            InitializeComponent();
            string summary = space+" "+TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString()+":"+TradeLink.Common.Util.DT2FT(DateTime.Now).ToString();
            assemblaTicketControl1.TicketFailed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketFailed);
            assemblaTicketControl1.TicketSucceed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketSucceed);
            assemblaTicketControl1.Update(space, summary, data, description,user,pass);
        }

        static string templatequest() { return templatequest(string.Empty); }
        static string templatequest(string reportdata)
        {
            return "what was expected, and what happened instead?"+Environment.NewLine+Environment.NewLine+"What steps lead you to seeing this?" + Environment.NewLine + Environment.NewLine + "1. " + Environment.NewLine + "2." + Environment.NewLine + "3." + Environment.NewLine + Environment.NewLine + "---------------------------------------------------------" + Environment.NewLine + reportdata;
        }
        public static string LogData(string path)
        {
            try
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(path);
                return sr.ReadToEnd();
            }
            catch { }
            return string.Empty;
        }
        public static void Report(string space, Log log, System.Threading.ThreadExceptionEventArgs e) { Report(space, log.Content, e.Exception); }
        public static void Report(string space, string data, System.Threading.ThreadExceptionEventArgs e) { Report(space, data, e.Exception); }
        public static void Report(string space, Log log) { Report(space, log.Content, (Exception)null); }
        public static void Report(string space, Log log, Exception ex) { Report(space, log.Content, ex, true); }
        public static void Report(string space, string data) { Report(space, data, null, true); }
        public static void Report(string space, string data, Exception ex) { Report(space, data, ex, true); }
        public static void Report(string space, string data, Exception ex, bool showtemplate) { Report(space, data, ex, showtemplate, string.Empty, string.Empty,null,false); }
        public static void Report(string space, string data, Exception ex, bool showtemplate, string user, string pass, LoginSucceedDel handlesuceed, bool pause)
        {
            string[] r = new string[] { "Product:" + space, "Exception:" + (ex != null ? ex.Message : "n/a"), "StackTrace:" + (ex != null ? ex.StackTrace : "n/a"), "CommandLine:" + Environment.CommandLine, "OS:" + Environment.OSVersion.VersionString + " " + (IntPtr.Size * 8).ToString() + "bit", "CLR:" + Environment.Version.ToString(4), "TradeLink:" + TradeLink.Common.Util.TLSIdentity(), "Memory:" + Environment.WorkingSet.ToString(), "Processors:" + Environment.ProcessorCount.ToString() };
            string desc = string.Join(Environment.NewLine, r);
            AssemblaTicketWindow atw = new AssemblaTicketWindow(space, user, pass, showtemplate ? templatequest(desc) : desc,data);
            if (ex != null)
            {
                atw.Text = "Create ticket for crash report";
                atw.Invalidate(true);
            }
            if (handlesuceed != null)
            {
                atw.LoginSucceeded+=new LoginSucceedDel(handlesuceed);
            }
            if (pause)
                atw.ShowDialog();
            else
                atw.Show();
        }

        public delegate void LoginSucceedDel(string u, string p);
        public event LoginSucceedDel LoginSucceeded;
        void assemblaTicketControl1_TicketSucceed()
        {
            if (LoginSucceeded!=null)
                LoginSucceeded(assemblaTicketControl1._user.Text, assemblaTicketControl1._pass.Text);
            Close();
        }

        void assemblaTicketControl1_TicketFailed()
        {
            
        }
    }

    public class ATW : AssemblaTicketWindow
    {
    }
}
