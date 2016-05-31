using System;
using Arebis.Mocking;

namespace Arebis.Extensions.Tests.Arebis.Testing.Mocking.Sample
{

	internal class CurrencyServiceMocker : IMocker
	{
		#region IMocker Members

		public void HandleCall(MockingProxy proxy, MockableCall call)
		{
			if (call.IsConstructorCall)
			{
				call.SetConstructionResult("CurrencyMock");
			}
			else
			{
				if (call.Method.Name.Equals("ConvertAmount"))
				{
					// Retrieve call args:
					decimal amount = (decimal)call.InArgs[0];
					// Mock the call:
					call.SetCallResult(amount);
				}
				else
				{
					// Return from a void call:
					call.SetCallResult();
				}
			}
		}

		#endregion
	}
}
