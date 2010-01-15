using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Redi_CSharp_Test
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnSendOrder;
		private System.Windows.Forms.Label lblReturn;
		private System.Windows.Forms.Button btnOpenMsg;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblConsole;
		private System.Windows.Forms.ListBox listBox1;

//Initialize Redi VB Class Library
        VBRediClasses.VBCacheClass CacheClass;
		bool IsMsgTableOpen=false;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnSendOrder = new System.Windows.Forms.Button();
			this.lblReturn = new System.Windows.Forms.Label();
			this.btnOpenMsg = new System.Windows.Forms.Button();
			this.lblConsole = new System.Windows.Forms.Label();
			this.listBox1 = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// btnSendOrder
			// 
			this.btnSendOrder.Location = new System.Drawing.Point(20, 17);
			this.btnSendOrder.Name = "btnSendOrder";
			this.btnSendOrder.Size = new System.Drawing.Size(96, 31);
			this.btnSendOrder.TabIndex = 0;
			this.btnSendOrder.Text = "Send Order";
			this.btnSendOrder.Click += new System.EventHandler(this.btnSendOrder_Click);
			// 
			// lblReturn
			// 
			this.lblReturn.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblReturn.Location = new System.Drawing.Point(145, 23);
			this.lblReturn.Name = "lblReturn";
			this.lblReturn.Size = new System.Drawing.Size(99, 21);
			this.lblReturn.TabIndex = 1;
			// 
			// btnOpenMsg
			// 
			this.btnOpenMsg.Location = new System.Drawing.Point(21, 67);
			this.btnOpenMsg.Name = "btnOpenMsg";
			this.btnOpenMsg.Size = new System.Drawing.Size(93, 38);
			this.btnOpenMsg.TabIndex = 2;
			this.btnOpenMsg.Text = "Open Msg Table";
			this.btnOpenMsg.Click += new System.EventHandler(this.btnOpenMsg_Click);
			// 
			// lblConsole
			// 
			this.lblConsole.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblConsole.Location = new System.Drawing.Point(143, 62);
			this.lblConsole.Name = "lblConsole";
			this.lblConsole.Size = new System.Drawing.Size(129, 47);
			this.lblConsole.TabIndex = 3;
			// 
			// listBox1
			// 
			this.listBox1.Location = new System.Drawing.Point(15, 123);
			this.listBox1.Name = "listBox1";
			this.listBox1.Size = new System.Drawing.Size(654, 264);
			this.listBox1.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(688, 408);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.listBox1,
																		  this.lblConsole,
																		  this.btnOpenMsg,
																		  this.lblReturn,
																		  this.btnSendOrder});
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
            try
            {
                CacheClass = new VBRediClasses.VBCacheClass();
                //Define/Map local Event Handler for Redi Cache Events
                this.CacheClass.VBRediCache.CacheEvent += new RediLib.ECacheControl_CacheEventEventHandler(this.RediCacheEvent);
            }
            catch (Exception ex)
            {
                listBox1.Items.Add("Error.   Did you forget to login to Redi?");
                listBox1.Items.Add(ex.Message + ex.StackTrace);
            }
		}

		private void btnSendOrder_Click(object sender, System.EventArgs e)
		{
            if (CacheClass == null) return;
//Initialize Redi VB Class Library
			VBRediClasses.VBOrderClass OrderClss = new VBRediClasses.VBOrderClass();
			string errstring="";
			bool mybool=false;
//Call to VB Class which in turn calls Redi Order Submit function
//VB Class needs to be expanded for more order fields
			OrderClss.VBSubmit(ref mybool, ref errstring,"ZXZZT", "Buy");
			lblReturn.Text = errstring;
		}

		private void btnOpenMsg_Click(object sender, System.EventArgs e)
		{
            if (CacheClass == null) return;
			string errstring="";
			if (IsMsgTableOpen)
			{
				string strErr="";
//Call to VB Class which in turn calls Redi Query Revoke
				CacheClass.VBRevokeObject(ref strErr);
			}
//Call to VB Class which in turn calls Redi Query Submit
			CacheClass.VBSubmit(ref IsMsgTableOpen,ref errstring);
		}
		public void RediCacheEvent(int action, int row)
		{
            if (CacheClass == null) return;
			lblConsole.Text=row + " --- " + action;
			object CellValue = new object();
			int ErrCode = 0;
			int i=0;
			if (action==1)
			{
				for (i=0;i < row;i++)
				{
//Call to VB Class which in turn calls Redi GetCell Sub
					CacheClass.VBGetCell(i,"Text",ref CellValue,ref ErrCode);
					listBox1.Items.Add(CellValue);
				}
			}
			else
			{
				i=row;
//Call to VB Class which in turn calls Redi GetCell Sub
				CacheClass.VBGetCell(i,"Text",ref CellValue,ref ErrCode);
				listBox1.Items.Add(CellValue);
			}
		}
	}
}
