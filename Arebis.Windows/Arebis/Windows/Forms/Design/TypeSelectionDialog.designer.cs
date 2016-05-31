namespace Arebis.Windows.Forms.Design
{
	partial class TypeSelectionDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TypeSelectionDialog));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.assembliesTreeView = new System.Windows.Forms.TreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.typesListView = new System.Windows.Forms.ListView();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.typeNameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(12, 38);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.assembliesTreeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.typesListView);
			this.splitContainer1.Size = new System.Drawing.Size(567, 307);
			this.splitContainer1.SplitterDistance = 240;
			this.splitContainer1.TabIndex = 2;
			this.splitContainer1.TabStop = false;
			// 
			// assembliesTreeView
			// 
			this.assembliesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.assembliesTreeView.HideSelection = false;
			this.assembliesTreeView.ImageIndex = 0;
			this.assembliesTreeView.ImageList = this.imageList1;
			this.assembliesTreeView.Location = new System.Drawing.Point(0, 0);
			this.assembliesTreeView.Name = "assembliesTreeView";
			this.assembliesTreeView.SelectedImageIndex = 0;
			this.assembliesTreeView.Size = new System.Drawing.Size(240, 307);
			this.assembliesTreeView.TabIndex = 0;
			this.assembliesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.assembliesTreeView_AfterSelect);
			// 
			// imageList1
			// 
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "cedpedtui_240_16.ico");
			this.imageList1.Images.SetKeyName(1, "cedpedtui_240_16.ico");
			this.imageList1.Images.SetKeyName(2, "vcpkgui_6087.ico");
			this.imageList1.Images.SetKeyName(3, "vcpkgui_6087.ico");
			this.imageList1.Images.SetKeyName(4, "vcpkgui_6084.ico");
			// 
			// typesListView
			// 
			this.typesListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.typesListView.FullRowSelect = true;
			this.typesListView.Location = new System.Drawing.Point(0, 0);
			this.typesListView.Name = "typesListView";
			this.typesListView.Size = new System.Drawing.Size(323, 307);
			this.typesListView.SmallImageList = this.imageList1;
			this.typesListView.TabIndex = 0;
			this.typesListView.UseCompatibleStateImageBehavior = false;
			this.typesListView.View = System.Windows.Forms.View.List;
			this.typesListView.SelectedIndexChanged += new System.EventHandler(this.typesListView_SelectedIndexChanged);
			this.typesListView.DoubleClick += new System.EventHandler(this.typesListView_DoubleClick);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(423, 351);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(504, 351);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// typeNameTextBox
			// 
			this.typeNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.typeNameTextBox.Location = new System.Drawing.Point(84, 12);
			this.typeNameTextBox.Name = "typeNameTextBox";
			this.typeNameTextBox.Size = new System.Drawing.Size(495, 20);
			this.typeNameTextBox.TabIndex = 1;
			this.typeNameTextBox.Enter += new System.EventHandler(this.typeNameTextBox_Enter);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Type name :";
			// 
			// TypeSelectionDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(591, 386);
			this.ControlBox = false;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.typeNameTextBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "TypeSelectionDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Type";
			this.Load += new System.EventHandler(this.TypeSelectionDialog_Load);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TreeView assembliesTreeView;
		private System.Windows.Forms.ListView typesListView;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox typeNameTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ImageList imageList1;

	}
}