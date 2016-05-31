// Infrastructure packages.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

// Custom packages.
using Arebis.Runtime.Serialization.Formatters.Xml.SerializationSurrogates;

namespace Arebis.Runtime.Serialization.Formatters.Xml
{
	/// <summary>
	/// Xml serialization formatter with support for generic types.
	/// </summary>
	public sealed class XmlFormatter : IFormatter
	{
        #region Fields

		private SerializationBinder binder;
		private StreamingContext context;
		private ISurrogateSelector userSurrogateSelector;
		private ISurrogateSelector systemSurrogateSelector;

		private bool enableEventSerialization;
		private FormatterAssemblyStyle assemblyFormat;

		private ListDictionary registeredReferenceObjects;
		private ObjectIDGenerator idGenerator;
		
		#endregion Fields

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlFormatter"/> class.
		/// </summary>
		public XmlFormatter()
		{
			this.context = new StreamingContext(StreamingContextStates.File);

			this.enableEventSerialization = true;
			this.assemblyFormat = FormatterAssemblyStyle.Full;

			this.initializeSystemSurrogateSelector(this.context, this.enableEventSerialization);

			this.registeredReferenceObjects = new ListDictionary(new ReferenceComparer());
		}

		#endregion Constructors

		#region IFormatter Members

		#region Properties
		/// <summary>
		/// Gets or sets the <see cref="T:System.Runtime.Serialization.SerializationBinder"></see> that performs type lookups during deserialization.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Runtime.Serialization.SerializationBinder"></see> that performs type lookups during deserialization.</returns>
		public SerializationBinder Binder
		{
			get { return this.binder; }
			set { this.binder = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="T:System.Runtime.Serialization.StreamingContext"></see> used for serialization and deserialization.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> used for serialization and deserialization.</returns>
		public StreamingContext Context
		{
			get	{
				return this.context;
			}
			set	{
				this.context = value;
				this.initializeSystemSurrogateSelector(this.context, this.enableEventSerialization);
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="T:System.Runtime.Serialization.SurrogateSelector"></see> used by the current formatter.
		/// </summary>
		/// <value></value>
		/// <returns>The <see cref="T:System.Runtime.Serialization.SurrogateSelector"></see> used by this formatter.</returns>
		public ISurrogateSelector SurrogateSelector
		{
			get { return this.userSurrogateSelector; }
			set { this.userSurrogateSelector = value; }
		}
		#endregion Properties

		#region Methods
		/// <summary>
		/// Deserializes the data on the provided stream and reconstitutes the graph of objects.
		/// </summary>
		/// <param name="serializationStream">The stream that contains the data to deserialize.</param>
		/// <returns>The top object of the deserialized graph.</returns>
		/// <exception cref="T:System.ArgumentNullException">The <para>serializationStream</para> cannot be null.</exception>
		public object Deserialize(Stream serializationStream)
		{
			if (null == serializationStream)
				throw new ArgumentNullException("serializationStream", "The Stream serializationStream cannot be null.");

			long initialStreamPosition = serializationStream.Position;
			XmlReader xmlReader = null;
			try {
				xmlReader = XmlReader.Create(serializationStream, this.createXmlReaderSettings());

				if (!xmlReader.IsStartElement())
					throw new SerializationException("Root element is missing.");

				this.registeredReferenceObjects.Clear();

				IFormatterConverter converter = new FormatterConverter();
				SerializationEntry graphEntry = this.deserialize(xmlReader, converter);

				return graphEntry.Value;
			}
			finally {
				if (null != xmlReader) {
					serializationStream.Position = this.getStreamPosition(xmlReader, serializationStream, initialStreamPosition);
					xmlReader.Close();
				}
			}
		}

		/// <summary>
		/// Serializes an object, or graph of objects with the given root to the provided stream.
		/// </summary>
		/// <param name="serializationStream">
		/// The stream where the formatter puts the serialized data. This stream can reference a variety
		/// of backing stores (such as files, network, memory, and so on).
		/// </param>
		/// <param name="graph">
		/// The object, or root of the object graph, to serialize. All child objects of this root object
		/// are automatically serialized.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">The <para>serializationStream</para> cannot be null.</exception>
		/// <exception cref="T:System.ArgumentNullException">The <para>graph</para> cannot be null.</exception>
		public void Serialize(Stream serializationStream, object graph)
		{
			if (null == serializationStream)
				throw new ArgumentNullException("serializationStream", "Stream serializationStream cannot be null.");

			if (null == graph)
				throw new ArgumentNullException("graph", "Object graph cannot be null.");

			XmlWriter xmlWriter = null; 
			try {
				xmlWriter = XmlWriter.Create(serializationStream, this.createXmlWriterSettings());
				
				this.registeredReferenceObjects.Clear();
				this.idGenerator = new ObjectIDGenerator();

                IFormatterConverter converter = new FormatterConverter();
                SerializationEntry graphEntry = this.createSerializationEntry(this.getName(graph.GetType()), graph, converter);
                this.serializeEntry(xmlWriter, graphEntry, converter);
				xmlWriter.WriteWhitespace(Environment.NewLine);
			}
			finally {
				if (null != xmlWriter)
					xmlWriter.Flush();
			}
		}

		#endregion Methods

		#endregion IFormatter Members

		#region Properties

		/// <summary>
		/// Gets or sets a value indicating whether event serialization is enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if event serialization is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool EnableEventSerialization
		{
			get {
				return this.enableEventSerialization; 
			}
			set {
				this.enableEventSerialization = value;
				this.initializeSystemSurrogateSelector(this.context, this.enableEventSerialization);
			}
		}

		/// <summary>
		/// Gets or sets the assembly format.
		/// </summary>
		/// <value>The assembly format.</value>
		public FormatterAssemblyStyle AssemblyFormat
		{
			get { return this.assemblyFormat; }
			set { this.assemblyFormat = value; }
		}
		#endregion Properties

		#region Private Methods

		private void initializeSystemSurrogateSelector(StreamingContext context, bool enableEventSerialization)
		{
			SurrogateSelector surrogateSelector = new SurrogateSelector();

			surrogateSelector.AddSurrogate(typeof(DateTime), context, new DateTimeSerializationSurrogate());

			surrogateSelector.AddSurrogate(typeof(ISerializable), context, new SerializableSerializationSurrogate());

			ObjectSerializationSurrogate objectSurrogate = new ObjectSerializationSurrogate();
			objectSurrogate.EnableEventSerialization = enableEventSerialization;
			surrogateSelector.AddSurrogate(typeof(object), context, objectSurrogate);

			this.systemSurrogateSelector = surrogateSelector;
		}

		private /*static*/ XmlReaderSettings createXmlReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings(); // Return value

			settings.CheckCharacters = false;
			settings.CloseInput = false;
			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.IgnoreComments = true;
			settings.IgnoreProcessingInstructions = true;
			settings.IgnoreWhitespace = true;
			settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
			settings.ValidationType = ValidationType.None;

			return settings;
		}

		private /*static*/ XmlWriterSettings createXmlWriterSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings(); // return value

			settings.ConformanceLevel = ConformanceLevel.Fragment;
			settings.Encoding = Encoding.UTF8;
			settings.Indent = true;
			settings.IndentChars = "\t";
			settings.NewLineChars = Environment.NewLine;
			settings.OmitXmlDeclaration = true;

			return settings;
		}

		private /*static*/ SerializationEntry createSerializationEntry(string name, object value, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(name), "The 'name' argument cannot be null or the empty string.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			Type valueType = typeof(object);
			if (null != value)
				valueType = value.GetType();

			// Create serialization entry
			SerializationInfo info = new SerializationInfo(valueType, converter);
			info.AddValue(name, value, valueType);

			// Return created serialization entry
			SerializationInfoEnumerator infoEnumerator = info.GetEnumerator();
			infoEnumerator.MoveNext();
			return infoEnumerator.Current;
		}

		private void serializeEntry(XmlWriter xmlWriter, SerializationEntry entry, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			if (entry.Name.Contains("+")) {
				string[] nameParts = entry.Name.Split(new char[] { '+' });

				xmlWriter.WriteStartElement(nameParts[1]);

				xmlWriter.WriteStartAttribute("declaringType");
				xmlWriter.WriteValue(nameParts[0]);
				xmlWriter.WriteEndAttribute();
			}
			else {
				xmlWriter.WriteStartElement(entry.Name);
			}

			object entryValue = entry.Value;
			if (null == entryValue) {
				xmlWriter.WriteStartAttribute("isNull");
				xmlWriter.WriteValue(true);
				xmlWriter.WriteEndAttribute();
			}
			else {
				this.serializeObject(xmlWriter, entryValue, converter);
			}

			xmlWriter.WriteEndElement();
		}
		private void serializeObject(XmlWriter xmlWriter, object obj, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != obj, "The 'obj' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			Type objType = obj.GetType();
			if (this.isSimpleType(objType)) {
				this.writeType(xmlWriter, objType, this.assemblyFormat);
				this.writeValue(xmlWriter, obj);
			}
			else if (objType.IsValueType) {
				SerializationInfo info = this.getSerializationInfo(obj, converter);
				this.serializeInfo(xmlWriter, info, converter);
			}
			else if (this.isRegisteredReferenceObject(obj)) {
				int registeredReferenceObjectId = this.getRegisteredReferenceObjectId(obj);
				this.writeHref(xmlWriter, registeredReferenceObjectId);
			}
			else {
				int id = this.registerReferenceObject(obj);

				this.writeId(xmlWriter, id);

				if (objType.IsArray) {
					this.serializeArray(xmlWriter, (Array)obj, converter);
				}
				else {
					SerializationInfo info = this.getSerializationInfo(obj, converter);
					this.serializeInfo(xmlWriter, info, converter);
				}
			}
		}
		private void serializeInfo(XmlWriter xmlWriter, SerializationInfo info, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != info, "The 'info' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			string assemblyQualifiedTypeName = string.Format("{0}, {1}", info.FullTypeName, info.AssemblyName);
			Type objectType = new TypeParser().Resolve(assemblyQualifiedTypeName);
			this.writeType(xmlWriter, objectType, this.assemblyFormat);

			foreach (SerializationEntry entry in info)
				this.serializeEntry(xmlWriter, entry, converter);
		}
		private void serializeArray(XmlWriter xmlWriter, Array array, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != array, "The 'array' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			this.writeType(xmlWriter, array, this.assemblyFormat);

			int rank = array.Rank;
			int itemCount = 1;
			int[] upperBounds = new int[rank];
			for (int i = 0; i < rank; i++) {
				upperBounds[i] = array.GetUpperBound(i);
				itemCount *= upperBounds[i] + 1;
			}

			SerializationEntry[] entries = new SerializationEntry[itemCount]; // Return value
			int[] indices = new int[rank];
			for (int itemIndex = 0; itemIndex < itemCount; itemIndex++) {
				entries[itemIndex] = this.createSerializationEntry("Item", array.GetValue(indices), converter);

				for (int dimensionIndex = rank - 1; dimensionIndex >= 0; dimensionIndex--) {
					if (indices[dimensionIndex] < upperBounds[dimensionIndex]) {
						indices[dimensionIndex]++;
						break;
					}
					else {
						indices[dimensionIndex] = 0;
					}
				}
			}

			foreach (SerializationEntry entry in entries)
				this.serializeEntry(xmlWriter, entry, converter);
		}

		private SerializationInfo getSerializationInfo(object graph, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != graph, "The 'graph' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null");

			Type graphType = graph.GetType();

			ISurrogateSelector selector = null;
			ISerializationSurrogate surrogate = this.getSerializationSurrogate(graphType, out selector);
			if (null == surrogate)
				throw new SerializationException(String.Format("No SerializationSurrogate found for Type '{0}' in Assembly '{1}'", graph.GetType().FullName, graph.GetType().Assembly.FullName));

			SerializationInfo graphInfo = new SerializationInfo(graphType, converter);
			surrogate.GetObjectData(graph, graphInfo, this.context);

			return graphInfo;
		}

		private ISerializationSurrogate getSerializationSurrogate(Type type, out ISurrogateSelector selector)
		{
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");

			ISerializationSurrogate serializationSurrogate = null;
			ISurrogateSelector surrogateSelector = null; // Added to keep compiler happy.

			if (null != this.userSurrogateSelector)
				serializationSurrogate = this.getSerializationSurrogate(type, out surrogateSelector, this.userSurrogateSelector);

			if (null == serializationSurrogate && null != this.systemSurrogateSelector)
				serializationSurrogate = this.getSerializationSurrogate(type, out surrogateSelector, this.systemSurrogateSelector);

			selector = surrogateSelector; // Added to keep compiler happy.
			return serializationSurrogate;

		}
		private ISerializationSurrogate getSerializationSurrogate(Type type, out ISurrogateSelector selector, ISurrogateSelector startingSelector)
		{
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != startingSelector, "The 'startingSelector' argument cannot be null.");

			ISerializationSurrogate serializationSurrogate =
				startingSelector.GetSurrogate(type, this.context, out selector);

			if (null == serializationSurrogate && typeof(ISerializable).IsAssignableFrom(type))
				serializationSurrogate = startingSelector.GetSurrogate(typeof(ISerializable), this.context, out selector);

			if (null == serializationSurrogate)
				serializationSurrogate = startingSelector.GetSurrogate(typeof(object), this.context, out selector);

			return serializationSurrogate;
		}


