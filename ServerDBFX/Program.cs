using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerDBFX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static ServerDBFXMain app;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                app = new ServerDBFXMain();
                Application.Run(app);
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.BugReport(ServerDBFXMain.PROGRAM, TradeLink.AppKit.CrashReport.Desc(ServerDBFXMain.PROGRAM), ex, app._dw.Content);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.BugReport(ServerDBFXMain.PROGRAM,TradeLink.AppKit.CrashReport.Desc(ServerDBFXMain.PROGRAM),(Exception)e.ExceptionObject,app._dw.Content);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.BugReport(ServerDBFXMain.PROGRAM, TradeLink.AppKit.CrashReport.Desc(ServerDBFXMain.PROGRAM), e.Exception, app._dw.Content);
        }
    }
}
