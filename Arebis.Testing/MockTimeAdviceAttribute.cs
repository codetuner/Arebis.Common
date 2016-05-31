using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;
using System.Factories.DateTime;
using Arebis.Testing.Factories;
using System.Globalization;

namespace Arebis.Testing
{
    public class MockTimeAdviceAttribute : AdviceAttribute
    {
        private IDateTimeFactory mock;

        /// <summary>
        /// Mocks time on a fixed date/time.
        /// </summary>
        /// <param name="dateTime">Provide date/time in format "yyyy/MM/dd HH:mm:ss"</param>
        public MockTimeAdviceAttribute(string dateTime)
            : base(false)
        {
            this.mock = new TestDateTimeFactory(DateTime.Parse(dateTime, CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Mocks time on a date/time advancing with real-time acceleration (x1 would be real-time, x2 would be double speed, x0.5 would be half speed).
        /// </summary>
        /// <param name="dateTime">Provide date/time in format "yyyy/MM/dd HH:mm:ss"</param>
        /// <param name="acceleration">Acceleration as 1 (x1=realtime), 0.5 (halfspeed), 2 (doublespeed), etc.</param>
        public MockTimeAdviceAttribute(string dateTime, double acceleration)
            : base(false)
        {
            this.mock = new TestDateTimeFactory(DateTime.Parse(dateTime, CultureInfo.InvariantCulture), acceleration);
        }

        /// <summary>
        /// Mocks time on a date/time with fixed increment on each date/time request.
        /// </summary>
        /// <param name="dateTime">Provide date/time in format "yyyy/MM/dd HH:mm:ss"</param>
        /// <param name="increment">Provide time increment between date/time requests in format "dd.hh:mm:ss.mmm"</param>
        public MockTimeAdviceAttribute(string dateTime, string increment)
            : base(false)
        {
            this.mock = new TestDateTimeFactory(
                DateTime.Parse(dateTime, CultureInfo.InvariantCulture),
                TimeSpan.Parse(increment)
            );
        }

        [System.Diagnostics.DebuggerHidden]
        public override void BeforeCall(ICallContext callContext)
        {
            callContext.SetProperty("datetimefactory", System.Current.DateTime);
            System.Current.DateTime = mock;
        }

        [System.Diagnostics.DebuggerHidden]
        public override void AfterCall(ICallContext callContext)
        {
            System.Current.DateTime = (IDateTimeFactory)callContext.GetProperty("datetimefactory", null);
        }
    }
}
