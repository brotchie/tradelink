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
    public partial class TickEventWindow : Form
    {
        ZenFire.Connection zf;
        string timeFmt;
        delegate void InsertRowCallback(int row, object[] values);
        InsertRowCallback insertRow;
        ZenFire.Connection.TickEventHandler tick;

        public TickEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            timeFmt = "h:mm:ss.fff";
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);

            exchangeComboBox.DisplayMember = "Text";
            exchangeComboBox.ValueMember = "ID";
            exchangeComboBox.DataSource = zf.ListExchanges();

            tick = new ZenFire.Connection.TickEventHandler(zf_TickEvent);
            zf.TickEvent += tick;// new ZenFire.Connection.TickEventHandler(zf_TickEvent);
        }

        /*
        protected override void Finalize()
        {
            try
            {
                zf.TickEvent -= Z
        */
        void zf_TickEvent(object sender, ZenFire.TickEventArgs e)
        {

            string[] row = {
                               e.TimeStamp.ToString(timeFmt),
                               Enum.GetName(typeof(ZenFire.TickType), e.Type),
                               e.Product.ToString(),
                               e.Price.ToString(),
                               e.Volume.ToString(),
                               Enum.GetName(typeof(ZenFire.TickFlags), e.Flags)
                           };
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }

        private void subscribeButton_Click(object sender, EventArgs e)
        {
            try
            {
                ZenFire.IProduct product = zf.GetProduct(productTextBox.Text, exchangeComboBox.SelectedItem.ToString());
                zf.Subscribe(product);
                zf.ReplayTicks(product, new DateTime(2009, 9, 24, 15, 00, 00));
            }
            catch(System.Exception x)
            {
                MessageBox.Show("Error subscribing " + x.Message);
            }
        }

    }
}
