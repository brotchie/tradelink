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
    public partial class LogViewer : Form
    {
        public const string PROGRAM = "LogViewer";
        string PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+"\\";
        const string WILDEXT = "*.txt";
        const string DOTEXT = ".txt";
        string namefilter = string.Empty;
        string user = string.Empty;
        string pw = string.Empty;
        System.IO.FileSystemWatcher fsw;
        AssemblaTicketWindow.LoginSucceedDel loginsucceed;
        public LogViewer() : this(string.Empty,null,string.Empty,string.Empty) { }
        public LogViewer(string spacefilter, AssemblaTicketWindow.LoginSucceedDel loginsuccess,string User, string Pw)
        {
            namefilter = spacefilter;
            loginsucceed = loginsuccess;
            user = User;
            pw = Pw;
            if (loginsucceed == null)
                loginsucceed = new AssemblaTicketWindow.LoginSucceedDel(succeed);
            InitializeComponent();
            if (namefilter!=string.Empty)
            {
                Text = PROGRAM + " " + namefilter;
                Invalidate();
            }
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("View", new EventHandler(view));
            ContextMenu.MenuItems.Add("Ticket", new EventHandler(aticket));
            ContextMenu.MenuItems.Add("Delete", new EventHandler(del));
            DoubleClick += new EventHandler(LogViewerMain_DoubleClick);
            _logs.DoubleClick += new EventHandler(_logs_DoubleClick);
            _logs.SelectionMode = SelectionMode.MultiExtended;
            _logs.Sorted = true;
            init();
            fsw =  new System.IO.FileSystemWatcher(PATH,WILDEXT);
            fsw.IncludeSubdirectories = false;
            fsw.Changed += new System.IO.FileSystemEventHandler(fsw_Changed);
            fsw.Created += new System.IO.FileSystemEventHandler(fsw_Created);
        }

        void _logs_DoubleClick(object sender, EventArgs e)
        {
            view(null, null);
        }

        void del(object o, EventArgs e)
        {
            // make sure something is selected
            if (sel.Length == 0) return;
            if (MessageBox.Show("Delete "+sel.Length+" log files?","Confirm delete", MessageBoxButtons.YesNo)!= DialogResult.Yes) return;
            foreach (int s in sel)
            {
                try
                {
                    System.IO.File.Delete(getpath(s));
                }
                catch { }
            }
            init();
        }

        void LogViewerMain_DoubleClick(object sender, EventArgs e)
        {
            view(null, null);
        }

        void init()
        {
            _logs.Items.Clear();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(PATH);
            System.IO.FileInfo [] fis = di.GetFiles(WILDEXT, System.IO.SearchOption.TopDirectoryOnly);
            foreach (System.IO.FileInfo fi in fis)
                addname(fi.Name);
            Invalidate();
        }

        bool addname(string name)
        {
            // make sure it passes filter
            if ((namefilter != string.Empty) && !name.Contains(namefilter)) return false;
            name = name.Replace(DOTEXT, string.Empty);
            _logs.Items.Add(name);
            return true;
        }

        void succeed(string u, string p)
        {
        }

        string getpath(int idx)
        {
            return PATH + _logs.Items[idx].ToString() + DOTEXT;
        }

        string getname(int idx)
        {
            return _logs.Items[idx].ToString() + DOTEXT;
        }

        void fsw_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            addname(e.Name);
        }

        void fsw_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            
        }
        string spacefromname(string name)
        {
            name = name.Replace(DOTEXT,string.Empty);
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("[.][0-9]+");
            return r.Replace(name, string.Empty);
        }
        void aticket(object o, EventArgs e)
        {
            if (sel.Length == 0) return;
            foreach (int s in sel)
            {
                string name = getname(s);
                string space = spacefromname(name);
                try
                {
                    System.IO.StreamReader sr = new System.IO.StreamReader(getpath(s));
                    string log = sr.ReadToEnd();
                    sr.Close();
                    ATW.Report(space, log, null, true, user, pw, loginsucceed, false);
                }
                catch { }
            }
        }



        int[] sel { get { List<int> i = new List<int>(_logs.SelectedIndices.Count); foreach (int sel in _logs.SelectedIndices) i.Add(sel); return i.ToArray(); } }
        void view(object o, EventArgs e)
        {
            // make sure something is selected
            if (sel.Length == 0) return;
            foreach (int s in sel)
            {
                System.Diagnostics.Process.Start("notepad.exe", getpath(s));
            }

        }
    }
}
