using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TradeLink.Common;
using TradeLink.API;


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
            editorbox.TextChanged +=new EventHandler(editorbox_TextChanged);
            output(Util.TLSIdentity());
        }

        void  editorbox_TextChanged(object sender, EventArgs e)
        {
            hasmodfiication = true;
        }

        void output(string outputtext)
        {
            if (outbox.InvokeRequired)
                outbox.Invoke(new DebugDelegate(output), new object[] { outputtext });
            else
                outbox.Text = outputtext + outbox.Text;
        }

        void run() { runbut_Click(null, null); }
        private void runbut_Click(object sender, EventArgs e)
        {
            outbox.Clear();
            // convert editor contents to stream
            MemoryStream _workingcopy = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(editorbox.Text));
            // run the script using the stream
            CSScriptCompiler.RunScript(_workingcopy, new string[] { }); // run stream
        }
        string filename = "";
        bool hasmodfiication = false;

        private void loadbut_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.DefaultExt = ".txt";
            od.Filter = "Tript Text Files|*.txt";
            od.Multiselect = false;
            if (od.ShowDialog() == DialogResult.OK)
            {
                editorbox.Clear();
                editorbox.LoadFile(od.FileName,RichTextBoxStreamType.PlainText);
                filename = od.FileName;
                hasmodfiication = false;
            }
        }

        private void newbut_Click(object sender, EventArgs e)
        {
            if (hasmodfiication)
            {
                DialogResult r = MessageBox.Show("Do you want to save changes to UNSAVED tript?","Config",MessageBoxButtons.YesNoCancel);
                if (r== DialogResult.Yes)
                    savebut_Click(null,null);
                else if (r== DialogResult.No)
                {
                    filename = "";
                    editorbox.Clear();
                    hasmodfiication = false;
                }
                else if (r== DialogResult.Cancel)
                    return;
            }
            filename="";
            editorbox.Clear();
            hasmodfiication = false;
        }
        private void savebut_Click(object sender, EventArgs e)
        {
            if (filename == "")
                saveas_Click(null, null);
            if (filename != "")
            {
                editorbox.SaveFile(filename, RichTextBoxStreamType.PlainText);
                hasmodfiication = false;
            }
        }

        private void saveas_Click(object sender, EventArgs e)
        {
            SaveFileDialog sd = new SaveFileDialog();
            sd.AddExtension = true;
            sd.DefaultExt = ".txt";
            sd.Filter = "Tript Text Files|*.txt";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                filename = sd.FileName;
            }

        }

        private void editorbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
                run();
        }
    }


}