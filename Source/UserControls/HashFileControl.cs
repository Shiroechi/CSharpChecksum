using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using CSharpChecksum.Entities;
using CSharpChecksum.Properties;

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace CSharpChecksum.UserControls
{
	public partial class HashFileControl : UserControl
	{
		#region Member

		private IDigest _Hash;
		private BackgroundWorker _BackgroundWorker;
		private long _LimitSize;
		private byte[] _BlockSize;
		private long _FileSize;

		#endregion Member

		#region Constructor & Destructor

		public HashFileControl()
		{
			InitializeComponent();

			this._LimitSize = Settings.Default.LimitSize * 1048576;
			this._BlockSize = new byte[Settings.Default.BlockSize];

			this._BackgroundWorker = new BackgroundWorker();
			this._BackgroundWorker.WorkerSupportsCancellation = true;
			this._BackgroundWorker.WorkerReportsProgress = true;
			this._BackgroundWorker.DoWork += Hashing_DoWork;
			this._BackgroundWorker.ProgressChanged += Hashing_ProgressChanged;
			this._BackgroundWorker.RunWorkerCompleted += Hahsing_RunWorkerCompleted;
		}

		~HashFileControl()
		{
			if (this._BackgroundWorker.IsBusy)
			{
				this._BackgroundWorker.CancelAsync();
			}
			
			this._BackgroundWorker.Dispose();
			
			if (this._BlockSize != null || this._BlockSize.Length != 0)
			{
				Array.Clear(this._BlockSize, 0, this._BlockSize.Length);
			}

			this.label1.Dispose();
			this.label2.Dispose();
			this.label4.Dispose();
			this.label5.Dispose();
			this.HashProgressBar.Dispose();
			this.FileLocationTextBox.Dispose();
			this.HashValueTextBox.Dispose();
			this.HashFunctionListComboBox.Dispose();
			this.BrowseButton.Dispose();
			this.CopyHashValueButton.Dispose();
			this.CancelHashButton.Dispose();
			this.HashButton.Dispose();
			this.SaveHashValueButton.Dispose();
		}

		#endregion Constructor & Destructor

		#region Form Event

		private void HashFileControl_Load(object sender, EventArgs e)
		{
			this.LoadHashList();
		}
		
		/// <summary>
		/// get file location that dropped inside form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HashFileControl_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
			{
				e.Effect = DragDropEffects.All;
			}

			var temp = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			this.FileLocationTextBox.Text = temp[0];
			this.HashValueTextBox.Text = "";
		}

		/// <summary>
		/// change icon if there file fragged inside form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HashFileControl_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}

		#endregion Form Event

		#region Button Event

		private void BrowseButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.InitialDirectory = "D:\\";
				openFileDialog.Title = "Select item";
				openFileDialog.Filter = "All file | *.*";
				openFileDialog.CheckFileExists = true;
				openFileDialog.CheckPathExists = true;
				openFileDialog.Multiselect = false;
				openFileDialog.RestoreDirectory = true;
				if (this.FileLocationTextBox.Text.Trim() != "")
				{
					openFileDialog.InitialDirectory = this.GetFileDirectory(FileLocationTextBox.Text);
				}
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					this.FileLocationTextBox.Text = openFileDialog.FileName.Trim();
				}
			}
		}

		private void CopyButton_Click(object sender, EventArgs e)
		{
			if (this.HashValueTextBox.Text.Trim() == "")
			{
				MessageBox.Show(this, "Please hash a file first.", "Attention");
			}
			else
			{
				Clipboard.SetText(this.HashValueTextBox.Text);
			}
		}

		private void SaveButton_Click(object sender, EventArgs e)
		{
			if (this.HashValueTextBox.Text.Trim() == "")
			{
				MessageBox.Show(this, "Please hash a file first.", "Attention");
				return;
			}
			
			string directoryName = Path.GetDirectoryName(this.FileLocationTextBox.Text);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.FileLocationTextBox.Text);
			
			try
			{
				string path = directoryName + "\\" + fileNameWithoutExtension + HashFunctionList.HashFileExtension(HashFunctionListComboBox.Text);
				File.WriteAllText(path, HashValueTextBox.Text);
			}
			catch
			{
				MessageBox.Show(this, "Can't save hash value.", "Attention");
			}
		}

		private void HashButton_Click(object sender, EventArgs e)
		{
			if (this.FileLocationTextBox.Text.Trim() == "")
			{
				MessageBox.Show(this, "Please select a file first.", "Attention");
				return;
			}

			if (Directory.Exists(this.FileLocationTextBox.Text))
			{
				MessageBox.Show(this, "Can't hash directory or folder.", "Attention");
				return;
			}

			if (!File.Exists(this.FileLocationTextBox.Text))
			{
				MessageBox.Show(this, "File not exist.", "Attention");
				return;
			}

			FileInfo fileInfo = new FileInfo(this.FileLocationTextBox.Text);
			
			if (fileInfo.Length > this._LimitSize)
			{
				MessageBox.Show(this, "File can't larger than " + Settings.Default.LimitSize + " MB.", "Attention");
				return;
			}

			this._FileSize = fileInfo.Length;
			this.HashButton.Enabled = false;
			this.HashProgressBar.Value = 0;
			this.PrepareHashFunction();
			this._BackgroundWorker.RunWorkerAsync();
		}

		private void CancelHashButton_Click(object sender, EventArgs e)
		{
			if (this._BackgroundWorker.WorkerSupportsCancellation)
			{
				this._BackgroundWorker.CancelAsync();
				this.HashButton.Enabled = true;
			}
		}

		#endregion Button Event

		#region TextBox Event
		private void FileLocationTextBox_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
			{
				e.Effect = DragDropEffects.All;
			}
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			this.FileLocationTextBox.Text = array[0];
			this.HashValueTextBox.Text = "";
		}

		private void FileLocationTextBox_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Link;
		}

		#endregion TextBox Event

		#region Backgroung Worker Event

		private void Hashing_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			byte[] array = new byte[this._Hash.GetDigestSize()];
			using (Stream stream = File.OpenRead(this.FileLocationTextBox.Text))
			{
				long total_read = 0L;
				int read_block;
				while ((read_block = stream.Read(this._BlockSize, 0, this._BlockSize.Length)) > 0)
				{
					if (backgroundWorker.CancellationPending)
					{
						e.Cancel = true;
						Array.Clear(this._BlockSize, 0, this._BlockSize.Length);
						break;
					}
					this._Hash.BlockUpdate(this._BlockSize, 0, read_block);
					total_read += read_block;
					float progress = total_read / this._FileSize * 100;
					backgroundWorker.ReportProgress((int)progress);
				}
			}

			this._Hash.DoFinal(array, 0);
			Array.Clear(this._BlockSize, 0, this._BlockSize.Length);

			e.Result = HashFunctionList.ByteArrayToHexString(array);
		}

		private void Hashing_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.HashProgressBar.Value = e.ProgressPercentage;
		}

		private void Hahsing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBox.Show(this, e.Error.Message);
			}
			else if (e.Cancelled)
			{
				MessageBox.Show("Canceled");
			}
			else
			{
				this.HashValueTextBox.Text = e.Result.ToString();
				MessageBox.Show("Done");
			}

			this.HashButton.Enabled = true;
		}

		#endregion Backgroung Worker Event

		#region Private Method

		private void LoadHashList()
		{
			string[] temp = Entities.HashFunctionList.GetHashList();

			for (int i = 0; i < temp.Length; i++)
			{
				this.HashFunctionListComboBox.Items.Add(temp[i]);
			}

			this.HashFunctionListComboBox.Text = temp[0];
		}

		private void PrepareHashFunction()
		{
			if (this._Hash != null)
			{
				this._Hash.Reset();
			}

			switch (this.HashFunctionListComboBox.Text)
			{
				case "Blake2b - 256 bit":
					this._Hash = new Blake2bDigest(256);
					break;
				case "Blake2b - 512 bit":
					this._Hash = new Blake2bDigest(512);
					break;
				case "SHA-1":
					this._Hash = new Sha1Digest();
					break;
				case "SHA-2 256 bit":
					this._Hash = new Sha256Digest();
					break;
				case "SHA-2 512 bit":
					this._Hash = new Sha512Digest();
					break;
				case "SHA-3 256 bit":
					this._Hash = new Sha3Digest(256);
					break;
				case "SHA-3 512 bit":
					this._Hash = new Sha3Digest(512);
					break;
				default:
					this._Hash = new Sha1Digest();
					break;
			}
			this._Hash.Reset();
		}
		
		private string GetFileDirectory(string path)
		{
			FileInfo fileInfo = new FileInfo(path);
			return fileInfo.DirectoryName;
		}

		#endregion Private Method
	}
}
