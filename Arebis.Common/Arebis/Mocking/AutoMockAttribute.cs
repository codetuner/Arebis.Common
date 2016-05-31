using System;
using System.Runtime.Remoting.Proxies;

namespace Arebis.Mocking
{

	/// <summary>
	/// Indicates that recording/playback mock objects will be created automatically for this class.
	/// </summary>
	/// <remarks>
	/// Classes decorated by this attribute must inherit from ContextBoundObject.
	/// </remarks>
	public class AutoMockAttribute : CustomMockAttribute
	{
		/// <summary>
		/// Initializes a new instance of the AutoMock attribute which is disabled by default.
		/// </summary>
		public AutoMockAttribute() : this(false) {
		}

		/// <summary>
		/// Initializes a new instance of the AutoMock attribute.
		/// </summary>
		/// <param name="enabledByDefault">Whether the mocking should be enabled by default.
		/// When enabled by default, there is no need to register the type as 'to mock' with
		/// the RecorderManager.</param>
		public AutoMockAttribute(bool enabledByDefault) : base((Type)null, enabledByDefault) {
		}

		/// <summary>
		/// Creates an eventually mocked instance for the given serverType.
		/// </summary>
		/// <param name="serverType">The type for which to create an instance.
		/// Should be the type decorated by this attribute.</param>
		protected override MarshalByRefObject CreateMockedInstance(Type serverType) {
			RealProxy rp;
			switch (RecorderManager.Action) {
				case RecorderState.Recording:
					// Create a recording proxy for the object:
					MarshalByRefObject target = CreateUnmockedInstance(serverType);
					rp = new MockingProxy(serverType, new RecordingMocker(target), RecorderManager.GetNextInstanceName(serverType));
					return (MarshalByRefObject)rp.GetTransparentProxy();
				case RecorderState.PlayBack:
					// Create a playback proxy:
					rp = new MockingProxy(serverType, new PlayBackMocker(), null);
					return (MarshalByRefObject)rp.GetTransparentProxy();
				default:
					// Create an instance without proxy:
					return CreateUnmockedInstance(serverType);
			}
		}
	}
}
