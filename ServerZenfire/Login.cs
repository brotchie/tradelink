using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZenFire;

namespace ZenFireDev
{
    public partial class Login : Form
    {
        public ZenFire.Connection zf;

        public Login(ZenFire.Connection z)
        {
            zf = z;
            InitializeComponent();
            Bind.List(env, zf.ListEnvironments());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                zf.Login(username.Text, password.Text, env.Text);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error logging in: " + exc);
                return;
            }
            Close();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void password_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
