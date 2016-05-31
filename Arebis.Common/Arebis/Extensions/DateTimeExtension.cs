using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Arebis.Extensions
{
	/// <summary>
	/// Provides extension methods to the DateTime struct.
	/// </summary>
	/// <remarks>
	/// As Extension Methods will be supported by the C# language, these
	/// methods will be changed into real Extension Methods.
	/// </remarks>
	public static class DateTimeExtension
	{
        /// <summary>
        /// Returns the smallest (earliest) of both dates.
        /// If one of both dates is null, the other is returned.
        /// If both dates are null, null is returned.
        /// </summary>
        public static DateTime? Min(DateTime? val1, DateTime? val2)
        {
            if (val1.HasValue && val2.HasValue)
                return (val1 < val2) ? val1 : val2;
            else
                return val1 ?? val2;
        }

        /// <summary>
        /// Returns the larges (latest) of both dates.
        /// If one of both dates is null, the other is returned.
        /// If both dates are null, null is returned.
        /// </summary>
        public static DateTime? Max(DateTime? val1, DateTime? val2)
        {
            if (val1.HasValue && val2.HasValue)
                return (val1 > val2) ? val1 : val2;
            else
                return val1 ?? val2;
        }

		/// <summary>
		/// Returns the difference between both datetimes defined as this - otherDateTime.
		/// </summary>
		public static TimeSpan Diff(this DateTime dt, DateTime otherDateTime)
		{
			return new TimeSpan(dt.Ticks - otherDateTime.Ticks);
		}

		/// <summary>
		/// Returns the absolute difference between both datetimes defined as Abs(this - otherDateTime).
		/// </summary>
		public static TimeSpan DiffAbs(this DateTime dt, DateTime otherDateTime)
		{
			return new TimeSpan(Math.Abs(dt.Ticks - otherDateTime.Ticks));
		}

		/// <summary>
		/// Returns the number of hours in the given date.
		/// This is 23, 24 or 25, depending on daylightsavings.
		/// Although technically, this could be a franctionated value.
		/// </summary>
		public static double NumberOfHours(this DateTime date)
		{
            DaylightTime daylightTime = Current.DateTime.CurrentTimeZone.GetDaylightChanges(date.Year);
			if (daylightTime.Start.Date == date.Date)
				return 24d + daylightTime.Delta.TotalHours;
			else if (daylightTime.End.Date == date.Date)
				return 24d - daylightTime.Delta.TotalHours;
			else
				return 24d;
		}

        /// <summary>
        /// Returns the given DateTime in Epoch time (aka Unix time).
        /// That is the number of elapsed milliseconds since 1970/01/01 00:00 UTC.
        /// </summary>
        public static long ToEpochTime(this DateTime dt)
        {
            TimeSpan t = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)t.TotalSeconds;
        }

        /// <summary>
        /// Returns the given DateTime as Excel numerical value.
        /// That is, the number of days since 1900/01/01 00:00 (which has value 1.0).
        /// </summary>
        public static double ToExcelTime(this DateTime dt)
        {
            TimeSpan t = dt - new DateTime(1899, 12, 31);
            return t.TotalDays;
        }

        /// <summary>
        /// The age in full years. Time component is ignored.
        /// </summary>
        public static int AgeInYears(this DateTime dt, DateTime onDate)
        {
            var age = (onDate.Year - dt.Year);
            if (dt.Month > onDate.Month || (dt.Month == onDate.Month && dt.Day < onDate.Day)) age--;
            return age;
        }

        /// <summary>
        /// The age in full months. Time component is ignored.
        /// </summary>
        public static int AgeInMonths(this DateTime dt, DateTime onDate)
        {
            var age = (onDate.Year - dt.Year) * 12
            + (onDate.Month - dt.Month)
            + ((onDate.Day < dt.Day) ? -1 : 0);

            return age;
        }

	}
}
