using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ASP
{
    public partial class ASPOptions : Form
    {
        public ASPOptions()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(ASPOptions_FormClosing);
        }

        void ASPOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }


    }
}
