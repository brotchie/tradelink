namespace ZenFireDev
{
    partial class PlaceOrderWindow
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label9;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaceOrderWindow));
            this.actionComboBox = new System.Windows.Forms.ComboBox();
            this.quantitySpinner = new System.Windows.Forms.NumericUpDown();
            this.productTextBox = new System.Windows.Forms.TextBox();
            this.accountComboBox = new System.Windows.Forms.ComboBox();
            this.durationComboBox = new System.Windows.Forms.ComboBox();
            this.typeComboBox = new System.Windows.Forms.ComboBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.exchangeComboBox = new System.Windows.Forms.ComboBox();
            this.priceSpinner = new System.Windows.Forms.NumericUpDown();
            this.triggerPriceSpinner = new System.Windows.Forms.NumericUpDown();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.quantitySpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.triggerPriceSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(9, 39);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(47, 13);
            label1.TabIndex = 7;
            label1.Text = "Account";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(130, 39);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(44, 13);
            label2.TabIndex = 9;
            label2.Text = "Product";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(481, 39);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(31, 13);
            label3.TabIndex = 10;
            label3.Text = "Type";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(416, 39);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(47, 13);
            label4.TabIndex = 11;
            label4.Text = "Duration";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(359, 39);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(46, 13);
            label5.TabIndex = 12;
            label5.Text = "Quantity";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(294, 39);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(37, 13);
            label6.TabIndex = 13;
            label6.Text = "Action";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(222, 39);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(55, 13);
            label7.TabIndex = 15;
            label7.Text = "Exchange";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(9, 81);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(31, 13);
            label8.TabIndex = 16;
            label8.Text = "Price";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(130, 81);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(64, 13);
            label9.TabIndex = 17;
            label9.Text = "TriggerPrice";
            // 
            // actionComboBox
            // 
            this.actionComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.actionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.actionComboBox.FormattingEnabled = true;
            this.actionComboBox.Location = new System.Drawing.Point(297, 57);
            this.actionComboBox.Name = "actionComboBox";
            this.actionComboBox.Size = new System.Drawing.Size(59, 21);
            this.actionComboBox.TabIndex = 4;
            // 
            // quantitySpinner
            // 
            this.quantitySpinner.Location = new System.Drawing.Point(362, 58);
            this.quantitySpinner.Name = "quantitySpinner";
            this.quantitySpinner.Size = new System.Drawing.Size(51, 20);
            this.quantitySpinner.TabIndex = 5;
            this.quantitySpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // productTextBox
            // 
            this.productTextBox.Location = new System.Drawing.Point(133, 58);
            this.productTextBox.Name = "productTextBox";
            this.productTextBox.Size = new System.Drawing.Size(86, 20);
            this.productTextBox.TabIndex = 2;
            this.productTextBox.Leave += new System.EventHandler(this.product_Changed);
            // 
            // accountComboBox
            // 
            this.accountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.accountComboBox.FormattingEnabled = true;
            this.accountComboBox.Location = new System.Drawing.Point(12, 57);
            this.accountComboBox.Name = "accountComboBox";
            this.accountComboBox.Size = new System.Drawing.Size(115, 21);
            this.accountComboBox.TabIndex = 1;
            // 
            // durationComboBox
            // 
            this.durationComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.durationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.durationComboBox.FormattingEnabled = true;
            this.durationComboBox.Location = new System.Drawing.Point(419, 57);
            this.durationComboBox.Name = "durationComboBox";
            this.durationComboBox.Size = new System.Drawing.Size(59, 21);
            this.durationComboBox.TabIndex = 6;
            // 
            // typeComboBox
            // 
            this.typeComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.typeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeComboBox.FormattingEnabled = true;
            this.typeComboBox.Location = new System.Drawing.Point(484, 57);
            this.typeComboBox.Name = "typeComboBox";
            this.typeComboBox.Size = new System.Drawing.Size(103, 21);
            this.typeComboBox.TabIndex = 7;
            this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.product_Changed);
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(512, 94);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 8;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // exchangeComboBox
            // 
            this.exchangeComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.exchangeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exchangeComboBox.FormattingEnabled = true;
            this.exchangeComboBox.Location = new System.Drawing.Point(225, 57);
            this.exchangeComboBox.Name = "exchangeComboBox";
            this.exchangeComboBox.Size = new System.Drawing.Size(66, 21);
            this.exchangeComboBox.TabIndex = 3;
            this.exchangeComboBox.Leave += new System.EventHandler(this.product_Changed);
            // 
            // priceSpinner
            // 
            this.priceSpinner.DecimalPlaces = 2;
            this.priceSpinner.Enabled = false;
            this.priceSpinner.Location = new System.Drawing.Point(12, 97);
            this.priceSpinner.Name = "priceSpinner";
            this.priceSpinner.Size = new System.Drawing.Size(115, 20);
            this.priceSpinner.TabIndex = 18;
            // 
            // triggerPriceSpinner
            // 
            this.triggerPriceSpinner.DecimalPlaces = 2;
            this.triggerPriceSpinner.Enabled = false;
            this.triggerPriceSpinner.Location = new System.Drawing.Point(133, 97);
            this.triggerPriceSpinner.Name = "triggerPriceSpinner";
            this.triggerPriceSpinner.Size = new System.Drawing.Size(86, 20);
            this.triggerPriceSpinner.TabIndex = 19;
            // 
            // PlaceOrderWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 151);
            this.Controls.Add(this.triggerPriceSpinner);
            this.Controls.Add(this.priceSpinner);
            this.Controls.Add(label9);
            this.Controls.Add(label8);
            this.Controls.Add(label7);
            this.Controls.Add(this.exchangeComboBox);
            this.Controls.Add(label6);
            this.Controls.Add(label5);
            this.Controls.Add(label4);
            this.Controls.Add(label3);
            this.Controls.Add(label2);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(label1);
            this.Controls.Add(this.typeComboBox);
            this.Controls.Add(this.durationComboBox);
            this.Controls.Add(this.accountComboBox);
            this.Controls.Add(this.productTextBox);
            this.Controls.Add(this.quantitySpinner);
            this.Controls.Add(this.actionComboBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PlaceOrderWindow";
            this.Text = "PlaceOrderForm";
            ((System.ComponentModel.ISupportInitialize)(this.quantitySpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.priceSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.triggerPriceSpinner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox actionComboBox;
        private System.Windows.Forms.NumericUpDown quantitySpinner;
        private System.Windows.Forms.TextBox productTextBox;
        private System.Windows.Forms.ComboBox accountComboBox;
        private System.Windows.Forms.ComboBox durationComboBox;
        private System.Windows.Forms.ComboBox typeComboBox;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.ComboBox exchangeComboBox;
        private System.Windows.Forms.NumericUpDown priceSpinner;
        private System.Windows.Forms.NumericUpDown triggerPriceSpinner;

    }
}