using System;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// Provides properties and methods to construct predefined IExpectedValues.
	/// </summary>
	public sealed class ExpectedValue {

		private static IExpectedValue any = new ExpectedValueAny();

		private ExpectedValue() {
			throw new InvalidOperationException("ExpectedValue is not to be instantiated.");
		}

		/// <summary>
		/// Marks the ExpectedValue as to be any value.
		/// </summary>
		public static IExpectedValue Any {
			get { return any; }
		}

		/// <summary>
		/// Expects a value of a given type or a subtype of the given type.
		/// </summary>
		/// <param name="expectedType">The value type to expect.</param>
		public static IExpectedValue OfType(Type expectedType) {
			return new ExpectedValueType(expectedType, true);
		}

		/// <summary>
		/// Expects a value of a given type.
		/// </summary>
		/// <param name="expectedType">The value type to expect.</param>
		/// <param name="allowCompatible">Whether the value is allowed to be of a compatible type instead.</param>
		public static IExpectedValue OfType(Type expectedType, bool allowCompatible) {
			return new ExpectedValueType(expectedType, allowCompatible);
		}

	}


	internal class ExpectedValueAny : IExpectedValue {
	
		public bool MatchesExpectation(object value) {
			return true;
		}
	}

	internal class ExpectedValueType : IExpectedValue {

		private Type expectedType;
		private bool allowCompatible;

		public ExpectedValueType(Type expectedType, bool allowCompatible) {
			this.expectedType = expectedType;
			this.allowCompatible = allowCompatible;
		}

		public bool MatchesExpectation(object value) {
			if (allowCompatible) {
				return expectedType.IsInstanceOfType(value);
			} else {
				return (expectedType == value.GetType());
			}
		}
	}

}
