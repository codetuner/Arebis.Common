using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public class TestableAdviceAttribute : AdviceAttribute
	{
		private static List<ICallContext> calls = new List<ICallContext>();
		private string name;

		public TestableAdviceAttribute()
			: base(true)
		{
			this.name = "<NoName>";
		}

		public TestableAdviceAttribute(String name)
			: base(true)
		{
			this.name = name;
		}

		public static void ResetCalls()
		{
			calls = new List<ICallContext>();
		}

		public override void BeforeCall(ICallContext callContext)
		{
			Console.WriteLine(">> TestableExtension.BeforeCall " + callContext.Method.Name);
			calls.Add(callContext);
			callContext.SetProperty("Name", this.name);
		}

		public override void AfterCall(ICallContext callContext)
		{
			Console.WriteLine(">> TestableExtension.AfterCall " + callContext.Method.Name);
		}

		public static List<ICallContext> Calls
		{
			get { return calls; }
		}
	}
}
