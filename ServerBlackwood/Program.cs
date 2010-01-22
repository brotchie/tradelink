using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerBlackwood
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static ServerBlackwoodMain app;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                app = new ServerBlackwoodMain();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.Report(ServerBlackwoodMain.PROGRAM, ex);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(ServerBlackwoodMain.PROGRAM,  (Exception)e.ExceptionObject);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(ServerBlackwoodMain.PROGRAM, e.Exception);
        }
    }
}
