// Infrastructure packages.
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Arebis.Runtime.Serialization.Formatters.Xml.SerializationSurrogates
{
	/// <summary>
	/// Serialization surrogate for the DateTime class.
	/// Serializes the DateTime object to a string with the following date time pattern:
	///		yyyy-MM-ddTHH:mm:ss.fffffffzzz
	/// </summary>
	public class DateTimeSerializationSurrogate : ISerializationSurrogate
	{
		private const string DATE_TIME_SERIALIZATION_INFO_NAME = "value";
		private const string DATE_TIME_SERIALIZATION_PATTERN = "yyyy-MM-ddTHH:mm:ss.fffffffzzz";

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DateTimeSerializationSurrogate"/> class.
		/// </summary>
		public DateTimeSerializationSurrogate()
		{
		}
		#endregion Constructors

		#region ISerializationSurrogate Members
		/// <summary>
		/// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the object.
		/// </summary>
		/// <param name="obj">The object to serialize.</param>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
		/// <exception cref="T:System.ArgumentNullException">The object to serialize cannot be null.</exception>
		/// <exception cref="T:System.ArgumentNullException">The SerializationInfo info cannot be null.</exception>
		/// <exception cref="T:System.SerializationException">The object to serialize is not of type DateTime.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			if (null == obj)
				throw new ArgumentNullException("obj", "Object obj cannot be null.");

			if (!(obj is DateTime))
				throw new SerializationException(String.Format("Type '{0}' in Assembly '{1}' is not of type '{2}'.", obj.GetType().FullName, obj.GetType().Assembly.FullName, typeof(DateTime).FullName));

			if (null == info)
				throw new ArgumentNullException("info", "SerializationInfo info cannot be null.");


			info.AddValue(DateTimeSerializationSurrogate.DATE_TIME_SERIALIZATION_INFO_NAME, ((DateTime)obj).ToString(DateTimeSerializationSurrogate.DATE_TIME_SERIALIZATION_PATTERN, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Populates the object using the information in the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see>.
		/// </summary>
		/// <param name="obj">The object to populate.</param>
		/// <param name="info">The information to populate the object.</param>
		/// <param name="context">The source from which the object is deserialized.</param>
		/// <param name="selector">The surrogate selector where the search for a compatible surrogate begins.</param>
		/// <returns>The populated deserialized object.</returns>
		/// <exception cref="T:System.ArgumentNullException">The SerializationInfo info cannot be null.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			if (null == info)
				throw new ArgumentNullException("info", "SerializationInfo info cannot be null.");


			string dateTimeSerializationString = info.GetString(DateTimeSerializationSurrogate.DATE_TIME_SERIALIZATION_INFO_NAME);
			if (dateTimeSerializationString == null)
			{
				obj = DateTime.MinValue;
			}
			else
			{
				if (dateTimeSerializationString.EndsWith("Z", StringComparison.Ordinal))
				{
					dateTimeSerializationString = dateTimeSerializationString.Substring(0, dateTimeSerializationString.Length - 1) + "-00:00";
				}

				obj = DateTime.ParseExact(dateTimeSerializationString, DateTimeSerializationSurrogate.DATE_TIME_SERIALIZATION_PATTERN, CultureInfo.InvariantCulture, DateTimeStyles.None);
			}

			return obj;
		}
		#endregion ISerializationSurrogate Members
	}
}
