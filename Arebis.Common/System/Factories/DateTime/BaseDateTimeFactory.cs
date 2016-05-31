using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Base class to implement unit-testable DateTime factories.
	/// </summary>
	public abstract class BaseDateTimeFactory : IDateTimeFactory
	{
		/// <summary>
		/// The current date.
		/// </summary>
		public global::System.DateTime Today
		{
			get { return this.Now.Date; }
		}

		/// <summary>
		/// The current universal time.
		/// </summary>
		public global::System.DateTime UtcNow
		{
			get { return this.Now.ToUniversalTime(); }
		}

		/// <summary>
		/// The current local time.
		/// </summary>
		public abstract global::System.DateTime Now { get; }

		/// <summary>
		/// The current timezone.
		/// </summary>
		public abstract TimeZone CurrentTimeZone { get; }
	}
}
