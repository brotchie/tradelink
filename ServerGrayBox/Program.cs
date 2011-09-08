using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.IO;
using System.Globalization;

namespace GbTLServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        [DllImport("dbghelp.dll")]
        public static extern bool MiniDumpWriteDump(
            IntPtr hProcess, Int32 ProcessId, IntPtr hFile, int DumpType,
            IntPtr ExceptionParam, IntPtr UserStreamParam, IntPtr CallackParam);


        static void Main()
        {
            
            try
            {
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(new GBTradeLink());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return;
        }

        public static class MINIDUMPTYPE
        {
            public const int MiniDumpNormal = 0x00000000;
            public const int MiniDumpWithDataSegs = 0x00000001;
            public const int MiniDumpWithFullMemory = 0x00000002;
            public const int MiniDumpWithHandleData = 0x00000004;
            public const int MiniDumpFilterMemory = 0x00000008;
            public const int MiniDumpScanMemory = 0x00000010;
            public const int MiniDumpWithUnloadedModules = 0x00000020;
            public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
            public const int MiniDumpFilterModulePaths = 0x00000080;
            public const int MiniDumpWithProcessThreadData = 0x00000100;
            public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
            public const int MiniDumpWithoutOptionalData = 0x00000400;
            public const int MiniDumpWithFullMemoryInfo = 0x00000800;
            public const int MiniDumpWithThreadInfo = 0x00001000;
            public const int MiniDumpWithCodeSegs = 0x00002000;
        }




        

        private static void CurrentDomainUnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            CreateMiniDump();
        }

        private static void CreateMiniDump()
        {

            DateTime endTime = DateTime.Now;
            string dt = endTime.ToString("yyyy.MM.dd.HH.mm.ss",
            DateTimeFormatInfo.InvariantInfo);

            string dumpFileName = "ServerTLGB" + dt + ".dmp";
            FileStream fs = new FileStream(dumpFileName, FileMode.Create);
            System.Diagnostics.Process process =
            System.Diagnostics.Process.GetCurrentProcess();

            MiniDumpWriteDump(process.Handle, 
                              process.Id,
                              fs.SafeFileHandle.DangerousGetHandle(),
                               MINIDUMPTYPE.MiniDumpWithFullMemory, 
                               IntPtr.Zero, 
                               IntPtr.Zero,
                               IntPtr.Zero);

        }
    }
}
