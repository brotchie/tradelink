using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASP
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
            ASP app = null;
            try
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                app = new ASP();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                report(ex,true);
            }
            try
            {
                app.Stop();
            }
            catch { }
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
        static void report(Exception e,bool pause)
        {
            if (Properties.Settings.Default.portal==string.Empty)
                TradeLink.AppKit.CrashReport.Report(ASP.PROGRAM, e);
            else
                TradeLink.AppKit.ATW.Report(Properties.Settings.Default.portal, string.Empty, e, true, Properties.Settings.Default.un, Properties.Settings.Default.pw, new TradeLink.AppKit.AssemblaTicketWindow.LoginSucceedDel(ASP.success),pause);
        }
    }
}