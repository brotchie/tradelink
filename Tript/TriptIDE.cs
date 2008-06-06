using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLib;


namespace Tript
{
    public partial class TriptIDE : Form
    {
        TextBoxWriter _redirect = null;
        public TriptIDE()
        {
            InitializeComponent();
            _redirect = new TextBoxWriter(outbox); 
            Console.SetOut(_redirect); // redirect console writes to output textbox in tript
        }

        void output(string outputtext)
        {
            if (outbox.InvokeRequired)
                outbox.Invoke(new DebugDelegate(output), new object[] { outputtext });
            else
                outbox.AppendText(outputtext);
        }

        private void runbut_Click(object sender, EventArgs e)
        {
            outbox.Clear();
            // convert editor contents to stream
            MemoryStream _workingcopy = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(editorbox.Text));
            // run the script using the stream
            CSScriptCompiler.RunScript(_workingcopy, new string[] { }); // run stream
        }
    }
}