using System;
using System.Collections.Generic;
using System.Text;
using System.Factories.DateTime;
using System.Globalization;

namespace Arebis.Testing.Factories
{
	public class TestDateTimeFactory : BaseDateTimeFactory
	{
		private DateTime seedTime;
		private int ticksLastCall;
		private double acceleration;
		private TimeSpan increment;
		private TimeZone currentTimeZone;

		public TestDateTimeFactory(DateTime seedTime)
			: this(seedTime, 0.0, TimeSpan.Zero)
		{ 
		}

		public TestDateTimeFactory(DateTime seedTime, double acceleration)
			: this(seedTime, acceleration, TimeSpan.Zero)
		{
		}

		public TestDateTimeFactory(DateTime seedTime, TimeSpan increment)
			: this(seedTime, 0.0, increment)
		{
		}

		private TestDateTimeFactory(DateTime seedTime, double acceleration, TimeSpan increment)
		{
			this.seedTime = seedTime.Add(new TimeSpan(-increment.Ticks)).ToUniversalTime();
			this.ticksLastCall = 0;
			this.acceleration = acceleration;
			this.increment = increment;
			this.currentTimeZone = TimeZone.CurrentTimeZone;
		}

		public override DateTime Now
		{
			get 
			{
				// Calculate new seedtime:
				int currentTicks = Environment.TickCount;
				if (ticksLastCall == 0) ticksLastCall = currentTicks;
				this.seedTime = this.seedTime.Add(increment).Add(new TimeSpan((long)((currentTicks - ticksLastCall) * acceleration * TimeSpan.TicksPerMillisecond)));
				// Adapt status for next call:
				ticksLastCall = currentTicks;
				// Return result:
				return this.seedTime.ToLocalTime();
			}
		}

		public override TimeZone CurrentTimeZone
		{
			get { return currentTimeZone; }
		}

		public void SetCurrentTimeZone(TimeZone currentTimeZone)
		{
			this.currentTimeZone = currentTimeZone;
		}
	}
}
