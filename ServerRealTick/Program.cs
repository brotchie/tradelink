using System;
using System.Windows.Forms;
using TalTrade.Toolkit.ClientAdapter;
using TalTrade.Toolkit;

namespace RealTickConnector
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            try
            {
                    Application.Run(new RealTickMain());
            }
            catch (ToolkitPermsException ex)
            {
                TradeLink.AppKit.CrashReport.Report(RealTickMain.PROGRAM, ex);
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(RealTickMain.PROGRAM, (Exception) e.ExceptionObject);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.AppKit.CrashReport.Report(RealTickMain.PROGRAM, e);
            }
    }
}
