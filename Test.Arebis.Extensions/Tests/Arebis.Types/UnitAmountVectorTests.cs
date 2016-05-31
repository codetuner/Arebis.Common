using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	/// <summary>
	/// Summary description for AmountVectorTests
	/// </summary>
	[TestClass]
	public class UnitAmountVectorTests
	{
		[TestMethod]
		public void Enumeration01Test()
		{
			AmountVector v = new AmountVector(4, LengthUnits.KiloMeter);
			v[0] = new Amount(500, LengthUnits.Meter);
			v[2] = new Amount(200, LengthUnits.Meter);

			// Enumerate:
			decimal sum = 0m;
			int loops = 0;
			foreach (Amount a in v.All())
			{
				Console.WriteLine((a == null) ? "-" : a.ToString());
				if (a != null) sum += a.Value;
				loops++;
			}

			// Checks:
			Assert.AreEqual(4, loops);
			Assert.AreEqual(0.7m, sum);
		}

		[TestMethod]
		public void Enumeration02Test()
		{
			AmountVector v = new AmountVector(4, LengthUnits.KiloMeter);
			v[0] = new Amount(500, LengthUnits.Meter);
			v[2] = new Amount(200, LengthUnits.Meter);

			// Enumerate:
			decimal sum = 0m;
			int loops = 0;
			foreach (Amount a in v.NonNulls())
			{
				Console.WriteLine((a == null) ? "-" : a.ToString());
				if (a != null) sum += a.Value;
				loops++;
			}

			// Checks:
			Assert.AreEqual(2, loops);
			Assert.AreEqual(0.7m, sum);
		}

		[TestMethod]
		public void AllZeros01Test()
		{
			AmountVector v = AmountVector.AllZeros(4, LengthUnits.Meter);
			v[2] = new Amount(0.2m, LengthUnits.KiloMeter);

			int loops = 0;
			foreach (Amount a in v.NonNulls())
			{
				loops++;
			}

			Assert.AreEqual(4, loops);
			Assert.AreEqual(new Amount(200m, LengthUnits.Meter), v.Sum);
		}

		[TestMethod]
		public void Sum01Test()
		{
			AmountVector v = new AmountVector(4, LengthUnits.Meter);
			v[0] = new Amount(150m, LengthUnits.Meter);
			v[2] = new Amount(0.2m, LengthUnits.KiloMeter);
			v[3] = new Amount(650m, LengthUnits.Meter);

			Assert.AreEqual(new Amount(1m, LengthUnits.KiloMeter), v.Sum);
		}

		[TestMethod]
		public void Average01Test()
		{
			AmountVector v = new AmountVector(4, LengthUnits.Meter);
			v[1] = new Amount(1m, LengthUnits.KiloMeter);
			v[3] = new Amount(500m, LengthUnits.Meter);

			Assert.AreEqual(new Amount(0.750m, LengthUnits.KiloMeter), v.Average);
		}

		[TestMethod]
		public void ConvertedTo01Test()
		{
			AmountVector v = new AmountVector(4, LengthUnits.Meter);
			v[0] = new Amount(150m, LengthUnits.Meter);
			v[2] = new Amount(0.2m, LengthUnits.KiloMeter);
			v[3] = new Amount(650m, LengthUnits.Meter);

			AmountVector c = v.ConvertedTo(LengthUnits.KiloMeter);

			Assert.AreEqual(1000m, v.Sum.Value);
			Assert.AreEqual(LengthUnits.Meter, v.Sum.Unit);
			Assert.AreEqual(1m, c.Sum.Value);
			Assert.AreEqual(LengthUnits.KiloMeter, c.Sum.Unit);
		}

		[TestMethod]
		public void MaxAndMin01Test()
		{
			AmountVector v = new AmountVector(5, LengthUnits.Meter);
			v[0] = new Amount(0.2m, LengthUnits.KiloMeter);
			v[2] = new Amount(150m, LengthUnits.Meter);
			v[3] = new Amount(650m, LengthUnits.Meter);
			v[4] = new Amount(500m, LengthUnits.Meter);

			Assert.AreEqual(new Amount(650m, LengthUnits.Meter), v.Max);
			Assert.AreEqual(new Amount(150m, LengthUnits.Meter), v.Min);
		}

		[TestMethod]
		public void ToArray01Test()
		{
			AmountVector v = new AmountVector(5, LengthUnits.Meter);
			v[0] = new Amount(0.2m, LengthUnits.KiloMeter);
			v[2] = new Amount(150m, LengthUnits.Meter);
			v[3] = new Amount(650m, LengthUnits.Meter);
			v[4] = new Amount(500m, LengthUnits.Meter);

			Amount[] array = v.ToArray();

			Assert.AreEqual(5, array.Length);
			Assert.AreEqual(v.Length, array.Length);
			Assert.AreEqual(v[0], array[0]);
			Assert.AreEqual(v[1], array[1]);
			Assert.AreEqual(v[2], array[2]);
			Assert.AreEqual(v[3], array[3]);
			Assert.AreEqual(v[4], array[4]);
		}

		[TestMethod]
		public void Length01Test()
		{
			AmountVector v = new AmountVector(new decimal?[] { 10m, 3m, null, 5m }, LengthUnits.Meter);

			Assert.AreEqual(4, v.Length);
			Assert.AreEqual(3, v.LengthNonNulls);
		}

		[TestMethod]
		public void FirstAndLast01Test()
		{
			AmountVector v = new AmountVector(new decimal?[] { 10m, 3m, null, 5m }, LengthUnits.Meter);
			AmountVector w = new AmountVector(new decimal?[] { null, 3m, null, null }, LengthUnits.Meter);
			AmountVector x = new AmountVector(new decimal?[] { null, null, null, null }, LengthUnits.Meter);

			Assert.AreEqual(new Amount(10m, LengthUnits.Meter), v.FirstNonNull);
			Assert.AreEqual(new Amount(3m, LengthUnits.Meter), w.FirstNonNull);
			Assert.AreEqual(null, x.FirstNonNull);
			Assert.AreEqual(new Amount(5m, LengthUnits.Meter), v.LastNonNull);
			Assert.AreEqual(new Amount(3m, LengthUnits.Meter), w.LastNonNull);
			Assert.AreEqual(null, x.LastNonNull);
			Assert.AreEqual(3, v.LengthNonNulls);
		}

		[TestMethod]
		public void OperatorAdd01Test()
		{
			AmountVector v = new AmountVector(new decimal?[] { 10m, null, null, 5m }, LengthUnits.Meter);
			AmountVector w = new AmountVector(new decimal?[] { null, null, 12m, 6m }, LengthUnits.Meter);

			AmountVector r = v + w;

			foreach (Amount a in r.All())
			{
				Console.WriteLine(a);
			}

			Assert.AreEqual(4, r.Length);
			Assert.AreEqual(v.Unit, r.Unit);
			Assert.AreEqual(10m, r[0].Value);
			Assert.AreEqual(null, r[1]);
			Assert.AreEqual(12m, r[2].Value);
			Assert.AreEqual(11m, r[3].Value);
		}

		[TestMethod]
		public void OperatorSubstract01Test()
		{
			AmountVector v = new AmountVector(new decimal?[] { 10m, null, null, 5m }, LengthUnits.Meter);
			AmountVector w = new AmountVector(new decimal?[] { null, null, 12m, 6m }, LengthUnits.Meter);

			AmountVector r = v - w;

			foreach (Amount a in r.All())
			{
				Console.WriteLine(a);
			}

			Assert.AreEqual(4, r.Length);
			Assert.AreEqual(v.Unit, r.Unit);
			Assert.AreEqual(10m, r[0].Value);
			Assert.AreEqual(null, r[1]);
			Assert.AreEqual(-12m, r[2].Value);
			Assert.AreEqual(-1m, r[3].Value);
		}

		[TestMethod]
		public void OperatorMultiply01Test()
		{
			// Suppose following rooms in a house have following areas:
			AmountVector areas = new AmountVector(new decimal?[] { 13m, 7.5m, 6m, 8m, 2m }, SurfaceUnits.Meter2);

			// Suppose all rooms have following height:
			Amount height = new Amount(2.8m, LengthUnits.Meter);

			// Volumes of rooms:
			AmountVector volumes = areas * height;

			foreach (Amount a in volumes.All())
			{
				Console.WriteLine(a);
			}
			Console.WriteLine("Sum: {0}", volumes.Sum);

			Assert.AreEqual(5, volumes.Length);
			Assert.AreEqual(VolumeUnits.Meter3, volumes.Unit);
			Assert.AreEqual(new Amount(36.4m, VolumeUnits.Meter3), volumes[0]);
			Assert.AreEqual(new Amount(21m, VolumeUnits.Meter3), volumes[1]);
			Assert.AreEqual(new Amount(16.8m, VolumeUnits.Meter3), volumes[2]);
			Assert.AreEqual(new Amount(22.4m, VolumeUnits.Meter3), volumes[3]);
			Assert.AreEqual(new Amount(5.6m, VolumeUnits.Meter3), volumes[4]);
			Assert.AreEqual(new Amount(102.2m, VolumeUnits.Meter3), volumes.Sum);
		}

		[TestMethod]
		public void OperatorMultiply02Test()
		{
			// Suppose I have following length measures for rooms:
			AmountVector lengths = new AmountVector(new decimal?[] { 13m, 7.5m, 6m, 8m, 2m }, LengthUnits.Meter);

			// And following width measures for rooms:
			AmountVector widths = new AmountVector(new decimal?[] { 2m, 2m, 3m, 1m, 4m}, LengthUnits.Meter);

			// What are the area's of the respective rooms ?
			AmountVector areas = lengths * widths;

			// Total area:
			Amount totalArea = areas.Sum.ConvertedTo(SurfaceUnits.Meter2);

			for (int i=0; i<lengths.Length; i++)
			{
				Console.WriteLine("{0}: {1} x {2} = {3}", i, lengths[i], widths[i], areas[i]);
			}
			Console.WriteLine("Total area: {0}", totalArea);

			Assert.AreEqual(5, areas.Length);
			Assert.AreEqual(SurfaceUnits.Meter2, areas.Unit);
			Assert.AreEqual(new Amount(26m, SurfaceUnits.Meter2), areas[0]);
			Assert.AreEqual(new Amount(15m, SurfaceUnits.Meter2), areas[1]);
			Assert.AreEqual(new Amount(18m, SurfaceUnits.Meter2), areas[2]);
			Assert.AreEqual(new Amount(8m, SurfaceUnits.Meter2), areas[3]);
			Assert.AreEqual(new Amount(8m, SurfaceUnits.Meter2), areas[4]);
			Assert.AreEqual(new Amount(75m, SurfaceUnits.Meter2), areas.Sum);
		}

		[TestMethod]
		public void OperatorDivide01Test()
		{
			AmountVector energies = new AmountVector(new decimal?[] { 83000m, null, 12600m, 99250m }, EnergyUnits.KiloWattHour);
			Amount ghv = new Amount(6600m, EnergyUnits.KiloWattHour / VolumeUnits.Meter3);

			AmountVector volumes = energies / ghv;

			foreach (Amount a in volumes.All())
			{
				Console.WriteLine(a);
			}
			Console.WriteLine("Sum: {0}", volumes.Sum);

			Assert.AreEqual(4, volumes.Length);
			Assert.AreEqual(VolumeUnits.Meter3, volumes.Unit);
			Assert.AreEqual(new Amount(12.58m, VolumeUnits.Meter3), volumes[0].ConvertedTo(VolumeUnits.Meter3, 2));
			Assert.AreEqual(null, volumes[1]);
			Assert.AreEqual(new Amount(1.91m, VolumeUnits.Meter3), volumes[2].ConvertedTo(VolumeUnits.Meter3, 2));
			Assert.AreEqual(new Amount(15.04m, VolumeUnits.Meter3), volumes[3].ConvertedTo(VolumeUnits.Meter3, 2));
			Assert.AreEqual(new Amount(29.52m, VolumeUnits.Meter3), volumes.Sum.ConvertedTo(VolumeUnits.Meter3, 2));
		}

		[TestMethod()]
		public void SerializeDeserialize01Test()
		{
			MemoryStream buffer = new MemoryStream();

			// Make some amountvector:
			AmountVector v1before = new AmountVector(new decimal?[] { 83000m, null, 12600m, 99250m }, EnergyUnits.KiloWattHour);
			AmountVector v2before = new AmountVector(new decimal?[] { 4m, null, null, 6m }, TimeUnits.Hour);

			// Serialize the units:
			BinaryFormatter f = new BinaryFormatter();
			f.Serialize(buffer, v1before);
			f.Serialize(buffer, v2before);

			// Reset stream:
			buffer.Seek(0, SeekOrigin.Begin);

			// Deserialize units:
			BinaryFormatter g = new BinaryFormatter();
			AmountVector v1after = (AmountVector)g.Deserialize(buffer);
			AmountVector v2after = (AmountVector)g.Deserialize(buffer);

			buffer.Close();

			Console.WriteLine("{0} => {1}", v1before, v1after);
			Console.WriteLine("{0} => {1}", v2before, v2after);
			Console.WriteLine("{0} => {1}", v1before / v2before, v1after / v2after);

			Assert.AreEqual(v1before.Sum, v1after.Sum);
			Assert.AreEqual(v2before.Sum, v2after.Sum);
		}
	}
}
