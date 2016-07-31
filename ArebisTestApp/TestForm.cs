using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Arebis.Windows.Forms.Design;
using Arebis.Data;
using System.Diagnostics;

namespace ArebisTestApp
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Console.WriteLine("It is actually {0}", DateTime.Now);
			onButton1MessageBox.Show(this);
		}

		private void button2_Click(object sender, EventArgs e)
		{
			Console.Error.WriteLine("This is written on the error log.");
		}

		private void button3_Click(object sender, EventArgs e)
		{
			SampleExtendable obj = new SampleExtendable();
			obj.SomeMethod1();
			obj.SomeMethod2();
		}

		private void panel1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void button4_Click(object sender, EventArgs e)
		{
			TypeSelectionDialog dlg = new TypeSelectionDialog();
			dlg.AddAssembly(System.Reflection.Assembly.GetEntryAssembly(), true);
			dlg.ShowDialog(this);
		}

        private void button5_Click(object sender, EventArgs e)
        {

        }
    }
}