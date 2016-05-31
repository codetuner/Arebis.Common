using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	[TestableAdvice]
	[MarkerAdvice("fromYourFooClass", true)]
	public class YourFoo : BaseFoo, IFoo
	{
		[MarkerAdvice("fromYourFooMethod", true)]
		DateTime IFoo.GetCurrentDateTime(int addingDays)
		{
			return new DateTime(2007, 1, 1).AddDays(addingDays);
		}
	}
}
