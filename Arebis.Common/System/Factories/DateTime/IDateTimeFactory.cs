using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Definition of a unit-testable DateTime factory.
	/// </summary>
	public interface IDateTimeFactory
	{
		/// <summary>
		/// The current date.
		/// </summary>
		global::System.DateTime Today { get; }

		/// <summary>
		/// The current local time.
		/// </summary>
		global::System.DateTime Now { get; }

		/// <summary>
		/// The current universal time.
		/// </summary>
		global::System.DateTime UtcNow { get; }

		/// <summary>
		/// The current timezone.
		/// </summary>
		global::System.TimeZone CurrentTimeZone { get; }
	}
}
