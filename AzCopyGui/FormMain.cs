using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AzCopyBatch;
using CraftSynth.BuildingBlocks.Common;
using CraftSynth.BuildingBlocks.IO.AzureStorage;
using CraftSynth.BuildingBlocks.Logging;

namespace AzCopyGui
{
	public partial class FormMain : Form
	{
		#region Private Members

		private CustomTraceLog _appLog;
		private string _originalValueInTextBox;
        #endregion

        #region Properties
        #endregion

        #region Public Methods
		internal delegate void AppendTextToTbLogDelegate(string line);
		internal void AppendTextToTbLog(string line)
		{

			if (this.tbLog.InvokeRequired)
			{
				this.tbLog.BeginInvoke(new AppendTextToTbLogDelegate(AppendTextToTbLog), line);
			}
			else
			{
				this.tbLog.AppendText("\r\n" + line);
			}
		}
        #endregion

        #region Constructors And Initialization	
		public FormMain(CustomTraceLog log)
		{
			this._appLog = log;
			InitializeComponent();

			EnableEvents();
			this.tbLog.Text = log.ToString();
			RefreshControls(true,true,false,true, true,true);
		}
        #endregion

        #region Deinitialization And Destructors
        #endregion        

        #region Event Handlers
		void btnUseTask_Click(object sender, EventArgs e)
		{
			RefreshControls(true,true,false,true, true, true);
		}
		void tbSourceKey_Leave(object sender, EventArgs e)
		{
			if (this.tbSourceKey.Text != this._originalValueInTextBox)
			{
				RefreshControls(false, false, false, true, false, true);
			}
		}

		void tbSourceLocation_Leave(object sender, EventArgs e)
		{
			if (this.tbSourceLocation.Text != this._originalValueInTextBox)
			{
				RefreshControls(false, false, false, true, false, true);
			}
		}

		private void btnSwapSourceAndDestination_Click_1(object sender, EventArgs e)
		{
			RefreshControls(true, false, true, true, true, true);
		}
		void tbDestinationKey_Leave(object sender, EventArgs e)
		{
			if (this.tbDestinationKey.Text != this._originalValueInTextBox)
			{
				RefreshControls(false, false, false, false, true, true);
			}
		}

		void tbDestinationLocation_Leave(object sender, EventArgs e)
		{
			if (this.tbDestinationLocation.Text != this._originalValueInTextBox)
			{
				RefreshControls(false, false, false, false, true, true);
			}
		}

		void cbFixEmptyFolders_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControls(false, false,false,false, false, true);
		}

		void cbDeleteAllItemsAtDestination_CheckedChanged(object sender, EventArgs e)
		{
			RefreshControls(false, false,false,false, false, false);
		}
		
		private void btnSourceBrowse_Click(object sender, EventArgs e)
		{
			if (this.fbdSourceLocation.ShowDialog() == DialogResult.OK)
			{
				DisableEvents();
				this.tbSourceLocation.Text = this.fbdSourceLocation.SelectedPath;
				this.tbSourceKey.Text = string.Empty;
				EnableEvents();
				RefreshControls(true,false,false,true,false,true);
			}
		}

		private void btnDestinationBrowse_Click(object sender, EventArgs e)
		{
			if (this.fbdDestinationLocation.ShowDialog() == DialogResult.OK)
			{
				DisableEvents();
				this.tbDestinationLocation.Text = this.fbdDestinationLocation.SelectedPath;
				this.tbDestinationKey.Text = string.Empty;
				EnableEvents();
				RefreshControls(true,false,false,false,true,true);
			}
		}	
		
