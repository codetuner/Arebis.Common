using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ArebisTestApp
{
	public partial class MyTestComponent : Component
	{
		public MyTestComponent()
		{
			InitializeComponent();
		}

		public MyTestComponent(IContainer container)
		{
			container.Add(this);

			InitializeComponent();
		}

		[Editor(typeof(Arebis.Windows.Forms.Design.TypeSelectionEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public Type RootType { get; set; }
	}
}
