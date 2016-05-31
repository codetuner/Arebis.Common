using System;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{
	internal interface ICurrencyService
	{
		decimal ConvertAmount(decimal amount, CurrencyUnit from, CurrencyUnit to);
		decimal GetRate(CurrencyUnit unit);
	}
}