		private SerializationEntry deserialize(XmlReader xmlReader, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(xmlReader.IsStartElement(), "The XmlReader xmlReader position must be at the beginning of an element.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			string entryName = xmlReader.Name;
			string declaringType = null;
			if (xmlReader.HasAttributes) {
				declaringType = xmlReader.GetAttribute("declaringType");
				if (!String.IsNullOrEmpty(declaringType))
					entryName = String.Format("{0}+{1}", declaringType, entryName);
			}

			object entryValue = null;
			if (this.isNull(xmlReader)) {
				entryValue = null;
				xmlReader.ReadStartElement();
			}
			else if (this.isHref(xmlReader)) {
				int referencedId = this.readHref(xmlReader);
				entryValue = this.getRegisteredReferenceObject(referencedId);
				if (entryValue is IObjectReference)
					entryValue = ((IObjectReference)entryValue).GetRealObject(this.context);

				xmlReader.ReadStartElement();
			}
			else {
				int id = this.readId(xmlReader);
				List<int> dimensionLengths = this.readArrayDimensions(xmlReader);
				Type entryType = this.readType(xmlReader);
				SerializationInfo info = new SerializationInfo(entryType, converter);
				entryValue = this.deserialize(xmlReader, id, info, dimensionLengths, converter);
			}

			SerializationEntry entry = this.createSerializationEntry(entryName, entryValue, converter);
			return entry;
		}
		private object deserialize(XmlReader xmlReader, int id, SerializationInfo info, List<int> dimensionLengths, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != info, "The 'info' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			string assemblyQualifiedTypeName = string.Format("{0}, {1}", info.FullTypeName, info.AssemblyName);
			Type objectType = new TypeParser().Resolve(assemblyQualifiedTypeName);

			object deserialized = null; // Return value
			if (this.isSimpleType(objectType)) {
				deserialized = this.readValue(xmlReader, objectType, converter);
			}
			else if (objectType.IsArray) {
				Type elementType = objectType.GetElementType();
				deserialized = this.deserializeArray(xmlReader, id, elementType, dimensionLengths, converter);
			}
			else {
				deserialized = this.deserializeObject(xmlReader, id, objectType, info, converter);
			}

			IObjectReference objectReference = deserialized as IObjectReference;
			if (null != objectReference)
				deserialized = objectReference.GetRealObject(this.context);

			IDeserializationCallback deserializationCallback = deserialized as IDeserializationCallback;
			if (null != deserializationCallback)
				deserializationCallback.OnDeserialization(this);

			if (XmlNodeType.EndElement == xmlReader.NodeType)
				xmlReader.ReadEndElement();

			return deserialized;
		}
		private Array deserializeArray(XmlReader xmlReader, int id, Type elementType, List<int> dimensionLengths, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != elementType, "The 'elementType' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != dimensionLengths, "The 'dimensionLengths' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			Array array = Array.CreateInstance(elementType, dimensionLengths.ToArray());

			// Register array
			this.registeredReferenceObjects.Add(array, id);

			int itemCount = 1;
			foreach (int length in dimensionLengths)
				itemCount *= length;

			int rank = array.Rank;
			int[] indices = new int[rank];
			for (int itemIndex = 0; itemIndex < itemCount; itemIndex++) {
				SerializationEntry itemEntry = this.deserialize(xmlReader, converter);
				array.SetValue(itemEntry.Value, indices);

				for (int dimensionIndex = rank - 1; dimensionIndex >= 0; dimensionIndex--) {
					if (indices[dimensionIndex] < dimensionLengths[dimensionIndex] - 1) {
						indices[dimensionIndex]++;
						break;
					}
					else {
						indices[dimensionIndex] = 0;
					}
				}
			}

			if (0 == itemCount && XmlNodeType.Element == xmlReader.NodeType)
				xmlReader.ReadStartElement();

			return array;
		}
		private object deserializeObject(XmlReader xmlReader, int id, Type objectType, SerializationInfo info, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != objectType, "The 'objectType' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != info, "The 'info' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			object deserialized = null; // Return value
			if (!objectType.IsValueType) {
				deserialized = FormatterServices.GetUninitializedObject(objectType);

				// Register object
				this.registeredReferenceObjects.Add(deserialized, id);
			}

			bool hasSerializableMembers = this.hasSerializableMembers(objectType, this.context);
			if (hasSerializableMembers) {
				while (xmlReader.IsStartElement()) {
					SerializationEntry entry = this.deserialize(xmlReader, converter);
					info.AddValue(entry.Name, entry.Value, entry.ObjectType);
				}
			}
			else {
				if (XmlNodeType.Element == xmlReader.NodeType)
					xmlReader.ReadStartElement();
			}

			ISurrogateSelector selector = null;
			ISerializationSurrogate surrogate = this.getSerializationSurrogate(objectType, out selector);
			deserialized = surrogate.SetObjectData(deserialized, info, this.context, selector);

			return deserialized;
		}
		private /*static*/ bool hasSerializableMembers(Type type, StreamingContext context)
		{
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");

			bool hasSerializableMembers = false;
			MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(type, context);
			EventInfo[] events = type.GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (MemberInfo serializableMember in serializableMembers) {
				if (serializableMember.MemberType == MemberTypes.Field) {
					FieldInfo serializableField = (FieldInfo)serializableMember;

					bool isEvent = false;
					if (!this.enableEventSerialization) {
						foreach (EventInfo eventInfo in events) {
							string partialFieldName = serializableField.Name;
							string[] fieldNameParts = serializableField.Name.Split(new char[] { '+' });
							if (0 < fieldNameParts.Length)
								partialFieldName = fieldNameParts[fieldNameParts.Length - 1];

							if ((eventInfo.Name == partialFieldName) && (eventInfo.EventHandlerType == serializableField.FieldType)) {
								isEvent = true;
								break;
							}
						}
					}

					if (!isEvent
						&& !serializableField.IsNotSerialized
						&& (serializableField.IsPublic || serializableField.IsAssembly || serializableField.IsFamily || serializableField.IsPrivate
							 || serializableField.IsFamilyAndAssembly || serializableField.IsFamilyOrAssembly)
						&& !(serializableField.IsLiteral || serializableField.IsStatic)) {
						hasSerializableMembers = true;
						break;
					}
				}
			}

			return hasSerializableMembers;
		}

		private /*static*/ long getStreamPosition(XmlReader xmlReader, Stream serializationStream, long initialStreamPosition)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "XmlReader xmlReader should not be null.");
			System.Diagnostics.Debug.Assert(null != serializationStream, "Stream serializationStream should not be null.");

