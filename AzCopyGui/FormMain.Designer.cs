namespace AzCopyGui
{
	partial class FormMain
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
			this.cbTasks = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnUseTask = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tbSourceLocation = new System.Windows.Forms.TextBox();
			this.tbSourceKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.tbDestinationKey = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbDestinationLocation = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.btnSwapSourceAndDestination = new System.Windows.Forms.Button();
			this.btnOverwriteDestination = new System.Windows.Forms.Button();
			this.tbLog = new System.Windows.Forms.TextBox();
			this.cbSourceDate = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.btnSourceBrowse = new System.Windows.Forms.Button();
			this.btnDestinationBrowse = new System.Windows.Forms.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.tbAzCopyCommand = new System.Windows.Forms.TextBox();
			this.cbDeleteAllItemsAtDestination = new System.Windows.Forms.CheckBox();
			this.btnDeleteAllItemsAtDestination = new System.Windows.Forms.Button();
			this.tbDeleteDestinationCommand = new System.Windows.Forms.TextBox();
			this.cbFixEmptyFolders = new System.Windows.Forms.CheckBox();
			this.lblPleaseWait = new System.Windows.Forms.Label();
			this.fbdSourceLocation = new System.Windows.Forms.FolderBrowserDialog();
			this.fbdDestinationLocation = new System.Windows.Forms.FolderBrowserDialog();
			this.SuspendLayout();
			// 
			// cbTasks
			// 
			this.cbTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.cbTasks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbTasks.FormattingEnabled = true;
			this.cbTasks.Location = new System.Drawing.Point(12, 44);
			this.cbTasks.Name = "cbTasks";
			this.cbTasks.Size = new System.Drawing.Size(760, 21);
			this.cbTasks.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(12, 12);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(760, 35);
			this.textBox1.TabIndex = 2;
			this.textBox1.Text = "1. These tasks were extracted from AzCopyBatch.ini file. Even ones that are comme" +
    "nted out are showed here. Use them to quickly find backup information that is be" +
    "ing used or was used before. ";
			// 
			// btnUseTask
			// 
			this.btnUseTask.Location = new System.Drawing.Point(12, 71);
			this.btnUseTask.Name = "btnUseTask";
			this.btnUseTask.Size = new System.Drawing.Size(269, 28);
			this.btnUseTask.TabIndex = 3;
			this.btnUseTask.Text = "2. Use information from backup task from above";
			this.btnUseTask.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label1.Location = new System.Drawing.Point(9, 111);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(101, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Source (backup)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 124);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(361, 13);
			this.label2.TabIndex = 5;
			this.label2.Text = "Location (Local folder path or Azure storage container, directory or blob url):";
			// 
			// tbSourceLocation
			// 
			this.tbSourceLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbSourceLocation.Location = new System.Drawing.Point(12, 140);
			this.tbSourceLocation.Name = "tbSourceLocation";
			this.tbSourceLocation.Size = new System.Drawing.Size(585, 20);
			this.tbSourceLocation.TabIndex = 6;
			// 
			// tbSourceKey
			// 
			this.tbSourceKey.Location = new System.Drawing.Point(12, 179);
			this.tbSourceKey.Name = "tbSourceKey";
			this.tbSourceKey.Size = new System.Drawing.Size(504, 20);
			this.tbSourceKey.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 163);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(172, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Key (if location is at Azure storage):";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(9, 293);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(252, 13);
			this.label4.TabIndex = 9;
			this.label4.Text = "4. Review source and destination information above";
			// 
			// tbDestinationKey
			// 
			this.tbDestinationKey.Location = new System.Drawing.Point(12, 270);
			this.tbDestinationKey.Name = "tbDestinationKey";
			this.tbDestinationKey.Size = new System.Drawing.Size(504, 20);
			this.tbDestinationKey.TabIndex = 14;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(9, 254);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(172, 13);
			this.label5.TabIndex = 13;
			this.label5.Text = "Key (if location is at Azure storage):";
			// 
			// tbDestinationLocation
			// 
			this.tbDestinationLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDestinationLocation.Location = new System.Drawing.Point(12, 231);
			this.tbDestinationLocation.Name = "tbDestinationLocation";
			this.tbDestinationLocation.Size = new System.Drawing.Size(585, 20);
			this.tbDestinationLocation.TabIndex = 12;
			this.tbDestinationLocation.Text = "http://craftsynthbackup.blob.core.windows.net/test/2014-06-16-02-46-27/";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(9, 215);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(357, 13);
			this.label6.TabIndex = 11;
			this.label6.Text = "Location (local folder path or Azure storage container, directory or blob url):";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label7.Location = new System.Drawing.Point(9, 202);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(71, 13);
			this.label7.TabIndex = 10;
			this.label7.Text = "Destination";
			// 
			// btnSwapSourceAndDestination
			// 
			this.btnSwapSourceAndDestination.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSwapSourceAndDestination.Location = new System.Drawing.Point(695, 218);
			this.btnSwapSourceAndDestination.Name = "btnSwapSourceAndDestination";
			this.btnSwapSourceAndDestination.Size = new System.Drawing.Size(77, 72);
			this.btnSwapSourceAndDestination.TabIndex = 15;
			this.btnSwapSourceAndDestination.Text = "Swap source and destination";
			this.btnSwapSourceAndDestination.UseVisualStyleBackColor = true;
			this.btnSwapSourceAndDestination.Click += new System.EventHandler(this.btnSwapSourceAndDestination_Click_1);
			// 
			// btnOverwriteDestination
			// 
			this.btnOverwriteDestination.Location = new System.Drawing.Point(414, 408);
			this.btnOverwriteDestination.Name = "btnOverwriteDestination";
			this.btnOverwriteDestination.Size = new System.Drawing.Size(358, 30);
			this.btnOverwriteDestination.TabIndex = 16;
			this.btnOverwriteDestination.Text = "9. Overwrite destination!";
			this.btnOverwriteDestination.UseVisualStyleBackColor = true;
			this.btnOverwriteDestination.Click += new System.EventHandler(this.btnOverwriteDestination_Click);
			// 
			// tbLog
			// 
			this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLog.Location = new System.Drawing.Point(12, 444);
			this.tbLog.Multiline = true;
			this.tbLog.Name = "tbLog";
			this.tbLog.ReadOnly = true;
			this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbLog.Size = new System.Drawing.Size(760, 105);
			this.tbLog.TabIndex = 17;
			// 
			// cbSourceDate
			// 
			this.cbSourceDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cbSourceDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbSourceDate.FormattingEnabled = true;
			this.cbSourceDate.Location = new System.Drawing.Point(603, 139);
			this.cbSourceDate.Name = "cbSourceDate";
			this.cbSourceDate.Size = new System.Drawing.Size(169, 21);
			this.cbSourceDate.TabIndex = 18;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(600, 123);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(112, 13);
			this.label8.TabIndex = 19;
			this.label8.Text = "3. Select backup date";
			// 
			// btnSourceBrowse
			// 
			this.btnSourceBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSourceBrowse.Location = new System.Drawing.Point(522, 160);
			this.btnSourceBrowse.Name = "btnSourceBrowse";
			this.btnSourceBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnSourceBrowse.TabIndex = 20;
			this.btnSourceBrowse.Text = "Browse";
			this.btnSourceBrowse.UseVisualStyleBackColor = true;
			this.btnSourceBrowse.Click += new System.EventHandler(this.btnSourceBrowse_Click);
			// 
			// btnDestinationBrowse
			// 
			this.btnDestinationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnDestinationBrowse.Location = new System.Drawing.Point(522, 251);
			this.btnDestinationBrowse.Name = "btnDestinationBrowse";
			this.btnDestinationBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnDestinationBrowse.TabIndex = 21;
			this.btnDestinationBrowse.Text = "Browse";
			this.btnDestinationBrowse.UseVisualStyleBackColor = true;
			this.btnDestinationBrowse.Click += new System.EventHandler(this.btnDestinationBrowse_Click);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(13, 366);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(168, 13);
			this.label9.TabIndex = 22;
			this.label9.Text = "7. Review final AzCopy command:";
			// 
			// tbAzCopyCommand
			// 
			this.tbAzCopyCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbAzCopyCommand.Location = new System.Drawing.Point(12, 382);
			this.tbAzCopyCommand.Name = "tbAzCopyCommand";
			this.tbAzCopyCommand.Size = new System.Drawing.Size(760, 20);
			this.tbAzCopyCommand.TabIndex = 23;
			// 
			// cbDeleteAllItemsAtDestination
			// 
			this.cbDeleteAllItemsAtDestination.AutoSize = true;
			this.cbDeleteAllItemsAtDestination.Checked = true;
			this.cbDeleteAllItemsAtDestination.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbDeleteAllItemsAtDestination.Location = new System.Drawing.Point(12, 319);
			this.cbDeleteAllItemsAtDestination.Name = "cbDeleteAllItemsAtDestination";
			this.cbDeleteAllItemsAtDestination.Size = new System.Drawing.Size(320, 17);
			this.cbDeleteAllItemsAtDestination.TabIndex = 24;
			this.cbDeleteAllItemsAtDestination.Text = "5. Delete all items at destination first. Review delete command:";
			this.cbDeleteAllItemsAtDestination.UseVisualStyleBackColor = true;
			// 
			// btnDeleteAllItemsAtDestination
			// 
			this.btnDeleteAllItemsAtDestination.Location = new System.Drawing.Point(12, 408);
			this.btnDeleteAllItemsAtDestination.Name = "btnDeleteAllItemsAtDestination";
			this.btnDeleteAllItemsAtDestination.Size = new System.Drawing.Size(358, 30);
			this.btnDeleteAllItemsAtDestination.TabIndex = 25;
			this.btnDeleteAllItemsAtDestination.Text = "8. Delete all items at destination!";
			this.btnDeleteAllItemsAtDestination.UseVisualStyleBackColor = true;
			this.btnDeleteAllItemsAtDestination.Click += new System.EventHandler(this.btnDeleteAllItemsAtDestination_Click);
			// 
			// tbDeleteDestinationCommand
			// 
			this.tbDeleteDestinationCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbDeleteDestinationCommand.Location = new System.Drawing.Point(338, 316);
			this.tbDeleteDestinationCommand.Name = "tbDeleteDestinationCommand";
			this.tbDeleteDestinationCommand.Size = new System.Drawing.Size(434, 20);
			this.tbDeleteDestinationCommand.TabIndex = 27;
			// 
			// cbFixEmptyFolders
			// 
			this.cbFixEmptyFolders.AutoSize = true;
			this.cbFixEmptyFolders.Checked = true;
			this.cbFixEmptyFolders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbFixEmptyFolders.Location = new System.Drawing.Point(12, 342);
			this.cbFixEmptyFolders.Name = "cbFixEmptyFolders";
			this.cbFixEmptyFolders.Size = new System.Drawing.Size(770, 17);
			this.cbFixEmptyFolders.TabIndex = 28;
			this.cbFixEmptyFolders.Text = "6. Fix empty folders. (Azure does not support empty folders. Workaround is to cre" +
    "ate dummy files in such folders before upload and delete them after download)";
			this.cbFixEmptyFolders.UseVisualStyleBackColor = true;
			// 
			// lblPleaseWait
			// 
			this.lblPleaseWait.AutoSize = true;
			this.lblPleaseWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.lblPleaseWait.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
			this.lblPleaseWait.Location = new System.Drawing.Point(288, 85);
			this.lblPleaseWait.Name = "lblPleaseWait";
			this.lblPleaseWait.Size = new System.Drawing.Size(84, 13);
			this.lblPleaseWait.TabIndex = 29;
			this.lblPleaseWait.Text = "Please wait...";
			this.lblPleaseWait.Visible = false;
			// 
			// fbdSourceLocation
			// 
			this.fbdSourceLocation.RootFolder = System.Environment.SpecialFolder.MyComputer;
			// 
			// fbdDestinationLocation
			// 
			this.fbdDestinationLocation.RootFolder = System.Environment.SpecialFolder.MyComputer;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.lblPleaseWait);
			this.Controls.Add(this.cbFixEmptyFolders);
			this.Controls.Add(this.tbDeleteDestinationCommand);
			this.Controls.Add(this.btnDeleteAllItemsAtDestination);
			this.Controls.Add(this.cbDeleteAllItemsAtDestination);
			this.Controls.Add(this.tbAzCopyCommand);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.btnDestinationBrowse);
			this.Controls.Add(this.btnSourceBrowse);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.cbSourceDate);
			this.Controls.Add(this.tbLog);
			this.Controls.Add(this.btnOverwriteDestination);
			this.Controls.Add(this.btnSwapSourceAndDestination);
			this.Controls.Add(this.tbDestinationKey);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.tbDestinationLocation);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbSourceKey);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.tbSourceLocation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnUseTask);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.cbTasks);
			this.Name = "FormMain";
			this.Text = "AzCopyGui (Restore)";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbTasks;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnUseTask;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbSourceLocation;
		private System.Windows.Forms.TextBox tbSourceKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbDestinationKey;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbDestinationLocation;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button btnSwapSourceAndDestination;
		private System.Windows.Forms.Button btnOverwriteDestination;
		private System.Windows.Forms.ComboBox cbSourceDate;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btnSourceBrowse;
		private System.Windows.Forms.Button btnDestinationBrowse;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbAzCopyCommand;
		private System.Windows.Forms.CheckBox cbDeleteAllItemsAtDestination;
		private System.Windows.Forms.Button btnDeleteAllItemsAtDestination;
		private System.Windows.Forms.TextBox tbDeleteDestinationCommand;
		private System.Windows.Forms.CheckBox cbFixEmptyFolders;
		private System.Windows.Forms.Label lblPleaseWait;
		private System.Windows.Forms.FolderBrowserDialog fbdSourceLocation;
		private System.Windows.Forms.FolderBrowserDialog fbdDestinationLocation;
		internal System.Windows.Forms.TextBox tbLog;
	}
}

