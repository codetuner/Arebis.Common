using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	[TestClass]
	public class UnitManagerTests
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
		public void RegisteredUnitsByTypeTest()
		{
			Console.WriteLine("Unitfamilies: ");
			foreach (UnitType type in UnitManager.GetUnitTypes())
			{
				Console.WriteLine(" - {0}", type);
				foreach (Unit u in UnitManager.GetUnits(type))
				{
					Console.WriteLine("    - {0:N}", u);
				}
			}
		}

		[TestMethod]
		public void GetUnit01Test()
		{
			Unit u = UnitManager.GetUnitByName("meter");
			Assert.AreEqual("meter", u.Name);
		}

		[TestMethod]
		public void GetUnit02Test()
		{
			Unit u = UnitManager.GetUnitByName("kilometer");
			Assert.AreEqual("kilometer", u.Name);
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownUnitException))]
		public void ResolveUnit01Test()
		{
			Unit u = UnitManager.GetUnitByName("unknownUnit");
		}

		[TestMethod]
		public void ResolveUnit02Test()
		{
			UnitManager.Instance.UnitResolve += new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
			try
			{
				ResetResolveCounters();
				Unit u = UnitManager.GetUnitByName("resolvedUnit");
				Assert.IsNotNull(u);
				Assert.AreEqual(1, this.resolveAttempts);
				Assert.AreEqual("resolvedUnit", u.Name);
			}
			finally 
			{
				UnitManager.Instance.UnitResolve -= new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
			}
		}

		[TestMethod]
		public void ResolveUnit03Test()
		{
			UnitManager.Instance.UnitResolve += new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
			try
			{
				ResetResolveCounters();
				Unit u;
				u = UnitManager.GetUnitByName("resolvedUnit");
				u = UnitManager.GetUnitByName("resolvedUnit");
				u = UnitManager.GetUnitByName("resolvedUnit");
				
				// ResolveAttempts should be no more than 1 as unit should be auto-registered
				// after 1st attempt:
				Assert.AreEqual(1, this.resolveAttempts);
				Assert.AreEqual("resolvedUnit", u.Name);
			}
			finally
			{
				UnitManager.Instance.UnitResolve -= new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(UnknownUnitException))]
		public void ResolveUnit04Test()
		{
			UnitManager.Instance.UnitResolve += new UnitResolveEventHandler(UnitResolveAlwaysFail);
			try
			{
				Unit u = UnitManager.GetUnitByName("resolvedUnit");
			}
			finally
			{
				UnitManager.Instance.UnitResolve -= new UnitResolveEventHandler(UnitResolveAlwaysFail);
			}
		}

		[TestMethod]
		public void ResolveUnit05Test()
		{
			UnitManager.Instance.UnitResolve += new UnitResolveEventHandler(UnitResolveAlwaysFail);
			UnitManager.Instance.UnitResolve += new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
			try
			{
				ResetResolveCounters();
				Unit u = UnitManager.GetUnitByName("resolvedUnit");
				Assert.IsNotNull(u);
				Assert.AreEqual(2, this.resolveAttempts);
				Assert.AreEqual("resolvedUnit", u.Name);
			}
			finally
			{
				UnitManager.Instance.UnitResolve -= new UnitResolveEventHandler(UnitResolveAlwaysSucceed);
				UnitManager.Instance.UnitResolve -= new UnitResolveEventHandler(UnitResolveAlwaysFail);
			}
		}

		[TestMethod()]
		public void ResolveToNamedUnit01Test()
		{
			Unit u = LengthUnits.Meter * LengthUnits.Meter;
			Unit m = UnitManager.ResolveToNamedUnit(u, true);
			Console.WriteLine(m);
			Assert.IsTrue(m.IsNamed);
			Assert.AreEqual("meter²", m.Name);
		}

		[TestMethod()]
		public void ResolveToNamedUnit02Test()
		{
			Unit u = LengthUnits.Meter * LengthUnits.Meter * LengthUnits.Meter;
			Unit m = UnitManager.ResolveToNamedUnit(u, true);
			Console.WriteLine(m);
			Assert.IsTrue(m.IsNamed);
			Assert.AreEqual("meter³", m.Name);
		}

		[TestMethod()]
		public void DefaultConversion01Test()
		{
			Amount a = new Amount(100m, LengthUnits.KiloMeter);
			Amount b = UnitManager.ConvertTo(a, LengthUnits.Meter);
			Console.WriteLine("{0} = {1}", a, b);
			Assert.AreEqual(100000m, b.Value);
			Assert.AreEqual(LengthUnits.Meter, b.Unit);
		}

		[TestMethod()]
		public void ComplexConversion01Test()
		{
			Amount kh = new Amount(100m, LengthUnits.KiloMeter / TimeUnits.Hour);
			Amount ms = kh.ConvertedTo(LengthUnits.Meter / TimeUnits.Second, 4);
			Console.WriteLine("{0} = {1}", kh, ms);
			Assert.AreEqual(27.7778m, ms.Value);
			Assert.AreEqual(LengthUnits.Meter / TimeUnits.Second, ms.Unit);
		}

		[TestMethod()]
		public void ComplexConversion02Test()
		{
			Amount kwh = new Amount(1000m, EnergyUnits.KiloWattHour);
			Amount gj = kwh.ConvertedTo(EnergyUnits.GigaJoule, 8);
			Console.WriteLine("{0} = {1}", kwh, gj);
			Assert.AreEqual(3.6m, gj.Value);
			Assert.AreEqual(EnergyUnits.GigaJoule, gj.Unit);
		}

		[TestMethod()]
		public void CustomConversion01Test()
		{
			Amount c = new Amount(25, TemperatureUnits.DegreeCelcius);
			Amount f = new Amount(2.3m, TemperatureUnits.DegreeFahrenheit);

			Amount s = (c + f);

			Amount k = s.ConvertedTo(TemperatureUnits.DegreeKelvin, 2);

			Console.WriteLine("{0} + {1} = {2} = {3}", c, f, s, k);

			Assert.AreEqual(TemperatureUnits.DegreeKelvin, k.Unit);
			Assert.AreEqual(281.65m, k.Value);
		}

		[TestMethod()]
		public void CustomConversion02Test()
		{
			Amount t = new Amount(25, TemperatureUnits.DegreeCelcius);
			Console.WriteLine("Temperature : {0}", t);

			t = t.ConvertedTo(TemperatureUnits.DegreeFahrenheit, 4);
			Console.WriteLine("Temperature : {0}", t);

			t = t.ConvertedTo(TemperatureUnits.DegreeKelvin, 4);
			Console.WriteLine("Temperature : {0}", t);

			t = t.ConvertedTo(TemperatureUnits.DegreeCelcius, 4);
			Console.WriteLine("Temperature : {0}", t);

			Assert.AreEqual(new Amount(25, TemperatureUnits.DegreeCelcius), t);
		}

		[TestMethod()]
		public void CustomConversion03Test()
		{
			Unit kk = new Unit("kilokelvin", "kk", 1000, TemperatureUnits.DegreeKelvin);
			Amount t = new Amount(3m, kk);

			t = t.ConvertedTo(TemperatureUnits.DegreeCelcius);

			Assert.AreEqual(2726.85m, t.Value);
		}

		#region Resolve handlers & utilities

		private int resolveAttempts = 0;

		private void ResetResolveCounters()
		{
			this.resolveAttempts = 0;
		}

		Unit UnitResolveAlwaysSucceed(object sender, ResolveEventArgs args)
		{
			Console.WriteLine("- Attempting to resolve unit '{0}' >> succeeding", args.Name);
			this.resolveAttempts++;
			return new Unit(args.Name, args.Name, new UnitType(args.Name));
		}

		Unit UnitResolveAlwaysFail(object sender, ResolveEventArgs args)
		{
			Console.WriteLine("- Attempting to resolve unit '{0}' >> failing", args.Name);
			this.resolveAttempts++;
			return null;
		}

		#endregion Resolve handlers & utilities
	}
}
