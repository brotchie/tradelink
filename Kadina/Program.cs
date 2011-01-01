using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Kadina
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                Application.Run(new kadinamain());
            }
            catch (Exception e)
            {
                report(e,true);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            report((Exception)e.ExceptionObject);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            report(e.Exception);
        }

        static void report(Exception e) { report(e, false); }
        static void report(Exception e, bool pause)
        {
            if (Properties.Settings.Default.portal == string.Empty)
                TradeLink.AppKit.CrashReport.Report(kadinamain.PROGRAM, e);
            else
                TradeLink.AppKit.ATW.Report(Properties.Settings.Default.portal, string.Empty, e, true, Properties.Settings.Default.user, Properties.Settings.Default.pw, new TradeLink.AppKit.AssemblaTicketWindow.LoginSucceedDel(kadinamain.success), true);
        }
    }
}