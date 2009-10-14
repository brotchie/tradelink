using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SterServer
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
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            try
            {
                Application.Run(new SterMain());
            }
            catch (Exception e) 
            { 
                TradeLink.AppKit.CrashReport.Report(SterMain.PROGRAM, e); 
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(SterMain.PROGRAM, (Exception)e.ExceptionObject); 
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(SterMain.PROGRAM, e); 
        }
    }
}
