using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    public partial class BookControl : UserControl
    {
        DataTable _dt = new DataTable();
        DataGridView _dg = new DataGridView();
        SafeBindingSource _bs = new SafeBindingSource();

        const string TIME = "Time";
        const string TRADE = "Trade";
        const string TSIZE = "TSize";
        const string TX = "TExch";
        const string BID = "Bid";
        const string BSIZE = "BSize";
        const string BX = "BExch";
        const string ASK = "Ask";
        const string ASIZE = "ASize";
        const string AX = "AExch";
        const string DP = "Depth";
        string _sym = string.Empty;
        public string Symbol { get { return _sym; } set { _sym = value; _dt.Clear(); Refresh(); } }
        public BookControl()
        {
            _dt.Columns.Add(TIME, typeof(int));
            /*
            _dt.Columns.Add(TRADE);
            _dt.Columns.Add(TSIZE);
            _dt.Columns.Add(TX);
             */
            _dt.Columns.Add(BID,typeof(decimal));
            _dt.Columns.Add(ASK,typeof(decimal));
            _dt.Columns.Add(BSIZE,typeof(int));
            _dt.Columns.Add(ASIZE,typeof(int));
            _dt.Columns.Add(BX);
            _dt.Columns.Add(AX);
            _dt.Columns.Add(DP, typeof(int));
            _bs.DataSource = _dt;
            _dg.DataSource = _bs;
            _dg.AllowUserToAddRows = false;
            _dg.AllowUserToDeleteRows = false;
            _dg.AllowUserToOrderColumns = true;
            _dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            _dg.Location = new System.Drawing.Point(0, 0);
            _dg.Margin = new System.Windows.Forms.Padding(4);
            _dg.Name = "tsgrid";
            _dg.ReadOnly = true;
            _dg.RowTemplate.Height = 24;
            _dg.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            _dg.Size = new System.Drawing.Size(827, 322);
            _dg.TabIndex = 0;
            _dg.DataError += new DataGridViewDataErrorEventHandler(_dg_DataError);
            _dg.RowHeadersVisible = false;
            _dg.ShowEditingIcon = false;
            _dg.ColumnHeadersVisible = true;
            _dg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
 
            _dg.Parent = this;
            _dg.Dock = DockStyle.Fill;
            Refresh();
            InitializeComponent();
        }

        void _dg_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            
        }

        int _count = 0;

        int _updateevery = 10;
        /// <summary>
        /// refresh grid every X updates
        /// </summary>
        public int RefreshEvery { get { return _updateevery; } set { _updateevery = value; } }

        public void GotBook(Book b)
        {
            if (b.Sym != Symbol) return;
            _dt.Clear();
            for (int i = 0; i < b.ActualDepth; i++)
            {
                _dt.Rows.Add(b.UpdateTime, b.bidprice[i], b.askprice[i], b.bidsize[i], b.asksize[i],b.bidex[i],b.askex[i],i);
                if (i == 0)
                {
                    _dg.Columns[BID].DefaultCellStyle.Format = "N2";
                    _dg.Columns[ASK].DefaultCellStyle.Format = "N2";
                }
            }

            if (_count++ % _updateevery == 0)
                Refresh();

        }

        public override void Refresh()
        {
            SafeBindingSource.refreshgrid(_dg, _bs);
        }
    }
}
