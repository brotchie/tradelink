using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Replay
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
            try
            {
                Application.Run(new Replay());
            }
            catch (Exception e)
            {
                TradeLink.AppKit.CrashReport.Report(Replay.PROGRAM, e);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(Replay.PROGRAM, e);
        }
    }
}