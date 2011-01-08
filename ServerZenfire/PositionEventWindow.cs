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
    public partial class PositionEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;

        public PositionEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);

            zf.PositionEvent += new ZenFire.Connection.PositionEventHandler(zf_PositionUpdate);
        }

        void zf_PositionUpdate(object sender, ZenFire.PositionEventArgs e)
        {
            ZenFire.IPosition pos = e.Position;
            string[] row = { pos.Account.ToString(), pos.Product.ToString(), pos.Size.ToString(), pos.OpenPL.ToString(), pos.ClosedPL.ToString()};
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }

    }
}
