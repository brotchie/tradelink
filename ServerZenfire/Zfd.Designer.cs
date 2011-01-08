namespace ZenFireDev
{
    partial class Zfd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Zfd));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mENUToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orderEventWatcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tickWatcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quoteBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.positionDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOrdersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.placeOrderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accountListBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.completedOrdersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.mENUToolStripMenuItem,
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(227, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loginToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.loginToolStripMenuItem.Text = "Login";
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.loginToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            // 
            // mENUToolStripMenuItem
            // 
            this.mENUToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accountEventToolStripMenuItem,
            this.alertToolStripMenuItem,
            this.orderEventWatcherToolStripMenuItem,
            this.positionEventToolStripMenuItem,
            this.tickWatcherToolStripMenuItem,
            this.accountSummaryToolStripMenuItem,
            this.quoteBoardToolStripMenuItem,
            this.positionDetailsToolStripMenuItem,
            this.openOrdersToolStripMenuItem,
            this.completedOrdersToolStripMenuItem});
            this.mENUToolStripMenuItem.Name = "mENUToolStripMenuItem";
            this.mENUToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.mENUToolStripMenuItem.Text = "Events";
            // 
            // accountEventToolStripMenuItem
            // 
            this.accountEventToolStripMenuItem.Name = "accountEventToolStripMenuItem";
            this.accountEventToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.accountEventToolStripMenuItem.Text = "Account";
            this.accountEventToolStripMenuItem.Click += new System.EventHandler(this.accountEventToolStripMenuItem_Click);
            // 
            // alertToolStripMenuItem
            // 
            this.alertToolStripMenuItem.Name = "alertToolStripMenuItem";
            this.alertToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.alertToolStripMenuItem.Text = "Alert";
            this.alertToolStripMenuItem.Click += new System.EventHandler(this.alertToolStripMenuItem_Click);
            // 
            // orderEventWatcherToolStripMenuItem
            // 
            this.orderEventWatcherToolStripMenuItem.Name = "orderEventWatcherToolStripMenuItem";
            this.orderEventWatcherToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.orderEventWatcherToolStripMenuItem.Text = "Order";
            this.orderEventWatcherToolStripMenuItem.Click += new System.EventHandler(this.orderEventWatcherToolStripMenuItem_Click);
            // 
            // positionEventToolStripMenuItem
            // 
            this.positionEventToolStripMenuItem.Name = "positionEventToolStripMenuItem";
            this.positionEventToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.positionEventToolStripMenuItem.Text = "Position";
            this.positionEventToolStripMenuItem.Click += new System.EventHandler(this.positionEventWatcherToolStripMenuItem_Click);
            // 
            // tickWatcherToolStripMenuItem
            // 
            this.tickWatcherToolStripMenuItem.Name = "tickWatcherToolStripMenuItem";
            this.tickWatcherToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.tickWatcherToolStripMenuItem.Text = "Tick";
            this.tickWatcherToolStripMenuItem.Click += new System.EventHandler(this.tickWatcherToolStripMenuItem_Click);
            // 
            // accountSummaryToolStripMenuItem
            // 
            this.accountSummaryToolStripMenuItem.Name = "accountSummaryToolStripMenuItem";
            this.accountSummaryToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.accountSummaryToolStripMenuItem.Text = "Account Summary";
            this.accountSummaryToolStripMenuItem.Click += new System.EventHandler(this.accountSummaryEventToolStripMenuItem_Click);
            // 
            // quoteBoardToolStripMenuItem
            // 
            this.quoteBoardToolStripMenuItem.Name = "quoteBoardToolStripMenuItem";
            this.quoteBoardToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.quoteBoardToolStripMenuItem.Text = "Quote Board";
            this.quoteBoardToolStripMenuItem.Click += new System.EventHandler(this.quoteBoardEventToolStripMenuItem_Click);
            // 
            // positionDetailsToolStripMenuItem
            // 
            this.positionDetailsToolStripMenuItem.Name = "positionDetailsToolStripMenuItem";
            this.positionDetailsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.positionDetailsToolStripMenuItem.Text = "Position Details";
            this.positionDetailsToolStripMenuItem.Click += new System.EventHandler(this.positionDetailsEventToolStripMenuItem_Click);
            // 
            // openOrdersToolStripMenuItem
            // 
            this.openOrdersToolStripMenuItem.Name = "openOrdersToolStripMenuItem";
            this.openOrdersToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.openOrdersToolStripMenuItem.Text = "Open Orders";
            this.openOrdersToolStripMenuItem.Click += new System.EventHandler(this.openOrdersEventToolStripMenuItem_Click);
            // 
            // completedOrdersToolStripMenuItem
            // 
            this.completedOrdersToolStripMenuItem.Name = "completedOrdersToolStripMenuItem";
            this.completedOrdersToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.completedOrdersToolStripMenuItem.Text = "Completed Orders";
            this.completedOrdersToolStripMenuItem.Click += new System.EventHandler(this.completedOrdersEventToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placeOrderToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.actionsToolStripMenuItem.Text = "Actions";
            // 
            // placeOrderToolStripMenuItem
            // 
            this.placeOrderToolStripMenuItem.Name = "placeOrderToolStripMenuItem";
            this.placeOrderToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.placeOrderToolStripMenuItem.Text = "Place Order";
            this.placeOrderToolStripMenuItem.Click += new System.EventHandler(this.placeOrderToolStripMenuItem_Click);
            // 
            // accountListBox
            // 
            this.accountListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.accountListBox.CheckOnClick = true;
            this.accountListBox.FormattingEnabled = true;
            this.accountListBox.Location = new System.Drawing.Point(12, 59);
            this.accountListBox.Name = "accountListBox";
            this.accountListBox.Size = new System.Drawing.Size(203, 124);
            this.accountListBox.TabIndex = 4;
            this.accountListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.accountListBox_ItemCheck);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Accounts (check to subscribe)";            
            // 
            // Zfd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(227, 206);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.accountListBox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Zfd";
            this.Text = "ZenFireDev";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mENUToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orderEventWatcherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tickWatcherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accountEventToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem placeOrderToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox accountListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem loginToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accountSummaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quoteBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem positionDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOrdersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem completedOrdersToolStripMenuItem;
    }
}

