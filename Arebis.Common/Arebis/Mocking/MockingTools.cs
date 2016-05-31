using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;

namespace Arebis.Mocking {

	/// <summary>
	/// MockingTools contains several static methods to assist the implementation
	/// of the mocking framework and mocking framework extensions.
	/// </summary>
	public sealed class MockingTools {

		private MockingTools() {
			throw new InvalidOperationException("MockingTools is not to be instantiated.");
		}

		/// <summary>
		/// Builds a method call return message based on an IMethodReturnMessage from the .NET framework.
		/// </summary>
		/// <remarks>
		/// This method is to be used during recording phase only. A .NET return message object
		/// is constructed based on the given information, where all non-serializable return
		/// values and output arguments are mocked for recording.
		/// </remarks>
		public static IMethodReturnMessage BuildReturnMessage(IMethodReturnMessage returnMessage, IMethodCallMessage callMessage) {
			if (returnMessage.Exception != null) {
				return BuildReturnMessage(returnMessage.Exception, callMessage);
			} else {
				// Build arguments array:
				object[] arguments = new object[callMessage.ArgCount];
				// Fill with input arguments:
				int inargpos = 0;
				foreach(ParameterInfo param in callMessage.MethodBase.GetParameters()) {
					if (!param.IsOut) {
						arguments[param.Position] = callMessage.InArgs[inargpos++];
					}
				}
				// Fill with output arguments:
				int outargpos = 0;
				foreach(ParameterInfo param in returnMessage.MethodBase.GetParameters()) {
					if (param.ParameterType.IsByRef) {
						arguments[param.Position] = returnMessage.OutArgs[outargpos++];
					}
				}
				// Delegate to overloaded variant:
				return BuildReturnMessage(returnMessage.ReturnValue, arguments, callMessage);
			}
		}

		/// <summary>
		/// Builds a method call return message given the returnvalue, out arguments and methodcall.
		/// </summary>
		/// <param name="returnValue">Return value of the methodcall.</param>
		/// <param name="arguments">Input and output argument values.</param>
		/// <param name="callMessage">The original methodcall object.</param>
		/// <remarks>
		/// This method is to be used during recording phase only. A .NET return message object
		/// is constructed based on the given information, where all non-serializable return
		/// values and output arguments are mocked for recording.
		/// </remarks>
		public static IMethodReturnMessage BuildReturnMessage(object returnValue, object[] arguments, IMethodCallMessage callMessage) {
			// Build return message:
			IConstructionCallMessage ccm = callMessage as IConstructionCallMessage;
			if (ccm != null) {
				// If constructor message, build returnmessage from construction:
				return EnterpriseServicesHelper.CreateConstructionReturnMessage(ccm, (MarshalByRefObject)returnValue);
			} else {
				// Wrap return value:
				object wrappedReturnValue;
				wrappedReturnValue = WrapObject(returnValue, ((MethodInfo)callMessage.MethodBase).ReturnType);
				// Copy arguments, wrapping output arguments:
				int outArgsCount = 0;
				object[] wrappedArgs = new object[arguments.Length];
				foreach(ParameterInfo param in callMessage.MethodBase.GetParameters()) {
					if (param.ParameterType.IsByRef) {
						wrappedArgs[param.Position] = WrapObject(arguments[param.Position], param.ParameterType);
						outArgsCount++;
					} else {
						wrappedArgs[param.Position] = arguments[param.Position];
					}
				}
				// Build return message:
				return new ReturnMessage(wrappedReturnValue, wrappedArgs, outArgsCount, callMessage.LogicalCallContext, callMessage);
			}
		}

		/// <summary>
		/// Builds a method call return message returning an exception.
		/// </summary>
		/// <remarks>
		/// This method is to be used during recording phase only. A .NET return message object
		/// is constructed based on the given information, where all non-serializable return
		/// values and output arguments are mocked for recording.
		/// </remarks>
		public static IMethodReturnMessage BuildReturnMessage(Exception ex, IMethodCallMessage callMessage) {
			return new ReturnMessage(ex, callMessage);
		}

		/// <summary>
		/// Constructs an object or a mocker (depending on the mocking framework state).
		/// </summary>
		/// <param name="instanceName">The instancename of the mock.</param>
		/// <param name="typeToConstruct">The type of object to construct.</param>
		/// <param name="typeOfMock">The type of mock to create (should be either the typeToConstruct if it is MarshalByRef, or an interface it supports).</param>
		/// <param name="constructorArguments">Arguments of the constructor.</param>
		/// <returns>Either an instance of the typeToConstruct, or a mock for it.</returns>
		/// <remarks>
		/// This method will create either a plain instance of typeToConstruct, a recording
		/// mock wrapping a plain instance of typeToConstruct, or a playback mock with no plain instance.
		/// Use this method as an alternative to factory implementations, where you need to be
		/// able to mock an instance created, but can't use the AutoMock or CustomMock attributes.
		/// Note that the typeToConstruct type must be registered to the MockService as a type to mock !
		/// </remarks>
		public static object Construct(string instanceName, Type typeToConstruct, Type typeOfMock, params object[] constructorArguments) {
			object result = null;
			if ((RecorderManager.IsPlaying) && MockService.IsTypeToMock(typeToConstruct)) {
				// If playback, no instance to create, just a playback mock:
				result = (new MockingProxy(typeOfMock, new PlayBackMocker(), instanceName)).GetTransparentProxy();
			} else {
				// Otherwise, a real instance should be created:
				result = typeToConstruct.InvokeMember(null, BindingFlags.CreateInstance, null, null, constructorArguments);
				if ((RecorderManager.IsRecording) && MockService.IsTypeToMock(typeToConstruct)) {
					// If recording, wrap the instance in a recording mock:
					result = (new MockingProxy(typeOfMock, new RecordingMocker(result), instanceName)).GetTransparentProxy();
				}
			}
			return result;
		}

		/// <summary>
		/// If the passed value is serializable, returns it as is, otherwise wraps it in a recording proxy.
		/// </summary>
		private static object WrapObject(object value, Type expectedType) {
			if (value == null) {
				// Leave result as is
				return null;
			} else if (RemotingServices.IsObjectOutOfAppDomain(value)) {
				// Wrap the result in a RecordingProxy on the method's returntype:
				return new MockingProxy(expectedType, new RecordingMocker((MarshalByRefObject)value), RecorderManager.GetNextInstanceName(expectedType)).GetTransparentProxy();
			} else if (value.GetType().IsMarshalByRef) {
				// Wrap the result in a RecordingProxy on the objects real type:
				return new MockingProxy(value.GetType(), new RecordingMocker((MarshalByRefObject)value), RecorderManager.GetNextInstanceName(value.GetType())).GetTransparentProxy();
			} else if (value.GetType().IsSerializable) {
				// Leave result as is
				return value;
			} else {
				// When result is neither serializable, nor MarchalByRef, throw:
				throw new TypeMockException(value.GetType());
			}
		}
	}
}
