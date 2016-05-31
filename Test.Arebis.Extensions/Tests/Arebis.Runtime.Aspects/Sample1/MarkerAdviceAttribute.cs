using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
	public class MarkerAdviceAttribute : AdviceAttribute
	{
		private string property;
		private bool value;

		public MarkerAdviceAttribute(string property, bool value)
			: base(false)
		{
			this.property = property;
			this.value = value;
		}

		public override void BeforeCall(ICallContext callContext)
		{
			Console.WriteLine("  Setting property: " + property);
			((BaseFoo)callContext.Instance).Markers.Add(property);
		}

		public override void AfterCall(ICallContext callContext)
		{
		}
	}
}
