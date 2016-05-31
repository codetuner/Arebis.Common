// Infrastructure packages.
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Arebis.Runtime.Serialization.Formatters.Xml.SerializationSurrogates
{
	/// <summary>
	/// Serialization surrogate for objects implementing the ISerializable interface.
	/// Uses the ISerializable interface method GetObjectData to serialize the object.
	/// To populate the object during deserialization the constructor with the signature constructor (SerializationInfo information, StreamingContext context) is invoked.
	/// </summary>
	public class SerializableSerializationSurrogate : ISerializationSurrogate
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SerializableSerializationSurrogate"/> class.
		/// </summary>
		public SerializableSerializationSurrogate()
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
		/// <exception cref="T:System.SerializationException">The type does not implement the ISerializable interface.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			if (null == obj)
				throw new ArgumentNullException("obj", "Object obj cannot be null.");

			if (!(obj is ISerializable))
				throw new SerializationException(string.Format("Type '{0}' in Assembly '{1}' does not implement Interface '{2}'.", obj.GetType().FullName, obj.GetType().Assembly.FullName, typeof(ISerializable).FullName));

			if (null == info)
				throw new ArgumentNullException("info", "SerializationInfo info cannot be null.");


			((ISerializable)obj).GetObjectData(info, context);
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
		/// <exception cref="T:System.SerializationException">The type does not implement the ISerializable interface.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			if (null == info)
				throw new ArgumentNullException("info", "Serialization info cannot be null.");

			//Assembly assembly = Assembly.Load(info.AssemblyName);
			//Type objectType = assembly.GetType(info.FullTypeName);
			string assemblyQualifiedTypeName = string.Format("{0}, {1}", info.FullTypeName, info.AssemblyName);
			Type objectType = new TypeParser().Resolve(assemblyQualifiedTypeName);
			if (!typeof(ISerializable).IsAssignableFrom(objectType))
				throw new SerializationException(String.Format("Type '{0}' in Assembly '{1}' does not implement Interface '{2}'.", objectType.FullName, objectType.Assembly.FullName, typeof(ISerializable).FullName));


			if (null == obj)
			{
				obj = FormatterServices.GetUninitializedObject(objectType);
			}

			ConstructorInfo constructor = objectType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] {typeof(SerializationInfo), typeof(StreamingContext)}, null);
			constructor.Invoke(obj, new object[] { info, context });
			
			return obj;
		}
		#endregion ISerializationSurrogate Members
	}
}
