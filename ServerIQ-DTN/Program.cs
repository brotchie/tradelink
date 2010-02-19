using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace IQFeedBroker
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
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new IQFeedFrm());
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.Report(IQFeedFrm.PROGRAM, string.Empty, string.Empty, string.Empty, ex, null, true);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(IQFeedFrm.PROGRAM, string.Empty, string.Empty, string.Empty, (Exception)e.ExceptionObject, null, true);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(IQFeedFrm.PROGRAM, string.Empty, string.Empty, string.Empty, e.Exception, null, true);
        }
    }
}
