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
        public AssemblaTicketWindow(string space, string description)
        {
            InitializeComponent();
            string summary = space+" "+TradeLink.Common.Util.ToTLDate(DateTime.Now).ToString()+":"+TradeLink.Common.Util.DT2FT(DateTime.Now).ToString();
            assemblaTicketControl1 = new AssemblaTicketControl(space, summary, description);
            assemblaTicketControl1.TicketFailed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketFailed);
            assemblaTicketControl1.TicketSucceed += new TradeLink.API.VoidDelegate(assemblaTicketControl1_TicketSucceed);
            Show();
        }

        public static void Report(string space, System.Threading.ThreadExceptionEventArgs e)
        {
            Report(space, e.Exception);
        }
        public static void Report(string space)
        {
            string[] r = new string[] { "Product:" + space, "Exception: n/a" , "StackTrace: n/a" , "CommandLine:" + Environment.CommandLine, "OS:" + Environment.OSVersion.VersionString, "CLR:" + Environment.Version.ToString(4), "TradeLink:" + TradeLink.Common.Util.TLSIdentity(), "Memory:" + Environment.WorkingSet.ToString(), "Processors:" + Environment.ProcessorCount.ToString() };
            string desc = string.Join(Environment.NewLine, r);
            AssemblaTicketWindow atw = new AssemblaTicketWindow(space, desc);
        }
        public static void Report(string space, Exception ex)
        {
            string[] r = new string[] { "Product:" + space, "Exception:" + ex.Message, "StackTrace:" + ex.StackTrace, "CommandLine:" + Environment.CommandLine, "OS:" + Environment.OSVersion.VersionString, "CLR:" + Environment.Version.ToString(4), "TradeLink:" + TradeLink.Common.Util.TLSIdentity(), "Memory:" + Environment.WorkingSet.ToString(), "Processors:" + Environment.ProcessorCount.ToString() };
            string desc = string.Join(Environment.NewLine, r);
            AssemblaTicketWindow atw = new AssemblaTicketWindow(space, desc);
        }

        void assemblaTicketControl1_TicketSucceed()
        {
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
