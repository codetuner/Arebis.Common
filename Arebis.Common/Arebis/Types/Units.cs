using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Definitions and conversion of units have often been taken from:
// http://en.wikipedia.org/wiki/Units_of_measurement

// Disable warning CS1591: Missing XML comment for publicly visible type or member
#pragma warning disable 1591

namespace Arebis.Types
{
	public static class SIUnitTypes
	{
		public static readonly UnitType Length = new UnitType("length");
		public static readonly UnitType Mass = new UnitType("mass");
		public static readonly UnitType Time = new UnitType("time");
		public static readonly UnitType ElectricCurrent = new UnitType("electric current");
		public static readonly UnitType ThermodynamicTemperature = new UnitType("thermodynamic temperature");
		public static readonly UnitType AmountOfSubstance = new UnitType("amount of substance");
		public static readonly UnitType LuminousIntensity = new UnitType("luminous intensity");
	}

	[UnitDefinitionClass]
	public static class LengthUnits
	{
		public static readonly Unit Meter = new Unit("meter", "m", SIUnitTypes.Length);
		public static readonly Unit MilliMeter = new Unit("milli~", "m~", 0.001m, Meter);
		public static readonly Unit CentiMeter = new Unit("centi~", "c~", 0.01m, Meter);
		public static readonly Unit DeciMeter = new Unit("deci~", "d~", 0.1m, Meter);
		public static readonly Unit DecaMeter = new Unit("deca~", "D~", 10m, Meter);
		public static readonly Unit HectoMeter = new Unit("hecto~", "H~", 100m, Meter);
		public static readonly Unit KiloMeter = new Unit("kilo~", "k~", 1000m, Meter);

		public static readonly Unit Inch = new Unit("inch", "in", 0.0254m, Meter);
		public static readonly Unit Foot = new Unit("foot", "ft", 12m, Inch);
		public static readonly Unit Yard = new Unit("yard", "yd", 36m, Inch);
		public static readonly Unit Mile = new Unit("mile", "mi", 5280m, Foot);
		public static readonly Unit NauticalMile = new Unit("nautical mile", "nmi", 1852m, Meter);
	}


	[UnitDefinitionClass]
	public static class SurfaceUnits
	{
		public static readonly Unit Meter2 = new Unit("meter²", "m²", 1m, LengthUnits.Meter.Power(2));
		public static readonly Unit Are = new Unit("are", "are", 100m, Meter2);
		public static readonly Unit HectAre = new Unit("hectare", "ha", 10000m, Meter2);
		public static readonly Unit KiloMeter2 = new Unit("kilometer²", "Km²", 1m, LengthUnits.KiloMeter.Power(2));
	}

	[UnitDefinitionClass]
	public static class VolumeUnits
	{
		public static readonly Unit Liter = new Unit("liter", "L", 1m, LengthUnits.DeciMeter.Power(3));
		public static readonly Unit MilliLiter = new Unit("milli~", "m~", 0.001m, Liter);
		public static readonly Unit CentiLiter = new Unit("centi~", "c~", 0.01m, Liter);
		public static readonly Unit DeciLiter = new Unit("deci~", "d~", 0.1m, Liter);
		public static readonly Unit Meter3 = new Unit("meter³", "m³", 1m, LengthUnits.Meter.Power(3));
	}

	[UnitDefinitionClass]
	public static class TimeUnits
	{
		public static readonly Unit Second = new Unit("second", "s", SIUnitTypes.Time);
		public static readonly Unit MicroSecond = new Unit("micro~", "μ~", 0.000001m, Second);
		public static readonly Unit MilliSecond = new Unit("milli~", "m~", 0.001m, Second);
		public static readonly Unit Minute = new Unit("minute", "min", 60m, Second);
		public static readonly Unit Hour = new Unit("hour", "h", 3600m, Second);
		public static readonly Unit Day = new Unit("day", "d", 24m, Hour);
	}


	[UnitDefinitionClass]
	public static class SpeedUnits
	{
		public static readonly Unit MeterPerSecond = new Unit("meter/second", "m/s", 1m, LengthUnits.Meter / TimeUnits.Second);
		public static readonly Unit KilometerPerHour = new Unit("kilometer/hour", "km/h", 1m, LengthUnits.KiloMeter / TimeUnits.Hour);
		public static readonly Unit MilePerHour = new Unit("mile/hour", "mi/h", 1m, LengthUnits.Mile / TimeUnits.Hour);
		public static readonly Unit Knot = new Unit("knot", "kn", 1.852m, SpeedUnits.KilometerPerHour);
	}

	[UnitDefinitionClass]
	public static class MassUnits
	{
		public static readonly Unit KiloGram = new Unit("kilogram", "Kg", SIUnitTypes.Mass);
		public static readonly Unit Gram = new Unit("gram", "g", 0.001m, KiloGram);
		public static readonly Unit MilliGram = new Unit("milligram", "mg", 0.001m, Gram);
		public static readonly Unit Ton = new Unit("ton", "ton", 1000m, KiloGram);
	}

	[UnitDefinitionClass]
	public static class ForceUnits
	{
		public static readonly Unit Newton = new Unit("newton", "N", 1m, LengthUnits.Meter * MassUnits.KiloGram * TimeUnits.Second.Power(-2));
	}

