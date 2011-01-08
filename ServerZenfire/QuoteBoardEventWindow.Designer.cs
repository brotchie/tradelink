namespace ZenFireDev
{
    partial class QuoteBoardEventWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QuoteBoardEventWindow));
            this.eventGrid = new System.Windows.Forms.DataGridView();
            this.colSymbol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNetChange = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBidSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBidPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastTradePrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastTradeSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOfferPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOfferSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLowPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHighPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTradeVolume = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.productTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // eventGrid
            // 
            this.eventGrid.AllowUserToAddRows = false;
            this.eventGrid.AllowUserToDeleteRows = false;
            this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSymbol,
            this.colNetChange,
            this.colBidSize,
            this.colBidPrice,
            this.colLastTradePrice,
            this.colLastTradeSize,
            this.colOfferPrice,
            this.colOfferSize,
            this.colLowPrice,
            this.colHighPrice,
            this.colTradeVolume,
            this.colMode});
            this.eventGrid.Location = new System.Drawing.Point(8, 38);
            this.eventGrid.Name = "eventGrid";
            this.eventGrid.ReadOnly = true;
            this.eventGrid.RowHeadersVisible = false;
            this.eventGrid.Size = new System.Drawing.Size(893, 339);
            this.eventGrid.TabIndex = 1;
            // 
            // colSymbol
            // 
            this.colSymbol.HeaderText = "Symbol";
            this.colSymbol.Name = "colSymbol";
            this.colSymbol.ReadOnly = true;
            // 
            // colNetChange
            // 
            this.colNetChange.HeaderText = "Net Change";
            this.colNetChange.Name = "colNetChange";
            this.colNetChange.ReadOnly = true;
            // 
            // colBidSize
            // 
            this.colBidSize.HeaderText = "Bid Size";
            this.colBidSize.Name = "colBidSize";
            this.colBidSize.ReadOnly = true;
            // 
            // colBidPrice
            // 
            this.colBidPrice.HeaderText = "Bid Price";
            this.colBidPrice.Name = "colBidPrice";
            this.colBidPrice.ReadOnly = true;
            // 
            // colLastTradePrice
            // 
            this.colLastTradePrice.HeaderText = "Last Trade Price";
            this.colLastTradePrice.Name = "colLastTradePrice";
            this.colLastTradePrice.ReadOnly = true;
            // 
            // colLastTradeSize
            // 
            this.colLastTradeSize.HeaderText = "Last Trade Size";
            this.colLastTradeSize.Name = "colLastTradeSize";
            this.colLastTradeSize.ReadOnly = true;
            // 
            // colOfferPrice
            // 
            this.colOfferPrice.HeaderText = "Offer Price";
            this.colOfferPrice.Name = "colOfferPrice";
            this.colOfferPrice.ReadOnly = true;
            // 
            // colOfferSize
            // 
            this.colOfferSize.HeaderText = "Offer Size";
            this.colOfferSize.Name = "colOfferSize";
            this.colOfferSize.ReadOnly = true;
            // 
            // colLowPrice
            // 
            this.colLowPrice.HeaderText = "Low Price";
            this.colLowPrice.Name = "colLowPrice";
            this.colLowPrice.ReadOnly = true;
            // 
            // colHighPrice
            // 
            this.colHighPrice.HeaderText = "High Price";
            this.colHighPrice.Name = "colHighPrice";
            this.colHighPrice.ReadOnly = true;
            // 
            // colTradeVolume
            // 
            this.colTradeVolume.HeaderText = "Trade Volume";
            this.colTradeVolume.Name = "colTradeVolume";
            this.colTradeVolume.ReadOnly = true;
            // 
            // colMode
            // 
            this.colMode.HeaderText = "Mode";
            this.colMode.Name = "colMode";
            this.colMode.ReadOnly = true;
            // 
            // productTextBox
            // 
            this.productTextBox.Location = new System.Drawing.Point(8, 12);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.Size = new System.Drawing.Size(100, 20);
            this.productTextBox.TabIndex = 2;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(129, 12);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 3;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // QuoteBoardEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(913, 389);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.productTextBox);
            this.Controls.Add(this.eventGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "QuoteBoardEventWindow";
            this.Text = "Quote Board Event Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSymbol;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNetChange;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBidSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBidPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastTradePrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastTradeSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOfferPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOfferSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLowPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHighPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTradeVolume;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMode;
        private System.Windows.Forms.TextBox productTextBox;
        private System.Windows.Forms.Button addButton;
    }
}