using System;
using System.Runtime.Remoting.Proxies;

namespace Arebis.Mocking {

	/// <summary>
	/// Indicates that custom mock objects will be created automatically for this class.
	/// Custom mock objects are mock objects that forward their calls to customMockers.
	/// </summary>
	/// <remarks>
	/// Classes decorated by this attribute must inherit from ContextBoundObject.
	/// </remarks>
	public class CustomMockAttribute : ProxyAttribute {

		private bool enabledByDefault = false;
		private string customMockerTypeName;
		private Type customMockerType;

		/// <summary>
		/// Initializes a new instance of the CustomMock attribute which is disabled by default.
		/// </summary>
		/// <param name="customMockerTypeName">Assembly qualified name of the type of the mocker to use.</param>
		public CustomMockAttribute(string customMockerTypeName) : this(customMockerTypeName, false) {
		}

		/// <summary>
		/// Initializes a new instance of the CustomMock attribute.
		/// </summary>
		/// <param name="customMockerTypeName">Assembly qualified name of the type of the mocker to use.</param>
		/// <param name="enabledByDefault">Whether the mocking should be enabled by default.
		/// When enabled by default, there is no need to register the type as 'to mock' with
		/// the RecorderManager.</param>
		public CustomMockAttribute(string customMockerTypeName, bool enabledByDefault) {
			this.customMockerTypeName = customMockerTypeName;
			this.enabledByDefault = enabledByDefault;
		}

		/// <summary>
		/// Initializes a new instance of the CustomMock attribute which is disabled by default.
		/// </summary>
		/// <param name="customMockerType">The type of the mocker to use.</param>
		public CustomMockAttribute(Type customMockerType) : this(customMockerType, false) {
		}

		/// <summary>
		/// Initializes a new instance of the CustomMock attribute.
		/// </summary>
		/// <param name="customMockerType">The type of the mocker to use.</param>
		/// <param name="enabledByDefault">Whether the mocking should be enabled by default.
		/// When enabled by default, there is no need to register the type as 'to mock' with
		/// the RecorderManager.</param>
		public CustomMockAttribute(Type customMockerType, bool enabledByDefault) {
			this.customMockerType = customMockerType;
			this.enabledByDefault = enabledByDefault;
		}

		/// <summary>
		/// Whether the mocking should be enabled by default.
		/// When enabled by default, there is no need to register the type as 'to mock' with
		/// the RecorderManager.
		/// </summary>
		public bool EnabledByDefault {
			get {
				return this.enabledByDefault;
			}
			set {
				this.enabledByDefault = value;
			}
		}
		
		/// <summary>
		/// Creates a mocked or unmocked instance of the given serverType.
		/// </summary>
		/// <param name="serverType">The type for which to create an instance.
		/// Should be the type decorated by this attribute.</param>
		public override MarshalByRefObject CreateInstance(Type serverType) {
			if (RecorderManager.Action == RecorderState.None)
				return CreateUnmockedInstance(serverType);
			else if (enabledByDefault || IsTypeToMock(serverType))
				return CreateMockedInstance(serverType);
			else
				return CreateUnmockedInstance(serverType);
		}

		/// <summary>
		/// Creates a mocked instance for the given serverType.
		/// </summary>
		/// <param name="serverType">The type for which to create an instance.
		/// Should be the type decorated by this attribute.</param>
		protected virtual MarshalByRefObject CreateMockedInstance(Type serverType) {
			IMocker customMocker;
			// Retrieve customMockerType from name:
			if (this.customMockerType == null)
				this.customMockerType = Type.GetType(customMockerTypeName);
			// Check customMockerType exists:
			if (this.customMockerType == null)
				throw new TypeLoadException(String.Format("The typename \"{0}\" configured as mocker on a CustomMockAttribute could not be resolved.", this.customMockerTypeName));
			// Create custom mocker:
			customMocker = (IMocker)customMockerType.GetConstructor(new Type[] {}).Invoke(new object[] {});
			// Create proxy, return transparent proxy:
			RealProxy rp;
			if (RecorderManager.IsRecording) {
				rp = new MockingProxy(serverType, customMocker, RecorderManager.GetNextInstanceName(serverType));
			} else {
				rp = new MockingProxy(serverType, customMocker, null);
			}
			return (MarshalByRefObject)rp.GetTransparentProxy();
		}
	
		/// <summary>
		/// Creates an unmocked instance of the given serverType.
		/// </summary>
		/// <param name="serverType">The type for which to create an instance.
		/// Should be the type decorated by this attribute.</param>
		protected MarshalByRefObject CreateUnmockedInstance(Type serverType) {
			return (MarshalByRefObject)base.CreateInstance(serverType);
		}

		/// <summary>
		/// Checks whether or not instances of the given serverType should be mocked.
		/// </summary>
		/// <param name="serverType">The type for which to create an instance.
		/// Should be the type decorated by this attribute.</param>
		protected virtual bool IsTypeToMock(Type serverType) {
			return MockService.IsTypeToMock(serverType);
		}
	}
}
