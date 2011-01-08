using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace ZenFireDev
{
    public partial class AlertEventWindow : Form
    {
        ZenFire.Connection zf;
        String timeFmt;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;

        public AlertEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            timeFmt = "h:mm:ss.fff";

            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);
            zf.AlertEvent += new ZenFire.Connection.AlertEventHandler(zf_AlertEvent);

        }

        void zf_AlertEvent(object sender, ZenFire.AlertEventArgs e)
        {
            ZenFire.AlertType alert = e.Type;
            string[] row = { DateTime.Now.ToString(timeFmt), alert.ToString() };
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }

        void zf_AccountUpdate(object sender, ZenFire.AccountEventArgs e)
        {
            ZenFire.IAccount acct = e.Account;
            string[] row = { acct.ToString(), acct.Balance.ToString(), acct.Margin.ToString(), acct.OpenPL.ToString(), acct.ClosedPL.ToString() };
            this.Invoke(insertRow, new object[] { 0, row });
        }

    }
}
