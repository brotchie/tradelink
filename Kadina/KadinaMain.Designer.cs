namespace Kadina
{
    partial class kadinamain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(kadinamain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.recent = new System.Windows.Forms.ToolStripDropDownButton();
            this.reslist = new System.Windows.Forms.ToolStripDropDownButton();
            this._tabs = new System.Windows.Forms.TabControl();
            this.tabmsg = new System.Windows.Forms.TabPage();
            this.msgbox = new System.Windows.Forms.RichTextBox();
            this.ticktab = new System.Windows.Forms.TabPage();
            this.itab = new System.Windows.Forms.TabPage();
            this.postab = new System.Windows.Forms.TabPage();
            this.ordertab = new System.Windows.Forms.TabPage();
            this.filltab = new System.Windows.Forms.TabPage();
            this.charttab = new System.Windows.Forms.TabPage();
            this._results = new System.Windows.Forms.TabPage();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this._stat = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1.SuspendLayout();
            this._tabs.SuspendLayout();
            this.tabmsg.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recent,
            this.reslist});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 5, 1, 34);
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(801, 45);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            this.toolTip1.SetToolTip(this.statusStrip1, "Study Options");
            // 
            // recent
            // 
            this.recent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.recent.Image = ((System.Drawing.Image)(resources.GetObject("recent.Image")));
            this.recent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recent.Name = "recent";
            this.recent.Size = new System.Drawing.Size(99, 29);
            this.recent.Text = "Add data";
            this.recent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.recent.ToolTipText = "Add historical tick files to study";
            this.recent.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recent_DropDownItemClicked);
            // 
            // reslist
            // 
            this.reslist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.reslist.Image = ((System.Drawing.Image)(resources.GetObject("reslist.Image")));
            this.reslist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reslist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.reslist.Name = "reslist";
            this.reslist.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.reslist.Size = new System.Drawing.Size(136, 29);
            this.reslist.Text = "Add response";
            this.reslist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.reslist.ToolTipText = "Choose response library and a response to study";
            this.reslist.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.libs_DropDownItemClicked);
            // 
            // _tabs
            // 
            this._tabs.AllowDrop = true;
            this._tabs.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this._tabs.Controls.Add(this.tabmsg);
            this._tabs.Controls.Add(this.ticktab);
            this._tabs.Controls.Add(this.itab);
            this._tabs.Controls.Add(this.postab);
            this._tabs.Controls.Add(this.ordertab);
            this._tabs.Controls.Add(this.filltab);
            this._tabs.Controls.Add(this.charttab);
            this._tabs.Controls.Add(this._results);
            this._tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabs.Location = new System.Drawing.Point(0, 45);
            this._tabs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this._tabs.Name = "_tabs";
            this._tabs.SelectedIndex = 0;
            this._tabs.Size = new System.Drawing.Size(801, 310);
            this._tabs.TabIndex = 2;
            this._tabs.DragDrop += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragDrop);
            this._tabs.DragEnter += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragEnter);
            // 
            // tabmsg
            // 
            this.tabmsg.Controls.Add(this.msgbox);
            this.tabmsg.Location = new System.Drawing.Point(4, 32);
            this.tabmsg.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabmsg.Name = "tabmsg";
            this.tabmsg.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabmsg.Size = new System.Drawing.Size(793, 274);
            this.tabmsg.TabIndex = 0;
            this.tabmsg.Text = "Messages";
            this.tabmsg.UseVisualStyleBackColor = true;
            // 
            // msgbox
            // 
            this.msgbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msgbox.Location = new System.Drawing.Point(4, 5);
            this.msgbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.msgbox.Name = "msgbox";
            this.msgbox.ReadOnly = true;
            this.msgbox.Size = new System.Drawing.Size(785, 264);
            this.msgbox.TabIndex = 0;
            this.msgbox.Text = "";
            // 
            // ticktab
            // 
            this.ticktab.Location = new System.Drawing.Point(4, 32);
            this.ticktab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ticktab.Name = "ticktab";
            this.ticktab.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ticktab.Size = new System.Drawing.Size(793, 274);
            this.ticktab.TabIndex = 1;
            this.ticktab.Text = "Ticks";
            this.ticktab.UseVisualStyleBackColor = true;
            // 
            // itab
            // 
            this.itab.Location = new System.Drawing.Point(4, 32);
            this.itab.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.itab.Name = "itab";
            this.itab.Size = new System.Drawing.Size(793, 274);
            this.itab.TabIndex = 2;
            this.itab.Text = "Indicators";
            this.itab.UseVisualStyleBackColor = true;
            // 
            // postab
            // 
            this.postab.Location = new System.Drawing.Point(4, 32);
            this.postab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.postab.Name = "postab";
            this.postab.Size = new System.Drawing.Size(793, 274);
            this.postab.TabIndex = 3;
            this.postab.Text = "Position";
            this.postab.UseVisualStyleBackColor = true;
            // 
            // ordertab
            // 
            this.ordertab.Location = new System.Drawing.Point(4, 32);
            this.ordertab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ordertab.Name = "ordertab";
            this.ordertab.Size = new System.Drawing.Size(793, 274);
            this.ordertab.TabIndex = 4;
            this.ordertab.Text = "Orders";
            this.ordertab.UseVisualStyleBackColor = true;
            // 
            // filltab
            // 
            this.filltab.Location = new System.Drawing.Point(4, 32);
            this.filltab.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.filltab.Name = "filltab";
            this.filltab.Size = new System.Drawing.Size(793, 274);
            this.filltab.TabIndex = 5;
            this.filltab.Text = "Fills";
            this.filltab.UseVisualStyleBackColor = true;
            // 
            // charttab
            // 
            this.charttab.Location = new System.Drawing.Point(4, 32);
            this.charttab.Name = "charttab";
            this.charttab.Size = new System.Drawing.Size(793, 274);
            this.charttab.TabIndex = 6;
            this.charttab.Text = "Chart";
            this.charttab.UseVisualStyleBackColor = true;
            // 
            // _results
            // 
            this._results.Location = new System.Drawing.Point(4, 32);
            this._results.Name = "_results";
            this._results.Padding = new System.Windows.Forms.Padding(3);
            this._results.Size = new System.Drawing.Size(793, 274);
            this._results.TabIndex = 7;
            this._results.Text = "Results";
            this._results.UseVisualStyleBackColor = true;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._stat});
            this.statusStrip2.Location = new System.Drawing.Point(0, 325);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Size = new System.Drawing.Size(801, 30);
            this.statusStrip2.SizingGrip = false;
            this.statusStrip2.TabIndex = 1;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // _stat
            // 
            this._stat.Name = "_stat";
            this._stat.Size = new System.Drawing.Size(519, 25);
            this._stat.Text = "Kadina lets you see your response to a given set of market data.";
            // 
            // kadinamain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 355);
            this.Controls.Add(this.statusStrip2);
            this.Controls.Add(this._tabs);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "kadinamain";
            this.Text = "Kadina";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this._tabs.ResumeLayout(false);
            this.tabmsg.ResumeLayout(false);
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton reslist;
        private System.Windows.Forms.TabControl _tabs;
        private System.Windows.Forms.TabPage tabmsg;
        private System.Windows.Forms.RichTextBox msgbox;
        private System.Windows.Forms.TabPage ticktab;
        private System.Windows.Forms.ToolStripDropDownButton recent;
        private System.Windows.Forms.TabPage itab;
        private System.Windows.Forms.TabPage postab;
        private System.Windows.Forms.TabPage ordertab;
        private System.Windows.Forms.TabPage filltab;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel _stat;
        private System.Windows.Forms.TabPage charttab;
        private System.Windows.Forms.TabPage _results;
    }
}

