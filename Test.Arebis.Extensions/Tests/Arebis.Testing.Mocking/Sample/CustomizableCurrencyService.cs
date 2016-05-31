using System;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{

	/// <summary>
	/// A CurrencyService that has a CustomMock attribute.
	/// </summary>
	[CustomMock(typeof(CurrencyServiceMocker))]
	internal class CustomizableCurrencyService : ContextBoundObject, ICurrencyService
	{
		public CustomizableCurrencyService()
		{
			throw new InvalidOperationException("To test mocking, this instance should never be created.");
		}

		public decimal ConvertAmount(decimal amount, CurrencyUnit from, CurrencyUnit to)
		{
			return amount / from.Rate * to.Rate;
		}

		public decimal GetRate(CurrencyUnit unit)
		{
			return unit.Rate;
		}
	}
}
