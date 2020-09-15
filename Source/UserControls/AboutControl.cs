using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace CSharpChecksum.UserControls
{
	public partial class AboutControl : UserControl
	{
		public AboutControl()
		{
			InitializeComponent();
		}

		~AboutControl()
		{
			label1.Dispose();
			GithubLabel.Dispose();
			GithubRepositoryLink.Dispose();
		}

		private void GithubRepositoryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("https://github.com/Shiroechi/CSharp-Checksum");
		}

		private void GithubLabel_Click(object sender, EventArgs e)
		{
			Process.Start("https://github.com/Shiroechi/CSharp-Checksum");
		}
	}
}
