using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Arebis.Windows.Forms.Design
{
	/// <summary>
	/// A dialog that allows selecting an (exported) Type from a list of assemblies.
	/// </summary>
	/// <example>
	/// <code>
	/// TypeSelectionDialog dlg = new TypeSelectionDialog();
	/// dlg.AddAssembly(typeof(System.Object).Assembly, false);
	/// dlg.ChoosenType = typeof(System.Object);
	/// if (dlg.ShowDialog() == DialogResult.OK)
	///    return dlg.ChoosenType;
	/// </code>
	/// </example>
	public partial class TypeSelectionDialog : Form
	{
		private SortedList<string, Assembly> assemblies = new SortedList<string,Assembly>();

		/// <summary>
		/// Constructs a new TypeSelectionDialog.
		/// </summary>
		public TypeSelectionDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Adds an assembly to the list of assemblies shown in the dialog.
		/// If recursive, adds also all referenced assemblies, their referenced assemblies, etc...
		/// </summary>
		public void AddAssembly(Assembly assembly, bool recursive)
		{
			if (!this.assemblies.ContainsValue(assembly))
			{
				this.assemblies.Add(assembly.FullName, assembly);

				if (recursive)
				{
					foreach (AssemblyName referencedAssemblyName in assembly.GetReferencedAssemblies())
					{
						this.AddAssembly(Assembly.Load(referencedAssemblyName), recursive);
					}
				}
			}
		}

		/// <summary>
		/// The type choosen in the dialog.
		/// </summary>
		public Type ChoosenType
		{
			get
			{
				if (typeNameTextBox.Text.Length == 0)
					return null;
				else
				{
					Type type = Type.GetType(typeNameTextBox.Text);
					if (type == null)
						throw new TypeLoadException("Could not load the requested type.");
					return type;
				}
			}
			set
			{
				if (value == null)
					typeNameTextBox.Text = String.Empty;
				else
					typeNameTextBox.Text = value.AssemblyQualifiedName;
			}
		}

		private void TypeSelectionDialog_Load(object sender, EventArgs e)
		{
			foreach (Assembly asm in this.assemblies.Values)
			{
				try
				{
					// Show node for assembly:
					TreeNode nd = this.assembliesTreeView.Nodes.Add(asm.FullName, asm.GetName().Name, 0, 1);

					// Collect namespace node information:
					SortedList<string, SortedList<string, Type>> namespaceinfos = new SortedList<string, SortedList<string, Type>>();
					foreach (Type t in asm.GetExportedTypes())
					{
						string ns = t.Namespace ?? "(default)";
						SortedList<string, Type> typeList;
						if (namespaceinfos.ContainsKey(ns))
							typeList = namespaceinfos[ns];
						else
							typeList = namespaceinfos[ns] = new SortedList<string, Type>();
						typeList.Add(t.ToString(), t);
					}

					// Display nodes for namespaces:
					foreach (string ns in namespaceinfos.Keys)
					{
						TreeNode nsnd = nd.Nodes.Add(ns, ns, 2, 3);
						nsnd.Tag = namespaceinfos[ns];
					}
				}
				catch (Exception ex)
				{
					Debug.WriteLine(String.Format("{0}: Loading members of assembly \"{1}\" failed: {2}",this.GetType(), asm.GetName().Name, ex.Message));
				}
			}
		}

		private void typeNameTextBox_Enter(object sender, EventArgs e)
		{
			typeNameTextBox.SelectAll();
		}

		private void assembliesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node.Tag is SortedList<string, Type>)
			{
				typesListView.Items.Clear();
				foreach (Type t in ((SortedList<string, Type>)e.Node.Tag).Values)
				{
					ListViewItem lvi = typesListView.Items.Add(t.Name, 4);
					lvi.Tag = t;
				}
			}
		}

		private void typesListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!(typesListView.SelectedItems.Count == 0))
			{
				if (typesListView.SelectedItems[0].Tag is Type)
				{
					Type t = (Type)typesListView.SelectedItems[0].Tag;
					this.ChoosenType = t;
				}
			}
		}

		private void typesListView_DoubleClick(object sender, EventArgs e)
		{
			okButton_Click(this, EventArgs.Empty);
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			try { 
				Type result = this.ChoosenType;
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			catch 
			{ 
				global::System.Windows.Forms.MessageBox.Show(this, "Invalid type name.", "Select Type", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}