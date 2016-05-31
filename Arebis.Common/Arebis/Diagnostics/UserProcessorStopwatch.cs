using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Arebis.Diagnostics
{
	/// <summary>
	/// A stopwatch implementation that measures time in terms of the User CPU time
	/// consumption by the current process.
	/// </summary>
	public sealed class UserProcessorStopwatch
	{
		private bool isRunning = false;
		private TimeSpan elapsedTime = new TimeSpan(0);
		private long tickCountAtStart;

		/// <summary>
		/// Resets the stopwatch to 0.
		/// </summary>
		/// <returns></returns>
		public UserProcessorStopwatch Reset()
		{
			this.isRunning = false;
			this.elapsedTime = new TimeSpan(0);
			return this;
		}

		/// <summary>
		/// Starts the stopwatch. First call Reset() to restart the stopwatch at 0.
		/// </summary>
		public UserProcessorStopwatch Start()
		{
			if (this.isRunning == true) throw new InvalidOperationException("Stopwatch is already started.");

			this.isRunning = true;
			this.tickCountAtStart = (long)Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds;
			return this;
		}

		/// <summary>
		/// Stops the stopwatch.
		/// </summary>
		public UserProcessorStopwatch Stop()
		{
			if (this.isRunning == false) throw new InvalidOperationException("Stopwatch is not started.");

			this.elapsedTime = this.ElapsedTime;
			this.isRunning = false;

			return this;
		}

		/// <summary>
		/// Whether the stopwatch is currently running.
		/// </summary>
		public bool IsRunning
		{
			get { return this.isRunning; }
		}

		/// <summary>
		/// Returns the time elapsed between the start of the stopwatch and
		/// now, or the stop of the stopwatch.
		/// </summary>
		public TimeSpan ElapsedTime
		{
			get 
			{
				TimeSpan result = this.elapsedTime;
				if (this.isRunning)
					result = new TimeSpan(result.Ticks + ((long)Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds - this.tickCountAtStart) * TimeSpan.TicksPerMillisecond);
				return result;
			}
		}

	}
}
