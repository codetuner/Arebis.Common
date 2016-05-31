using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	[TestClass()]
	public class UnitAmountRange
	{
		[TestMethod()]
		public void AmountRange01Test()
		{
			Range<Amount> intr = new Range<Amount>(new Amount(5m, LengthUnits.Meter), new Amount(200m, LengthUnits.CentiMeter));
			Assert.IsFalse(intr.Includes(new Amount(1.9m, LengthUnits.Meter)));
			Assert.IsTrue(intr.Includes(new Amount(2.0m, LengthUnits.Meter)));
			Assert.IsTrue(intr.Includes(new Amount(4.0m, LengthUnits.Meter)));
			Assert.IsTrue(intr.Includes(new Amount(5.0m, LengthUnits.Meter)));
			Assert.IsFalse(intr.Includes(new Amount(5.1m, LengthUnits.Meter)));
		}
	}
}
