using System;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{
	[Serializable]
	internal class CurrencyUnit
	{

		public static CurrencyUnit EUR = new CurrencyUnit("EUR", 1m);
		public static CurrencyUnit USD = new CurrencyUnit("USD", 1.2m);
		public static CurrencyUnit GBP = new CurrencyUnit("GBP", 0.7m);
		public static CurrencyUnit NULL = new CurrencyUnit("NULL", 0m);

		private string symbol;
		private decimal rate;

		public CurrencyUnit(string symbol, decimal rate)
		{
			this.symbol = symbol;
			this.rate = rate;
		}

		public string Symbol
		{
			get { return this.symbol; }
		}

		public decimal Rate
		{
			get { return this.rate; }
		}
	}
}
