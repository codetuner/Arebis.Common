using System;
using System.Collections.Generic;
using System.Text;

namespace Arebis.CodeAnalysis.Static.Processors.Rules
{
    /// <summary>
    /// Represents the context of a processing run. Allows processor and
    /// processor component instances to share data for a processing session.
    /// </summary>
	public class RuleRunContext
	{
		private Dictionary<string, object> properties = new Dictionary<string, object>();

		public Dictionary<string, object> Properties
		{
			get { return properties; }
		}
	}
}
