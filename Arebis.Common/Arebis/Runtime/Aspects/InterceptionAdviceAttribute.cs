using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Runtime.Aspects
{
    /// <summary>
    /// An AdviceAttribute that redirects interception calls to the
    /// decorated object's IInterceptible methods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class InterceptionAdviceAttribute : AdviceAttribute
    {
        /// <summary>
        /// InterceptionAdviceAttribute constructor.
        /// </summary>
        public InterceptionAdviceAttribute()
            : base(false)
        { }

        /// <summary>
        /// Method called before a call is issued.
        /// </summary>
        public override void BeforeCall(ICallContext callContext)
        {
            // Should happen only on constructor methods which are not supported:
            if (callContext.Instance == null)
                return;

            // Call IInterceptible.BeforeCall method, or throw exception:
            var instance = callContext.Instance as IInterceptible;
            if (instance != null)
                instance.BeforeCall(callContext);
            else
                throw new InvalidOperationException(String.Format("{0} should implement IInterceptible.", callContext.Instance.GetType()));
        }

        /// <summary>
        /// Method called after a call is issued.
        /// </summary>
        public override void AfterCall(ICallContext callContext)
        {
            // Should happen only on constructor methods which are not supported:
            if (callContext.Instance == null)
                return;

            // Call IInterceptible.AfterCall method, or throw exception:
            var instance = callContext.Instance as IInterceptible;
            if (instance != null)
                instance.AfterCall(callContext);
            else
                throw new InvalidOperationException(String.Format("{0} should implement IInterceptible.", callContext.Instance.GetType()));
        }
    }
}
