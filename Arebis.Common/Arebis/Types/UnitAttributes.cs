using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Types
{
	/// <summary>
	/// Attribute to mark classes having static unit fields to be registered
	/// by the UnitManager's RegisterUnits method.
	/// </summary>
	/// <see cref="UnitManager.RegisterUnits(System.Reflection.Assembly)"/>
	[AttributeUsage(AttributeTargets.Class)]
	public class UnitDefinitionClassAttribute : Attribute
	{
	}

	/// <summary>
	/// Attribute to mark classes having static methods that register
	/// conversion functions. The UnitConvert class uses this attribute to
	/// identify classes with unit conversion methods in its RegisterConversions
	/// method.
	/// </summary>
	/// <see cref="UnitManager.RegisterConversions(System.Reflection.Assembly)"/>
	[AttributeUsage(AttributeTargets.Class)]
	public class UnitConversionClassAttribute : Attribute
	{
	}
}
