using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Types
{
	/// <summary>
	/// This interface represents a consumer of a unit, such as an Amount or an AmountVector.
	/// </summary>
	public interface IUnitConsumer
	{
		/// <summary>
		/// The unit of the consumer.
		/// </summary>
		Unit Unit { get; }
	}
}
