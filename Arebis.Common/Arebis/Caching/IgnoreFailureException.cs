using System;
using System.Runtime.Serialization;

namespace Arebis.Caching
{
	/// <summary>
	/// An IgnoreFailureException is an exception that, when thrown by a cache ValueProvider,
	/// make the cache ignore the failure and return the unupdated cache value.
	/// </summary>
	[Serializable]
	public class IgnoreFailureException : ApplicationException
	{
		/// <summary>
		/// Creates a new instance of IgnoreFailureException.
		/// </summary>
		public IgnoreFailureException()
			: base()
		{ }

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected IgnoreFailureException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}
