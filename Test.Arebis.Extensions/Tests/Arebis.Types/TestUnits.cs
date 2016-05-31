using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Types;

namespace Arebis.Extensions.Tests.Arebis.Types
{
	[UnitDefinitionClass]
	public static class MonetaryUnits
	{
		public static Unit Euro = new Unit("euro", "€", new UnitType("Currency"));
		public static Unit KEuro = new Unit("Keuro", "K€", 1000m, Euro);
	}
}
