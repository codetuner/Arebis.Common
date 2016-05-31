using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;
using System.Windows.Forms;

namespace ArebisTestApp
{
	internal class ConsoleLoggerAttribute : AdviceAttribute
	{
		public ConsoleLoggerAttribute()
			: base(true)
		{ }

		public override void BeforeCall(ICallContext callContext)
		{
			Console.WriteLine("> Entering {0}", callContext.Method.Name);
		}

		public override void AfterCall(ICallContext callContext)
		{
			Console.WriteLine("> Leaving {0}", callContext.Method.Name);
			if (callContext.CallFailed) {
				Console.WriteLine("  Exception: {0}", callContext.Exception);
			}
		}
	}

	internal class SampleExtendable : Advisable {

		public void SomeMethod1() {
			Console.WriteLine("...Running method one (no extension)...");
		}

		[ConsoleLogger]
		public void SomeMethod2()
		{
			Console.WriteLine("...Running method two (with extension)...");
			if (MessageBox.Show("Raise exception ?", "SomeMethod2 - Extension sample", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes) {
				throw new ApplicationException("User requested exception.");
			}
		}
	}
}
