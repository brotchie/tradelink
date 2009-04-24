using System;
using System.Drawing;
using System.Windows.Forms;
using TradeLink.API;

namespace TradeLink.Common
{
	/// <summary>
	/// Param Prompt quickly creates a GUI to edit properties of your Response
	/// </summary>
	public partial class ParamPrompt : Form
	{
        private Button applyoptionsbut;
        private PropertyGrid propertyGrid1;

        /// <summary>
        /// You create a param prompt to display a gui for your Response
        /// </summary>
        /// <param name="DisplayParamsOfObject">The Response for which you want the user to specify or alter the properties for.</param>
		public ParamPrompt(Response DisplayParamsOfObject)
		{

			InitializeComponent();
			this.Text = DisplayParamsOfObject.Name + " Options";
            propertyGrid1.PropertySort = PropertySort.Categorized;
			propertyGrid1.SelectedObject = (object)DisplayParamsOfObject;

		}
        /// <summary>
        /// popup the parameters for selected response
        /// </summary>
        public static void Popup(Response displayParamsAvail) { Popup(displayParamsAvail, false,false); }
        public static void Popup(Response displayParamsAvail, bool skip) { Popup(displayParamsAvail, false, skip); }
        /// <summary>
        /// pop up the parameters, allows you to pausing the application to do so (also can skip prompt)
        /// </summary>
        /// <param name="pauseapp"></param>
        public static void Popup(Response displayParamsAvail, bool pauseapp, bool skip)
        {
            if (skip) return;
            ParamPrompt p = new ParamPrompt(displayParamsAvail);

            p.Invalidate(true);
            if (pauseapp)
                p.ShowDialog();
            else
                p.Show();
            p.Invalidate(true);
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
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(292, 261);
            this.propertyGrid1.TabIndex = 0;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // applyoptionsbut
            // 
            this.applyoptionsbut.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.applyoptionsbut.Location = new System.Drawing.Point(0, 231);
            this.applyoptionsbut.Name = "applyoptionsbut";
            this.applyoptionsbut.Size = new System.Drawing.Size(292, 30);
            this.applyoptionsbut.TabIndex = 1;
            this.applyoptionsbut.Text = "Accept";
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
