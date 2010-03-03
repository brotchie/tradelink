using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;
using TradeLink.AppKit;

namespace IQFeedBroker
{
    public partial class IQFeedFrm : Form
    {
        public const string PROGRAM = "IQFeedConnector";

        private IQFeedHelper _helper;
        DebugWindow _dw = new DebugWindow();


        public IQFeedFrm()
        {
            InitializeComponent();
            this.FormClosing += IQFeedFrm_FormClosing;
            _helper = new IQFeedHelper();
            _helper.MktCodes = parsemkts(Properties.Resources.marketcenters);
            _helper.SendDebug += new DebugDelegate(_dw.GotDebug);
            _helper.Connected += new IQFeedHelper.booldel(_helper_Connected);


        }

        public static Dictionary<int, string> parsemkts(string data)
        {
            string[] lines = data.Split(Environment.NewLine.ToCharArray(),  StringSplitOptions.RemoveEmptyEntries);
            Dictionary<int, string> c2m = new Dictionary<int, string>();
            foreach (string line in lines)
            {
                string [] r = line.Split('\t');
                int c = 0;
                if (int.TryParse(r[0], out c))
                    c2m.Add(c, r[1]);
            }
            return c2m;
        }

        delegate void booldel(bool v);
        void _helper_Connected(bool v)
        {
            if (InvokeRequired)
                Invoke(new booldel(_helper_Connected), new object[] { v });
            else
            {
                BackColor = v ? Color.Green : Color.Red;
                Invalidate(true);
            }
        }



        private void IQFeedFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();

            _helper.Stop();
        }



        private void IQFeedFrm_Load(object sender, EventArgs e)
        {
            // auto login if login data present are already present
            if ((_user.Text != string.Empty) && (_pass.Text != string.Empty) && (_prod.Text!=string.Empty))
                _login_Click(null, null);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            // toggle log message viewing
            _dw.Toggle();
        }

        private void _report_Click(object sender, EventArgs e)
        {
            // report bugs
            CrashReport.Report(IQFeedFrm.PROGRAM, string.Empty, string.Empty, _dw.Content, null, null, false);
        }

        private void _login_Click(object sender, EventArgs e)
        {
            _helper.Start(_user.Text, _pass.Text, Properties.Settings.Default.PROGRAM_NAME,0);
            
        }

        private void _prod_TextChanged(object sender, EventArgs e)
        {

        }

    }
}