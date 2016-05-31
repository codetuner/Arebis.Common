using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Types
{
	[Serializable]
	public sealed class Unit : IComparable, IComparable<Unit>, IEquatable<Unit>, IFormattable
	{
		private static Unit none = new Unit(String.Empty, String.Empty, UnitType.None);

		private string name;
		private string symbol;
		private decimal factor;
		private UnitType unitType;
		private bool isNamed;

		#region Constructor methods

		public Unit(string name, string symbol, UnitType unitType)
			: this(name, symbol, 1m, unitType, true)
		{
		}

		public Unit(string name, string symbol, decimal factor, Unit baseUnit)
			: this(name.Replace("~", baseUnit.name), symbol.Replace("~", baseUnit.symbol), factor * baseUnit.factor, baseUnit.unitType, true)
		{
		}

		private Unit(string name, string symbol, decimal factor, UnitType unitType, bool isNamed)
		{
			this.name = name;
			this.symbol = symbol;
			this.factor = factor;
			this.unitType = unitType;
			this.isNamed = isNamed;
		}

		public static Unit None
		{
			get { return Unit.none; }
		}

		#endregion Constructor methods

		#region Public implementation

		/// <summary>
		/// The name of the unit.
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// The symbol of the unit.
		/// </summary>
		public string Symbol
		{
			get { return this.symbol; }
		}

		/// <summary>
		/// The factor of the unit.
		/// </summary>
		public decimal Factor
		{
			get { return this.factor; }
		}

		/// <summary>
		/// Whether the unit is named.
		/// </summary>
		public bool IsNamed
		{
			get { return this.isNamed; }
		}

		/// <summary>
		/// Type of the unit.
		/// </summary>
		public UnitType UnitType
		{
			get { return this.unitType; }
		}

		/// <summary>
		/// Checks whether the given unit is compatible to this one.
		/// Raises an exception if not compatible.
		/// </summary>
		/// <exception cref="UnitConversionException">Raised when units are not compatible.</exception>
		public void AssertCompatibility(Unit compatibleUnit)
		{
			if (!this.CompatibleTo(compatibleUnit)) throw new UnitConversionException(this, compatibleUnit);
		}

		/// <summary>
		/// Checks whether the passed unit is compatible with this one.
		/// </summary>
		public bool CompatibleTo(Unit otherUnit)
		{
			return (this.unitType == (otherUnit ?? Unit.none).unitType);
		}

		/// <summary>
		/// Returns a unit by raising the present unit to the specified power.
		/// I.e. meter.Power(3) would return a cubic meter unit.
		/// </summary>
		public Unit Power(int power)
		{
			return new Unit(String.Concat('(', this.name, '^', power, ')'), this.symbol + '^' + power, Unit.DecimalPower(this.factor, power), this.unitType.Power(power), false);
		}

		/// <summary>
		/// Tests equality of both objects.
		/// </summary>
		public override bool Equals(object obj)
		{
			return (this == (obj as Unit));
		}

		/// <summary>
		/// Tests equality of both objects.
		/// </summary>
		public bool Equals(Unit unit)
		{
			return (this == unit);
		}

		/// <summary>
		/// Returns the hashcode of this unit.
		/// </summary>
		public override int GetHashCode()
		{
			return this.factor.GetHashCode() ^ this.unitType.GetHashCode();
		}

		/// <summary>
		/// Returns a string representation of the unit.
		/// </summary>
		public override string ToString()
		{
			return this.ToString(null, null);
		}

		/// <summary>
		/// Returns a string representation of the unit.
		/// </summary>
		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		/// <summary>
		/// Returns a string representation of the unit.
		/// </summary>
		public string ToString(IFormatProvider formatProvider)
		{
			return this.ToString(null, formatProvider);
		}

		/// <summary>
		/// Returns a string representation of the unit.
		/// </summary>
		/// <remarks>
		/// The format string can be either 'N' (Name) or 'S' (Symbol).
		/// </remarks>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == null) format = "S";

			if (formatProvider != null)
			{
				ICustomFormatter formatter = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
				if (formatter != null)
				{
					return formatter.Format(format, this, formatProvider);
				}
			}

			switch (format)
			{
				case "N":
					return this.Name;
				case "S":
				default:
					return this.Symbol;
			}
		}

		#endregion Public implementation

		#region Operator overloads

		public static bool operator ==(Unit left, Unit right)
		{
			// Special cases:
			if (Object.ReferenceEquals(left, right))
				return true;

			// Compare content:
			left = left ?? Unit.none;
			right = right ?? Unit.none;
			return (left.factor == right.factor) && (left.unitType == right.unitType);
		}

		public static bool operator !=(Unit left, Unit right)
		{
			return !(left == right);
		}

		public static Unit operator *(Unit left, Unit right)
		{
			left = left ?? Unit.none;
			right = right ?? Unit.none;
			return new Unit(String.Concat('(', left.name, '*', right.name, ')'), left.symbol+'*'+right.symbol, left.factor * right.factor, left.unitType * right.unitType, false);
		}

		public static Unit operator *(Unit left, decimal right)
		{
			return (right * left);
		}

		public static Unit operator *(decimal left, Unit right)
		{
			right = right ?? Unit.none;
			return new Unit(String.Concat('(', left.ToString(), '*', right.name, ')'), left.ToString() + '*' + right.symbol, left * right.factor, right.unitType, false);
		}

		public static Unit operator /(Unit left, Unit right)
		{
			left = left ?? Unit.none;
			right = right ?? Unit.none;
			return new Unit(String.Concat('(', left.name, '/', right.name, ')'), left.symbol + '/' + right.symbol, left.factor / right.factor, left.unitType / right.unitType, false);
		}

		public static Unit operator /(decimal left, Unit right)
		{
			right = right ?? Unit.none;
			return new Unit(String.Concat('(', left.ToString(), '*', right.name, ')'), left.ToString() + '*' + right.symbol, left / right.factor, right.unitType.Power(-1), false);
		}

		public static Unit operator /(Unit left, decimal right)
		{
			left = left ?? Unit.none;
			return new Unit(String.Concat('(', left.name, '/', right.ToString(), ')'), left.symbol + '/' + right.ToString(), left.factor / right, left.unitType, false);
		}

		#endregion Operator overloads

		#region IComparable implementation

		/// <summary>
		/// Compares the passed unit to the current one. Allows sorting units of the same type.
		/// </summary>
		/// <remarks>Only compatible units can be compared.</remarks>
		int IComparable.CompareTo(object obj)
		{
			return ((IComparable<Unit>)this).CompareTo((Unit)obj);
		}

		/// <summary>
		/// Compares the passed unit to the current one. Allows sorting units of the same type.
		/// </summary>
		/// <remarks>Only compatible units can be compared.</remarks>
		int IComparable<Unit>.CompareTo(Unit other)
		{
			this.AssertCompatibility(other);
			if (this.factor < other.factor) return -1;
			else if (this.factor > other.factor) return +1;
			else return 0;
		}

		#endregion IComparable implementation

		#region Additional helper methods

		/// <summary>
		/// Returns the specified value raised to the specified power.
		/// </summary>
		/// <remarks>
		/// Math.Pow is not usable sint it only works with doubles causing
		/// rouding errors with decimals.
		/// </remarks>
		internal static decimal DecimalPower(decimal value, int power)
		{
			// Enhance performance for most common cases:
			switch (power)
			{
				case 0:
					return 1m;
				case 1:
					return value;
				case -1:
					return 1m / value;
			}

			// General implementation:
			if (power >= 0)
			{
				decimal result = 1m;
				for (int p = 0; p < power; p++)
					result *= value;
				return result;
			}
			else
			{
				return 1m / DecimalPower(value, -power);
			}
		}

		#endregion
	}
}
