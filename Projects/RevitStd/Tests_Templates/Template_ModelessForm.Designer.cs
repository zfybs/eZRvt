// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports



namespace RevitStd.Tests_Templates
{
public partial class Template_ModelessForm : System.Windows.Forms.Form
	{
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.BtnPick = new System.Windows.Forms.Button();
			this.BtnPick.Click += new System.EventHandler(this.Button1_Click);
			this.BtnDelete = new System.Windows.Forms.Button();
			this.BtnDelete.Click += new System.EventHandler(this.Button2_Click);
			this.SuspendLayout();
			//
			//BtnPick
			//
			this.BtnPick.Location = new System.Drawing.Point(32, 12);
			this.BtnPick.Name = "BtnPick";
			this.BtnPick.Size = new System.Drawing.Size(75, 23);
			this.BtnPick.TabIndex = 0;
			this.BtnPick.Text = "选择";
			this.BtnPick.UseVisualStyleBackColor = true;
			//
			//BtnDelete
			//
			this.BtnDelete.Location = new System.Drawing.Point(122, 12);
			this.BtnDelete.Name = "BtnDelete";
			this.BtnDelete.Size = new System.Drawing.Size(75, 23);
			this.BtnDelete.TabIndex = 0;
			this.BtnDelete.Text = "删除";
			this.BtnDelete.UseVisualStyleBackColor = true;
			//
			//ModelessForm
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (6.0F), (float) (12.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(227, 51);
			this.Controls.Add(this.BtnDelete);
			this.Controls.Add(this.BtnPick);
			this.Name = "ModelessForm";
			this.Text = "ModelessForm";
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.Button BtnPick;
		internal System.Windows.Forms.Button BtnDelete;
	}
}
