using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample1
{
	//[MarkerExtension("fromIFooInterface", true)] -> not allowed (does not compile)
	public interface IFoo
	{
		[MarkerAdvice("fromIFooMethod", true)]
		DateTime GetCurrentDateTime(int addingDays);
	}
}
