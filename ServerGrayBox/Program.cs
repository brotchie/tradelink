using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TradeLinkTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException+=new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException+=new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.Run(new GBTradeLink());
            }
            catch (Exception ex)
            {
                report(ex, true);
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
                TradeLink.AppKit.CrashReport.Report(GBTradeLink.PROGRAM, e);
        }
    }
}
