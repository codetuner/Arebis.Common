using System.Runtime.Remoting.Messaging;

namespace Arebis.Runtime.Aspects
{
	/// <summary>
	/// Defines the interface for advisor attributes.
	/// </summary>
	public interface IAdvice
	{
        /// <summary>
        /// Whether constructor calls should be intercepted too.
        /// </summary>
        bool IncludeConstructorCalls { get; }

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
