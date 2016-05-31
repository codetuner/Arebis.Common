using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Aspects;
using System.Globalization;
using System.Threading;

namespace Arebis.Testing
{
	public class CultureAdviceAttribute : AdviceAttribute
	{
		private CultureInfo culture = null;

		public CultureAdviceAttribute(string cultureName)
			: base(false)
		{
			this.culture = new CultureInfo(cultureName);
		}

		public override void BeforeCall(ICallContext callContext)
		{
			callContext.SetProperty("CurrentCulture", Thread.CurrentThread.CurrentCulture);
			Thread.CurrentThread.CurrentCulture = this.culture;
		}

		public override void AfterCall(ICallContext callContext)
		{
			Thread.CurrentThread.CurrentCulture = (CultureInfo)callContext.GetProperty("CurrentCulture");
		}
	}
}
