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
    public partial class TextPrompt : Form
    {
        public TextPrompt()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void _ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        /// <summary>
        /// prompt for a value
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="prompt"></param>
        /// <returns></returns>
        public static string Prompt(string caption, string prompt) { return Prompt(caption, prompt, string.Empty); }
        /// <summary>
        /// prompt for a value
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="prompt"></param>
        /// <param name="defaultval"></param>
        /// <returns></returns>
        public static string Prompt(string prompt, string caption, string defaultval) { return Prompt(prompt,caption, defaultval, 0, 0); }
        public static string Prompt(string prompt, string caption, string defaultval, int x, int y)
        {
            TextPrompt tp = new TextPrompt();
            tp.Text = caption;
            tp._msg.Text = prompt;
            tp._txt.Text = defaultval;
            if (tp.ShowDialog() == DialogResult.OK)
                return tp._txt.Text;
            return defaultval;
        }


    }
}
