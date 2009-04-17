using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WinGauntlet
{
    static class Program
    {
        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int dwProcessId);
        private const int ATTACH_PARENT_PROCESS = -1;
        // minimization stuff
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr HWND, int CMDSHOW);
        const int SW_MINIMIZE = 6;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            try
            {
                Application.Run(new Gauntlet());
            }
            catch (Exception ex) { TradeLink.Common.CrashReport.Report(Gauntlet.PROGRAM, ex); }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            TradeLink.Common.CrashReport.Report(Gauntlet.PROGRAM, e.Exception);
        }
    }
}