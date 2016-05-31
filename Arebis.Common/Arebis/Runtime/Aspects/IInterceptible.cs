using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arebis.Runtime.Aspects
{
    /// <summary>
    /// Interface to be implemented by [InterceptionAdvice] classes to
    /// have interception working.
    /// </summary>
    public interface IInterceptible
    {
        /// <summary>
        /// Method called before a call is issued.
        /// </summary>
        void BeforeCall(ICallContext callContext);

        /// <summary>
        /// Method called after a call is issued.
        /// </summary>
        void AfterCall(ICallContext callContext);
    }
}
