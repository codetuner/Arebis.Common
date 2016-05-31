using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	/// <summary>
	/// Summary description for ScenarioTests
	/// </summary>
	[TestClass]
	public class UnitScenarioTests
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
		public void Scenario01Test()
		{ 
			// What is the height of 36 liter water spread over an area of 45cm x 2m ?

			Amount volume = new Amount(36m, "liter");
			Amount area = new Amount(45m, "centimeter") * new Amount(2m, "meter");

			Amount height = volume / area;

			Console.WriteLine("Volume : {0}", volume);
			Console.WriteLine("Area : {0}", area);
			Console.WriteLine("Height : {0}", height);
			Console.WriteLine("Height : {0}", height.ConvertedTo("centimeter", 2));

			Assert.AreEqual(new Amount(4m, "centimeter"), height);
		}

		[TestMethod()]
		public void Scenario02Test()
		{
			// Driving 240 km in 2 hours, what is the average speed ?

			Amount distance = new Amount(240, LengthUnits.KiloMeter);
			Amount time = new Amount(2, TimeUnits.Hour);

			Amount speed = distance / time;

			Console.WriteLine("Speed : {0}", speed);
			Console.WriteLine("Speed : {0}", speed.ConvertedTo(LengthUnits.Meter / TimeUnits.Second));
			
			Assert.AreEqual(new Amount(120, LengthUnits.KiloMeter / TimeUnits.Hour), speed);
		}

		[TestMethod()]
		public void Scenario03Test()
		{
			// Driving 15 min at a speed of 120 km/h, what distance is made ?

			Amount speed = new Amount(120, LengthUnits.KiloMeter / TimeUnits.Hour);
			Amount time = new Amount(15, TimeUnits.Minute);

			Amount distance = speed * time;

			Console.WriteLine("Distance : {0}", distance);
			Console.WriteLine("Distance : {0}", distance.ConvertedTo("kilometer", 4));

			Assert.AreEqual(new Amount(30, LengthUnits.KiloMeter), distance);
		}

		[TestMethod()]
		public void Scenario04Test()
		{
			// What is the sum of 500 meter and 2 km ?

			Amount a = new Amount(500, LengthUnits.Meter);
			Amount b = new Amount(2, LengthUnits.KiloMeter);

			Amount sum = a + b;

			Console.WriteLine("Sum : {0}", sum);

			Assert.AreEqual(new Amount(2.5m, LengthUnits.KiloMeter), sum);
		}

		[TestMethod]
		public void Scenario05Test()
		{
			// Scenario calculating stop-distance:
			// A car drives at 120 km/h. What distance does the car need to stop
			// at a deceleration of 6m/s² if the driver needs 1 second to react ?

			// Parameters:
			Amount speed = new Amount(120, LengthUnits.KiloMeter / TimeUnits.Hour);
			Amount reactiontime = new Amount(1, TimeUnits.Second);
			Amount deceleration = new Amount(6, LengthUnits.Meter / TimeUnits.Second.Power(2));

			// Formula:
			Amount distance;
			distance = (speed * reactiontime) + speed.Power(2) / (2 * deceleration);

			Console.WriteLine("Distance : {0}", distance);
			Console.WriteLine("Distance : {0}", distance.ConvertedTo(LengthUnits.Meter, 1));

			// Result:
			Assert.AreEqual(new Amount(125.9m, LengthUnits.Meter), distance.ConvertedTo(LengthUnits.Meter, 1));
		}

		[TestMethod]
		public void Scenario06Test()
		{ 
			// A bottle of 50 liter gas compressed at 80 bar.
			// How many m³ does this represent at 1 atmosphere ?

			Amount bottleVolume = new Amount(50m, "liter");
			Amount bottlePressure = new Amount(80m, "bar");
			Amount outerPressure = new Amount(1m, "atmosphere");

			Amount outerVolume = bottleVolume * bottlePressure / outerPressure;

			Console.WriteLine("Volume : {0}", outerVolume);
			Console.WriteLine("Volume : {0}", outerVolume.ConvertedTo("meter³", 2));

			Assert.AreEqual(new Amount(3.95m, VolumeUnits.Meter3), outerVolume.ConvertedTo("meter³", 2));
		}

		[TestMethod]
		public void Scenario07Test()
		{ 
			// What is the energetic value of an amount of LNG ?
			Unit kWhpm3 = new Unit("kWh/m³", "kWh/m³", 1m, EnergyUnits.KiloWattHour / VolumeUnits.Meter3);
			Amount lng = new Amount(83.24m, VolumeUnits.Meter3);
			Amount ghv = new Amount(6699m, kWhpm3);

			Amount energy = lng * ghv;

			Console.WriteLine("LNG volume : {0}", lng);
			Console.WriteLine("GHV : {0}", ghv);
			Console.WriteLine("Energy : {0}", energy);
			Console.WriteLine("Energy : {0}", energy.ConvertedTo("kilowatt-hour"));

			Assert.AreEqual(new Amount(557625m, EnergyUnits.KiloWattHour), energy.ConvertedTo(EnergyUnits.KiloWattHour, 0));
		}
	}
}
