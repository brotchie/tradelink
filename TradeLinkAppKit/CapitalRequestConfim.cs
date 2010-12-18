using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TradeLink.AppKit
{
    delegate bool capcondel(Results r, bool s, TradeLink.API.DebugDelegate d);
    public partial class CapitalRequestConfim : Form
    {
        public CapitalRequestConfim()
        {
            InitializeComponent();
            DialogResult = DialogResult.No;
        }

        public bool AskAgain { get { return !checkBox1.Checked; } }
        public string Email { get { return _email.Text; } }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((_email.Text == string.Empty) || !_email.Text.Contains("@") || !_email.Text.Contains("."))
            {
                emaillab.ForeColor = Color.Red;
                emaillab.Invalidate();
                return;
            }
            DialogResult = DialogResult.Yes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }
        public static bool ConfirmCapitalRequest()
        {
            CapitalRequestConfim crc = new CapitalRequestConfim();
            DialogResult dr = crc.ShowDialog();
            if (!crc.AskAgain)
            {
                // disable tracking

            }
            return dr == DialogResult.Yes;
        }


        static TradeLink.API.DebugDelegate d;
        static void debug(string msg)
        {
            if (d != null)
                d(msg);
        }
        public static bool ConfirmSubmitCapitalRequest(TradeLink.AppKit.Results rs) { return ConfirmSubmitCapitalRequest(rs, false,null); }
        public static bool ConfirmSubmitCapitalRequest(TradeLink.AppKit.Results rs, bool skip, TradeLink.API.DebugDelegate deb)

        {
            d = deb;
            if (skip)
            {
                return false;
            }

            CapitalRequestConfim crc = new CapitalRequestConfim();
            bool ok = false;
            if (crc.InvokeRequired)
                crc.Invoke(new capcondel(ConfirmSubmitCapitalRequest), new object[] { rs, skip, deb });
            else
            {
                DialogResult dr = crc.ShowDialog();
                if (!crc.AskAgain)
                {
                    // disable tracking

                }
                ok = dr == DialogResult.Yes;
            }
            if (!ok) return false;
            return CapitalRequest.Submit(crc.Email, rs, deb);
        }
    }

    public class CRC : CapitalRequestConfim
    {

    }
}
