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
    public partial class AccountEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;

        public AccountEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);

            zf.AccountEvent += new ZenFire.Connection.AccountEventHandler(zf_AccountUpdate);
        }

        void zf_AccountUpdate(object sender, ZenFire.AccountEventArgs e)
        {
            ZenFire.IAccount acct = e.Account;
            string[] row = { acct.ToString(), acct.Balance.ToString(), acct.Margin.ToString(), acct.OpenPL.ToString(), acct.ClosedPL.ToString() };
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }
    }
}
