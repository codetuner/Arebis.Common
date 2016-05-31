using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	[TestableAdvice]
	[MarkerAdvice("fromMyFooClass", true)]
	public class MyFoo : BaseFoo, IFoo
	{
		private DateTime initialDateTime;
		private object tag;

		public MyFoo()
		{
			this.InitialDateTime = new DateTime(2008, 1, 1);
		}

		public DateTime InitialDateTime
		{
			get { return initialDateTime; }
			set { initialDateTime = value; }
		}

		[MarkerAdvice("fromMyFooMethod", true)]
		public virtual DateTime GetCurrentDateTime(int addingDays)
		{
			return this.InitialDateTime.AddDays(addingDays);
		}

		public object Tag
		{
			get { return tag; }
			set { tag = value; }
		}
	}
}
