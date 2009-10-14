using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerEsignal
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
                Application.Run(new EsignalMain());
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.Report(EsignalMain.PROGRAM, ex);
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(EsignalMain.PROGRAM, (Exception)e.ExceptionObject); 

        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = e.Exception;
            TradeLink.AppKit.CrashReport.Report(EsignalMain.PROGRAM, ex);
        }
    }
}
