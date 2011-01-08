namespace ZenFireDev
{
    partial class OrderEventWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderEventWindow));
            this.eventGrid = new System.Windows.Forms.DataGridView();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colProduct = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOrderType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantityOpen = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantityfilled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantityCanceled = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTriggerPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFillPrice = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZenTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTag = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // eventGrid
            // 
            this.eventGrid.AllowUserToAddRows = false;
            this.eventGrid.AllowUserToDeleteRows = false;
            this.eventGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.eventGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.eventGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colType,
            this.colID,
            this.colAccount,
            this.colProduct,
            this.colOrderType,
            this.colStatus,
            this.colReason,
            this.colQuantity,
            this.colQuantityOpen,
            this.colQuantityfilled,
            this.colQuantityCanceled,
            this.colPrice,
            this.colTriggerPrice,
            this.colFillPrice,
            this.colZenTag,
            this.colTag});
            this.eventGrid.Location = new System.Drawing.Point(12, 12);
            this.eventGrid.Name = "eventGrid";
            this.eventGrid.ReadOnly = true;
            this.eventGrid.RowHeadersVisible = false;
            this.eventGrid.Size = new System.Drawing.Size(674, 249);
            this.eventGrid.TabIndex = 1;
            // 
            // colType
            // 
            this.colType.HeaderText = "Type";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colID
            // 
            this.colID.HeaderText = "ID";
            this.colID.Name = "colID";
            this.colID.ReadOnly = true;
            // 
            // colAccount
            // 
            this.colAccount.HeaderText = "Account";
            this.colAccount.Name = "colAccount";
            this.colAccount.ReadOnly = true;
            // 
            // colProduct
            // 
            this.colProduct.HeaderText = "Product";
            this.colProduct.Name = "colProduct";
            this.colProduct.ReadOnly = true;
            // 
            // colOrderType
            // 
            this.colOrderType.HeaderText = "Order.Type";
            this.colOrderType.Name = "colOrderType";
            this.colOrderType.ReadOnly = true;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            // 
            // colReason
            // 
            this.colReason.HeaderText = "Reason";
            this.colReason.Name = "colReason";
            this.colReason.ReadOnly = true;
            // 
            // colQuantity
            // 
            this.colQuantity.HeaderText = "Quantity";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            // 
            // colQuantityOpen
            // 
            this.colQuantityOpen.HeaderText = "QuantityOpen";
            this.colQuantityOpen.Name = "colQuantityOpen";
            this.colQuantityOpen.ReadOnly = true;
            // 
            // colQuantityfilled
            // 
            this.colQuantityfilled.HeaderText = "QuantityFilled";
            this.colQuantityfilled.Name = "colQuantityfilled";
            this.colQuantityfilled.ReadOnly = true;
            // 
            // colQuantityCanceled
            // 
            this.colQuantityCanceled.HeaderText = "QuantityCanceled";
            this.colQuantityCanceled.Name = "colQuantityCanceled";
            this.colQuantityCanceled.ReadOnly = true;
            // 
            // colPrice
            // 
            this.colPrice.HeaderText = "Price";
            this.colPrice.Name = "colPrice";
            this.colPrice.ReadOnly = true;
            // 
            // colTriggerPrice
            // 
            this.colTriggerPrice.HeaderText = "TriggerPrice";
            this.colTriggerPrice.Name = "colTriggerPrice";
            this.colTriggerPrice.ReadOnly = true;
            // 
            // colFillPrice
            // 
            this.colFillPrice.HeaderText = "FillPrice";
            this.colFillPrice.Name = "colFillPrice";
            this.colFillPrice.ReadOnly = true;
            // 
            // colZenTag
            // 
            this.colZenTag.HeaderText = "ZenTag";
            this.colZenTag.Name = "colZenTag";
            this.colZenTag.ReadOnly = true;
            // 
            // colTag
            // 
            this.colTag.HeaderText = "Tag";
            this.colTag.Name = "colTag";
            this.colTag.ReadOnly = true;
            // 
            // OrderEventWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 273);
            this.Controls.Add(this.eventGrid);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OrderEventWindow";
            this.Text = "Order Event Watcher";
            ((System.ComponentModel.ISupportInitialize)(this.eventGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView eventGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProduct;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOrderType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colReason;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantityOpen;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantityfilled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantityCanceled;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTriggerPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFillPrice;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZenTag;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTag;
    }
}