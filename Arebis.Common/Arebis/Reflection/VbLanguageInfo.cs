using System;

namespace Arebis.Reflection
{
	/// <summary>
	/// Class providing code language information for the VB.NET language.
	/// </summary>
	[Serializable]
	public class VbLanguageInfo : DefaultLanguageInfo
	{
		/// <summary>
		/// Returns a friendlyname for the given type.
		/// </summary>
		protected override string GetFiendlyName(string forTypeNamed)
		{
			if (forTypeNamed == "System.Int32")
				return "Integer";
			else if (forTypeNamed == "System.Int64")
				return "Long";
			else if (forTypeNamed == "System.Int16")
				return "Short";
			else if (forTypeNamed == "System.String")
				return "String";
			else if (forTypeNamed == "System.Object")
				return "Object";
			else
				return base.GetFiendlyName(forTypeNamed);
		}
	}
}
