using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.Runtime.Aspects
{
    /// <summary>
    /// Base class for advice attributes. When inheriting, also add an AttributeUsage
	/// attribute to specify the exact usage of your attribute.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public abstract class AdviceAttribute : Attribute, IAdvice
    {
        private bool includeConstructorCalls;

        /// <summary>
        /// AdviceAttribute constructor.
        /// </summary>
        /// <param name="includeConstructorCalls">Whether constructor calls should be intercepted too.</param>
        public AdviceAttribute(bool includeConstructorCalls)
        {
            this.includeConstructorCalls = includeConstructorCalls;
        }

        /// <summary>
        /// Whether constructor calls should be intercepted too.
        /// </summary>
        public bool IncludeConstructorCalls
        {
            get { return this.includeConstructorCalls; }
        }

        /// <summary>
        /// Method called before a call is issued.
        /// </summary>
        public abstract void BeforeCall(ICallContext callContext);

        /// <summary>
        /// Method called after a call is issued.
        /// </summary>
        public abstract void AfterCall(ICallContext callContext);
    }
}
