using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	[MarkerAdvice("fromMyGreatFooBarClass", true)]
	public class MyGreatFooBar : MyFooBar
	{
		[MarkerAdvice("fromMyGreatFooBarMethod", true)]
		public override DateTime GetCurrentDateTime(int addingDays)
		{
			return new DateTime(2010, 1, 1).AddDays(addingDays);
		}
	}
}
