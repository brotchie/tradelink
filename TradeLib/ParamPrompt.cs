/*
 * Created by SharpDevelop.
 * User: josh
 * Date: 1/30/2008
 * Time: 4:29 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TradeLib
{
	/// <summary>
	/// Param Prompts use .NET property grids to allow users to edit box properties from a GUI interface without needing to edit source code.
	/// </summary>
	public partial class ParamPrompt : Form
	{
        private Button applyoptionsbut;
        private PropertyGrid propertyGrid1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParamPrompt"/> class.
        /// </summary>
        /// <param name="DisplayParamsOfObject">The Box for which you want the user to specify or alter the properties for.</param>
		public ParamPrompt(Box DisplayParamsOfObject)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			this.Text = DisplayParamsOfObject.Name + " Options";
            propertyGrid1.PropertySort = PropertySort.Categorized;
			propertyGrid1.SelectedObject = (object)DisplayParamsOfObject;
		
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void ApplybutClick(object sender, EventArgs e)
		{
			this.Close();
		}

        private void InitializeComponent()
        {
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.applyoptionsbut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CommandsVisibleIfAvailable = false;
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(292, 225);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // applyoptionsbut
            // 
            this.applyoptionsbut.Location = new System.Drawing.Point(114, 231);
            this.applyoptionsbut.Name = "applyoptionsbut";
            this.applyoptionsbut.Size = new System.Drawing.Size(75, 23);
            this.applyoptionsbut.TabIndex = 1;
            this.applyoptionsbut.Text = "Apply";
            this.applyoptionsbut.UseVisualStyleBackColor = true;
            this.applyoptionsbut.Click += new System.EventHandler(this.ApplybutClick);
            // 
            // ParamPrompt
            // 
            this.ClientSize = new System.Drawing.Size(292, 261);
            this.Controls.Add(this.applyoptionsbut);
            this.Controls.Add(this.propertyGrid1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ParamPrompt";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.ResumeLayout(false);

        }
	}
}
