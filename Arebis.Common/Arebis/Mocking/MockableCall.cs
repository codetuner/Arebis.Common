using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Activation;
using System.Runtime.Serialization;
using System.Text;

namespace Arebis.Mocking {

	/// <summary>
	/// Represents the mockable call to a method. A MockableCall can stand for a regular
	/// method call, a constructor call, or a get or set of a property.
	/// </summary>
	/// <remarks>MockableCall is a serializable class.</remarks>
	[Serializable]
	//[DebuggerDisplayAttribute("MockableCall: {MethodSignature}")]
	public class MockableCall : ISerializable {

		[NonSerialized] private IMethodCallMessage originalCall;
		private MockingProxy callee;
		private MethodBase method;
		private object[] inArgs;
		private object[] outArgs;
		private object returnValue;
		private Exception exception;
		[NonSerialized] private int callCreationTime;
		private int callDuration = -1;
		private bool isConstructorCall;
		private bool isCompleted;


		#region Constructors

		/// <summary>
		/// Instantiates a new MockableCall object based on a IMethodCallMessage.
		/// </summary>
		public MockableCall(MockingProxy callee, IMethodCallMessage callmsg) {
			if (callee == null) throw new ArgumentNullException("callee");
			if (callmsg == null) throw new ArgumentNullException("callmsg");
			this.originalCall = callmsg;
			this.callCreationTime = Environment.TickCount;
			this.callee = callee;
			this.method = callmsg.MethodBase;
			this.inArgs = callmsg.InArgs;
			this.isConstructorCall = (callmsg is IConstructionCallMessage);
			this.isCompleted = false;
		}

		/// <summary>
		/// Instantiates a new MockableCall object based on a MethodBase.
		/// </summary>
		/// <param name="callee">The proxy of the called object.</param>
		/// <param name="method">The method of this call.</param>
		/// <param name="arguments">The arguments passed to the methodcall (output arguments must be provided but they can have any value).</param>
		public MockableCall(MockingProxy callee, System.Reflection.MethodBase method, object[] arguments){
			if (callee == null) throw new ArgumentNullException("callee");
			if (method == null) throw new ArgumentNullException("method");
			this.originalCall = null;
			this.callCreationTime = Environment.TickCount;
			this.callee = callee;
			this.method = method;
			this.inArgs = new object[this.GetInParameters().Length];
			if (arguments != null) {
				int i = 0;
				foreach(ParameterInfo param in this.GetInParameters()) {
					this.inArgs[i++] = arguments[param.Position];
				}
			}
			this.isConstructorCall = (method.IsConstructor);
			this.isCompleted = false;
		}

		#endregion

		#region Accessors

		/// <summary>
		/// The original call object from the .NET Framework.
		/// </summary>
		public System.Runtime.Remoting.Messaging.IMethodCallMessage OriginalCall {
			get {
				return this.originalCall;
			}
		}

		/// <summary>
		/// The instance that receives the call.
		/// </summary>
		public virtual MockingProxy Callee {
			get {
				return this.callee;
			}
		}
	
		/// <summary>
		/// The method that is called.
		/// </summary>
		public virtual System.Reflection.MethodBase Method {
			get {
				return this.method;
			}
		}

		/// <summary>
		/// Returns the method signature string.
		/// </summary>
		public string MethodSignature {
			get {
				System.Text.StringBuilder sig = new System.Text.StringBuilder();
				string argseparator = "";
				if (this.isConstructorCall) {
					ConstructorInfo ci = (ConstructorInfo)this.method;
					sig.Append(ci.DeclaringType.Name);
				} else {
					MethodInfo mi = (MethodInfo)this.method;
					sig.Append(mi.ReturnType.Name);
					sig.Append(' ');
					sig.Append(mi.Name);
				}
				sig.Append("(");
				foreach(ParameterInfo param in this.method.GetParameters()) {
					sig.Append(argseparator);
					if (param.IsIn) sig.Append("in ");
					if (param.IsOut) sig.Append("out ");
					sig.Append(param.ParameterType.Name);
					sig.Append(' ');
					sig.Append(param.Name);
					argseparator = ", ";
				}
				sig.Append(")");
				return sig.ToString();
			}
		}
	
