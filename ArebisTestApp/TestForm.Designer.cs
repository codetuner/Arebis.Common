namespace ArebisTestApp
{
	partial class TestForm
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
            this.redirOutText = new System.Windows.Forms.TextBox();
            this.reditErrText = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.outTabPage = new System.Windows.Forms.TabPage();
            this.errTabPage = new System.Windows.Forms.TabPage();
            this.outRedirector = new Arebis.Windows.Forms.TextBoxStreamer();
            this.errRedirector = new Arebis.Windows.Forms.TextBoxStreamer();
            this.onButton1MessageBox = new Arebis.Windows.Forms.MessageBox();
            this.myTestComponent1 = new ArebisTestApp.MyTestComponent(this.components);
            this.button5 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.outTabPage.SuspendLayout();
            this.errTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // redirOutText
            // 
            this.redirOutText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.redirOutText.Location = new System.Drawing.Point(0, 0);
            this.redirOutText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.redirOutText.Multiline = true;
            this.redirOutText.Name = "redirOutText";
            this.redirOutText.ReadOnly = true;
            this.redirOutText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.redirOutText.Size = new System.Drawing.Size(689, 151);
            this.redirOutText.TabIndex = 1;
            this.redirOutText.WordWrap = false;
            // 
            // reditErrText
            // 
            this.reditErrText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reditErrText.Location = new System.Drawing.Point(0, 0);
            this.reditErrText.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.reditErrText.Multiline = true;
            this.reditErrText.Name = "reditErrText";
            this.reditErrText.ReadOnly = true;
            this.reditErrText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.reditErrText.Size = new System.Drawing.Size(689, 151);
            this.reditErrText.TabIndex = 2;
            this.reditErrText.WordWrap = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(697, 383);
            this.splitContainer1.SplitterDistance = 198;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(697, 198);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(128, 52);
            this.button4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 28);
            this.button4.TabIndex = 12;
            this.button4.Text = "button4";
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(20, 52);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 28);
            this.button3.TabIndex = 11;
            this.button3.Text = "button3";
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(127, 16);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 28);
            this.button2.TabIndex = 10;
            this.button2.Text = "button2";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(20, 16);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 28);
            this.button1.TabIndex = 9;
            this.button1.Text = "button1";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.outTabPage);
            this.tabControl1.Controls.Add(this.errTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(697, 180);
            this.tabControl1.TabIndex = 4;
            // 
            // outTabPage
            // 
            this.outTabPage.Controls.Add(this.redirOutText);
            this.outTabPage.Location = new System.Drawing.Point(4, 25);
            this.outTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.outTabPage.Name = "outTabPage";
            this.outTabPage.Size = new System.Drawing.Size(689, 151);
            this.outTabPage.TabIndex = 0;
            this.outTabPage.Text = "Out";
            // 
            // errTabPage
            // 
            this.errTabPage.Controls.Add(this.reditErrText);
            this.errTabPage.Location = new System.Drawing.Point(4, 25);
            this.errTabPage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.errTabPage.Name = "errTabPage";
            this.errTabPage.Size = new System.Drawing.Size(689, 151);
            this.errTabPage.TabIndex = 1;
            this.errTabPage.Text = "Error";
            // 
            // outRedirector
            // 
            this.outRedirector.AutoMoveSelection = true;
            this.outRedirector.Binding = Arebis.Windows.Forms.TextBoxStreamerBinding.StandardOut;
            this.outRedirector.TextBox = this.redirOutText;
            // 
            // errRedirector
            // 
            this.errRedirector.AutoMoveSelection = true;
            this.errRedirector.Binding = Arebis.Windows.Forms.TextBoxStreamerBinding.StandardErr;
            this.errRedirector.TextBox = this.reditErrText;
            // 
            // onButton1MessageBox
            // 
            this.onButton1MessageBox.Icon = System.Windows.Forms.MessageBoxIcon.Asterisk;
            this.onButton1MessageBox.Text = "Current date/time is written on Out.";
            this.onButton1MessageBox.Title = "Button 1";
            // 
            // myTestComponent1
            // 
            this.myTestComponent1.RootType = typeof(System.Win32.User32);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(234, 16);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(100, 28);
            this.button5.TabIndex = 13;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 383);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.outTabPage.ResumeLayout(false);
            this.outTabPage.PerformLayout();
            this.errTabPage.ResumeLayout(false);
            this.errTabPage.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private Arebis.Windows.Forms.TextBoxStreamer outRedirector;
		private Arebis.Windows.Forms.TextBoxStreamer errRedirector;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage outTabPage;
		private System.Windows.Forms.TextBox redirOutText;
		private System.Windows.Forms.TabPage errTabPage;
		private System.Windows.Forms.TextBox reditErrText;
		private Arebis.Windows.Forms.MessageBox onButton1MessageBox;
		private MyTestComponent myTestComponent1;
		private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}