using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System.Reflection;

namespace Arebis.Windows.Forms.Design
{
	/// <summary>
	/// A UITypeEditor for properties of type System.Type.
	/// </summary>
	public class TypeSelectionEditor : UITypeEditor
	{
		/// <summary>
		/// Gets the editor style used by the EditValue() method.
		/// </summary>
		public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// Edits the value of the specified object using the editor style indicated
		/// by the GetEditStyle() method.
		/// </summary>
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			IWindowsFormsEditorService service = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

			if (service != null)
			{
				// Load dialog with all assemblies of current AppDomain:
				TypeSelectionDialog dialog = new TypeSelectionDialog();
				foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
					dialog.AddAssembly(asm, false);

				// Set default value:
				dialog.ChoosenType = (Type)value;

				// Show dialog:
				if (service.ShowDialog(dialog) == DialogResult.OK)
				{
					value = dialog.ChoosenType;
				}
			}

			return value;
		}
	}
}
