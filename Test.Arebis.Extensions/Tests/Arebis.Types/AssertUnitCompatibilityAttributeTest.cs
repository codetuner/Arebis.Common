using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Types;
using Arebis.Runtime.Validation;

namespace Test.Arebis.Extensions.Tests.Arebis.Types
{
	[TestClass]
	public class AssertUnitCompatibilityAttributeTest
	{
		#region Initialize & cleanup

		private UnitManager defaultUnitManager;

		[TestInitialize()]
		public void MyTestInitialize()
		{
			Console.Write("Resetting the UnitManager instance...");
			this.defaultUnitManager = UnitManager.Instance;
			UnitManager.Instance = new UnitManager();
			UnitManager.RegisterByAssembly(typeof(global::Arebis.Types.LengthUnits).Assembly);
			Console.WriteLine(" done.");
		}

		[TestCleanup()]
		public void MyTestCleanup()
		{
			UnitManager.Instance = this.defaultUnitManager;
		}

		#endregion Initialize & cleanup

		[TestMethod]
		public void ValidAmountTest()
		{
			this.SomeLengthAmount = new Amount(100m, LengthUnits.Meter);

			Assert.AreEqual(new Amount(100m, LengthUnits.Meter), this.SomeLengthAmount);

			this.AssertValid();
		}

		[TestMethod]
		[ExpectedException(typeof(AssertionFailedException))]
		public void InvalidAmountTest()
		{
			this.SomeLengthAmount = new Amount(100m, TimeUnits.Minute);

			Assert.AreEqual(new Amount(100m, TimeUnits.Minute), this.SomeLengthAmount);

			this.AssertValid();
		}

		[AssertUnitCompatibility("meter")]
		public Amount SomeLengthAmount { get; set; }
	}
}
