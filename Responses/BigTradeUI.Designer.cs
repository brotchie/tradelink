namespace Responses
{
    partial class BigTradeUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this._dg = new System.Windows.Forms.DataGridView();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this._dg)).BeginInit();
            this.SuspendLayout();
            // 
            // _dg
            // 
            this._dg.AllowUserToAddRows = false;
            this._dg.AllowUserToDeleteRows = false;
            this._dg.AllowUserToOrderColumns = true;
            this._dg.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._dg.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dg.Location = new System.Drawing.Point(0, 0);
            this._dg.Name = "_dg";
            this._dg.ReadOnly = true;
            this._dg.RowHeadersVisible = false;
            this._dg.RowTemplate.Height = 28;
            this._dg.ShowEditingIcon = false;
            this._dg.Size = new System.Drawing.Size(532, 369);
            this._dg.TabIndex = 0;
            this.toolTip1.SetToolTip(this._dg, "List of 10-biggest trades occuring.   Right click to access buy/sell functions.");
            // 
            // BigTradeUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 369);
            this.Controls.Add(this._dg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "BigTradeUI";
            this.Text = "BigTradeUI";
            ((System.ComponentModel.ISupportInitialize)(this._dg)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView _dg;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}