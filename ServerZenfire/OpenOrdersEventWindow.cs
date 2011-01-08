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
    public partial class OpenOrdersEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;

        public OpenOrdersEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);

            zf.OrderEvent += new ZenFire.Connection.OrderEventHandler(zf_OpenOrdersUpdate);            
        }

        void zf_OpenOrdersUpdate(object sender, ZenFire.OrderEventArgs e)
        {
            ZenFire.IOrder order = e.Order;

            string[] row = { order.Account.ToString(), /*order.Number,*/ order.Status.ToString(), order.Product.Symbol.ToString(),
                             /*order.Side,*/ order.Type.ToString(), order.Quantity.ToString(), order.QuantityFilled.ToString(),
                            /*order.LimitPrice,*/ /*order.StopPrice,*/ order.FillPrice.ToString(), /*order.Ordertime*/};
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }
    }
}
