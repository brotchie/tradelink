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
        public static string Summary(string space)
        {
            return space + " " + TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString() + ":" + TradeLink.Common.Util.DT2FT(DateTime.Now).ToString();
        }
        public AssemblaTicketWindow(string space, string user, string pass, string description, string data) : this(space, user, pass, description, data, Summary(space),string.Empty,string.Empty) { }
        
        public AssemblaTicketWindow(string space, string user, string pass, string description, string data,string summary,string suceedurl,string failurl)
        {
            InitializeComponent();
            assemblaTicketControl1.TicketFailed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketFailed);
            assemblaTicketControl1.TicketSucceed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketSucceed);
            assemblaTicketControl1.Update(space, summary, data, description,user,pass,suceedurl,failurl);
        }
        public const string LINE = "\r\n---------------------------------------------------------\r\n" ;
        public static string templatequest()
        {
            return "what was expected, and what happened instead?"+Environment.NewLine+Environment.NewLine+"What steps lead you to seeing this?" + Environment.NewLine + Environment.NewLine + "1. " + Environment.NewLine + "2." + Environment.NewLine + "3." +Environment.NewLine;
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
            Report(space, data, ex, showtemplate, user, pass, handlesuceed, pause,Summary(space));
        }
        public static string Header() { return Header("n/a", null); }
        public static string Header(string product) { return Header(product, null); }
        public static string Header(string product,Exception ex)
        {
            string[] r = new string[] { 
                "Product:" + product, 
                "Exception:" + (ex != null ? ex.Message : "n/a"), 
                "StackTrace:" + (ex != null ? ex.StackTrace : "n/a"), 
                "CommandLine:" + Environment.CommandLine, 
                "OS:" + Environment.OSVersion.VersionString + " " + (IntPtr.Size * 8).ToString() + "bit", 
                "CLR:" + Environment.Version.ToString(4), 
                "TradeLink:" + TradeLink.Common.Util.TLSIdentity(), 
                "Memory:" + Environment.WorkingSet.ToString(), 
                "Processors:" + Environment.ProcessorCount.ToString(), 
                "MID:" + Auth.GetNetworkAddress(), 
                "Date/Time: " + TradeLink.Common.Util.ToTLDate() + "/" + TradeLink.Common.Util.ToTLTime(), 
                "Culture: " + System.Globalization.CultureInfo.CurrentCulture.EnglishName };
            string desc = LINE+string.Join(Environment.NewLine, r)+LINE;
            return desc;


        }
        public static void Report(string space, string data, Exception ex, bool showtemplate, string user, string pass, LoginSucceedDel handlesuceed, bool pause,string summary)
        {
            Report(space, data, ex, (showtemplate ? templatequest() : string.Empty), user, pass, handlesuceed, pause,summary,string.Empty,string.Empty); 
        }
        public static void Report(string space, string data, Exception ex, string highlight, string user, string pass, LoginSucceedDel handlesuceed, bool pause, string summary)
        {
            Report(space, data, ex, highlight, user, pass, handlesuceed, pause, summary, string.Empty, string.Empty);
        }
        public static void Report(string space, string data, Exception ex, string highlight, string user, string pass, LoginSucceedDel handlesuceed, bool pause,string summary,string suceedurl, string failurl)
        {
            string header = Header(space, ex);
            AssemblaTicketWindow atw = 
                new AssemblaTicketWindow(space, user, pass, 
                    highlight + header,data,summary,suceedurl,failurl);
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

        public event LoginSucceedDel LoginSucceeded;
        public delegate void LoginSucceedDel(string u, string p);
        void assemblaTicketControl1_TicketSucceed()
        {
            if (LoginSucceeded!=null)
                LoginSucceeded(assemblaTicketControl1.user.Text, assemblaTicketControl1.pass.Text);
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
