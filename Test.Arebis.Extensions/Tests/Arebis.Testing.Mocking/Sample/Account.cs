using System;
using Arebis.Mocking;
using Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{
	[AutoMock]
	internal class Account : ContextBoundObject
	{
		private CurrencyUnit unit;
		private decimal balance = 0m;

		public Account(CurrencyUnit unit) {
			this.unit = unit;
		}

		public decimal Balance { 
			get { return this.balance; } 
		}

		public bool IsBalancePositive { 
			get { return (this.balance >= 0m); } 
		}

		public void Deposit(decimal amount) {
			this.balance += amount;
		}

		public void Withdraw(decimal amount) {
			this.balance -= amount;
		}

		public void SwitchCurrency(CurrencyUnit unit) {
			ICurrencyService srv = CurrencyServiceFactory.NextInstance;
			this.balance = srv.ConvertAmount(this.balance, this.unit, unit);
			this.unit = unit;
		}
	}
}
