using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	[MarkerAdvice("fromMyFooBarClass", true)]
	public class MyFooBar : MyFoo
	{
		public MyFooBar()
		{
			this.InitialDateTime = new DateTime(2009, 1, 1);
		}

	}
}
