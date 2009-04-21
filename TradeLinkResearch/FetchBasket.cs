using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;
using TradeLink.API;

namespace TradeLink.Research
{
    /// <summary>
    /// a handy popup box that lets a user fetch symbols from URL, using the Fetch class.
    /// </summary>
    public partial class FetchBasket : Form
    {
        public delegate string FetchUsernameDelegate();
        public event FetchUsernameDelegate FetchUsername;
        FetchTarget _ft = new FetchTarget();
        public FetchBasket() : this(new FetchTarget()) { }
        public FetchBasket(List<FetchTarget> recentlist)
        {
            InitializeComponent();
            foreach (FetchTarget ft in recentlist)
                urlnamebox.Items.Add(ft);
            urlnamebox.Items.Add("");

        }
        public bool NameInUse(string newname)
        {
            foreach (object i in urlnamebox.Items)
            {
                try 
                {
                    FetchTarget ft = (FetchTarget)i;
                    if (newname == ft.Name) return true;
                    continue;
                }
                catch (InvalidCastException) { continue; }
                
            }
            return false;
        }
        public FetchBasket(FetchTarget ft)
        {
            InitializeComponent();
            Target = ft;

        }
        Basket _basket = new BasketImpl();

        public Basket Basket { get { return _basket; }  }
        public FetchTarget Target 
        { 
            get { return _ft; }
            set
            {
                _ft = value;
                urlnamebox.Text = _ft.Name;
                urlbox.Text = _ft.Url;
                nysebut.Checked = _ft.ParseNYSE;
                nasdaqbut.Checked = _ft.ParseNASD;
                linkedonlybut.Checked = _ft.ClickableOnly;
            }
        }

        private void okbut_Click(object sender, EventArgs e)
        {
            _ft.Name = urlnamebox.Text;
            _ft.Url = urlbox.Text;
            _ft.ParseNASD = nasdaqbut.Checked;
            _ft.ParseNYSE = nysebut.Checked;
            _ft.ClickableOnly = linkedonlybut.Checked;
            string username = "";
            if (FetchUsername != null) username = FetchUsername();
            _basket = _ft.Go(username);
            if (_basket.Count>0)
                DialogResult = DialogResult.OK;
            else
                DialogResult = DialogResult.Abort;
            this.Close();
        }

        private void cancelbut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void urlnamebox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                FetchTarget ft = (FetchTarget)urlnamebox.Items[urlnamebox.SelectedIndex];
                Target = ft;
            }
            catch (InvalidCastException ex) { return; }
        }

        private void urlbox_DoubleClick(object sender, EventArgs e)
        {

            // otherwise do nothing
        }

        private void urlbox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        private void urlbox_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                urlbox.Text = (string)e.Data.GetData(DataFormats.Text);
            }
            catch (Exception) { }
        }

        private void fileurlbut_CheckedChanged(object sender, EventArgs e)
        {
            if (fileurlbut.Checked)
            {
                fileurlbut.Text = "File";
                OpenFileDialog od = new OpenFileDialog();
                od.Multiselect = false;
                od.Title = "Select a file to import stocks from";
                DialogResult res = od.ShowDialog();
                if (res == DialogResult.OK)
                {
                    _ft.File = od.FileName;
                    urlbox.Text = od.FileName;


                }
            }
            else
                fileurlbut.Text = "URL";
        }
    }
}