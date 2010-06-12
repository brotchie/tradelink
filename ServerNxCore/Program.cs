using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerNxCore
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static ServerNxCoreMain app;
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                app = new ServerNxCoreMain();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.Report(ServerNxCoreMain.PROGRAM, ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(ServerNxCoreMain.PROGRAM, e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(ServerNxCoreMain.PROGRAM, (Exception)e.ExceptionObject);
        }
    }
}
