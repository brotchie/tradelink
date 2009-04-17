using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASP
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
            try
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.Run(new ASP());
            }
            catch (Exception ex)
            {
                TradeLink.Common.CrashReport.Report(ASP.PROGRAM, ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.Common.CrashReport.Report(ASP.PROGRAM, e.Exception);
        }
    }
}