		private void btnDeleteAllItemsAtDestination_Click(object sender, EventArgs e)
		{
			CommandDelete deleteCommand = null;
			try
			{
				deleteCommand = new CommandDelete(this.tbDestinationLocation.Text, string.IsNullOrWhiteSpace(this.tbDestinationKey.Text) ? null : this.tbDestinationKey.Text, true);
			}
			catch (Exception ex)
			{
				deleteCommand = new CommandDelete("{local-path-or-azure-storage-item-url}",null, true);
			}

			if (deleteCommand.DestinationLocation.Contains("[T]"))
			{
				MessageBox.Show("Destination location can not contain wildcard [T].", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.tbDestinationLocation.Focus();
			}
			else if ((!deleteCommand.DestinationLocation.ToLower().StartsWith("http://") &&
			          !deleteCommand.DestinationLocation.ToLower().StartsWith("https://")) &&
			         !Directory.Exists(deleteCommand.DestinationLocation))
			{
				MessageBox.Show("Destination location does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.tbDestinationLocation.Focus();
			}
			else if (deleteCommand.DestinationLocation == "{local-path-or-azure-storage-item-url}")
			{
				MessageBox.Show("Destination location not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.tbDestinationLocation.Focus();
			}
			else if ((deleteCommand.DestinationLocation.ToLower().StartsWith("http://")||
				deleteCommand.DestinationLocation.ToLower().StartsWith("https://"))&&
				(deleteCommand.DestinationKey==null ||deleteCommand.DestinationKey== "{azure-storage-key}"))
			{
				MessageBox.Show("Destination key not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.tbDestinationKey.Focus();
			}else if ((!deleteCommand.DestinationLocation.ToLower().StartsWith("http://")&&
				!deleteCommand.DestinationLocation.ToLower().StartsWith("https://"))&&
				(deleteCommand.DestinationKey!=null && deleteCommand.DestinationKey.Length>0))
			{
				MessageBox.Show("Destination key should not be specified for local folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				this.tbDestinationKey.Focus();
			}else
			{
				if (MessageBox.Show("This will empty '" + deleteCommand.DestinationLocation + "'. Proceed?", "Confirmation",MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
				{
					try
					{
						bool r = AzCopyBatch.HandlerForTask_Delete.Execute(0, this.tbDeleteDestinationCommand.Text, null, this._appLog);
						this._appLog.AddLine(r ? "Operation succeeded!" : "Operation failed!");
					}
					catch (Exception exx)
					{
						HandlerForLoging.LogException(exx, this._appLog);
						this._appLog.AddLine("Operation failed.");
					}
				}
			}
		}

		private void btnOverwriteDestination_Click(object sender, EventArgs e)
		{
			CommandAzCopy commandAzCopy = null;
			try
			{
				commandAzCopy = CommandAzCopy.Parse(this.tbAzCopyCommand.Text, false,this._appLog);
			}
			catch (Exception ex)
			{
				HandlerForLoging.LogException(ex, this._appLog);
				//commandAzCopy = CommandAzCopy.Parse("azcopy {local-path-or-azure-storage-item-url}", null, true);
			}

			if (commandAzCopy != null)
			{
				if (commandAzCopy.SourceLocation.Contains("[T]"))
				{
					MessageBox.Show("Source location can not contain wildcard [T]. Make sure that you selected date from dropdown box.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbSourceLocation.Focus();
				}
				else if ((!commandAzCopy.SourceLocation.ToLower().StartsWith("http://") &&
						  !commandAzCopy.SourceLocation.ToLower().StartsWith("https://")) &&
						 !Directory.Exists(commandAzCopy.SourceLocation))
				{
					MessageBox.Show("Source location does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbSourceLocation.Focus();
				}
				else if (commandAzCopy.SourceLocation == "{local-path-or-azure-storage-item-url}")
				{
					MessageBox.Show("Source location not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbSourceLocation.Focus();
				}
				else if ((commandAzCopy.SourceLocation.ToLower().StartsWith("http://") ||
						  commandAzCopy.SourceLocation.ToLower().StartsWith("https://")) &&
						 (commandAzCopy.SourceKey == null || commandAzCopy.SourceKey == "{azure-storage-key}"))
				{
					MessageBox.Show("Source key not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbSourceKey.Focus();
				}
				else if ((!commandAzCopy.SourceLocation.ToLower().StartsWith("http://") &&
						  !commandAzCopy.SourceLocation.ToLower().StartsWith("https://")) &&
						 !string.IsNullOrEmpty(commandAzCopy.SourceKey))
				{
					MessageBox.Show("Source key should not be specified for local folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbSourceKey.Focus();
				}else//-----------------
				if (commandAzCopy.DestinationLocation.Contains("[T]"))
				{
					MessageBox.Show("Destination location can not contain wildcard [T].", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
					this.tbDestinationLocation.Focus();
				}
				else if ((!commandAzCopy.DestinationLocation.ToLower().StartsWith("http://") &&
				          !commandAzCopy.DestinationLocation.ToLower().StartsWith("https://")) &&
				         !Directory.Exists(commandAzCopy.DestinationLocation))
				{
					MessageBox.Show("Destination location does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbDestinationLocation.Focus();
				}
				else if (commandAzCopy.DestinationLocation == "{local-path-or-azure-storage-item-url}")
				{
					MessageBox.Show("Destination location not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbDestinationLocation.Focus();
				}
				else if ((commandAzCopy.DestinationLocation.ToLower().StartsWith("http://") ||
				          commandAzCopy.DestinationLocation.ToLower().StartsWith("https://")) &&
				         (commandAzCopy.DestinationKey == null || commandAzCopy.DestinationKey == "{azure-storage-key}"))
				{
					MessageBox.Show("Destination key not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					this.tbDestinationKey.Focus();
				}
				else if ((!commandAzCopy.DestinationLocation.ToLower().StartsWith("http://") &&
				          !commandAzCopy.DestinationLocation.ToLower().StartsWith("https://")) &&
				         !string.IsNullOrEmpty(commandAzCopy.DestinationKey))
				{
					MessageBox.Show("Destination key should not be specified for local folder.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Error);
					this.tbDestinationKey.Focus();
				}
				else
				{
					if (MessageBox.Show("This will overwrite '" + commandAzCopy.DestinationLocation + "'. Proceed?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK) { 
						try
						{
							bool r = AzCopyBatch.HandlerForTask_AzCopy.Execute(this._appLog, 0, this.tbAzCopyCommand.Text, DateTime.Now.ToDateAndTimeInSortableFormatForAzureBlob()+"_Manual_", 30);
							this._appLog.AddLine(r ? "Operation succeeded!" : "Operation failed!");
						}
						catch (Exception exx)
						{
							HandlerForLoging.LogException(exx, this._appLog);
							this._appLog.AddLine("Operation failed.");
						}
					}
				}
			}
		}
        #endregion

        #region Private Methods

		private void PopulateTasks()
		{
			var azCopyCommands = ExtractAzCopyCommandsFromIni();
			this.cbTasks.Items.Clear();
			foreach (CommandAzCopy azCopyCommand in azCopyCommands)
			{
				this.cbTasks.Items.Add(azCopyCommand);
			}
		}

		private void EnableEvents()
		{
			this.btnUseTask.Click += btnUseTask_Click;
			this.tbSourceLocation.Leave += tbSourceLocation_Leave;
			this.tbSourceKey.Leave += tbSourceKey_Leave;
			this.cbSourceDate.SelectedIndexChanged += cbSourceDate_SelectedIndexChanged;
			this.tbDestinationLocation.Leave += tbDestinationLocation_Leave;
			this.tbDestinationKey.Leave += tbDestinationKey_Leave;
			this.cbDeleteAllItemsAtDestination.CheckedChanged += cbDeleteAllItemsAtDestination_CheckedChanged;
			this.cbFixEmptyFolders.CheckedChanged += cbFixEmptyFolders_CheckedChanged;

			this.tbSourceLocation.Enter += tb_Enter;
			this.tbSourceKey.Enter += tb_Enter;
			this.tbDestinationLocation.Enter += tb_Enter;
			this.tbDestinationKey.Enter += tb_Enter;
		}

		void tb_Enter(object sender, EventArgs e)
		{
			this._originalValueInTextBox = (sender as TextBox).Text;
		}

		void cbSourceDate_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshControls(true,false,false,false,false,true);
		}


		private void DisableEvents()
		{
			this.btnUseTask.Click -= btnUseTask_Click;
			this.tbSourceLocation.Leave -= tbSourceLocation_Leave;
			this.tbSourceKey.Leave -= tbSourceKey_Leave;
			this.cbSourceDate.SelectedIndexChanged -= cbSourceDate_SelectedIndexChanged;
			this.tbDestinationLocation.Leave -= tbDestinationLocation_Leave;
			this.tbDestinationKey.Leave -= tbDestinationKey_Leave;
			this.cbDeleteAllItemsAtDestination.CheckedChanged -= cbDeleteAllItemsAtDestination_CheckedChanged;
			this.cbFixEmptyFolders.CheckedChanged -= cbFixEmptyFolders_CheckedChanged;

			this.tbSourceLocation.Enter -= tb_Enter;
			this.tbSourceKey.Enter -= tb_Enter;
			this.tbDestinationLocation.Enter -= tb_Enter;
			this.tbDestinationKey.Enter -= tb_Enter;
		}

		private void DisableControls()
		{
			SetEnabledStateOfControls(false);
		}

		private void EnableControls()
		{
			SetEnabledStateOfControls(true);
		}
		private void SetEnabledStateOfControls(bool newEnabledState)
		{
			this.cbTasks.Enabled = newEnabledState;
			this.btnUseTask.Enabled = newEnabledState;
			this.tbSourceLocation.Enabled = newEnabledState;
			this.cbSourceDate.Enabled = newEnabledState;
			this.btnSourceBrowse.Enabled = newEnabledState;
			this.tbSourceKey.Enabled = newEnabledState;
			this.tbDestinationLocation.Enabled = newEnabledState;
			this.btnDestinationBrowse.Enabled = newEnabledState;
			this.tbDestinationKey.Enabled = newEnabledState;
			this.btnSwapSourceAndDestination.Enabled = newEnabledState;
			this.cbDeleteAllItemsAtDestination.Enabled = newEnabledState;
			this.tbDeleteDestinationCommand.Enabled = newEnabledState;
			this.cbFixEmptyFolders.Enabled = newEnabledState;
			this.tbAzCopyCommand.Enabled = newEnabledState;
			this.btnDeleteAllItemsAtDestination.Enabled = newEnabledState;
			this.btnOverwriteDestination.Enabled = newEnabledState;
		}

		private string lastQuestion = string.Empty;
		private void RefreshControls(bool temporarlyDisableControls, bool useInfoFromTask, bool swapSourceAndDestination, bool repopulateBackupDates, bool regenerateDeleteCommand, bool regenerateAzCopyCommand)
		{
			DisableEvents();
			if (temporarlyDisableControls)
			{
				DisableControls();
			}
			this.lblPleaseWait.Visible = true;
			Application.DoEvents();

			string currentlySelectedTask = this.cbTasks.SelectedIndex<0?string.Empty:(this.cbTasks.SelectedItem as CommandAzCopy).ToString();
			PopulateTasks();
			if (!string.IsNullOrEmpty(currentlySelectedTask))
			{
				try
				{
					int i = 0;
					foreach (var item in this.cbTasks.Items)
					{
						if ((item as CommandAzCopy).ToString() == currentlySelectedTask)
						{
							this.cbTasks.SelectedIndex = i;
							break;
						}
						i++;
					}
				}
				catch (Exception)
				{
				}
				Application.DoEvents();
			}
			

			CommandAzCopy task = null;
			
			if (useInfoFromTask && this.cbTasks.SelectedIndex >= 0)
			{
				task = this.cbTasks.SelectedItem as CommandAzCopy;

				this.tbSourceLocation.Text = task.DestinationLocation;
				this.tbSourceKey.Text = task.DestinationKey;

				this.tbDestinationLocation.Text = task.SourceLocation;
				this.tbDestinationKey.Text = task.SourceKey;

				Application.DoEvents();
			}

			if (swapSourceAndDestination)
			{
				string t = this.tbSourceLocation.Text;
				this.tbSourceLocation.Text = this.tbDestinationLocation.Text;
				this.tbDestinationLocation.Text = t;

				t = this.tbSourceKey.Text;
				this.tbSourceKey.Text = this.tbDestinationKey.Text;
				this.tbDestinationKey.Text = t;

				Application.DoEvents();
			}

			if (repopulateBackupDates)
			{
				DateTime? currentSourceDate = this.cbSourceDate.SelectedIndex < 0 ? (DateTime?)null : ((Backup)this.cbSourceDate.SelectedItem).Date;
				this.cbSourceDate.Items.Clear();
				List<KeyValuePair<object, DateTime> > pathsAndDates = new List<KeyValuePair<object, DateTime>>();
				if (!string.IsNullOrWhiteSpace(this.tbSourceLocation.Text) && this.tbSourceLocation.Text.Contains("[T]"))
				{
					try
					{
						pathsAndDates = AzCopyBatch.HandlerForPaths.GetDestinationsFromDestinationWithWildcard(this.tbSourceLocation.Text, string.IsNullOrWhiteSpace(this.tbSourceKey.Text) ? null : this.tbSourceKey.Text);

					}
					catch (Exception e)
					{
						HandlerForLoging.LogException(e, this._appLog);
					}
				}
				this.cbSourceDate.DisplayMember = "DateAsString";
				foreach (KeyValuePair<object, DateTime> pathAndDate in pathsAndDates)
				{
					this.cbSourceDate.Items.Add(new Backup(){Date = pathAndDate.Value, PathOrUrl = pathAndDate.Key, Key = this.tbSourceKey.Text});
				}
				if (currentSourceDate!=null)
				{
					try
					{
						int i = 0;
						foreach (Backup item in this.cbSourceDate.Items)
						{
							if (item.Date == currentSourceDate)
							{
								this.cbSourceDate.SelectedIndex = i;
								break;
							}
							i++;
						}
					}
					catch (Exception)
					{
					}
				}

				Application.DoEvents();
			}

			this.tbDeleteDestinationCommand.Enabled = this.cbDeleteAllItemsAtDestination.Checked;
			//if (this.cbDeleteAllItemsAtDestination.Checked)
			//{
			//	CommandDelete deleteCommand = null;
			//	try
			//	{
			//		deleteCommand = new CommandDelete(this.tbDestinationLocation.Text, string.IsNullOrWhiteSpace(this.tbDestinationKey.Text) ? null : this.tbDestinationKey.Text, true);
			//	}
			//	catch (Exception e)
			//	{
			//		deleteCommand = null;
			//	}

			//	if (deleteCommand == null)
			//	{
			//		this.btnDeleteAllItemsAtDestination.Enabled = false;
			//	}
			//	else
			//	{
			//		this.tbDeleteDestinationCommand.Text = deleteCommand.ToString();
			//	}

			//	Application.DoEvents();
			//}
			if (regenerateDeleteCommand)
			{
				CommandDelete deleteCommand = null;
				try
				{
					deleteCommand = new CommandDelete(this.tbDestinationLocation.Text,
						string.IsNullOrWhiteSpace(this.tbDestinationKey.Text) ? null : this.tbDestinationKey.Text, true);
				}
				catch (Exception e)
				{
					deleteCommand = null;
				}

				string destLocation = this.tbDestinationLocation.Text.ToNonNullNonEmptyString("{local-path-or-azure-storage-item-url}").Trim();
				if (deleteCommand == null)
				{
					this.tbDeleteDestinationCommand.Text = "delete " + destLocation + " /DeleteOnlyContent";
				}
				else
				{
					this.tbDeleteDestinationCommand.Text = deleteCommand.ToString();
				}

				bool destKeyPresent = this.tbDeleteDestinationCommand.Text.GetParameterPresence("/destKey", true, false, '/', ':');
				if (!destKeyPresent && (destLocation.ToLower().StartsWith("http://") || destLocation.ToLower().StartsWith("https://")))
				{
					string destKey = this.tbDestinationKey.Text.ToNonNullNonEmptyString("{azure-storage-key}");
					this.tbDeleteDestinationCommand.Text = this.tbDeleteDestinationCommand.Text.Trim() + " /destKey:" + destKey;
				}
			}

			if (regenerateAzCopyCommand)
			{
				//if (task != null)
				//{
				//	this.tbAzCopyCommand.Text = task.ToString();
				//}
				//else
				//{
					string sourceLocation = this.tbSourceLocation.Text.ToNonNullNonEmptyString("{local-path-or-azure-storage-item-url}").Trim();
					string destLocation = this.tbDestinationLocation.Text.ToNonNullNonEmptyString("{local-path-or-azure-storage-item-url}").Trim();
					string sourceKey = this.tbSourceKey.Text.ToNonNullNonEmptyString("{azure-storage-key}");
					string destKey = this.tbDestinationKey.Text.ToNonNullNonEmptyString("{azure-storage-key}");
					this.tbAzCopyCommand.Text = string.Format("AzCopy {0} {1}", sourceLocation, destLocation);
					if (sourceLocation.ToLower().StartsWith("http://") || sourceLocation.ToLower().StartsWith("https://"))
					{
						this.tbAzCopyCommand.Text = this.tbAzCopyCommand.Text.Trim() + " /sourceKey:"+sourceKey;
					}
					if (destLocation.ToLower().StartsWith("http://") || destLocation.ToLower().StartsWith("https://"))
					{
						this.tbAzCopyCommand.Text = this.tbAzCopyCommand.Text.Trim() + " /destKey:" + destKey;
					}
					else
					{
						if (destLocation != "{local-path-or-azure-storage-item-url}" && !Directory.Exists(destLocation))
						{
							string newQuestion = "Folder '" + destLocation + "' does not exist. Create?";
							if (newQuestion != lastQuestion)
							{
								lastQuestion = newQuestion;
								if (MessageBox.Show(lastQuestion, "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
								    DialogResult.Yes)
								{
									try
									{
										DirectoryInfo di = Directory.CreateDirectory(destLocation);
										this._appLog.AddLine(string.Format("Folder '{0}' created.", di.FullName));
									}
									catch (Exception de)
									{
										HandlerForLoging.LogException(de, this._appLog);
										this._appLog.AddLine("Failed to create folder.");
									}
								}
							}
						}
					}
				//}

				if (this.tbAzCopyCommand.Text.Contains("[T]") && !string.IsNullOrWhiteSpace(this.cbSourceDate.Text))
				{
					this.tbAzCopyCommand.Text = tbAzCopyCommand.Text.Replace("[T]", this.cbSourceDate.Text);
				}

				if (!string.IsNullOrWhiteSpace(this.tbAzCopyCommand.Text) && 
					!this.tbAzCopyCommand.Text.ToUpper().Contains("/S ") && !this.tbAzCopyCommand.Text.ToUpper().Trim().EndsWith("/S"))
				{
					this.tbAzCopyCommand.Text = this.tbAzCopyCommand.Text.Trim() + " /S";
				}

				bool fixEmptyFoldersParam = !this.tbAzCopyCommand.Text.GetParameterPresence("/skipFixingEmptyFolders", true, false, '/', null);
				if (!fixEmptyFoldersParam && this.cbFixEmptyFolders.Checked)
				{
					this.tbAzCopyCommand.Text = this.tbAzCopyCommand.Text.RemoveParameter("/skipFixingEmptyFolders", true, this.tbAzCopyCommand.Text, '/','"',null);
				}
				else if (fixEmptyFoldersParam && !this.cbFixEmptyFolders.Checked)
				{
					this.tbAzCopyCommand.Text = this.tbAzCopyCommand.Text.Trim() + " /skipFixingEmptyFolders";
				}
			}

			this.btnDeleteAllItemsAtDestination.Enabled = this.cbDeleteAllItemsAtDestination.Checked && !string.IsNullOrEmpty(this.tbDeleteDestinationCommand.Text);
			this.btnOverwriteDestination.Enabled = !string.IsNullOrWhiteSpace(this.tbAzCopyCommand.Text);


			this._appLog.AddLine("Successfully refreshed controls.");
			this.lblPleaseWait.Visible = false;
			EnableEvents();
			if (temporarlyDisableControls)
			{
				EnableControls();
			}
			
			RefreshLog();
			Application.DoEvents();

			if (this._scheduledButton != null)
			{
				this._scheduledButton = null;
				this._scheduledButton.PerformClick();
			}
		}

		private Button _scheduledButton;

		private List<CommandAzCopy> ExtractAzCopyCommandsFromIni()
		{
			List<CommandAzCopy> r = null;
			try
			{
				string iniFilePath = CraftSynth.BuildingBlocks.Common.Misc.ApplicationRootFolderPath + "AzCopyBatch.ini";
				var lines = File.ReadLines(iniFilePath);
				foreach (string line in lines)
				{
					if (line.Trim().StartsWith("[Tasks]"))
					{
						r = new List<CommandAzCopy>();
					}
					else if (r != null && line.Trim().Length > 0)// && !line.StartsWith("--"))
					{
						var newCommand = CommandAzCopy.Parse(line, true, new CustomTraceLog());
						if (newCommand != null)
						{
							r.Add(newCommand);
						}
					}
				}
			
			}
			catch (Exception exception)
			{
				AzCopyGui.HandlerForLoging.LogException(exception, this._appLog);
			}

			return r;
		}


		private void RefreshLog()
		{
				//if (this.tbLog.Text.Length > 2)
			//{
			//	this.tbLog.SelectionStart =this.tbLog.Text.Length;
			//	this.tbLog.ScrollToCaret();
			//}
			this.tbLog.AppendText(" ");
			
		}
		#endregion

	
		
        #region Helpers
        #endregion
	}

	internal class Backup
	{
		public string DateAsString
		{
			get
			{
				return this.Date.ToDateAndTimeInSortableFormatForAzureBlob();
			}	
		}
		public DateTime Date;
		public object PathOrUrl;

		public string PathOrUrlAsString
		{
			get
			{
				if (this.PathOrUrl is string)
				{
					return this.PathOrUrl.ToString();
				}
				else if (this.PathOrUrl is BlobUrl)
				{
					return (this.PathOrUrl as BlobUrl).Url;
				}
				throw new Exception("PathOrUrlAsString not recognized.");
			}
		}
		public string Key;

	}
}
