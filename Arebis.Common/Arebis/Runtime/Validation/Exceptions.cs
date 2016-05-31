using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Arebis.Runtime.Validation
{
	/// <summary>
	/// Thrown when an assert failed.
	/// </summary>
	[Serializable]
	public class AssertionFailedException : ArgumentException 
	{
		/// <summary>
		/// Constructs a new AssertionFailedException.
		/// </summary>
		public AssertionFailedException(string message)
			: base(message)
		{ }

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected AssertionFailedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
