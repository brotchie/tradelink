using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Tript
{
    public class TextBoxWriter : System.IO.TextWriter
    {
        TextBoxBase control;
        StringBuilder Builder;

        public TextBoxWriter(RichTextBox existingtextbox)
        {
            control = existingtextbox;
            control.HandleCreated += new EventHandler(control_HandleCreated);
        }

        void control_HandleCreated(object sender, EventArgs e)
        {
            if (Builder!=null)
            {
                AppendText(Builder.ToString());
                Builder = null;
            }
        }

        public override void Write(char value)
        {
            this.Write(value.ToString());
        }

        public override void Write(string value)
        {
            if (control.IsHandleCreated)
            {
                control.AppendText(value);
            }
            else BufferText(value);
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value + Environment.NewLine);
            control.SelectionStart = control.Text.Length;
            control.SelectionLength = 0;
            control.ScrollToCaret();
        }

        void BufferText(string s)
        {
            if (Builder == null)
                Builder = new StringBuilder();
            Builder.Append(s);
        }

        void AppendText(string s)
        {
            if (Builder != null)
            {
                control.AppendText(Builder.ToString());
                Builder = null;
            }
            control.AppendText(s);
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }

    }
}
