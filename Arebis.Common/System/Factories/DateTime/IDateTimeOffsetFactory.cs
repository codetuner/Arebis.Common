using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Definition of a unit-testable DateTimeOffset factory.
	/// </summary>
	public interface IDateTimeOffsetFactory
	{
		/// <summary>
		/// The current date.
		/// </summary>
		global::System.DateTimeOffset Today { get; }

		/// <summary>
		/// The current local time.
		/// </summary>
		global::System.DateTimeOffset Now { get; }

		/// <summary>
		/// The current universal time.
		/// </summary>
		global::System.DateTimeOffset UtcNow { get; }

		/// <summary>
		/// The current timezone.
		/// </summary>
		global::System.TimeZone CurrentTimeZone { get; }
	}
}
