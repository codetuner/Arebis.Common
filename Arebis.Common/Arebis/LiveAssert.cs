using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arebis
{
    /// <summary>
    /// Assertion methods to ease validation in business methods.
    /// The class is named LiveAssert to avoid conflicts withe the UnitTesting Assert methods,
    /// and because it asserts during 'live' execution.
    /// </summary>
    public static class LiveAssert
    {
        /// <summary>
        /// Throws an ArgumentNullException if value is null.
        /// </summary>
        public static void ArgumentNotNull(string argName, object value)
        {
            if (Object.ReferenceEquals(value, null)) throw new ArgumentNullException(argName);
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is out of range.
        /// </summary>
        public static void ArgumentNotLessThan(string argName, int value, int minimumAllowedValue)
        {
            if (value < minimumAllowedValue)
                throw new ArgumentOutOfRangeException(argName, value, String.Format("Value should not be less than {0}.", minimumAllowedValue));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is out of range.
        /// </summary>
        public static void ArgumentNotLargerThan(string argName, int value, int maximumAllowedValue)
        {
            if (value > maximumAllowedValue)
                throw new ArgumentOutOfRangeException(argName, value, String.Format("Value should not be larger than {0}.", maximumAllowedValue));
        }

        /// <summary>
        /// Throws an ArgumentOutOfRangeException if value is out of range.
        /// </summary>
        public static void ArgumentBetween(string argName, int value, int minimumAllowedValue, int maximumAllowedValue)
        {
            if (value < minimumAllowedValue)
                throw new ArgumentOutOfRangeException(argName, value, String.Format("Value should not be less than {0}.", minimumAllowedValue));
            if (value > maximumAllowedValue)
                throw new ArgumentOutOfRangeException(argName, value, String.Format("Value should not be larger than {0}.", maximumAllowedValue));
        }

        /// <summary>
        /// Throws an ArgumentException if condition is not met.
        /// </summary>
        public static void ArgumentPrecondition(string argName, bool condition, string messageOnFailure = null)
        {
            if (condition == false)
                throw new ArgumentException(messageOnFailure ?? "Argument does not meet preconditions.", argName);
        }

        /// <summary>
        /// Throws an InvalidOperationException if condition is not met.
        /// </summary>
        public static void Precondition(bool condition, string messageOnFailure = null)
        {
            if (condition == false)
                throw new InvalidOperationException(messageOnFailure ?? "Argument does not meet preconditions.");
        }

        /// <summary>
        /// Throw an instance of the given TException if condition is not met.
        /// </summary>
        public static void Custom<TException>(bool condition)
            where TException : Exception, new()
        {
            if (condition == false)
                throw new TException();
        }
    }
}
