using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TradeLink.Common;

namespace TradeLink.AppKit
{
    public partial class BookView : Form
    {
        public BookView()
        {
            InitializeComponent();
            FormClosing += new FormClosingEventHandler(BookView_FormClosing);
        }

        public string Symbol { get { return bookControl1.Symbol; } set { bookControl1.Symbol = value; } }

        public override void Refresh()
        {
            bookControl1.Refresh();
        }

        public void GotBook(Book b)
        {
            bookControl1.GotBook(b);
        }

        void BookView_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Toggle();
        }

        public void Toggle()
        {
            Visible = !Visible;
            Invalidate(true);
        }


    }
}
