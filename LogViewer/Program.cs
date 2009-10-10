using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TradeLink.AppKit;

namespace LogViewer
{
    static class Program
    {
        static Form app;
        static string PROGRAM = TradeLink.AppKit.LogViewer.PROGRAM;
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
            string namefilter = string.Empty;
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                namefilter = Environment.GetCommandLineArgs()[1];
            }
            try
            {
                app = new TradeLink.AppKit.LogViewer(namefilter,succeed,Properties.Settings.Default.au,Properties.Settings.Default.ap);
                Application.Run(app);
            }
            catch (Exception ex)
            {
                TradeLink.AppKit.CrashReport.Report(PROGRAM, ex);
            }
        }

        static void succeed(string u, string p)
        {
            Properties.Settings.Default.au = u;
            Properties.Settings.Default.ap = p;
            Properties.Settings.Default.Save();
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(PROGRAM, e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(PROGRAM, (Exception)e.ExceptionObject);
        }
    }
}
