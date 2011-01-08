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
    public partial class QuoteBoardEventWindow : Form
    {
        ZenFire.Connection zf;
        delegate void InsertRowCallBack(int row, object[] values);
        InsertRowCallBack insertRow;
        
        public QuoteBoardEventWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            insertRow = new InsertRowCallBack(eventGrid.Rows.Insert);

            zf.TickEvent += new ZenFire.Connection.TickEventHandler(zf_QuoteBoardUpdate);
        }

        void zf_QuoteBoardUpdate(object sender, ZenFire.TickEventArgs e)
        {            
            switch (e.Type.ToString())
            {
                case "BestBid":
                    //e.Product.
                    break;                    

            }
            ZenFire.IProduct product = zf.GetProduct(productTextBox.Text, "CME");
            string[] row = { e.Product.Symbol, };
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorkerMethod), (object)row);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                ZenFire.IProduct product = zf.GetProduct(productTextBox.Text, "CME");
                zf.Subscribe(product);
                zf.ReplayTicks(product, new DateTime(2009, 9, 24, 15, 00, 00));
            }
            catch (System.Exception x)
            {
                MessageBox.Show("Error subscribing " + x.Message);
            }           
        }

        private void WorkerMethod(object obj)
        {
            this.Invoke(insertRow, new object[] { 0, obj });
        }
    }
}
