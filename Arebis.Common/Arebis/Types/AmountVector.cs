using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Arebis.Types
{
	/// <summary>
	/// An AmountVector represents a serie of amounts expressed all in the same
	/// unit. It can be compared with an array of amounts. AmountVectors support
	/// calculations as +, -, * and / between vectors and amounts.
	/// </summary>
	[Serializable]
	public class AmountVector : ICloneable, IDeserializationCallback, INotifyListChanged, IUnitConsumer
	{
		private int length;
		private Unit unit;
		private decimal?[] values;
		[NonSerialized] private Amount[] amounts;

		/// <summary>
		/// Initializes an AmountVector with given length and of given unit.
		/// </summary>
		public AmountVector(int length, Unit unit)
		{
			this.length = length;
			this.unit = unit;
			this.values = new decimal?[length];
			this.amounts = new Amount[length];
		}

		/// <summary>
		/// Initializes an AmountVector with given length and of the values unit,
		/// where all values are equal to the given amount.
		/// </summary>
		public AmountVector(int length, Amount amount)
		{
			this.length = length;
			this.unit = amount.Unit;
			this.values = new decimal?[length];
			this.amounts = new Amount[length];
			decimal? d = amount.Value;
			for (int i = 0; i < length; i++)
			{
				this.values[i] = d;
				this.amounts[i] = amount;
			}
		}

		/// <summary>
		/// Constructs a new AmountVector of the given unit containing the given values.
		/// </summary>
		public AmountVector(decimal?[] values, Unit unit)
		{
			this.length = values.Length;
			this.unit = unit;
			this.values = (decimal?[])values.Clone();
			this.amounts = new Amount[this.length];
			for (int i = 0; i < this.length; i++)
			{
				if (this.values[i].HasValue) this.amounts[i] = new Amount(this.values[i].Value, unit);
			}
		}

		/// <summary>
		/// Implements IClonable
		/// </summary>
		public object Clone()
		{
			AmountVector result = new AmountVector(this.length, this.unit);
			result.values = (decimal?[])this.values.Clone();
			result.amounts = (Amount[])this.amounts.Clone();
			return result;
		}

		/// <summary>
		/// Creates an AmountVector with given length and of given unit,
		/// where all values are zero.
		/// </summary>
		public static AmountVector AllZeros(int length, Unit unit)
		{
			AmountVector v = new AmountVector(length, unit);
			Amount zero = new Amount(0m, unit);
			for (int i = 0; i < length; i++)
			{
				v.values[i] = 0m;
				v.amounts[i] = zero;
			}
			return v;
		}

		/// <summary>
		/// The length of this vector.
		/// </summary>
		public int Length
		{
			get { return this.length; }
		}

		/// <summary>
		/// Returns the number of non-null values in this vector.
		/// </summary>
		public int LengthNonNulls
		{
			get
			{
				int c = 0;
				foreach (Amount a in this.NonNulls()) c++;
				return c;
			}
		}

		/// <summary>
		/// The unit this vector is expressed in.
		/// To change the unit, use a ConvertedTo() method.
		/// </summary>
		public Unit Unit
		{
			get { return this.unit; }
		}

		/// <summary>
		/// Get or set a specific amount.
		/// </summary>
		public Amount this[int index]
		{
			get
			{
				if (this.values[index].HasValue == false)
				{
					return null;
				}
				else if (this.amounts[index] == null)
				{
					this.amounts[index] = new Amount(this.values[index].Value, this.unit);
				}
				return this.amounts[index];
			}
			set
			{
				if (value == null)
				{
					this.amounts[index] = null;
					this.values[index] = null;
				}
				else
				{
					this.amounts[index] = value.ConvertedTo(this.unit);
					this.values[index] = this.amounts[index].Value;
				}
				this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
			}
		}

		/// <summary>
		/// Enumerate all values.
		/// </summary>
		public IEnumerable<Amount> All()
		{
			for (int i = 0; i < this.length; i++)
			{
				yield return this[i];
			}
		}

		/// <summary>
		/// Enumerate all non-null values.
		/// </summary>
		public IEnumerable<Amount> NonNulls()
		{
			for (int i = 0; i < this.length; i++)
			{
				Amount a = this[i];
				if (a != null) yield return a;
			}
		}

		/// <summary>
		/// Returns the first non-null amount.
		/// Returns null if no non-null amount is available.
		/// </summary>
		public Amount FirstNonNull
		{
			get 
			{
				foreach (Amount amount in this.NonNulls())
				{
					return amount;
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the last non-null amount.
		/// Returns null if no non-null amount is available.
		/// </summary>
		public Amount LastNonNull
		{
			get 
			{
				for (int i = this.length - 1; i >= 0; i--)
				{
					Amount amount = this[i];
					if (amount != null) return amount;
				}
				return null;
			}
		}

		/// <summary>
		/// The sum of all values of the vector.
		/// </summary>
		public Amount Sum
		{
			get
			{
				decimal sum = 0m;
				foreach (decimal? d in this.values)
				{
					if (d.HasValue) sum += d.Value;
				}
				return new Amount(sum, this.unit);
			}
		}

		/// <summary>
		/// The average of all values of the vector.
		/// </summary>
		public Amount Average
		{
			get
			{
				decimal sum = 0m;
				int count = 0;
				foreach (decimal? d in this.values)
				{
					if (d.HasValue)
					{
						sum += d.Value;
						count++;
					}
				}
				return new Amount(sum / count, this.unit);
			}
		}

		/// <summary>
		/// Returns the highest amount in the vector.
		/// </summary>
		public Amount Max
		{
			get
			{
				Amount result = null;
				foreach (Amount a in this.NonNulls())
				{
					if (result == null) result = a;
					else if (result < a) result = a;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns the lowest amount in the vector.
		/// </summary>
		public Amount Min
		{
			get
			{
				Amount result = null;
				foreach (Amount a in this.NonNulls())
				{
					if (result == null) result = a;
					else if (a < result) result = a;
				}
				return result;
			}
		}

		/// <summary>
		/// Returns an AmountVector being the current vector with its values converted
		/// to the given unit.
		/// </summary>
		public AmountVector ConvertedTo(Unit targetUnit)
		{
			AmountVector result = new AmountVector(this.length, targetUnit);
			for (int i = 0; i < this.length; i++)
			{
				if (this.values[i].HasValue)
				{
					Amount a = this[i].ConvertedTo(targetUnit);
					result[i] = a;
				}
			}
			return result;
		}

		/// <summary>
		/// Returns an AmountVector being the current vector with its values converted
		/// to the given unit and rounded to the given number of decimals.
		/// </summary>
		public AmountVector ConvertedTo(Unit targetUnit, int decimals)
		{
			AmountVector result = new AmountVector(this.length, targetUnit);
			for (int i = 0; i < this.length; i++)
			{
				if (this.values[i].HasValue)
				{
					Amount a = this[i].ConvertedTo(targetUnit, decimals);
					result[i] = a;
				}
			}
			return result;
		}

		/// <summary>
		/// Returns an AmountVector being the current vector with its values rounded 
		/// to the given number of decimals.
		/// </summary>
		public AmountVector Round(int decimals)
		{
			AmountVector result = new AmountVector(this.length, this.unit);
			for (int i = 0; i < this.length; i++)
			{
				if (this.values[i].HasValue)
				{
					result[i] = this[i].ConvertedTo(this.unit, decimals);
				}
			}
			return result;
		}

		/// <summary>
		/// Convert the vector to an array of values.
		/// </summary>
		public Amount[] ToArray()
		{
			Amount[] result = new Amount[this.Length];
			for (int i = 0; i < this.length; i++)
			{
				result[i] = this[i];
			}
			return result;
		}

		/// <summary>
		/// Returns a string representation of the vector.
		/// </summary>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append('[');
			sb.Append((this.values[0].HasValue) ? this.values[0].Value.ToString("0.####") : "-");
			for (int i = 1; i < this.length; i++) {
				sb.Append("; ");
				sb.Append((this.values[i].HasValue) ? this.values[i].Value.ToString("0.####") : "-");
			}
			sb.Append(']');
			sb.Append(' ');
			sb.Append(this.Unit.Symbol);

			return sb.ToString();
		}

		#region INotifyListChanged Members

		/// <summary>
		/// Occurs when an amount in the vector changes.
		/// </summary>
		public event ListChangedEventHandler ListChanged;

		/// <summary>
		/// Raises the INotifyListChanged.ListChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if (this.ListChanged != null) this.ListChanged(this, e);
		}

		#endregion

		#region Operators

		/// <summary>
		/// Additions two vectors of equal length and compatible unit.
		/// </summary>
		public static AmountVector operator +(AmountVector left, AmountVector right)
		{
			if (left.length != right.length) throw new InvalidOperationException("Can not add or substract AmountVectors of different length.");
			if (left.unit.CompatibleTo(right.unit) == false) throw new UnitConversionException("Can not add or substract AmountVectors of incompatible units.");
			AmountVector result = new AmountVector(left.length, left.unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					result[i] = left[i];
					if (right[i] != null) result[i] += right[i];
				}
				else if (right[i] != null) result[i] = right[i];
			}
			return result;
		}

		/// <summary>
		/// Substracts two vectors of equal length and compatible unit.
		/// </summary>
		public static AmountVector operator -(AmountVector left, AmountVector right)
		{
			if (left.length != right.length) throw new InvalidOperationException("Can not add or substract AmountVectors of different length.");
			if (left.unit.CompatibleTo(right.unit) == false) throw new UnitConversionException("Can not add or substract AmountVectors of incompatible units.");
			AmountVector result = new AmountVector(left.length, left.unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					result[i] = left[i];
					if (right[i] != null) result[i] -= right[i];
				}
				else if (right[i] != null) result[i] = -right[i];
			}
			return result;
		}

		/// <summary>
		/// Multiplies two vectors of equal length.
		/// </summary>
		public static AmountVector operator *(AmountVector left, AmountVector right)
		{
			if (left.length != right.length) throw new InvalidOperationException("Can not multiply or divide AmountVectors of different length.");
			AmountVector result = new AmountVector(left.length, left.unit * right.Unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					if (right[i] != null)
					{
						result[i] = new Amount(left.values[i].Value * right.values[i].Value, result.unit);
					}
					else
					{
						result[i] = null;
					}
				}
				else 
				{
					result[i] = null;
				}
			}
			return result;
		}

		/// <summary>
		/// Multiplies each value of a vector with an amount.
		/// </summary>
		public static AmountVector operator *(AmountVector left, Amount right)
		{
			AmountVector result = new AmountVector(left.length, left.unit * right.Unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					result[i] = new Amount(left.values[i].Value * right.Value, result.unit);
				}
			}
			return result;
		}

		/// <summary>
		/// Divides two vectors of equal length.
		/// </summary>
		public static AmountVector operator /(AmountVector left, AmountVector right)
		{
			if (left.length != right.length) throw new InvalidOperationException("Can not multiply or divide AmountVectors of different length.");
			AmountVector result = new AmountVector(left.length, left.unit / right.Unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					if (right[i] != null)
					{
						result[i] = new Amount(left.values[i].Value / right.values[i].Value, result.unit);
					}
					else
					{
						result[i] = null;
					}
				}
				else
				{
					result[i] = null;
				}
			}
			return result;
		}

		/// <summary>
		/// Divides each value of a vector with an amount.
		/// </summary>
		public static AmountVector operator /(AmountVector left, Amount right)
		{
			AmountVector result = new AmountVector(left.length, left.unit / right.Unit);
			for (int i = 0; i < left.length; i++)
			{
				if (left[i] != null)
				{
					result[i] = new Amount(left.values[i].Value / right.Value, result.unit);
				}
			}
			return result;
		}
		
		#endregion Operators

		#region Serialization/Deserialization

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			// Restore amounts field:
			this.amounts = new Amount[this.length];
			for (int i = 0; i < this.length; i++)
			{
				if (this.values[i].HasValue)
					this.amounts[i] = new Amount(this.values[i].Value, this.unit);
			}
		}

		#endregion Serialization/Deserialization
	}
}
