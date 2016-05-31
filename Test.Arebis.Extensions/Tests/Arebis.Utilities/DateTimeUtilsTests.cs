using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Arebis.Testing.Factories;
using Misc;
using Arebis.Utils;

namespace Arebis.Extensions.Tests.Arebis.Utilities
{
	[TestClass()]
	public class DateTimeUtilsTests
	{
		[TestMethod]
		public void NumberOfHoursTest()
		{
			Assert.AreEqual(24d, (new DateTime(2007, 03, 12)).NumberOfHours());
			Assert.AreEqual(25d, (new DateTime(2007, 03, 25)).NumberOfHours());
			Assert.AreEqual(24d, (new DateTime(2007, 07, 21)).NumberOfHours());
			Assert.AreEqual(23d, (new DateTime(2007, 10, 28)).NumberOfHours());
			Assert.AreEqual(24d, (new DateTime(2007, 12, 31)).NumberOfHours());
		}

		[TestMethod]
		public void TZITest()
		{
			// Listing all timezones registered on computer:
			List<TimeZoneInformation> timeZones = TimeZoneInformation.TimeZones;
			timeZones.Sort();
			foreach (TimeZoneInformation tz in timeZones)
			{
				Console.WriteLine("{0,-64} {1,10} {2,10}", tz.DisplayName, tz.StandardOffset, tz.DaylightOffset);
			}

		}

		[TestMethod]
		public void DateTimeAddTest()
		{
			// Just to test the behaviour of the AddMinutes method of DateTime and see
			// it's operation is not DST-aware:
			DateTime dt1 = new DateTime(2007, 3, 25, 02, 55, 0, DateTimeKind.Local);
			DateTime dt2 = dt1.AddMinutes(10.0);
			Console.WriteLine("{0:o}, {1:o}", dt1, dt1.ToUniversalTime());
			Console.WriteLine("{0:o}, {1:o}", dt2, dt2.ToUniversalTime());
			Console.WriteLine(dt2.Subtract(dt1));
			Console.WriteLine((dt2.Ticks - dt1.Ticks) / TimeSpan.TicksPerMinute);
			Console.WriteLine(dt2.ToUniversalTime().Subtract(dt1.ToUniversalTime()));
			Console.WriteLine((dt2.ToUniversalTime().Ticks - dt1.ToUniversalTime().Ticks) / TimeSpan.TicksPerMinute);
		}
	}
}
