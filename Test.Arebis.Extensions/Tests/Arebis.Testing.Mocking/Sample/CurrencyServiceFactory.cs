using System;
using Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{
	internal sealed class CurrencyServiceFactory
	{
		private static ICurrencyService nextInstance;

		public static ICurrencyService NextInstance
		{
			get
			{
				if (nextInstance == null)
				{
					nextInstance = new CustomizableCurrencyService();
				}
				return nextInstance;
			}
			set
			{
				nextInstance = value;
			}
		}
	}
}
