using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Default implementation of IDateTimeFactory.
	/// </summary>
	class DefaultDateTimeFactory : BaseDateTimeFactory
	{
		public override System.DateTime Now
		{
			get { return global::System.DateTime.Now; }
		}

		public override TimeZone CurrentTimeZone
		{
			get { return global::System.TimeZone.CurrentTimeZone; }
		}
	}
}
