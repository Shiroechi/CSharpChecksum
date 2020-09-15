using System.Windows.Forms;

namespace CSharpChecksum.UserControls
{
	partial class AboutControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.GithubLabel = new System.Windows.Forms.Label();
			this.GithubRepositoryLink = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(30, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(570, 150);
			this.label1.TabIndex = 0;
			this.label1.Text = "Created by Ricky Setiawan\r\n\r\n\r\nThis application is free to use but all forms of r" +
    "esponsibility are the responsibility of the user, not the responsibility of the " +
    "developer.\r\n";
			// 
			// GithubLabel
			// 
			this.GithubLabel.Image = global::CSharpChecksum.Properties.Resources.icons8_github_32;
			this.GithubLabel.Location = new System.Drawing.Point(30, 180);
			this.GithubLabel.Name = "GithubLabel";
			this.GithubLabel.Size = new System.Drawing.Size(32, 32);
			this.GithubLabel.TabIndex = 1;
			this.GithubLabel.Click += new System.EventHandler(this.GithubLabel_Click);
			// 
			// GithubRepositoryLink
			// 
			this.GithubRepositoryLink.Location = new System.Drawing.Point(70, 180);
			this.GithubRepositoryLink.Name = "GithubRepositoryLink";
			this.GithubRepositoryLink.Size = new System.Drawing.Size(100, 32);
			this.GithubRepositoryLink.TabIndex = 2;
			this.GithubRepositoryLink.TabStop = true;
			this.GithubRepositoryLink.Text = "Shiroechi";
			this.GithubRepositoryLink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.GithubRepositoryLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GithubRepositoryLink_LinkClicked);
			// 
			// AboutControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.GithubRepositoryLink);
			this.Controls.Add(this.GithubLabel);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Consolas", 12F);
			this.Name = "AboutControl";
			this.Size = new System.Drawing.Size(634, 281);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private Label GithubLabel;
		private LinkLabel GithubRepositoryLink;
	}
}
