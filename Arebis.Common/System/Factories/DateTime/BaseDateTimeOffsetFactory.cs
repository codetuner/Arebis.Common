using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Base class to implement unit-testable DateTimeOffset factories.
	/// </summary>
	public abstract class BaseDateTimeOffsetFactory : IDateTimeOffsetFactory
	{
		/// <summary>
		/// The current date.
		/// </summary>
		public global::System.DateTimeOffset Today
		{
			get { return this.Now.Date; }
		}

		/// <summary>
		/// The current universal time.
		/// </summary>
		public global::System.DateTimeOffset UtcNow
		{
			get { return this.Now.ToUniversalTime(); }
		}

		/// <summary>
		/// The current local time.
		/// </summary>
		public abstract global::System.DateTimeOffset Now { get; }

		/// <summary>
		/// The current timezone.
		/// </summary>
		public abstract TimeZone CurrentTimeZone { get; }
	}
}
