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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(kadinamain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.recent = new System.Windows.Forms.ToolStripDropDownButton();
            this.boxlist = new System.Windows.Forms.ToolStripDropDownButton();
            this.playtobut = new System.Windows.Forms.ToolStripDropDownButton();
            this.filter = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabmsg = new System.Windows.Forms.TabPage();
            this.statusStrip2 = new System.Windows.Forms.StatusStrip();
            this.statuslab = new System.Windows.Forms.ToolStripStatusLabel();
            this.msgbox = new System.Windows.Forms.RichTextBox();
            this.ticktab = new System.Windows.Forms.TabPage();
            this.itab = new System.Windows.Forms.TabPage();
            this.postab = new System.Windows.Forms.TabPage();
            this.ordertab = new System.Windows.Forms.TabPage();
            this.filltab = new System.Windows.Forms.TabPage();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabmsg.SuspendLayout();
            this.statusStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.AllowItemReorder = true;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.statusStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.recent,
            this.boxlist,
            this.playtobut,
            this.filter});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 4, 1, 27);
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(72, 178);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // recent
            // 
            this.recent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.recent.Image = ((System.Drawing.Image)(resources.GetObject("recent.Image")));
            this.recent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.recent.Name = "recent";
            this.recent.Size = new System.Drawing.Size(67, 24);
            this.recent.Text = "Recent";
            this.recent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.recent.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recent_DropDownItemClicked);
            // 
            // boxlist
            // 
            this.boxlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.boxlist.Image = ((System.Drawing.Image)(resources.GetObject("boxlist.Image")));
            this.boxlist.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.boxlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.boxlist.Name = "boxlist";
            this.boxlist.Size = new System.Drawing.Size(67, 24);
            this.boxlist.Text = "Boxes";
            this.boxlist.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // playtobut
            // 
            this.playtobut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.playtobut.Image = ((System.Drawing.Image)(resources.GetObject("playtobut.Image")));
            this.playtobut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.playtobut.Name = "playtobut";
            this.playtobut.Size = new System.Drawing.Size(67, 24);
            this.playtobut.Text = "Play To";
            this.playtobut.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // filter
            // 
            this.filter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.filter.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3});
            this.filter.Image = ((System.Drawing.Image)(resources.GetObject("filter.Image")));
            this.filter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.filter.Name = "filter";
            this.filter.Size = new System.Drawing.Size(67, 24);
            this.filter.Text = "Filter";
            this.filter.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.filter.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.filter_DropDownItemClicked);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Checked = true;
            this.toolStripMenuItem1.CheckOnClick = true;
            this.toolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripMenuItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenuItem1.Text = "NYS";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenuItem2.Text = "PSE";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.CheckOnClick = true;
            this.toolStripMenuItem3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(110, 24);
            this.toolStripMenuItem3.Text = "NMS";
            // 
            // tabControl1
            // 
            this.tabControl1.AllowDrop = true;
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabmsg);
            this.tabControl1.Controls.Add(this.ticktab);
            this.tabControl1.Controls.Add(this.itab);
            this.tabControl1.Controls.Add(this.postab);
            this.tabControl1.Controls.Add(this.ordertab);
            this.tabControl1.Controls.Add(this.filltab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(72, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(704, 178);
            this.tabControl1.TabIndex = 2;
            this.tabControl1.DragDrop += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragDrop);
            this.tabControl1.DragEnter += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragEnter);
            // 
            // tabmsg
            // 
            this.tabmsg.Controls.Add(this.statusStrip2);
            this.tabmsg.Controls.Add(this.msgbox);
            this.tabmsg.Location = new System.Drawing.Point(4, 28);
            this.tabmsg.Margin = new System.Windows.Forms.Padding(4);
            this.tabmsg.Name = "tabmsg";
            this.tabmsg.Padding = new System.Windows.Forms.Padding(4);
            this.tabmsg.Size = new System.Drawing.Size(696, 146);
            this.tabmsg.TabIndex = 0;
            this.tabmsg.Text = "Messages";
            this.tabmsg.UseVisualStyleBackColor = true;
            // 
            // statusStrip2
            // 
            this.statusStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statuslab});
            this.statusStrip2.Location = new System.Drawing.Point(4, 117);
            this.statusStrip2.Name = "statusStrip2";
            this.statusStrip2.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip2.Size = new System.Drawing.Size(688, 25);
            this.statusStrip2.TabIndex = 1;
            this.statusStrip2.Text = "statusStrip2";
            // 
            // statuslab
            // 
            this.statuslab.Name = "statuslab";
            this.statuslab.Size = new System.Drawing.Size(140, 20);
            this.statuslab.Text = "Welcome to Kadina";
            // 
            // msgbox
            // 
            this.msgbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.msgbox.Location = new System.Drawing.Point(4, 4);
            this.msgbox.Margin = new System.Windows.Forms.Padding(4);
            this.msgbox.Name = "msgbox";
            this.msgbox.ReadOnly = true;
            this.msgbox.Size = new System.Drawing.Size(688, 138);
            this.msgbox.TabIndex = 0;
            this.msgbox.Text = "";
            // 
            // ticktab
            // 
            this.ticktab.Location = new System.Drawing.Point(4, 28);
            this.ticktab.Margin = new System.Windows.Forms.Padding(4);
            this.ticktab.Name = "ticktab";
            this.ticktab.Padding = new System.Windows.Forms.Padding(4);
            this.ticktab.Size = new System.Drawing.Size(696, 146);
            this.ticktab.TabIndex = 1;
            this.ticktab.Text = "Ticks";
            this.ticktab.UseVisualStyleBackColor = true;
            // 
            // itab
            // 
            this.itab.Location = new System.Drawing.Point(4, 28);
            this.itab.Margin = new System.Windows.Forms.Padding(4);
            this.itab.Name = "itab";
            this.itab.Size = new System.Drawing.Size(696, 146);
            this.itab.TabIndex = 2;
            this.itab.Text = "Indicators";
            this.itab.UseVisualStyleBackColor = true;
            // 
            // postab
            // 
            this.postab.Location = new System.Drawing.Point(4, 28);
            this.postab.Name = "postab";
            this.postab.Size = new System.Drawing.Size(696, 146);
            this.postab.TabIndex = 3;
            this.postab.Text = "Position";
            this.postab.UseVisualStyleBackColor = true;
            // 
            // ordertab
            // 
            this.ordertab.Location = new System.Drawing.Point(4, 28);
            this.ordertab.Name = "ordertab";
            this.ordertab.Size = new System.Drawing.Size(696, 146);
            this.ordertab.TabIndex = 4;
            this.ordertab.Text = "Orders";
            this.ordertab.UseVisualStyleBackColor = true;
            // 
            // filltab
            // 
            this.filltab.Location = new System.Drawing.Point(4, 28);
            this.filltab.Name = "filltab";
            this.filltab.Size = new System.Drawing.Size(696, 146);
            this.filltab.TabIndex = 5;
            this.filltab.Text = "Fills";
            this.filltab.UseVisualStyleBackColor = true;
            // 
            // kadinamain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(776, 178);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "kadinamain";
            this.Text = "Kadina";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.kadinamain_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabmsg.ResumeLayout(false);
            this.tabmsg.PerformLayout();
            this.statusStrip2.ResumeLayout(false);
            this.statusStrip2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripDropDownButton boxlist;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabmsg;
        private System.Windows.Forms.RichTextBox msgbox;
        private System.Windows.Forms.ToolStripDropDownButton playtobut;
        private System.Windows.Forms.TabPage ticktab;
        private System.Windows.Forms.ToolStripDropDownButton recent;
        private System.Windows.Forms.ToolStripSplitButton filter;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.StatusStrip statusStrip2;
        private System.Windows.Forms.ToolStripStatusLabel statuslab;
        private System.Windows.Forms.TabPage itab;
        private System.Windows.Forms.TabPage postab;
        private System.Windows.Forms.TabPage ordertab;
        private System.Windows.Forms.TabPage filltab;
    }
}

