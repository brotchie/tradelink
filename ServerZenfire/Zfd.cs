using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZenFireDev
{
    public partial class Zfd : Form
    {
        ZenFire.Connection zf;
        AccountEventWindow aew;

        public Zfd(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;
            /*
            insertRow = new InsertRowCallback(eventGrid.Rows.Insert);
            zf.Alert += new ZenFire.Connection.AlertEventHandler(zf_Alert);
             */

            accountListBox.Items.AddRange(zf.ListAccounts());
        }
        /*
        void zf_Alert(object sender, ZenFire.AlertEventArgs e)
        {
            ZenFire.AlertType alert = e.Type;
            string[] row = { alert.ToString()};
            this.Invoke(insertRow, new object[] { 0, row });
        }
         */
         
        private void accountEventButton_Click_1(object sender, EventArgs e)
        {
            new PlaceOrderWindow(zf).Show();

        }

        private void positionEventWatcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PositionEventWindow(zf).Show();
        }

        private void orderEventWatcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OrderEventWindow oew = new OrderEventWindow(zf);
            oew.Show();
        }

        private void accountEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aew = new AccountEventWindow(zf);
            aew.Show();
        }

        private void accountSummaryEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AccountSummaryEventWindow asew = new AccountSummaryEventWindow(zf);
            asew.Show();
        }

        private void quoteBoardEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuoteBoardEventWindow qbew = new QuoteBoardEventWindow(zf);
            qbew.Show();
        }

        private void positionDetailsEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PositionDetailsEventWindow pdew = new PositionDetailsEventWindow(zf);
            pdew.Show();
        }

        private void openOrdersEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenOrdersEventWindow ooew = new OpenOrdersEventWindow(zf);
            ooew.Show();
        }

        private void completedOrdersEventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompletedOrdersEventWindow coew = new CompletedOrdersEventWindow(zf);
            coew.Show();
        }
         
        private void placeOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaceOrderWindow pow = new PlaceOrderWindow(zf);
            pow.Show();
        }

        private void accountListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            ZenFire.IAccount acct = (ZenFire.IAccount)accountListBox.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
                zf.SubscribeAccount(acct);
            else
                zf.UnsubscribeAccount(acct);
        }

        private void tickWatcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TickEventWindow(zf).Show();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Login(zf).Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().Show();
        }

        private void alertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AlertEventWindow(zf).Show();
        }

        


    }
}
