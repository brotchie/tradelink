using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Record
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
                Application.Run(new RecordMain());
            }
            catch (Exception e)
            {
                TradeLink.Common.CrashReport.Report(RecordMain.PROGRAM, e);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.Common.CrashReport.Report(RecordMain.PROGRAM, e);
        }
    }
}