using System;

namespace Arebis.Testing.Mocking.Manual
{

	/// <summary>
	/// Represents an expected value for argument checking.
	/// </summary>
	public interface IExpectedValue {

		/// <summary>
		/// Checks the received argument value.
		/// </summary>
		/// <returns>True if the received argument value matches the expectations.</returns>
		bool MatchesExpectation(object value);

	}
}