			if (xmlReader.EOF || XmlNodeType.Element != xmlReader.NodeType)
				return initialStreamPosition;

			int streamPosition = Convert.ToInt32(initialStreamPosition);

			// Get string representation of the next element:
			string nextElement = string.Empty;
			using (MemoryStream ms = new MemoryStream())
			{
				XmlWriter nextElementWriter = XmlWriter.Create(ms, this.createXmlWriterSettings());
				try
				{
					nextElementWriter.WriteNode(xmlReader, false);
					nextElementWriter.Flush();

					ms.Position = 0;
					byte[] memoryBuffer = new byte[Convert.ToInt32(ms.Length)];
					ms.Read(memoryBuffer, 0, Convert.ToInt32(ms.Length));
					nextElement = Encoding.Default.GetString(memoryBuffer);
					nextElement = nextElement.Substring(nextElement.IndexOf("<"));
				}
				finally
				{
					if (null != nextElementWriter)
						nextElementWriter.Close();
				}
			}

			// Get string representation of the serializationStream:
			serializationStream.Position = 0;
			byte[] buffer = new byte[Convert.ToInt32(serializationStream.Length)];
			serializationStream.Read(buffer, 0, Convert.ToInt32(serializationStream.Length));
			string serializationString = Encoding.Default.GetString(buffer);

