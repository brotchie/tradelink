using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ServerMB
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
                Application.Run(new ServerMBMain());
            }
            catch (Exception e)
            {
                TradeLink.AppKit.CrashReport.Report(ServerMBMain.PROGRAM, e);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(ServerMBMain.PROGRAM, e);
        }
    }
}
