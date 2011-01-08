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
    public partial class OrderEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;

        public OrderEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);

            zf.OrderEvent += new ZenFire.Connection.OrderEventHandler(zf_OrderEvent);
        }

        void zf_OrderEvent(object sender, ZenFire.OrderEventArgs e)
        {
            ZenFire.IOrder order = e.Order;
            String price, trigger;
            
            try { price = order.Price.ToString(); }
            catch { price = "N/A"; }
            try { trigger = order.TriggerPrice.ToString(); }
            catch { trigger = "N/A"; }

            int reas = (int)order.Reason;
            string[] row = { 
                               Enum.GetName(e.Type.GetType(), e.Type),
                               order.ID.ToString(), 
                               order.Account.ToString(),
                               order.Product.ToString(),
                               Enum.GetName(typeof(ZenFire.Order.Type), order.Type),
                               Enum.GetName(typeof(ZenFire.Order.Status), order.Status),
                      //         Enum.GetName(typeof(ZenFire.Order.Reason), order.Reason),
                               reas.ToString() + " -" + Enum.GetName(typeof(ZenFire.Order.Reason), order.Reason) ,
                               order.Quantity.ToString(),
                               order.QuantityOpen.ToString(),
                               order.QuantityFilled.ToString(),
                               order.QuantityCanceled.ToString(),
                               order.Price.ToString(),
                               order.TriggerPrice.ToString(),
                               order.FillPrice.ToString(),
                               order.ZenTag,
                               order.Tag,
                                
                           };
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row );
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }

     }
}
