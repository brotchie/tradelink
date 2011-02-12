using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.AppKit
{
    public partial class DebugControl : UserControl
    {
        bool _timestamp = true;
        public bool TimeStamps { get { return _timestamp; } set { _timestamp = value; } }
        public DebugControl() : this(true) { }
        public DebugControl(bool timestamp)
        {
            InitializeComponent();
            ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add("Create Ticket", new EventHandler(createticket));
            toggleselectall();
        }
        public event DebugDelegate NewCreateTicketEvent;

        void createticket(object o, EventArgs e)
        {
            if (NewCreateTicketEvent != null)
            {
                NewCreateTicketEvent(selectedtext());
            }
        }
        public string Content { get { return sb.ToString(); } }
        StringBuilder sb = new StringBuilder();

        bool _oksearch = true;
        public bool EnableSearching { get { return _oksearch; } set { _oksearch = value; } }

        string search = string.Empty;
        int sidx = 0;
        int lastsidx = -1;

        bool selall = true;
        void toggleselectall()
        {
            selall = !selall;
            if (selall)
            {
                for (int i = 0; i < _msg.Items.Count; i++)
                    _msg.SelectedIndices.Add(i);
            }
            else
                _msg.SelectedIndices.Clear();
            Invalidate(true);
        }

        void update()
        {
            string[] r = sb.ToString().ToUpper().Split(Environment.NewLine.ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
            for (int i = sidx; i < r.Length; i++)
            {
                if (r[i].IndexOf(search, 0) != -1)
                {
                    sidx = i;
                    break;
                }
            }
            if (lastsidx != -1)
            {
                try
                {
                    _msg.SetSelected(lastsidx, false);
                    lastsidx = -1;
                }
                catch { }
            }
            if (sidx != lastsidx)
            {
                try
                {
                    _msg.SetSelected(sidx, true);
                    lastsidx = sidx;
                }
                catch { }
            }
            newsearch();
        }
        public event DebugDelegate NewSearchEvent;
        private void newsearch()
        {
            if (NewSearchEvent != null)
                NewSearchEvent(search);
        }
        string selectedtext()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _msg.SelectedItems.Count; i++)
                sb.AppendLine(_msg.SelectedItems[i].ToString());
            return sb.ToString();
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_oksearch)
            {
                string csearch = search;
                bool updateok = true;
                if (keyData == Keys.Enter)
                {
                    sidx++;
                    if (sidx == _msg.Items.Count)
                        sidx = 0;
                    // go to next one
                    update();
                }
                else if (keyData == (Keys.A | Keys.Control))
                {
                    toggleselectall();
                }
                else if (keyData == (Keys.C | Keys.Control))
                {
                    Clipboard.SetText(selectedtext());
                }
                else if ((keyData == Keys.OemPipe))
                {
                    search += "\\";
                }
                else if (keyData == Keys.OemQuestion)
                {
                    search += "/";
                }
                else if (keyData == Keys.OemPeriod)
                {
                    search += ".";
                }
                else if ((keyData == Keys.Escape) || (keyData == Keys.Delete))
                {
                    search = "";
                    // don't modify screen if deleting search term
                    updateok = false;
                    newsearch();
                }
                else if ((keyData == Keys.Back) && (search.Length > 0))
                {
                    search = search.Substring(0, search.Length - 1);
                }
                else if ((((int)keyData >= (int)Keys.A) && ((int)keyData <= (int)Keys.Z))
                    || (((int)keyData >= (int)Keys.D0) && ((int)keyData <= (int)Keys.D9)) || (keyData == Keys.Space))
                {
                    string val = "";
                    char v = (char)keyData;
                    val += v;
                    search += val;
                }
                else if (keyData == Keys.OemPeriod)
                {
                    search += ".";
                }
                else if (keyData == Keys.Space)
                {
                    search += " ";
                }
                else if (keyData == Keys.OemMinus)
                {
                    search += "-";
                }
                // if there was a change
                if (updateok && (csearch!=search))
                    update();
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        public void GotDebug(Debug msg)
        {
            debug(msg.Msg);
            
        }
        bool _eu = true;
        public void BeginUpdate()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(BeginUpdate));
            else
            {
                _eu = false;
                _msg.BeginUpdate();
            }
        }

        public void EndUpdate()
        {
            if (InvokeRequired)
                Invoke(new VoidDelegate(BeginUpdate));
            else
            {
                _eu = true;
                _msg.EndUpdate();
                _msg.Invalidate();
            }
        }

        public void GotDebug(string msg)
        {
            debug(msg);
        }

        public void Clear()
        {
            if (_msg.InvokeRequired)
                Invoke(new VoidDelegate(Clear));
            else
            {

                _msg.Items.Clear();
                _msg.Invalidate(true);
                sb = new StringBuilder();
            }
        }

        delegate void stringdel(string msg);
        bool _useexttime = false;
        /// <summary>
        /// toggle whether an external time stamp is used (timestamps must be enabled)
        /// </summary>
        public bool UseExternalTimeStamp { get { return _useexttime; } set { _useexttime = value; } }
        int _exttime = 0;
        /// <summary>
        /// set an external time stamp
        /// </summary>
        public int ExternalTimeStamp { get { return _exttime; } set { _exttime = value; } }
        void debug(string msg)
        {
            if (_msg.InvokeRequired)
            {
                try
                {
                    _msg.Invoke(new stringdel(debug), new object[] { msg });
                }
                catch (ObjectDisposedException) { }
                catch (System.Threading.ThreadInterruptedException) { }
            }
            else
            {
                try
                {
                    if (!TimeStamps)
                        _msg.Items.Add(msg);
                    else if (UseExternalTimeStamp)
                        _msg.Items.Add(_exttime + ": " + msg);
                    else
                        _msg.Items.Add(DateTime.Now.ToString("HHmmss") + ": " + msg);
                    _msg.SelectedIndex = _msg.Items.Count - 1;
                    if (_eu)
                        _msg.Invalidate(true);
                    sb.AppendLine(msg.Replace(Environment.NewLine,string.Empty));
                }
                catch { }
            }
        }

    }
}
