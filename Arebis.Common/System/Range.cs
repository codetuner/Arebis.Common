using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
	/// <summary>
	/// Abstract representation of an inclusive range.
	/// </summary>
	public abstract class AbstractRange<T>
	{
		private T lowerBound;
		private T upperBound;

		/// <summary>
		/// Create a range given two boundaries in random order.
		/// </summary>
		public AbstractRange(T boundary1, T boundary2)
		{
			// Make sure lowerBound is lowest of both, and upperBound is highest:
			this.lowerBound = this.Min(boundary1, boundary2);
			this.upperBound = this.Max(boundary1, boundary2);
		}

		/// <summary>
		/// The lower boundary of the range.
		/// </summary>
		public virtual T LowerBound
		{
			get { return this.lowerBound; }
		}

		/// <summary>
		/// The upper boundary of the range.
		/// </summary>
		public virtual T UpperBound
		{
			get { return this.upperBound; }
		}

		/// <summary>
		/// Checks whether this range includes the given value.
		/// </summary>
		public virtual bool Includes(T value)
		{
			return ((this.CompareTo(this.LowerBound, value) <= 0) 
				&& (this.CompareTo(this.UpperBound, value) >= 0));
		}

		/// <summary>
		/// Checks whether this range includes the given range.
		/// </summary>
		public virtual bool Includes(AbstractRange<T> range)
		{
			return ((this.Includes(range.LowerBound)) && (this.Includes(range.UpperBound)));
		}

		/// <summary>
		/// Checks whether this range partially or totally overlaps the given range.
		/// </summary>
		public virtual bool Overlaps(AbstractRange<T> range)
		{
			return ((this.CompareTo(this.LowerBound, range.UpperBound) <= 0) 
				&& (this.CompareTo(this.UpperBound, range.LowerBound) >= 0));
		}

		/// <summary>
		/// Returns the union of this range with the given range. If fillGap is true,
		/// an eventual gap between both ranges will be filled. If fillGap is false,
		/// and there exist a gap between the two ranges, an ArgumentException is
		/// thrown.
		/// </summary>
		public virtual R Union<R>(R range, bool fillGap) where R : AbstractRange<T>
		{
			// Check for gaps:
			if ((fillGap == false) && (this.Overlaps(range) == false))
			{
				throw new ArgumentException("Cannot calculate union of ranges when a gap exists and fillGap is set to false.");
			}

			// Create a new instance representing the union:
			try
			{
				return (R)Activator.CreateInstance(this.GetType(), this.Min(this.LowerBound, range.LowerBound), this.Max(this.UpperBound, range.UpperBound));
			}
			catch (MissingMethodException)
			{
				throw new ApplicationException("This range type should have a constructor taking two boundaries.");
			}
		}

		/// <summary>
		/// Returns a new range being the intersection between this range and the given range.
		/// Returns null if no intersection exists.
		/// </summary>
		public virtual R Intersection<R>(R range) where R : AbstractRange<T>
		{
			if (this.Overlaps(range))
			{
				// Create a new instance representing the intersection:
				try
				{
					return (R)Activator.CreateInstance(this.GetType(), this.Max(this.LowerBound, range.LowerBound), this.Min(this.UpperBound, range.UpperBound));
				}
				catch (MissingMethodException)
				{
					throw new ApplicationException("This range type should have a constructor taking two boundaries.");
				}
			}
			else
			{
				// If not overlaps, return null:
				return null;
			}
		}

		/// <summary>
		/// Tests whether both ranges are equal (have same boundaries).
		/// </summary>
		public override bool Equals(object obj)
		{
			AbstractRange<T> range = (obj as AbstractRange<T>);
			if (range == null) return false;
			return ((this.CompareTo(this.LowerBound, range.LowerBound) == 0)
				&& (this.CompareTo(this.UpperBound, range.UpperBound) == 0));
		}

		/// <summary>
		/// Returns a hascode for this range.
		/// </summary>
		public override int GetHashCode()
		{
			return this.LowerBound.GetHashCode() ^ this.UpperBound.GetHashCode();
		}

		/// <summary>
		/// Represents the range as string.
		/// </summary>
		public override string ToString()
		{
			return String.Format("[{0} … {1}]", this.LowerBound.ToString(), this.UpperBound.ToString());
		}

		/// <summary>
		/// Compares two values. 
		/// If left is less than right, return a strict negative value,
		/// if left is greather than right, return a strict positive value,
		/// if left and right are equal, return 0.
		/// </summary>
		protected abstract int CompareTo(T left, T right);

		/// <summary>
		/// Returns the highest of both values.
		/// </summary>
		private T Max(T left, T right)
		{
			return (this.CompareTo(left, right) >= 0) ? left : right;
		}

		/// <summary>
		/// Returns the lowest of both values.
		/// </summary>
		private T Min(T left, T right)
		{
			return (this.CompareTo(left, right) < 0) ? left : right;
		}
	}


	/// <summary>
	/// Represents an inclusive range of IComparables.
	/// </summary>
	public class Range<T> : AbstractRange<T> where T : IComparable
	{
		/// <summary>
		/// Constructs a new Range given two boundaries in random order.
		/// </summary>
		public Range(T boundary1, T boundary2) : base(boundary1, boundary2) { }

		/// <summary>
		/// Compares two values by using the IComparable interface.
		/// </summary>
		protected override int CompareTo(T left, T right)
		{
			return left.CompareTo(right);
		}
	}
}
