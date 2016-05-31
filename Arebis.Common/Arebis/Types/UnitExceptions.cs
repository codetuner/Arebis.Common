using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Types
{
	/// <summary>
	/// Exception thrown when a unit conversion failed, i.e. because you are converting
	/// amounts from one unit into another non-compatible unit.
	/// </summary>
	[Serializable]
	public class UnitConversionException : InvalidOperationException
	{
		public UnitConversionException() : base() { }

		public UnitConversionException(string message) : base(message) { }

		public UnitConversionException(Unit fromUnit, Unit toUnit) : this(String.Format("Failed to convert from unit '{0}' to unit '{1}'. Units are not compatible and no conversions are defined.", fromUnit.Name, toUnit.Name)) { }

		protected UnitConversionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}

	/// <summary>
	/// Exception thrown whenever an exception is referenced by name, but no
	/// unit with the given name is known (registered to the UnitManager).
	/// </summary>
	[Serializable]
	public class UnknownUnitException : ApplicationException
	{

		public UnknownUnitException() : base() { }

		public UnknownUnitException(string message) : base(message) { }

		protected UnknownUnitException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
