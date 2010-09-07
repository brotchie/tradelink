using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.AppKit
{
    public partial class AuthInfoPrompt : Form
    {
        public delegate void AuthInfoDelegate(TradeLink.AppKit.AuthInfo ai);
        public event AuthInfoDelegate NewAuthInfo;

        public AuthInfoPrompt() : this(new TradeLink.AppKit.AuthInfo()) { }
        public AuthInfoPrompt(TradeLink.AppKit.AuthInfo authinfo)
        {
            ai = authinfo;
            InitializeComponent();
            _un.Text = ai.Username;
            _pw.Text = ai.Password;
            Invalidate(true);
            
        }

        public void Toggle()
        {
            if (Visible)
                Hide();
            else
                Show();
        }

        TradeLink.AppKit.AuthInfo ai;

        public TradeLink.AppKit.AuthInfo Accepted { get { return ai; } }

        private void button1_Click(object sender, EventArgs e)
        {
            ai.Username = _un.Text;
            ai.Password = _pw.Text;
            if (NewAuthInfo != null)
                NewAuthInfo(ai);
	        DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
            
        }

        static DebugDelegate d = null;
        static AuthInfoPrompt aip;
        static string PROGRAM = string.Empty;
        static string BASEPATH = string.Empty;
        public static void Prompt(string program) { Prompt(program, Auth.GetProgramAuth(program), true, null); }
        public static void Prompt(string program, AuthInfo ai) { Prompt(program, ai, true, null); }
        public static void Prompt(string program, bool pause) { Prompt(program, Auth.GetProgramAuth(program), pause, null); }
        public static void Prompt(string program, AuthInfo ai, bool pause) { Prompt(program, ai, pause, null); }
        public static void Prompt(string program, AuthInfo ai, bool pause, DebugDelegate deb) { Prompt(program, Common.Util.ProgramData(program),ai, pause, null); }
        public static void Prompt(string program, string basepath, AuthInfo ai, bool pause, DebugDelegate deb)
        {
            PROGRAM = program;
            BASEPATH = basepath+"\\";
            aip = new AuthInfoPrompt(ai);
            aip.NewAuthInfo += new AuthInfoDelegate(aip_NewAuthInfo);
            if (pause)
                aip.ShowDialog();
            else
                aip.Show();
        }

        static void aip_NewAuthInfo(AuthInfo ai)
        {
            SetProgramAuth(BASEPATH,PROGRAM, aip.Accepted, d);
        }

        static void debug(string msg)
        {
            if (d != null)
            {
                d(msg);
            }
        }

        public static bool SetProgramAuth(string program, AuthInfo ai) { return SetProgramAuth(program, ai,null); }
        public static bool SetProgramAuth(string program, AuthInfo ai, DebugDelegate deb) { return SetProgramAuth(Common.Util.ProgramData(program), ai, deb); }
        public static bool SetProgramAuth(string basepath, string program, AuthInfo ai, DebugDelegate deb)
        {
            return SetAuthInfo(basepath+program + Auth.AuthFile, ai, deb);
        }
        public static bool SetAuthInfo(string file, AuthInfo ai) { return SetAuthInfo(file, ai, null); }
        public static bool SetAuthInfo(string file, AuthInfo ai, DebugDelegate deb)
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(file, false);
                sw.WriteLine(ai.Username);
                sw.WriteLine(ai.Password);
                sw.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (deb != null)
                {
                    deb("error saving file: "+file+" with: "+ex.Message + ex.StackTrace);
                }
            }
            return false;
        }
    }

    public class AIP : AuthInfoPrompt
    {
    }
}
