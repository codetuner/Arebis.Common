using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security;

using AdvicesNode = System.Collections.Generic.KeyValuePair<Arebis.Runtime.Aspects.IAdvice, System.Collections.Generic.Dictionary<string, object>>;

namespace Arebis.Runtime.Aspects
{
    /// <summary>
    /// Marks a class as allowing advisable attributes. Advisable attributes intercept
    /// method calls and can perform code before or after method calls.
    /// </summary>
    /// <remarks>
    /// Classes marked with the Advisable attribute should inherit from System.ContextBoundObject.
    /// </remarks>
    /// <seealso cref="Arebis.Runtime.Aspects.Advisable"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    [SecurityCritical]
    public sealed class AdvisableAttribute : ContextAttribute
    {
        /// <summary>
        /// A default AdvisableAttribute.
        /// </summary>
        public AdvisableAttribute()
            : base(typeof(AdvisableAttribute).ToString())
        { }

        /// <summary>
        /// Wheter the original context is to be used (or a new is to be created).
        /// </summary>
        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
        [SecurityCritical]
        public override bool IsContextOK(Context ctx, IConstructionCallMessage ctorMsg)
        {
            // A new context is requires to activate interception,
            // so always request a new context:
            return false;
        }

        /// <summary>
        /// Adds properties for the context.
        /// </summary>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        [SecurityCritical]
        public override void GetPropertiesForNewContext(System.Runtime.Remoting.Activation.IConstructionCallMessage ctorMsg)
        {
            base.GetPropertiesForNewContext(ctorMsg);
            ctorMsg.ContextProperties.Add(new AdvisableContextProperty(this, ctorMsg.ActivationType));
        }
    }

    /// <summary>
    /// A context property for advice. Responsible for installing AdvisableMessageSinks.
    /// </summary>
    internal class AdvisableContextProperty : IContextProperty, IContributeServerContextSink
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private AdvisableAttribute ownerAttribute;
        private Type advicedType;

        [SecuritySafeCritical]
        internal AdvisableContextProperty(AdvisableAttribute ownerAttribute, Type advicedType)
        {
            this.ownerAttribute = ownerAttribute;
            this.advicedType = advicedType;
        }

        /// <summary>
        /// Gets the name of the property under which it will be added to the context.
        /// </summary>
        public string Name
        {
            [SecurityCritical]
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Called when the context is frozen.
        /// </summary>
        [SecurityCritical]
        public void Freeze(Context newContext)
        {
        }

        /// <summary>
        /// Returns a Boolean value indicating whether the context property is compatible
        /// with the new context.
        /// </summary>
        [SecurityCritical]
        public bool IsNewContextOK(Context newCtx)
        {
            return true;
        }

        /// <summary>
        /// Returnes an AdvisableMessageSink chained to the next sink.
        /// </summary>
        [SecurityCritical]
        public IMessageSink GetServerContextSink(IMessageSink nextSink)
        {
            return new AdvisableMessageSink(this, advicedType, nextSink);
        }
    }

    /// <summary>
    /// A MessageSink that intercepts method calls and calls the methodAdvisors.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal class AdvisableMessageSink : IMessageSink
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private AdvisableContextProperty ownerContextProperty;
        private Type advicedType;
        private WeakReference advicedInstance;
        private IMessageSink nextSink;

        internal AdvisableMessageSink(AdvisableContextProperty ownerContextProperty, Type advicedType, IMessageSink nextSink)
        {
            this.ownerContextProperty = ownerContextProperty;
            this.advicedType = advicedType;
            this.advicedInstance = new WeakReference(null);
            this.nextSink = nextSink;
        }

        /// <summary>
        /// Gets the next message sink in the sink chain.
        /// </summary>
        public IMessageSink NextSink
        {
            [SecurityCritical]
            get { return this.nextSink; }
        }

        /// <summary>
        /// Asynchronously processes the given message.
        /// </summary>
        [SecurityCritical]
        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return this.nextSink.AsyncProcessMessage(msg, replySink);
        }

        /// <summary>
        /// Synchronously processes the given message.
        /// </summary>
        [SecurityCritical]
        public IMessage SyncProcessMessage(IMessage msg)
        {
            List<AdvicesNode> beforeAdvices = new List<AdvicesNode>();
            List<AdvicesNode> afterAdvices = new List<AdvicesNode>();
            IMethodCallMessage callMessage = msg as IMethodCallMessage;
            IMethodReturnMessage returnMessage;

            // Retrieve methodBase:
            MethodBase methodBase = callMessage.MethodBase;
            bool isConstructionCall = (callMessage is IConstructionCallMessage);

            // Retrieve methodBase on target type if methodBase is on interface:
            if (methodBase.DeclaringType.IsInterface)
            {
                InterfaceMapping map = this.advicedType.GetInterfaceMap(methodBase.DeclaringType);
                for (int i = 0; i < map.InterfaceMethods.Length; i++)
                {
                    if (map.InterfaceMethods[i].Name == methodBase.Name)
                    {
                        methodBase = map.TargetMethods[i];
                        break;
                    }
                }
            }

            // Retrieve classAdvisors and build propertyContainers for them:
            foreach (IAdvice advice in this.advicedType.GetCustomAttributes(typeof(IAdvice), true))
                beforeAdvices.Add(new AdvicesNode(advice, new Dictionary<string, object>()));

            // Retrieve methodAdvisors and build propertyContainers for them:
            foreach (IAdvice advice in methodBase.GetCustomAttributes(typeof(IAdvice), true))
                beforeAdvices.Add(new AdvicesNode(advice, new Dictionary<string, object>()));

            // Create a callContext:
            CallContext callContext = new CallContext(callMessage, methodBase, this.advicedInstance.Target);

            // Call all advisors's BeforeCall methods:
            foreach (AdvicesNode advicePair in beforeAdvices)
            {
                if ((isConstructionCall) && (advicePair.Key.IncludeConstructorCalls == false)) continue;

                // Call BeforeCall with correct property container:
                callContext.SetPropertyContainer(advicePair.Value);
                advicePair.Key.BeforeCall(callContext);

                // Prepare advice for AfterCall handling:
                afterAdvices.Insert(0, advicePair);
            }

            // Dispatch call to next sink:
            returnMessage = (IMethodReturnMessage)this.nextSink.SyncProcessMessage(msg);

            // Update the callContext:
            callContext.SetReturnMessage(returnMessage);

            // Call advisors's AfterCall methods:
            foreach (AdvicesNode advicePair in afterAdvices)
            {
                // Call AfterCall with correct property container:
                callContext.SetPropertyContainer(advicePair.Value);
                advicePair.Key.AfterCall(callContext);
            }

            // If construction call, remember constructed instance:
            if ((isConstructionCall) && (returnMessage.Exception == null))
                this.advicedInstance = new WeakReference(callContext.ReturnValue);

            // Return the returnMessage:
            return callContext.ReturnMessage;
        }
    }
}