	[UnitDefinitionClass]
	public static class ElectricUnits
	{
		public static readonly Unit Ampere = new Unit("ampere", "amp", SIUnitTypes.ElectricCurrent);
		public static readonly Unit Coulomb = new Unit("coulomb", "C", 1m, TimeUnits.Second * Ampere);
		public static readonly Unit Volt = new Unit("volt", "V", 1m, EnergyUnits.Watt / Ampere);
		public static readonly Unit Ohm = new Unit("ohm", "Ω", 1m, Volt / Ampere);
		public static readonly Unit Farad = new Unit("farad", "F", 1m, Coulomb / Volt);
	}

	[UnitDefinitionClass]
	public static class EnergyUnits
	{
		public static readonly Unit Joule = new Unit("joule", "J", 1m, LengthUnits.Meter.Power(2) * MassUnits.KiloGram * TimeUnits.Second.Power(-2));
		public static readonly Unit KiloJoule = new Unit("kilo~", "k~", 1000m, Joule);
		public static readonly Unit MegaJoule = new Unit("mega~", "M~", 1000000m, Joule);
		public static readonly Unit GigaJoule = new Unit("giga~", "G~", 1000000000m, Joule);

		public static readonly Unit Watt = new Unit("watt", "W", 1m, Joule / TimeUnits.Second);
		public static readonly Unit KiloWatt = new Unit("kilo~", "k~", 1000m, Watt);
		public static readonly Unit MegaWatt = new Unit("mega~", "M~", 1000000m, Watt);

		public static readonly Unit WattSecond = new Unit("watt-second", "Wsec", 1m, Watt * TimeUnits.Second);
		public static readonly Unit WattHour = new Unit("watt-hour", "Wh", 1m, Watt * TimeUnits.Hour);
		public static readonly Unit KiloWattHour = new Unit("kilo~", "k~", 1000m, WattHour);

		public static readonly Unit Calorie = new Unit("calorie", "cal", 4.1868m, Joule);
		public static readonly Unit KiloCalorie = new Unit("kilo~", "k~", 1000m, Calorie);

		public static readonly Unit HorsePower = new Unit("horsepower", "hp", 0.73549875m, KiloWatt);
	}

	[UnitDefinitionClass, UnitConversionClass]
	public static class TemperatureUnits
	{
		public static readonly Unit DegreeKelvin = new Unit("degree Kelvin", "K", SIUnitTypes.ThermodynamicTemperature);
		public static readonly Unit DegreeCelcius = new Unit("degree celcius", "°C", new UnitType("celcius temperature"));
		public static readonly Unit DegreeFahrenheit = new Unit("degree fahrenheit", "°F", new UnitType("fahrenheit temperature"));

		#region Conversion functions

		public static void RegisterConversions()
		{
			// Register conversion functions:

			// Convert Celcius to Fahrenheit:
			UnitManager.RegisterConversion(DegreeCelcius, DegreeFahrenheit, delegate(Amount amount)
			{
				return new Amount(amount.Value * 9m / 5m + 32m, DegreeFahrenheit);
			}
			);

			// Convert Fahrenheit to Celcius:
			UnitManager.RegisterConversion(DegreeFahrenheit, DegreeCelcius, delegate(Amount amount)
			{
				return new Amount((amount.Value - 32m) / 9m * 5m, DegreeCelcius);
			}
			);

			// Convert Celcius to Kelvin:
			UnitManager.RegisterConversion(DegreeCelcius, DegreeKelvin, delegate(Amount amount)
			{
				return new Amount(amount.Value + 273.15m, DegreeKelvin);
			}
			);

			// Convert Kelvin to Celcius:
			UnitManager.RegisterConversion(DegreeKelvin, DegreeCelcius, delegate(Amount amount)
			{
				return new Amount(amount.Value - 273.15m, DegreeCelcius);
			}
			);

			// Convert Fahrenheit to Kelvin:
			UnitManager.RegisterConversion(DegreeFahrenheit, DegreeKelvin, delegate(Amount amount)
			{
				return amount.ConvertedTo(DegreeCelcius).ConvertedTo(DegreeKelvin);
			}
			);

			// Convert Kelvin to Fahrenheit:
			UnitManager.RegisterConversion(DegreeKelvin, DegreeFahrenheit, delegate(Amount amount)
			{
				return amount.ConvertedTo(DegreeCelcius).ConvertedTo(DegreeFahrenheit);
			}
			);
		}

		#endregion Conversion functions
	}

	[UnitDefinitionClass]
	public static class PressureUnits
	{
		public static readonly Unit Pascal = new Unit("pascal", "Pa", 1m, ForceUnits.Newton * LengthUnits.Meter.Power(-2));
		public static readonly Unit HectoPascal = new Unit("hecto%", "h%", 100m, Pascal);
		public static readonly Unit KiloPascal = new Unit("kilo%", "K%", 1000m, Pascal);
		public static readonly Unit Bar = new Unit("bar", "bar", 100000m, Pascal);
		public static readonly Unit MilliBar = new Unit("milli%", "m%", 0.001m, Bar);
		public static readonly Unit Atmosphere = new Unit("atmosphere", "atm", 101325m, Pascal);
	}

	[UnitDefinitionClass]
	public static class FrequencyUnits
	{
		public static readonly Unit Hertz = new Unit("Hertz", "hz", 1m, TimeUnits.Second.Power(-1));
		public static readonly Unit MegaHerts = new Unit("Mega%", "M%", 1000000m, Hertz);
		public static readonly Unit RPM = new Unit("Rounds per minute", "rpm", 1, TimeUnits.Minute.Power(-1));
	}
}