			// Determine position of the next XML element in the stream:
			//	 Find 1st opening bracket '<'
			int startIndex = serializationString.IndexOf('<', streamPosition);
			//	 Find beginning of next element
			int nextElementStartIndex = serializationString.IndexOf(nextElement, Math.Max(startIndex, streamPosition));
			if (nextElementStartIndex <= Math.Max(startIndex, streamPosition))
				nextElementStartIndex = serializationString.IndexOf(nextElement, Math.Min(serializationString.Length, Math.Max(startIndex, streamPosition) + nextElement.Length));

			return Convert.ToInt64(nextElementStartIndex);
		}

		private int registerReferenceObject(object referenceObject)
		{
			System.Diagnostics.Debug.Assert(null != referenceObject, "The 'referenceObject' argument cannot be null.");

			bool firstTime = true;
			long id = this.idGenerator.GetId(referenceObject, out firstTime);

			return Convert.ToInt32(id);
		}
		private bool isRegisteredReferenceObject(object referenceObject)
		{
			System.Diagnostics.Debug.Assert(null != referenceObject, "The 'referenceObject' argument cannot be null.");

			bool firstTime = false;
			this.idGenerator.HasId(referenceObject, out firstTime);
			return !firstTime;
		}
		private int getRegisteredReferenceObjectId(object registeredReferenceObject)
		{
			System.Diagnostics.Debug.Assert(null != registeredReferenceObject, "The 'registeredReferenceObject' argument cannot be null.");
			System.Diagnostics.Debug.Assert(this.isRegisteredReferenceObject(registeredReferenceObject), "The object registeredReferenceObject must be registered");

			bool firstTime = false;
			long id = this.idGenerator.HasId(registeredReferenceObject, out firstTime);

			return Convert.ToInt32(id);
		}
		private object getRegisteredReferenceObject(int id)
		{
			foreach (object registeredReferenceObject in this.registeredReferenceObjects.Keys) {
				int registeredReferenceObjectId = (int)this.registeredReferenceObjects[registeredReferenceObject];
				if (id == registeredReferenceObjectId)
					return registeredReferenceObject;
			}

