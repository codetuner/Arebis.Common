using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Extensions.Tests.Arebis.Runtime.Extensibility.Sample2
{
	interface SampleInterface
	{
		[TestableAdvice("IFace.A1")]
		void InterfaceMethodA();

		[TestableAdvice("IFace.A2")]
		void InterfaceMethodA(int i);

		[TestableAdvice("IFace.B1")]
		void InterfaceMethodB();

		[TestableAdvice("IFace.B2")]
		void InterfaceMethodB(int i);
	}
}
