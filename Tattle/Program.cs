using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tattle
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
                Application.Run(new TattleMain());
            }
            catch (Exception e)
            {
                TradeLink.AppKit.CrashReport.Report(TattleMain.PROGRAM, e);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(TattleMain.PROGRAM, e);
        }
    }
}