		/// <summary>
		/// Whether the called method is a constructor.
		/// </summary>
		public virtual bool IsConstructorCall {
			get {
				return this.isConstructorCall;
			}
		}

		/// <summary>
		/// The input arguments of the call.
		/// </summary>
		public virtual object[] InArgs {
			get {
				return this.inArgs;
			}
		}

		/// <summary>
		/// The output arguments of the call.
		/// </summary>
		public virtual object[] OutArgs {
			get {
				return this.outArgs;
			}
		}

		/// <summary>
		/// The arguments of the call (input and output arguments combined).
		/// </summary>
		public virtual object[] Args {
			get {
				int i;
				// Prepare array:
				object[] args = new object[this.GetParameters().Length];
				// Copy input arguments:
				i = 0;
				foreach(ParameterInfo param in GetInParameters()) {
					args[param.Position] = this.inArgs[i++];
				}
				// Overwrite with output arguments if yet available:
				if (this.outArgs != null) {
					i = 0;
					foreach(ParameterInfo param in GetOutParameters()) {
						args[param.Position] = this.outArgs[i++];
					}
				}
				// Return result:
				return args;
			}
		}

		/// <summary>
		/// The returnvalue of the call.
		/// </summary>
		public virtual object ReturnValue {
			get {
				return this.returnValue;
			}
		}
	
		/// <summary>
		/// The exception the call raises.
		/// </summary>
		public virtual System.Exception Exception {
			get {
				return this.exception;
			}
		}

		/// <summary>
		/// The duration of the recorded call in milliseconds.
		/// </summary>
		public virtual int CallDuration {
			get {
				return this.callDuration;
			}
		}

		/// <summary>
		/// Returns whether the call has completed.
		/// </summary>
		/// <remarks>Asynchonous calls and playback calls are initially not
		/// completed and will be completed by setting their result.</remarks>
		public virtual bool IsCompleted {
			get {
				return this.isCompleted;
			}
		}


		/// <summary>
		/// Returns information about all parameters (input and output).
		/// </summary>
		public virtual ParameterInfo[] GetParameters() {
			return this.Method.GetParameters();
		}
		
		
		/// <summary>
		/// Returns information about the input parameters.
		/// </summary>
		public virtual ParameterInfo[] GetInParameters() {
			ArrayList paraminfos = new ArrayList();
			foreach(ParameterInfo paraminfo in this.Method.GetParameters()) {
				if (!paraminfo.IsOut) paraminfos.Add(paraminfo);
			}
			return (ParameterInfo[])paraminfos.ToArray(typeof(ParameterInfo));
		}


		/// <summary>
		/// Returns information about the output parameters.
		/// </summary>
		public virtual ParameterInfo[] GetOutParameters() {
			ArrayList paraminfos = new ArrayList();
			foreach(ParameterInfo paraminfo in this.Method.GetParameters()) {
				if ((paraminfo.ParameterType.IsByRef)) paraminfos.Add(paraminfo);
			}
			return (ParameterInfo[])paraminfos.ToArray(typeof(ParameterInfo));
		}

		/// <summary>
		/// Whether the given parameter is an input or input-output parameter.
		/// </summary>
		/// <param name="parameterIndex">The index of the parameter.</param>
		public virtual bool IsParameterIn(int parameterIndex) {
			return !this.Method.GetParameters()[parameterIndex].IsOut;
		}

		/// <summary>
		/// Whether the given parameter is an output or input-output parameter.
		/// </summary>
		/// <param name="parameterIndex">The index of the parameter.</param>
		public virtual bool IsParameterOut(int parameterIndex) {
			return this.Method.GetParameters()[parameterIndex].ParameterType.IsByRef;
		}

		/// <summary>
		/// Returns the type the method of this call returns.
		/// </summary>
		public virtual Type GetReturnType() {
			if (this.IsConstructorCall) {
				return this.Callee.ServerType;
			} else {
				MethodInfo meth = this.Method as System.Reflection.MethodInfo;
				if (meth != null) {
					return meth.ReturnType;
				} else {
					return null;
				}
			}
		}

