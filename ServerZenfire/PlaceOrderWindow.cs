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
    public partial class PlaceOrderWindow : Form
    {
        ZenFire.Connection zf;
        ZenFire.IProduct product;


        public PlaceOrderWindow(ZenFire.Connection z)
        {
            InitializeComponent();
            zf = z;

            Bind.List(accountComboBox, zf.ListAccounts());
            Bind.List(actionComboBox, Bind.FromEnum(ZenFire.Order.Action.Buy));
            Bind.List(durationComboBox, Bind.FromEnum(ZenFire.Order.Duration.Day));
            Bind.List(typeComboBox, Bind.FromEnum(ZenFire.Order.Type.Market));

            exchangeComboBox.DisplayMember = "Text";
            exchangeComboBox.ValueMember = "ID";
            exchangeComboBox.DataSource = zf.ListExchanges();
        }


        private void product_Changed(object sender, EventArgs e)
        {
        }

        private void type_Changed(object sender, EventArgs e)
        {
            try
            {
                product = zf.GetProduct(productTextBox.Text, exchangeComboBox.SelectedItem.ToString());
            }
            catch
            { MessageBox.Show("Invalid product " + productTextBox.Text + "." + exchangeComboBox.SelectedItem.ToString()); }

        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                product = zf.GetProduct(productTextBox.Text, exchangeComboBox.SelectedItem.ToString());
            }
            catch
            {
                MessageBox.Show("Invalid product " + productTextBox.Text + "." + exchangeComboBox.SelectedItem.ToString());
            }

            ZenFire.OrderArgs args = new ZenFire.OrderArgs();

            args.Account = (ZenFire.IAccount)accountComboBox.SelectedValue;
            args.Product = product;
            args.Action = (ZenFire.Order.Action)actionComboBox.SelectedValue;
            args.Quantity = Decimal.ToInt32(quantitySpinner.Value);
            args.Duration = (ZenFire.Order.Duration)durationComboBox.SelectedValue;
            args.Type = (ZenFire.Order.Type)typeComboBox.SelectedValue;

            try
            {
                zf.PlaceOrder(args);
            }
            catch
            {
                MessageBox.Show("Failed to send order");
            }
        }

    }
}
