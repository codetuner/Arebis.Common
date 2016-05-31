using System;
using System.Collections;

using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;

using Arebis.Mocking;

namespace Arebis.Testing.Mocking.Remoting
{

	/// <summary>
	/// The RemoteMockingSinkProvider is a FormatterSink provider for remoting channels
	/// that provides support for remote mocking.
	/// </summary>
	public class RemotingMockingSinkProvider : IClientFormatterSinkProvider {

		private IClientChannelSinkProvider next = null;

		/// <summary>
		/// Constructs a RemotingMockingSinkProvider.
		/// </summary>
		public RemotingMockingSinkProvider() {
		}

		/// <summary>
		/// Constructs a RemotingMockingSinkProvider.
		/// </summary>
		public RemotingMockingSinkProvider(IDictionary properties, ICollection providerData) {
		}

		/// <summary>
		/// Creates a sink chain.
		/// </summary>
		public IClientChannelSink CreateSink (IChannelSender channel, string url, object remoteChannelData) {
			// Create next sink:
			IClientChannelSink nextSink = next.CreateSink(channel,url,remoteChannelData);
			// Return current sink chained to next sink:
			IMessageSink nextMessageSink = nextSink as IMessageSink;
			if (nextMessageSink != null) {
				return new RemotingMockingSink(nextMessageSink);
			} else {
				throw new RemotingException(String.Format("Remoting configuration error : the '{0}' provider should be defined as provider in the clientProviders section of the channel, before any formatter.", this.GetType()));
			}
		}
	
		/// <summary>
		/// Gets or sets the next sink provider in the channel sink provider chain.
		/// </summary>
		public IClientChannelSinkProvider Next {
			get{
				return next;
			}
			set{
				next = value;
			}
		}
	}


	/// <summary>
	/// A FormatterSink for remoting channels that provides support for remote mocking.
	/// </summary>
	internal class RemotingMockingSink : IClientFormatterSink {

		private IMessageSink nextMessageSink;

        /// <summary>
        /// Constructs a RemotingMockingSink chained to it's next sink.
        /// </summary>
		public RemotingMockingSink(IMessageSink nextMessageSink) {
			this.nextMessageSink = nextMessageSink;
		}

		#region IMessageSink Members

		/// <summary>
		/// Process synchronous messages through the sink chain. Mocking can be applied here.
		/// </summary>
		public IMessage SyncProcessMessage(IMessage msg) {
			IMethodCallMessage mcm = (IMethodCallMessage)msg;
			IMethodReturnMessage result;
			if (RecorderManager.IsPlaying && RemotingMockService.IsUriToMock(mcm.Uri)) {
				MockingProxy proxy = new MockingProxy(Type.GetType(mcm.TypeName), new RemotingPlayBackMocker(), mcm.Uri);
				result = (IMethodReturnMessage)proxy.Invoke(msg);
			} else if (RecorderManager.IsRecording && !RecorderManager.IsInCall && RemotingMockService.IsUriToMock(mcm.Uri)) {
				MockingProxy proxy = new MockingProxy(Type.GetType(mcm.TypeName), null, mcm.Uri);
				MockableCall call = new MockableCall(proxy, mcm);
				using (new RecorderManager.Lock()) {
					result = SyncProcessMessageOnServer(mcm);
					call.SetResult(result);
					RecorderManager.RecordCall(call);
				}
			} else {
				result = (IMethodReturnMessage)nextMessageSink.SyncProcessMessage(msg);
			}
			return result;
		}

		private IMethodReturnMessage SyncProcessMessageOnServer(IMethodCallMessage mcm) {
			IMethodReturnMessage response;
			// Invoke instance method:
			response = (IMethodReturnMessage)nextMessageSink.SyncProcessMessage(mcm);
			// Return a wrapped result:
			return MockingTools.BuildReturnMessage(response, mcm);
		}

		/// <summary>
		/// Process asynchronous messages through the sink chain. Mocking can be applied here.
		/// </summary>
		public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink) {
			if (RecorderManager.IsPlaying) {
				MockableCall call = new MockableCall(null, (IMethodCallMessage)msg);
				AsyncCallHandler handler = new AsyncCallHandler(replySink, call);
				RecorderManager.RecordCall(call);


			} else if (RecorderManager.IsRecording) {
			
			} else {
				return NextSink.AsyncProcessMessage(msg, replySink);
			}
			return nextMessageSink.AsyncProcessMessage(
				msg,
				new AsyncCallHandler(
				replySink,
				new MockableCall(null, (IMethodCallMessage)msg)
				) 
				);
		}

		/// <summary>
		/// The next MessageSink in the chain.
		/// </summary>
		public IMessageSink NextSink {
			get {
				return nextMessageSink;
			}
		}

		private class AsyncCallHandler : IMessageSink {

			private IMessageSink next;
			private MockableCall call;

			public AsyncCallHandler(IMessageSink next, MockableCall call) {
				this.next = next;
				this.call = call;
			}

			#region IMessageSink Members

			public IMessage SyncProcessMessage(IMessage msg) {
				return next.SyncProcessMessage(msg);
			}

			public IMessageSink NextSink {
				get {
					return next;
				}
			}

			public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink) {
				// Return messages of asynchonous calls are synchonous !
				throw new NotSupportedException();
			}

			#endregion

		}


		#endregion

		#region IClientChannelSink Members

		/// <summary>
		/// Not supported. As this sink should be the first in the chain it only implmenets IMessageSink.
		/// </summary>
		public void AsyncProcessRequest(IClientChannelSinkStack sinkStack, IMessage msg, ITransportHeaders headers, System.IO.Stream stream) {
			// Should not be called if first in chain.
			throw new NotSupportedException();
		}

		/// <summary>
		/// Not supported. As this sink should be the first in the chain it only implmenets IMessageSink.
		/// </summary>
		public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, System.IO.Stream requestStream, out ITransportHeaders responseHeaders, out System.IO.Stream responseStream) {
			// Should not be called if first in chain.
			throw new NotSupportedException();
		}

		/// <summary>
		/// Not supported. As this sink should be the first in the chain it only implmenets IMessageSink.
		/// </summary>
		public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, System.IO.Stream stream) {
			// Should not be called if first in chain.
			throw new NotSupportedException();
		}

		/// <summary>
		/// Not supported. As this sink should be the first in the chain it only implmenets IMessageSink.
		/// </summary>
		public System.IO.Stream GetRequestStream(IMessage msg, ITransportHeaders headers) {
			throw new NotSupportedException();
		}

		/// <summary>
		/// The next ClientChannelSink in the chain.
		/// </summary>
		public IClientChannelSink NextChannelSink {
			get {
				return (IClientChannelSink)nextMessageSink;
			}
		}

		#endregion

		#region IChannelSinkBase Members

		/// <summary>
		/// Properties of the ChannelSink.
		/// </summary>
		public System.Collections.IDictionary Properties {
			get {
				return ((IChannelSinkBase)nextMessageSink).Properties;
			}
		}

		#endregion

	}



}
