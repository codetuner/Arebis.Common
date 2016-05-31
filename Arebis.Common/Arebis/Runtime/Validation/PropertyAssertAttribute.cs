using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Base class for property assert attributes that will allow validation of property values.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=true, Inherited=true)]
	public abstract class PropertyAssertAttribute : Attribute
	{
		private string onFailExceptionMessage;

		/// <summary>
		/// Constructs a PropertyAssertAttribute given the message to be shown on failure.
		/// </summary>
		/// <param name="onFailExceptionMessage"></param>
		public PropertyAssertAttribute(string onFailExceptionMessage)
		{
			this.onFailExceptionMessage = onFailExceptionMessage;
		}

		/// <summary>
		/// Returns true if the given property value is valid, false otherwise.
		/// </summary>
		public abstract bool Validate(object value);

		/// <summary>
		/// Checks the given property value is valid, otherwise throw an AssertionFailedException.
		/// </summary>
		public void Assert(object value)
		{
			if (this.Validate(value) == false) throw new AssertionFailedException(this.onFailExceptionMessage);
		}
	}
}
