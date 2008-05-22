using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Kadina
{
    public class CellColorArgs
    {
        public CellColorArgs(Brush fg, Brush bg, int row, int col)
        {
            _bg = bg;
            _fg = fg;
            _row = row;
            _col = col;
        }
        private Brush _bg;
        private Brush _fg;
        private int _row;
        private int _col;
        public Brush BackColor { get { return _bg; } set { _bg = value; } }
        public Brush ForeColor { get { return _fg; } set { _fg = value; } }
        public int Row { get { return _row; } }
        public int Col { get { return _col; } }
    }
    public class DataGridCell : DataGridTextBoxColumn
    {
        private int SelectedRow = -1;
        public delegate void CellColorDelegate(CellColorArgs a);
        public event CellColorDelegate ColorDataGridCellEvent;

        protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string nstantText, bool cellIsVisible)
        {
            //make sure previous selection is valid 
            if (SelectedRow > -1 && SelectedRow < source.List.Count + 1)
                this.DataGridTableStyle.DataGrid.UnSelect(SelectedRow);
            SelectedRow = rowNum;
            this.DataGridTableStyle.DataGrid.Select(SelectedRow);
        }
        protected override void Paint(System.Drawing.Graphics g, 
               System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager 
               source, int rowNum, System.Drawing.Brush backBrush, System.Drawing.Brush 
               foreBrush, bool alignToRight) 
          { 
          // the idea is to conditionally set the foreBrush and/or backbrush 
          // depending upon some criteria on the cell value 
          // Here, we color anything that begins with a letter higher than 'F' 
               try{ 
                    object o = this.GetColumnValueAtRow(source, rowNum); 
                    if( o!= null) 
                        if (ColorDataGridCellEvent!=null)
                        {
                            CellColorArgs a = new CellColorArgs(foreBrush,backBrush,rowNum,source.Position);
                            ColorDataGridCellEvent(a);
                        }
               } 
               catch(Exception) 
               { 
                   //empty catch 
               } 
               finally
               { 
                    // make sure the base class gets called to do the drawing with 
                    // the possibly changed brushes 
                    base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight); 
               } 
          } 

    }

    /*DG NOTES
     * 
     * this datagrid faq site is the bomb:
     * http://www.syncfusion.com/FAQ/windowsforms/faq_c44c.aspx
     * 
     * To progamatically select a row, you need to call: 
[C#] 
//select row 1 
this.dataGrid1.Select(1);
     * 
     * 
     * 
     * 
     * example of apply DGTBC to a datatable.datacolumn grid style
     DataTable myTable= new DataTable();

     // Add a new DataColumn to the DataTable.
     DataColumn myColumn = new DataColumn("myTextBoxColumn");
     myColumn.DataType = System.Type.GetType("System.String");
     myColumn.DefaultValue="default string";
     myTable.Columns.Add(myColumn);
     // Get the CurrencyManager for the DataTable.
     CurrencyManager cm = (CurrencyManager)this.BindingContext[myTable];
     // Use the CurrencyManager to get the PropertyDescriptor for the new column.
     PropertyDescriptor pd = cm.GetItemProperties()["myTextBoxColumn"];
     DataGridTextBoxColumn myColumnTextColumn;
     // Create the DataGridTextBoxColumn with the PropertyDescriptor.
     myColumnTextColumn = new DataGridTextBoxColumn(pd);
     // Add the new DataGridColumn to the GridColumnsCollection.
     dataGrid1.DataSource= myTable;
     dataGrid1.TableStyles.Add(new DataGridTableStyle());
     dataGrid1.TableStyles[0].GridColumnStyles.Add(myColumnTextColumn);
     */
}
