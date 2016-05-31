using System;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{

	/// <summary>
	/// A CurrencyService implementation is not mockable byt implements an interface.
	/// </summary>
	internal class PlainCurrencyService : ICurrencyService
	{

		private static bool isInstanceCreated = false;

		public static void WatchInstanceCreation()
		{
			isInstanceCreated = false;
		}

		public static bool IsInstanceCreated
		{
			get { return isInstanceCreated; }
		}

		public PlainCurrencyService()
		{
			isInstanceCreated = true;
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
