using System;
using System.Collections;

namespace Arebis.Mocking {

	/// <summary>
	/// MockService provides general services for applying mocking.
	/// </summary>
	public sealed class MockService {

		private static ArrayList locallyTypesToMock = new ArrayList();
		private static ArrayList globallyTypesToMock = new ArrayList();

		private MockService() {
			throw new InvalidOperationException("MockService is not to be instantiated.");
		}

		/// <summary>
		/// Registers a type that is not mocked by default, as to mock in the
		/// currently running recording or playback session.
		/// </summary>
		/// <remarks>
		/// The type must be decorated with one of the available mock attributes
		/// in order for mocking to be effectively done.
		/// </remarks>
		public static void AddTypeToMock(Type type) {
			if (RecorderManager.Action == RecorderState.None) {
				throw new InvalidOperationException("Types to mock can only be added inside a running playback or recording session.");
			}
			locallyTypesToMock.Add(type);
		}

		/// <summary>
		/// Registers a type that is not mocked by default, as to mock in all
		/// next recording or playback recorder sessions.
		/// </summary>
		/// <remarks>
		/// The type must be decorated with one of the available mock attributes
		/// in order for mocking to be effectively done.
		/// </remarks>
		public static void AddGlobalTypeToMock(Type type) {
			if (RecorderManager.Action != RecorderState.None) {
				throw new InvalidOperationException("Global types to mock can only be added outside a running playback or recording session.");
			}
			globallyTypesToMock.Add(type);
		}

		/// <summary>
		/// Resets the list of types to mock in the currently running recording
		/// or playback session.
		/// </summary>
		/// <remarks>
		/// This should only be done by the RecorderManager at the end of a session.
		/// </remarks>
		internal static void ResetSession() {
			locallyTypesToMock = new ArrayList();
		}

		/// <summary>
		/// Checks whether the given type is to be mocked in the currently
		/// running recording or playback session.
		/// </summary>
		/// <remarks>
		/// This method checks whether the given type is to be mocked in the
		/// currently running session, either because the type is to be mocked
		/// by default, or because it is registered as to be mocked in the
		/// currently running session.
		/// </remarks>
		public static bool IsTypeToMock(Type type) {
			// Check if the type has a default-enabled mock attribute:
			object[] mockAttribs = type.GetCustomAttributes(typeof(CustomMockAttribute), true);
			foreach(CustomMockAttribute cma in mockAttribs) if (cma.EnabledByDefault) return true;
			// Check if the type has been added to the types to mock:
			foreach(Type t in locallyTypesToMock) if (t.IsAssignableFrom(type)) return true;
			foreach(Type t in globallyTypesToMock) if (t.IsAssignableFrom(type)) return true;
			// False if all checks failed:
			return false;
		}

		/// <summary>
		/// Tells whether the given object is mocked.
		/// </summary>
		/// <remarks>
		/// The method checks if the object is backed by a MockingProxy. Both
		/// automocked and custom mocked objects are backed by a MockingProxy.
		/// </remarks>
		public static bool IsMock(object obj) {
			if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(obj)) {
				return (System.Runtime.Remoting.RemotingServices.GetRealProxy(obj) is MockingProxy);
			} else {
				return false;
			}
		}

		/// <summary>
		/// Returns the instance name of the given mock instance.
		/// If the passed object is not a mock, null is returned.
		/// </summary>
		public static string GetInstanceName(object mock) {
			if (System.Runtime.Remoting.RemotingServices.IsTransparentProxy(mock)) {
				MockingProxy proxy = System.Runtime.Remoting.RemotingServices.GetRealProxy(mock) as MockingProxy;
				if (proxy == null) {
					return null;
				} else {
					return proxy.InstanceName;
				}
			} else {
				return null;
			}
		}
	}
}
