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
    public partial class CompletedOrdersEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallBack(int row, object[] values);
        InsertRowCallBack insertRow;

        public CompletedOrdersEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallBack(eventGrid.Rows.Insert);

            zf.OrderEvent += new ZenFire.Connection.OrderEventHandler(zf_CompletedOrdersUpdate);
        }

        void zf_CompletedOrdersUpdate(object sender, ZenFire.OrderEventArgs e)
        {
            ZenFire.IOrder order = e.Order;

            string[] row = { order.Account.ToString(), /*order.Number,*/"0", order.Status.ToString(), order.Product.Symbol.ToString(),
                             /*order.Side,*/"0", order.Type.ToString(), order.Quantity.ToString(), order.QuantityFilled.ToString(),
                             /*order.LimitPrice,*/"0", /*order.StopPrice,*/"0", order.FillPrice.ToString(), /*order.OrderTime,*/"0",
                             /*order.Remarks,*/" " };

            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }
    }
}