		#endregion

		#region Setting results

		/// <summary>
		/// Set the result of the call by it's return message.
		/// </summary>
		public virtual void SetResult(IMethodReturnMessage returnmsg) {
			if (returnmsg.Exception != null) {
				this.SetExceptionResult(returnmsg.Exception);
			} else if (this.IsConstructorCall) {
				this.SetConstructionResult(callee.InstanceName, returnmsg.OutArgs);
			} else {
				this.SetCallResult(returnmsg.ReturnValue, returnmsg.OutArgs);
			}
		}

		/// <summary>
		/// Set the result of the call by copying the result of another call.
		/// </summary>
		public virtual void SetResult(MockableCall call) {
			if (call.Exception != null) {
				this.SetExceptionResult(call.Exception);
			} else if (this.IsConstructorCall) {
				this.SetConstructionResult(call.callee.InstanceName, call.OutArgs);
			} else {
				this.SetCallResult(call.ReturnValue, call.OutArgs);
			}
		}

		/// <summary>
		/// Set the result of construction call.
		/// </summary>
		/// <param name="instanceName">Name to be assigned to the created instance.</param>
		// Not to be virtual, as merely forwaring to virtual method.
		public void SetConstructionResult(string instanceName) {
			SetConstructionResult(instanceName, new object[] {});
		}

		/// <summary>
		/// Set the result of construction call.
		/// </summary>
		/// <param name="instanceName">Name to be assigned to the created instance.</param>
		/// <param name="outArgs">The values of output arguments returned by the call.</param>
		public virtual void SetConstructionResult(string instanceName, object[] outArgs) {
			// Set return values:
			this.callee.InstanceName = instanceName;
			this.returnValue = callee.GetTransparentProxy();
			this.outArgs = outArgs;
			this.callDuration = Environment.TickCount - callCreationTime;
			this.isCompleted = true;
		}

		/// <summary>
		/// Set the call completed without return value.
		/// </summary>
		/// <remarks>Use this method to mark a void() call complete.</remarks>
		// Not to be virtual, as merely forwaring to virtual method.
		public void SetCallResult() {
			this.SetCallResult(null);
		}

		/// <summary>
		/// Set the call completed, returning the given output arguments.
		/// </summary>
		/// <param name="outArgs">The values of output arguments returned by the call.</param>
		public virtual void SetCallResult(object[] outArgs) {
			this.SetCallResult(null, outArgs);
		}

		/// <summary>
		/// Set the call return value.
		/// </summary>
		/// <param name="returnValue">The value returned by the call.</param>
		// Not to be virtual, as merely forwaring to virtual method.
		public void SetCallResult(object returnValue) {
			this.SetCallResult(returnValue, new object[] {});
		}

		/// <summary>
		/// Set the call return value.
		/// </summary>
		/// <param name="returnValue">The value returned by the call.</param>
		/// <param name="outArgs">The values of output arguments returned by the call.</param>
		public virtual void SetCallResult(object returnValue, object[] outArgs) {
			this.returnValue = returnValue;
			this.outArgs = outArgs;
			this.callDuration = Environment.TickCount - callCreationTime;
			this.isCompleted = true;
		}

		/// <summary>
		/// Set the exception thrown by the call.
		/// </summary>
		/// <param name="ex">The exception thrown by the call.</param>
		public virtual void SetExceptionResult(Exception ex) {
			this.returnValue = null;
			this.outArgs = new object[] {};
			this.exception = ex;
			this.callDuration = Environment.TickCount - callCreationTime;
			this.isCompleted = true;
		}

		#endregion

		#region Serialization

