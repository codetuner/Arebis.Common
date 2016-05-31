using System;
using System.Collections.Generic;
using System.Text;
using Arebis.Runtime.Validation;

namespace Arebis.Types
{
	/// <summary>
	/// Asserts a property of type IUnitConsumer has a value expressed in a unit
	/// compatible to the given one.
	/// </summary>
	/// <remarks>
	/// The named unit must be registered with the UnitManager!
	/// </remarks>
	public class AssertUnitCompatibilityAttribute : PropertyAssertAttribute
	{
		private Unit unit;

		/// <summary>
		/// Asserts a property of type IUnitConsumer has a value expressed in a unit
		/// compatible to the given one.
		/// </summary>
		public AssertUnitCompatibilityAttribute(string unitName)
			: base("Value is expressed in incompatible unit.")
		{
			unit = UnitManager.GetUnitByName(unitName);
		}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public override bool Validate(object value)
		{
			return ((value == null) || ((value as IUnitConsumer).Unit.CompatibleTo(unit)));
		}
	}
}
