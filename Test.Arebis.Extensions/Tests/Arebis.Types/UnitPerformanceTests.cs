using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	/// <summary>
	/// Summary description for PerformanceTests
	/// </summary>
	[TestClass]
	public class UnitPerformanceTests
	{
		const double MAX_ACCEPTABLE_VARIANCE = +0.25;
		const double MIN_ACCEPTABLE_VARIANCE = -0.25;

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
		public void AmountAdditionPerformanceTest()
		{
			Amount a = new Amount(15m, LengthUnits.Meter);
			Amount sum = new Amount(0m, LengthUnits.Meter);


			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				sum += a;
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.031) / 0.031;

			Console.WriteLine("Time to perform 100K additions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var*100);

			Assert.AreEqual(1500000m, sum.Value);
			
			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			//if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountDerivedAdditionPerformanceTest()
		{
			Amount sum = new Amount(0m, LengthUnits.KiloMeter);


			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				sum += new Amount(15m, LengthUnits.Meter);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.219) / 0.219;

			Console.WriteLine("Sum = {0}", sum);
			Console.WriteLine("Time to perform 100K additions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var*100);

			Assert.AreEqual(1500m, sum.Value);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountSimpleDivisionPerformanceTest()
		{
			Amount a = new Amount(15m, LengthUnits.Meter);
			Amount b = new Amount(3m, TimeUnits.Second);
			Amount q = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				q = a / b;
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.11) / 0.11;

			Console.WriteLine("Time to perform 100K simple divisions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);
			
			Assert.AreEqual(5m, q.Value);
			Assert.IsTrue(q.Unit.CompatibleTo(LengthUnits.Meter / TimeUnits.Second));

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountComplexDivisionPerformanceTest()
		{
			Amount a = new Amount(15m, LengthUnits.KiloMeter);
			Amount b = new Amount(3m, TimeUnits.Hour);
			Amount q = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				q = a / b;
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.11) / 0.11;
			
			Console.WriteLine("Time to perform 100K complex divisions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.AreEqual(5m, q.Value);
			Assert.AreEqual(5m, q.ConvertedTo(LengthUnits.KiloMeter / TimeUnits.Hour, 8).Value);
			Assert.IsTrue(q.Unit.CompatibleTo(LengthUnits.Meter / TimeUnits.Second));

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountScenario01PerformanceTest()
		{
			Amount distance;
			Amount speed;
			Amount duration = new Amount(0m, TimeUnits.Day);

			long t = Environment.TickCount;
			for (int n = 1; n <= 10000; n++)
			{
				distance = new Amount(50m, new Unit("myfoot", "myft", 44m, LengthUnits.CentiMeter));
				speed = new Amount(n, LengthUnits.KiloMeter / TimeUnits.Hour);
				duration += distance / speed;
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.234) / 0.234;

			duration = duration.ConvertedTo(TimeUnits.Minute, 1);
			
			Console.WriteLine("Time to perform 10K complex scenario: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.AreEqual(12.9m, duration.Value);
			Assert.IsTrue(duration.Unit.CompatibleTo(TimeUnits.Second));

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountSimpleConvertPerformanceTest()
		{
			Amount a = new Amount(15m, LengthUnits.KiloMeter);
			Amount b = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				b = a.ConvertedTo(LengthUnits.Meter, 8);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.172) / 0.172;

			Console.WriteLine("Result = {0}", b);
			Console.WriteLine("Time to perform 100K convertions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.AreEqual(15000m, b.Value);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void AmountComplexConvertPerformanceTest()
		{
			Amount a = new Amount(15m, (LengthUnits.KiloMeter * LengthUnits.Meter / LengthUnits.MilliMeter / (TimeUnits.Hour * TimeUnits.Minute / TimeUnits.MilliSecond)));
			Amount b = null;
			Unit targetUnit = LengthUnits.Meter / TimeUnits.Second;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				b = a.ConvertedTo(targetUnit, 8);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.203) / 0.203;

			Console.WriteLine("Original = {0}", a);
			Console.WriteLine("Result = {0}", b);
			Console.WriteLine("Time to perform 100K convertions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.AreEqual(0.06944444m, b.Value);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void UnitManagerResolveNamedToNamedTest()
		{
			// Try resolving a named unit:

			Unit u = LengthUnits.KiloMeter;
			Unit v = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				v = UnitManager.ResolveToNamedUnit(u, true);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.015) / 0.015;

			Console.WriteLine("Original = {0}", u.Name);
			Console.WriteLine("Result = {0}", v.Name);
			Console.WriteLine("Time to perform 100K resolutions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.IsTrue(v.IsNamed);
			Assert.AreEqual("kilometer", v.Name);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void UnitManagerResolveKnownToNamedTest()
		{
			// Try resolving a known unit:

			Unit u = LengthUnits.Meter / TimeUnits.Second;
			Unit v = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				v = UnitManager.ResolveToNamedUnit(u, true);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.219) / 0.219;

			Console.WriteLine("Original = {0}", u.Name);
			Console.WriteLine("Result = {0}", v.Name);
			Console.WriteLine("Time to perform 100K resolutions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.IsTrue(v.IsNamed);
			Assert.AreEqual("meter/second", v.Name);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void UnitManagerResolveUnknownToNamedTest()
		{
			// Try resolving an unknown unit of a known family:

			Unit u = LengthUnits.NauticalMile / TimeUnits.Minute;
			Unit v = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				v = UnitManager.ResolveToNamedUnit(u, true);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.297) / 0.297;

			Console.WriteLine("Original = {0}", u.Name);
			Console.WriteLine("Result = {0}", v.Name);
			Console.WriteLine("Time to perform 100K resolutions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.IsFalse(v.IsNamed);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}

		[TestMethod]
		public void UnitManagerResolveUnfamiliarToNamedTest()
		{
			// Try resolving a unit of an unknown family:

			Unit u = LengthUnits.Meter * ElectricUnits.Ohm;
			Unit v = null;

			long t = Environment.TickCount;
			for (int n = 0; n < 100000; n++)
			{
				v = UnitManager.ResolveToNamedUnit(u, true);
			}
			double time = (Environment.TickCount - t) / 1000.0;
			double var = (time - 0.032) / 0.032;

			Console.WriteLine("Original = {0}", u.Name);
			Console.WriteLine("Result = {0}", v.Name);
			Console.WriteLine("Time to perform 100K resolutions: {0} sec.", time);
			Console.WriteLine("Variation: {0:0}%.", var * 100);

			Assert.IsFalse(v.IsNamed);

			Assert.IsTrue(var < MAX_ACCEPTABLE_VARIANCE, "Performance lost detected!");
			if (var < MIN_ACCEPTABLE_VARIANCE) Assert.Inconclusive("Performance was much better than expected.");
		}
	}
}