		private MockableCall(SerializationInfo info, StreamingContext context) {
			// Retrieve serialization version:
			string assemblyVersion = info.GetString("assemblyVersion");
			decimal serializationVersion = info.GetDecimal("serializationVersion");
			// Retrieve callee:
			this.callee = new MockingProxy(Type.GetType(info.GetString("calleeType")), new PlayBackMocker(), info.GetString("calleeInstanceName"));
			// Retrieve isConstructorCall:
			this.isConstructorCall = info.GetBoolean("isConstructorCall");
			// Retrieve method:
			Type methodType = Type.GetType(info.GetString("methodType"));
			string[] methodSignatureStr = (string[])info.GetValue("methodSignature", typeof(string[]));
			Type[] methodSignature = new Type[methodSignatureStr.Length];
			for(int i=0; i<methodSignatureStr.Length; i++) methodSignature[i] = Type.GetType(methodSignatureStr[i]);
			if (this.isConstructorCall) {
				this.method = methodType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodSignature, null);
			} else {
				this.method = methodType.GetMethod(info.GetString("methodName"), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodSignature, null);
			}
			// Retrieve inArgs:
			this.inArgs = (object[])info.GetValue("inArgs", typeof(object[]));
			object[] inArgsSubstitutions = (object[])info.GetValue("inArgsMockers", typeof(object[]));
			foreach(object[] subst in inArgsSubstitutions) {
				MockingProxy proxy = new MockingProxy(Type.GetType((string)subst[1]), new PlayBackMocker(), (string)subst[2]);
				this.inArgs[(int)subst[0]] = proxy.GetTransparentProxy();
			}
			// Retrieve outArgs:
			this.outArgs = (object[])info.GetValue("outArgs", typeof(object[]));
			object[] outArgsSubstitutions = (object[])info.GetValue("outArgsMockers", typeof(object[]));
			if (outArgs != null) {
				foreach(object[] subst in outArgsSubstitutions) {
					MockingProxy proxy = new MockingProxy(Type.GetType((string)subst[1]), new PlayBackMocker(), (string)subst[2]);
					this.outArgs[(int)subst[0]] = proxy.GetTransparentProxy();
				}
			}
			// Retrieve returnValue:
			bool returnValueMocked = info.GetBoolean("returnValueMocked");
			Type returnValueType = Type.GetType(info.GetString("returnValueType"));
			if (returnValueMocked) {
				MockingProxy proxy = new MockingProxy(Type.GetType(info.GetString("returnValueType")), new PlayBackMocker(), info.GetString("returnValueName"));
				this.returnValue = proxy.GetTransparentProxy();
			} else {
				this.returnValue = info.GetValue("returnValue", returnValueType);
			}
			// Retrieve exception:
			this.exception = (Exception)info.GetValue("exception", typeof(Exception));
			if (exception == null) {
				string exceptionType = info.GetString("exceptionType");
				if (exceptionType != null) {
					this.exception = (Exception)Type.GetType(exceptionType).GetConstructor(new Type[] {}).Invoke(new object[] {});
				}
			}
			// Retrieve iscompleted & duration:
			this.isCompleted = info.GetBoolean("isCompleted");
			this.callDuration = info.GetInt32("callDuration");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) {
			// Serialize serialization version:
			info.AddValue("assemblyVersion", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			info.AddValue("serializationVersion", 2.0m);
			// Serialize callee:
			info.AddValue("calleeType", TypeRef(callee.GetProxiedType()));
			info.AddValue("calleeInstanceName", callee.InstanceName);
			// Serialize method:
			info.AddValue("methodType", TypeRef(this.method.DeclaringType));
			info.AddValue("methodName", this.method.Name);
			ParameterInfo[] parameters = this.method.GetParameters();
			string[] methodSignatureStr = new string[parameters.Length];
			for(int i=0; i<parameters.Length; i++) methodSignatureStr[i] = TypeRef(parameters[i].ParameterType);
			info.AddValue("methodSignature", methodSignatureStr);
			// Serialize isConstructorCall:
			info.AddValue("isConstructorCall", this.isConstructorCall);
			// Serialize inArgs:
			object[] inArgsSubstituted;
			object[] inArgsSubstitutions;
			SubstituteMockers(this.inArgs, out inArgsSubstituted, out inArgsSubstitutions);
			info.AddValue("inArgs", inArgsSubstituted);
			info.AddValue("inArgsMockers", inArgsSubstitutions);
			// Serialize outArgs:
			object[] outArgsSubstituted;
			object[] outArgsSubstitutions;
			if (outArgs == null) {
				outArgsSubstituted = null;
				outArgsSubstitutions = null;
			} else {
				SubstituteMockers(this.outArgs, out outArgsSubstituted, out outArgsSubstitutions);
			}
			info.AddValue("outArgs", outArgsSubstituted);
			info.AddValue("outArgsMockers", outArgsSubstitutions);
			// Serialize returnValue:
			if (this.returnValue == null) {
				info.AddValue("returnValueMocked", false);
				info.AddValue("returnValueType", TypeRef(this.GetReturnType()));
				info.AddValue("returnValue", null);
			} else if (RemotingServices.IsTransparentProxy(this.returnValue)) {
				MockingProxy rp = RemotingServices.GetRealProxy(this.returnValue) as MockingProxy;
				info.AddValue("returnValueMocked", true);
				info.AddValue("returnValueType", TypeRef(rp.ServerType));
				info.AddValue("returnValueName", rp.InstanceName);
			} else {
				info.AddValue("returnValueMocked", false);
				info.AddValue("returnValueType", TypeRef(this.GetReturnType()));
				info.AddValue("returnValue", this.returnValue);
			}
			// Serialize exception:
			if (this.exception == null) {
				info.AddValue("exception", null);
				info.AddValue("exceptionType", null);
			} else if (this.exception.GetType().IsSerializable) {
				info.AddValue("exception", this.exception);
				info.AddValue("exceptionType", null);
			} else {
				info.AddValue("exception", null);
				info.AddValue("exceptionType", TypeRef(this.exception.GetType()));
			}
			// Serialize icompleted & duration:
			info.AddValue("isCompleted", this.isCompleted);
			info.AddValue("callDuration", this.callDuration);
		}

		/// <summary>
		/// Returns a string refering to the given type such that passing this string to
		/// Type.GetType() should return the current version of the passed type.
		/// </summary>
		private static string TypeRef(Type t) {
			StringBuilder sb = new StringBuilder();
			TypeRefInternal(t, sb);
			return sb.ToString();
		}

		/// <summary>
		/// Internal implementation of TypeRef, supports recursive calls for generic types.
		/// </summary>
		private static void TypeRefInternal(Type t, StringBuilder sb)
		{
			Type t2 = (t.IsArray) ? t.GetElementType() : t;
			if (t2.IsGenericType)
			{
				sb.Append(t2.GetGenericTypeDefinition().FullName);
				string sep = "";
				sb.Append("[[");
				foreach (Type gt in t2.GetGenericArguments())
				{
					sb.Append(sep);
					TypeRefInternal(gt, sb);
					sep = "],[";
				}
				sb.Append("]]");
			}
			else
			{
				sb.Append(t2.FullName);
			}
			if (t.IsArray)
			{
				sb.Append("[]");
			}
			sb.Append(", ");
			sb.Append(t2.Assembly.GetName().Name);
		}

		/// <summary>
		/// Receiving an array of values, it returns an array of the same values where
		/// mockers are replaced by null (substitutedValues), and an array of substitutions
		/// indicating the positions, typenames and instancenames of the mockers (substitutions).
		/// </summary>
		/// <param name="values">The original values.</param>
		/// <param name="substitutedValues">The substituted values (original values or null for mockers).</param>
		/// <param name="substitutions">Position, type and name information of mockers.</param>
		private static void SubstituteMockers(object[] values, out object[] substitutedValues, out object[] substitutions) {
			ArrayList substitutionsTemp = new ArrayList();
			substitutedValues = new object[values.Length];
			for(int i=0; i<values.Length; i++) {
				if (values[i] == null) {
					substitutedValues[i] = null;
				} else if (RemotingServices.IsTransparentProxy(values[i])) {
					substitutedValues[i] = null;
					MockingProxy rp = RemotingServices.GetRealProxy(values[i]) as MockingProxy;
					substitutionsTemp.Add(new object[]{i, TypeRef(rp.ServerType), rp.InstanceName});
				} else if (values[i].GetType().IsSerializable) {
					substitutedValues[i] = values[i];
				} else {
					substitutedValues[i] = null;
				}
			}
			substitutions = substitutionsTemp.ToArray();
		}

		#endregion

	}
}
