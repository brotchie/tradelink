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

            Quote q = new Quote();

            Application.Run(q);
        }
    }
}