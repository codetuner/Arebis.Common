// Infrastructure packages.
using System;
using System.Runtime.Serialization;

// Custom packages.
using Arebis.Runtime.Serialization.Formatters.Xml.SerializationSurrogates;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
	/// <summary>
	/// The internal SerializationSurrogate selector used by the GenericObjectFormatter.
	/// Contains SerializationSurrogates for the following types or interfaces:
	///		- System.DateTime
	///		- System.Runtim.Serialization.ISerializable
	///		- System.Object
	/// </summary>
	internal class SystemSurrogateSelector : SurrogateSelector
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemSurrogateSelector"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public SystemSurrogateSelector(StreamingContext context)
			: base()
		{
			this.initialize(context);
		}
		#endregion Constructors

		#region Private Methods
		/// <summary>
		/// Initializes the SystemSurrogateSelector using the specified context.
		/// The following SerializationSurrogates are added to the SurrogateSelector:
		///		- System.DateTime								->	DateTimeSerializationSurrogate
		///		- System.Runtime.Serialization.ISerializable	->	SerializableSerializationSurrogate
		///		- System.Object									->	ObjectSerializationSurrogate
		/// </summary>
		/// <param name="context">The context.</param>
		private void initialize(StreamingContext context)
		{
			this.AddSurrogate(typeof(DateTime), context, new DateTimeSerializationSurrogate());
			
			this.AddSurrogate(typeof(ISerializable), context, new SerializableSerializationSurrogate());
			this.AddSurrogate(typeof(object), context, new ObjectSerializationSurrogate());
		}
		#endregion Private Methods
	}
}
