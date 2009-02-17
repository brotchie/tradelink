/*
 * Created by SharpDevelop.
 * User: josh
 * Date: 1/30/2008
 * Time: 4:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace TradeLib
{
	partial class ParamPrompt
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.applybut = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.CommandsVisibleIfAvailable = false;
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid1.Size = new System.Drawing.Size(292, 262);
			this.propertyGrid1.TabIndex = 0;
			// 
			// applybut
			// 
			this.applybut.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.applybut.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.applybut.Location = new System.Drawing.Point(0, 240);
			this.applybut.Name = "applybut";
			this.applybut.Size = new System.Drawing.Size(292, 22);
			this.applybut.TabIndex = 1;
			this.applybut.Text = "Apply Options";
			this.applybut.UseVisualStyleBackColor = true;
			this.applybut.Click += new System.EventHandler(this.ApplybutClick);
			// 
			// ParamPrompt
			// 
			this.AcceptButton = this.applybut;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(292, 262);
			this.Controls.Add(this.applybut);
			this.Controls.Add(this.propertyGrid1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ParamPrompt";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "ParamPrompt";
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button applybut;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
	}
}
