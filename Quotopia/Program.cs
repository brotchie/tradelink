using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quotopia
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
                Quote q = new Quote();

                Application.Run(q);
            }
            catch (Exception ex) { TradeLink.Common.CrashReport.Report(Quote.PROGRAM, ex); }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.Common.CrashReport.Report(Quote.PROGRAM, e.Exception);
        }
    }
}