using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ZenFireDev
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

            ZenFire.Connection zf = new ZenFire.Connection("../../cert");
            zf.SetOption("intraday_tick", 1);
            Application.Run(new Login(zf));
            if (zf.IsConnected())
                Application.Run(new Zfd(zf));
            else
                MessageBox.Show("Error connecting");
            }
    }
}
