// Infrastructure packages.
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace Arebis.Runtime.Serialization.Formatters.Xml.SerializationSurrogates
{
	/// <summary>
	/// Serialization surrogate for objects that are marked as serializable.
	/// All serializable members will be serialized.
	/// </summary>
	public class ObjectSerializationSurrogate : ISerializationSurrogate
	{
		#region Fields
		private bool enableEventSerialization;
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectSerializationSurrogate"/> class.
		/// </summary>
		public ObjectSerializationSurrogate()
		{
			this.enableEventSerialization = true;
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether event serialization is enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if event serialization is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool EnableEventSerialization
		{
			get { return this.enableEventSerialization; }
			set { this.enableEventSerialization = value; }
		}
		#endregion Properties

		#region ISerializationSurrogate Members
		/// <summary>
		/// Populates the provided <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with the data needed to serialize the object.
		/// </summary>
		/// <param name="obj">The object to serialize.</param>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> to populate with data.</param>
		/// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"></see>) for this serialization.</param>
		/// <exception cref="T:System.ArgumentNullException">The object to serialize cannot be null.</exception>
		/// <exception cref="T:System.ArgumentNullException">The SerializationInfo info cannot be null.</exception>
		/// <exception cref="T:System.SerializationException">The object type is not marked as serializable.</exception>
		/// <exception cref="T:System.SerializationException">The object type Array is not supported.</exception>
		/// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
		{
			if (null == obj)
				throw new ArgumentNullException("obj", "Object obj cannot be null.");

			if (null == info)
				throw new ArgumentNullException("info", "SerializationInfo info cannot be null.");

			Type objectType = obj.GetType();
			if (!objectType.IsSerializable)
				throw new SerializationException(String.Format("Type '{0}' in Assembly '{1}' is not marked as serializable.", objectType.FullName, objectType.Assembly.FullName));

			if (objectType.IsArray)
				throw new SerializationException(String.Format("The ObjectSerializationSurrogate does not support Array types. Type '{0}' in Assembly '{1}' is an Array type.", objectType.FullName, objectType.Assembly.FullName));


			MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(objectType, context);
			EventInfo[] events = objectType.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MemberInfo serializableMember in serializableMembers)
			{
				if (serializableMember.MemberType == MemberTypes.Field)
				{
					FieldInfo serializableField = (FieldInfo)serializableMember;

					bool isEvent = false;
					if (!this.enableEventSerialization)
					{
						foreach (EventInfo eventInfo in events)
						{
							string partialFieldName = serializableField.Name;
							string[] fieldNameParts = serializableField.Name.Split(new char[] { '+' } );
							if (0 < fieldNameParts.Length)
							{
								partialFieldName = fieldNameParts[fieldNameParts.Length - 1];
							}

							if ((eventInfo.Name == partialFieldName) && (eventInfo.EventHandlerType == serializableField.FieldType))
							{
								isEvent = true;
								break;
							}
						}
					}

					if (   !isEvent
						&& !serializableField.IsNotSerialized
						&& (serializableField.IsPublic || serializableField.IsAssembly || serializableField.IsFamily || serializableField.IsPrivate
							 || serializableField.IsFamilyAndAssembly || serializableField.IsFamilyOrAssembly)
						&& !(serializableField.IsLiteral || serializableField.IsStatic))
					{
						info.AddValue(serializableField.Name, serializableField.GetValue(obj), serializableField.FieldType);
					}
				}
			}
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
		/// <exception cref="T:System.SerializationException">Array types are not supported.</exception>
		/// /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
		public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
		{
			if (null == info)
				throw new ArgumentNullException("info", "Serialization info cannot be null.");

			//Assembly assembly = Assembly.Load(info.AssemblyName);
			//Type objectType = assembly.GetType(info.FullTypeName);
			string assemblyQualifiedTypeName = string.Format("{0}, {1}", info.FullTypeName, info.AssemblyName);
			Type objectType = new TypeParser().Resolve(assemblyQualifiedTypeName);
			if (objectType.IsArray)
				throw new SerializationException(String.Format("The ObjectSerializationSurrogate does not support Array types. Type '{0}' in Assembly '{1}' is an Array type.", objectType.FullName, objectType.Assembly.FullName));


			if (null == obj)
			{
				obj = FormatterServices.GetUninitializedObject(objectType);
			}
			
			List<FieldInfo> fields = new List<FieldInfo>();
			List<object> values = new List<object>();
			MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(objectType, context);
			foreach (SerializationEntry entry in info)
			{
				FieldInfo field = ObjectSerializationSurrogate.getField(serializableMembers, entry.Name);
				
				fields.Add(field);
				values.Add(entry.Value);
			}
			FormatterServices.PopulateObjectMembers(obj, fields.ToArray(), values.ToArray());

			return obj;
		}
		#endregion ISerializationSurrogate Members

		#region Private Methods
		private static FieldInfo getField(MemberInfo[] serializableMembers, string fieldName)
		{
			System.Diagnostics.Debug.Assert(null != serializableMembers, "The 'serializableMembers' argument cannot be null.");
			System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(fieldName), "The 'fieldName' argument cannot be null or the empty string.");

			foreach (MemberInfo serializableMember in serializableMembers)
			{
				if (   serializableMember.MemberType == MemberTypes.Field
					&& serializableMember.Name == fieldName)
				{
					return (FieldInfo)serializableMember;
				}
			}

			// Not found.
			return null;
		}
		#endregion Private Methods
	}
}
