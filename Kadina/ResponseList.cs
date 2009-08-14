using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace Kadina
{
    public partial class ResponseList : Form
    {
        public ResponseList()
        {
            InitializeComponent();
        }

        public ResponseList(List<string> responses)
        {
            InitializeComponent();
            _list.Items.Clear();
            foreach (string r in responses)
                _list.Items.Add(r);
            _list.Invalidate(true);
        }

        public event DebugDelegate ResponseSelected;


        private void _choose_Click(object sender, EventArgs e)
        {

        }

        private void _list_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((_list.SelectedIndex < 0)) return;
            string r = _list.Items[_list.SelectedIndex].ToString();
            if (ResponseSelected != null)
                ResponseSelected(r);
            this.DialogResult = DialogResult.OK;
            Visible = false;
            Invalidate(true);
        }
    }
}