			// Id not found.
			throw new SerializationException("Object not found.");
		}


		private /*static*/ void writeId(XmlWriter xmlWriter, int id)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");

			xmlWriter.WriteStartAttribute("id");
			xmlWriter.WriteString(String.Format("ref-{0}", id));
			xmlWriter.WriteEndAttribute();
		}

		private /*static*/ void writeType(XmlWriter xmlWriter, Array array, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != array, "The 'array' argument cannot be null.");

			string typeName = this.getQualifiedTypeName(array, assemblyFormat);
			xmlWriter.WriteStartAttribute("type");
			xmlWriter.WriteString(typeName);
			xmlWriter.WriteEndAttribute();
		}
		private /*static*/ void writeType(XmlWriter xmlWriter, Type type, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");
			System.Diagnostics.Debug.Assert(!type.IsArray, "The Type type cannot be an Array.");

			string typeName = this.getQualifiedTypeName(type, assemblyStyle);
			xmlWriter.WriteStartAttribute("type");
			xmlWriter.WriteString(typeName);
			xmlWriter.WriteEndAttribute();
		}

		private /*static*/ void writeValue(XmlWriter xmlWriter, object value)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != value, "The 'value' argument cannot be null.");
			System.Diagnostics.Debug.Assert(this.isSimpleType(value.GetType()), "The object value is not a simple type.");

			Type valueType = value.GetType();
			if (valueType.IsPrimitive || value is string) {
				xmlWriter.WriteValue(value);
			}
			else if (valueType.IsEnum) {
				xmlWriter.WriteValue((int)value);
			}
			else if (value is Guid) {
				xmlWriter.WriteValue(((Guid)value).ToString("D").ToLower());
			}
		}
		private /*static*/ void writeHref(XmlWriter xmlWriter, int id)
		{
			System.Diagnostics.Debug.Assert(null != xmlWriter, "The 'xmlWriter' argument cannot be null.");

			xmlWriter.WriteStartAttribute("href");
			xmlWriter.WriteString(String.Format("#ref-{0}", id));
			xmlWriter.WriteEndAttribute();
		}

		private /*static*/ bool isSimpleType(Type objectType)
		{
			System.Diagnostics.Debug.Assert(null != objectType, "The 'objectType' argument cannot be null.");

			bool isSimpleType = objectType.IsPrimitive;
			isSimpleType |= typeof(System.String) == objectType;
			isSimpleType |= objectType.IsEnum;
			isSimpleType |= typeof(System.Guid) == objectType;

			return isSimpleType;
		}

		private /*static*/ string getName(Type graphType)
		{
			System.Diagnostics.Debug.Assert(null != graphType, "The 'graphType' argument cannot be null.");

			string name = graphType.Name;
			if (graphType.IsArray) {
				name = "ArrayOfAnyType";
			}
			else if (graphType.IsGenericType) {
				name = "GenericType";
			}

			return name;
		}

		private /*static*/ string getQualifiedTypeName(Array array, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != array, "The 'array' argument cannot be null.");

			int rank = array.Rank;
			string dimensionLenghts = string.Empty;
			for (int i = 0; i < rank; i++) {
				dimensionLenghts += (array.GetUpperBound(i) + 1);
				if (i < rank - 1)
					dimensionLenghts += ",";
			}

			Type elementType = array.GetType().GetElementType();

			string qualifiedTypeName = this.getTypeName(elementType, assemblyStyle); // Return value
			qualifiedTypeName += string.Format("[{0}]", dimensionLenghts);
			string assemblyName = this.getAssemblyName(elementType.Assembly, assemblyStyle);
			if (!string.IsNullOrEmpty(assemblyName))
				qualifiedTypeName += string.Format(", {0}", assemblyName);
			return qualifiedTypeName;
		}
		private /*static*/ string getQualifiedTypeName(Type type, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");

			string qualifiedTypeName = this.getTypeName(type, assemblyStyle);
			string assemblyName = this.getAssemblyName(type.Assembly, assemblyStyle);
			if (!string.IsNullOrEmpty(assemblyName))
				qualifiedTypeName += string.Format(", {0}", assemblyName);
			return qualifiedTypeName;
		}
		private /*static*/ string getTypeName(Type type, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");

			string typeName = type.FullName;
			if (type.IsGenericType) {
				typeName = type.GetGenericTypeDefinition().FullName;
				if (!type.IsGenericTypeDefinition) {
					typeName += "[";
					Type[] genericArguments = type.GetGenericArguments();
					for (int i = 0; i < genericArguments.Length; i++) {
						string genericArgumentTypeName = this.getQualifiedTypeName(genericArguments[i], assemblyStyle);
						typeName += string.Format("[{0}]", genericArgumentTypeName);
						if (i < genericArguments.Length - 1)
							typeName += ",";
					}
					typeName += "]";
				}
			}
			return typeName;
		}
		private /*static*/ string getAssemblyName(Assembly assembly, FormatterAssemblyStyle assemblyStyle)
		{
			System.Diagnostics.Debug.Assert(null != assembly, "The 'assembly' argument cannot be null.");

			AssemblyInfo assemblyInfo = new AssemblyInfo(assembly);
			if (assemblyInfo.Name == "mscorlib" || assemblyInfo.Name == "System")
				return null;

			string assemblyName = assemblyInfo.ToString();
			if (FormatterAssemblyStyle.Simple == assemblyStyle)
				assemblyName = assemblyInfo.Name;
			return assemblyName;
		}

		private /*static*/ bool isNull(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			bool isNull = false; // Return value
			if (xmlReader.HasAttributes) {
				string isNullRepresentation = xmlReader.GetAttribute("isNull");
				if (!String.IsNullOrEmpty(isNullRepresentation))
					isNull = bool.Parse(isNullRepresentation);
			}
			return isNull;
		}
		private /*static*/ bool isHref(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			bool isHref = false; // Return value
			if (xmlReader.HasAttributes) {
				string href = xmlReader.GetAttribute("href");
				if (!String.IsNullOrEmpty(href))
					isHref = true;
			}
			return isHref;
		}
		private /*static*/ int readHref(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			string href = null;
			if (xmlReader.HasAttributes)
				href = xmlReader.GetAttribute("href");

			System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(href), "The href attribute is not present or its value is the empty string.");
			int id = int.Parse(href.Substring(href.LastIndexOf('-') + 1));
			return id;
		}

		private /*static*/ int readId(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			int id = -1; // Return value
			if (xmlReader.HasAttributes) {
				string idRepresentation = xmlReader.GetAttribute("id");
				if (!String.IsNullOrEmpty(idRepresentation))
					id = int.Parse(idRepresentation.Substring(idRepresentation.LastIndexOf('-') + 1));
			}
			return id;
		}
		private /*static*/ List<int> readArrayDimensions(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != xmlReader.GetAttribute("type"), "The 'type' attribute does not exist.");

			List<int> dimensionLengths = null; // Return value

			string typeName = xmlReader.GetAttribute("type");
			if (!String.IsNullOrEmpty(typeName))
				dimensionLengths = new TypeParser().GetArrayDimensions(typeName);

			return dimensionLengths;
		}
		private /*static*/ Type readType(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			string typeRepresentation = xmlReader.GetAttribute("type");

			// Is still here for backward compatibility
			bool withTypeDetails = this.hasTypeDetails(xmlReader);
			if (withTypeDetails) {
				xmlReader.ReadStartElement();
				this.readTypeDetails(xmlReader);
			}
			else {
				if (!xmlReader.IsEmptyElement)
					xmlReader.ReadStartElement();
			}

			return new TypeParser().Resolve(typeRepresentation); ;
		}
		// Still here for backward compatibility
		private /*static*/ bool hasTypeDetails(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			bool hasTypeDetails = false; // Return value
			string withTypeDetailsRepresentation = xmlReader.GetAttribute("withTypeDetails");
			if (!String.IsNullOrEmpty(withTypeDetailsRepresentation))
				hasTypeDetails = bool.Parse(withTypeDetailsRepresentation);

			return hasTypeDetails;
		}
		// Still here for backward compatibility
		private /*static*/ void readTypeDetails(XmlReader xmlReader)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");

			string typeName = xmlReader.GetAttribute("name");
			Type type = new TypeParser().Resolve(typeName);
			if (type.IsGenericTypeDefinition) {
				xmlReader.ReadStartElement("Type");
				while (xmlReader.IsStartElement("Type")) {
					this.readTypeDetails(xmlReader);
					if (xmlReader.IsStartElement("Type"))
						xmlReader.ReadStartElement();
				}
			}

			if (!xmlReader.IsEmptyElement)
				xmlReader.ReadEndElement();
		}

		private /*static*/ object readValue(XmlReader xmlReader, Type type, IFormatterConverter converter)
		{
			System.Diagnostics.Debug.Assert(null != xmlReader, "The 'xmlReader' argument cannot be null.");
			System.Diagnostics.Debug.Assert(null != type, "The 'type' argument cannot be null.");
			System.Diagnostics.Debug.Assert(this.isSimpleType(type), "The Type type is not a simple type.");
			System.Diagnostics.Debug.Assert(null != converter, "The 'converter' argument cannot be null.");

			object value = null; // Return value			
			string valueRepresentation = xmlReader.ReadString();
			if (type.IsPrimitive || typeof(System.String) == type) {
				value = converter.Convert(valueRepresentation, type);
			}
			else if (type.IsEnum) {
				value = Enum.Parse(type, valueRepresentation);
			}
			else if (typeof(System.Guid) == type) {
				value = new GuidConverter().ConvertFromInvariantString(valueRepresentation);
			}

			// When the value is the empty string the xml element is empty and there is not an xml end element.
			// Read the following element.
			if (xmlReader.IsEmptyElement)
				xmlReader.ReadStartElement();

			return value;
		}

		#endregion Private Methods
	}
}
