using System;
using System.Collections.Generic;
using System.Text;

namespace System.Factories.DateTime
{
	/// <summary>
	/// Default implementation of IDateTimeOffsetFactory.
	/// </summary>
	class DefaultDateTimeOffsetFactory : BaseDateTimeOffsetFactory
	{
		public override System.DateTimeOffset Now
		{
			get { return global::System.DateTimeOffset.Now; }
		}

		public override TimeZone CurrentTimeZone
		{
			get { return global::System.TimeZone.CurrentTimeZone; }
		}
	}
}
