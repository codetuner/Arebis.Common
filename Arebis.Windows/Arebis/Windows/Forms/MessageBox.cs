using System;
using System.Windows.Forms;

namespace Arebis.Windows.Forms
{
	/// <summary>
	/// A component that represents a MessageBox and allows defining
	/// messageboxes at designtime.
	/// </summary>
	[System.ComponentModel.DesignTimeVisible(true)]
	[System.ComponentModel.Browsable(true)]
	[System.Drawing.ToolboxBitmap(typeof(MessageBox), "MessageBox")]
	public class MessageBox : System.ComponentModel.Component
	{
		private string title;
		private string text = "";
		private MessageBoxButtons buttons;
		private MessageBoxDefaultButton defaultButton;
		private MessageBoxIcon icon;

		/// <summary>
		/// The text to display in the title bar of the message box.
		/// </summary>
		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("MessageBox")]
		[System.ComponentModel.Description("The text to display in the title bar of the message box.")]
		[System.ComponentModel.Localizable(true)]
		public virtual string Title
		{
			get { return title; }
			set { title = value; }
		}

		/// <summary>
		/// The text to display in the message box.
		/// </summary>
		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("MessageBox")]
		[System.ComponentModel.Description("The text to display in the message box.")]
		[System.ComponentModel.Localizable(true)]
		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}

		/// <summary>
		/// One of the System.Windows.Forms.MessageBoxButtons that specifies which buttons to display in the message box.
		/// </summary>
		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("MessageBox")]
		[System.ComponentModel.Description("Specifies which buttons to display in the message box.")]
		[System.ComponentModel.DefaultValue(MessageBoxButtons.OK)]
		public virtual MessageBoxButtons Buttons
		{
			get { return buttons; }
			set { buttons = value; }
		}

		/// <summary>
		/// One fo the System.Windows.Forms.MessageBoxDefaultButton values which specifies which is the default button for the message box.
		/// </summary>
		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("MessageBox")]
		[System.ComponentModel.Description("Specifies which is the default button for the message box.")]
		[System.ComponentModel.DefaultValue(MessageBoxDefaultButton.Button1)]
		public virtual MessageBoxDefaultButton DefaultButton
		{
			get { return defaultButton; }
			set { defaultButton = value; }
		}

		/// <summary>
		/// One of the System.Windows.Forms.MessageBoxIcon values that specifies which icon to display in the message box.
		/// </summary>
		[System.ComponentModel.Browsable(true)]
		[System.ComponentModel.Category("MessageBox")]
		[System.ComponentModel.Description("Specifies which icon to display in the message box.")]
		[System.ComponentModel.DefaultValue(MessageBoxIcon.None)]
		public virtual MessageBoxIcon Icon
		{
			get { return icon; }
			set { icon = value; }
		}

		/// <summary>
		/// Displays the message box.
		/// </summary>
		/// <param name="args">An System.Object array containing zero or more objects to be formatted.</param>
		/// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
		public virtual DialogResult Show(params object[] args)
		{
			string text = this.text.Replace(@"\n", System.Environment.NewLine);
			return System.Windows.Forms.MessageBox.Show(String.Format(text, args), title, buttons, icon, defaultButton);
		}

		/// <summary>
		/// Displays the message box.
		/// </summary>
		/// <param name="owner">The System.Windows.Forms.IWin32Window the message box will display in front of.</param>
		/// <param name="args">An System.Object array containing zero or more objects to be formatted.</param>
		/// <returns>One of the System.Windows.Forms.DialogResult values.</returns>
		public virtual DialogResult Show(System.Windows.Forms.IWin32Window owner, params object[] args)
		{
			string text = this.text.Replace(@"\n", System.Environment.NewLine);
			return System.Windows.Forms.MessageBox.Show(owner, String.Format(text, args), title, buttons, icon, defaultButton);
		}

	}
}
