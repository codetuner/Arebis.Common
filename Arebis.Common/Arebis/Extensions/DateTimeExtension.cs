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
	}
}
